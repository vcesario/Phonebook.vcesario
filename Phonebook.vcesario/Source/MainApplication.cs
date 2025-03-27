using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace vcesario.Phonebook;

public static class MainApplication
{
    enum MenuOption
    {
        SendMail,
        SendSms,
        ManageContacts,
        Exit,
    }

    public static async Task Run()
    {
        DataService.Initialize();

        MenuOption chosenOption;
        do
        {
            Console.Clear();

            AnsiConsole.MarkupLine($"[darkmagenta]{AppTexts.LABEL_APPTITLE}[/]");

            Console.WriteLine();
            chosenOption = AnsiConsole.Prompt(
               new SelectionPrompt<MenuOption>()
               .Title(AppTexts.PROMPT_ACTION)
               .AddChoices(Enum.GetValues<MenuOption>())
               .UseConverter(ConvertMenuOption)
           );

            switch (chosenOption)
            {
                case MenuOption.SendMail:
                    await new Mailer().Open();
                    break;
                case MenuOption.SendSms:
                    await SendSms();
                    break;
                case MenuOption.ManageContacts:
                    await new ContactManager().Open();
                    break;
                case MenuOption.Exit:
                    break;
                default:
                    Console.WriteLine("Implement option: " + chosenOption);
                    Console.ReadLine();
                    break;
            }
        }
        while (chosenOption != MenuOption.Exit);
    }

    private static async Task SendSms()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDSMS);
        AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_CANCEL}[/]");

        List<Contact> contacts;
        using (var dbContext = new PhonebookContext())
        {
            try
            {
                contacts = await dbContext.Contacts.Where(c => !string.IsNullOrEmpty(c.PhoneNumber)).OrderBy(c => c.Name).ToListAsync();
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
        Console.WriteLine($"{AppTexts.FIELD_CONTACT}: {contact.Name} <{contact.Email}>");

        var content = AnsiConsole.Prompt(new TextPrompt<string>(AppTexts.PROMPT_MESSAGE));
        if (content.Equals("."))
        {
            return;
        }

        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_SENDSMS);
        Console.WriteLine(AppTexts.LABEL_MESSAGEPREVIEW);

        Console.WriteLine();
        Console.WriteLine($"{AppTexts.FIELD_TO}: {contact.Name} <{contact.PhoneNumber}>");
        Console.WriteLine($"{AppTexts.FIELD_MESSAGE}:\n\n{content}");

        Console.WriteLine();
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(AppTexts.PROMPT_CONFIRMMESSAGE)
            {
                DefaultValue = false
            });

        if (!confirmation)
        {
            Console.WriteLine(AppTexts.LOG_MESSAGECANCELLED);
            Console.ReadLine();
            return;
        }

        Console.WriteLine();
        AnsiConsole.MarkupLine($"[orange3]{AppTexts.SMS_LOG_SUCCESS}[/]");
        Console.ReadLine();
    }

    private static string ConvertMenuOption(MenuOption option)
    {
        switch (option)
        {
            case MenuOption.SendMail: return AppTexts.MENUOPTION_SENDMAIL;
            case MenuOption.SendSms: return AppTexts.MENUOPTION_SENDSMS;
            case MenuOption.ManageContacts: return AppTexts.MENUOPTION_MANAGECONTACTS;
            case MenuOption.Exit: return AppTexts.MENUOPTION_EXIT;
            default: return option.ToString();
        }
    }
}