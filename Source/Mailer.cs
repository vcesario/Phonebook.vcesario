using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using vcesario.Flashcards;

public class Mailer
{
    public async Task Open()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDMAIL);
        AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_CANCEL}[/]");

        var panel = new Panel("This application uses [indianred]Twilio's Sendgrid[/]."
                                + "\nYou'll need to provide a SendGrid API Key,"
                                + "\nalong with the sender's name and email.")
        {
            Header = new PanelHeader("[gold3_1]Disclaimer[/]"),
            Border = BoxBorder.Double,
            Padding = new Padding(2, 1, 2, 1)
        };

        Console.WriteLine();
        AnsiConsole.Write(panel);

        Console.WriteLine();
        var apiKey = AnsiConsole.Prompt(new TextPrompt<string>("Enter SendGrid API Key:"));
        if (apiKey.Equals("."))
        {
            return;
        }

        var senderName = AnsiConsole.Prompt(new TextPrompt<string>("Enter sender name:"));
        if (senderName.Equals("."))
        {
            return;
        }

        var validator = new UserInputValidator();
        var senderEmail = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter sender email:")
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
            .Title("Choose a contact to send your email to:")
            .AddChoices(contacts)
            .UseConverter(c => c.Name);
        prompt.AddChoice(new Contact() { Id = -1, Name = AppTexts.MENUOPTION_RETURN });

        Console.WriteLine();
        var contact = AnsiConsole.Prompt(prompt);

        if (contact.Id == -1)
        {
            return;
        }
        Console.WriteLine($"Contact: {contact.Name} <{contact.Email}>");

        // ask for subject
        var subject = AnsiConsole.Prompt(new TextPrompt<string>("Enter email subject:"));
        // ask for message
        var message = AnsiConsole.Prompt(new TextPrompt<string>("Enter message:\n\n"));

        // print message
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDMAIL);
        Console.WriteLine("Mail preview");

        Console.WriteLine();
        Console.WriteLine($"SendGrid API Key: {apiKey}");
        Console.WriteLine($"From: {senderName} <{senderEmail}>");
        Console.WriteLine($"To: {contact.Name} <{contact.Email}>");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message:\n\n{message}");
        Console.WriteLine();

        // send?
        Console.WriteLine();
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt("Send message?")
            {
                DefaultValue = false
            });

        if (!confirmation)
        {
            Console.WriteLine("Message discarded.");
            Console.ReadLine();
            return;
        }

        // try send, log result
        // ...
    }
}