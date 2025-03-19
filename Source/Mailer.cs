using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using Spectre.Console;
using vcesario.Flashcards;

public class Mailer
{
    public async Task Open()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDMAIL);
        AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_CANCEL}[/]");

        var panel = new Panel(AppTexts.MAILER_DISCLAIMER)
        {
            Header = new PanelHeader($"[gold3_1]{AppTexts.MAILER_DISCLAIMER_HEADER}[/]"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1, 2, 1)
        };

        Console.WriteLine();
        AnsiConsole.Write(panel);

        Console.WriteLine();
        var apiKey = AnsiConsole.Prompt(new TextPrompt<string>(AppTexts.MAILER_PROMPT_APIKEY));
        if (apiKey.Equals("."))
        {
            return;
        }

        var senderName = AnsiConsole.Prompt(new TextPrompt<string>(AppTexts.MAILER_PROMPT_SENDERNAME));
        if (senderName.Equals("."))
        {
            return;
        }

        var validator = new UserInputValidator();
        var senderEmail = AnsiConsole.Prompt(
            new TextPrompt<string>(AppTexts.MAILER_PROMPT_SENDEREMAIL)
            .Validate(validator.ValidateEmailOrPeriod));
        if (senderEmail.Equals("."))
        {
            return;
        }

        List<Contact> contacts;
        using (var dbContext = new PhonebookContext())
        {
            try
            {
                contacts = await dbContext.Contacts.Where(c => !string.IsNullOrEmpty(c.Email)).OrderBy(c => c.Name).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
                return;
            }
        }

        var prompt = new SelectionPrompt<Contact>()
            .Title(AppTexts.PROMPT_CHOOSECONTACT)
            .AddChoices(contacts)
            .UseConverter(c => c.Name);
        prompt.AddChoice(new Contact() { Id = -1, Name = AppTexts.MENUOPTION_RETURN });

        Console.WriteLine();
        var contact = AnsiConsole.Prompt(prompt);

        if (contact.Id == -1)
        {
            return;
        }
        Console.WriteLine($"{AppTexts.MAILER_FIELD_CONTACT}: {contact.Name} <{contact.Email}>");

        // ask for subject
        var subject = AnsiConsole.Prompt(new TextPrompt<string>(AppTexts.MAILER_PROMPT_SUBJECT));
        // ask for message
        var mailContent = AnsiConsole.Prompt(new TextPrompt<string>(AppTexts.MAILER_PROMPT_MESSAGE));

        // print message
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDMAIL);
        Console.WriteLine(AppTexts.MAILER_LABEL_PREVIEW);

        Console.WriteLine();
        Console.WriteLine($"{AppTexts.MAILER_FIELD_APIKEY}: {apiKey}");
        Console.WriteLine($"{AppTexts.MAILER_FIELD_FROM}: {senderName} <{senderEmail}>");
        Console.WriteLine($"{AppTexts.MAILER_FIELD_TO}: {contact.Name} <{contact.Email}>");
        Console.WriteLine($"{AppTexts.MAILER_FIELD_SUBJECT}: {subject}");
        Console.WriteLine($"{AppTexts.MAILER_FIELD_MESSAGE}:\n\n{mailContent}");
        Console.WriteLine();

        // send?
        Console.WriteLine();
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(AppTexts.MAILER_SENDCONFIRM)
            {
                DefaultValue = false
            });

        if (!confirmation)
        {
            Console.WriteLine(AppTexts.MAILER_LOG_CANCELLED);
            Console.ReadLine();
            return;
        }

        // try send, log result
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(senderEmail, senderName);
        var to = new EmailAddress(contact.Email, contact.Name);
        var message = MailHelper.CreateSingleEmail(
            from,
            to,
            subject,
            mailContent,
            mailContent
        );

        var response = await client.SendEmailAsync(message);

        switch ((int)response.StatusCode)
        {
            case 200:
            case 201:
            case 204:
                Console.WriteLine();
                AnsiConsole.MarkupLine($"[green]{AppTexts.MAILER_LOG_SUCCESS}[/]");
                Console.ReadLine();
                break;
            default:
                Console.WriteLine();
                Console.WriteLine($"Error: {response.StatusCode} ({(int)response.StatusCode})");
                Console.ReadLine();
                break;
        }
    }
}