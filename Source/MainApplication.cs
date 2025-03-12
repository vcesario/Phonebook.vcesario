using System.Threading.Tasks;
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

    public static void Run()
    {
        DataService.Initialize();

        MenuOption chosenOption;
        do
        {
            Console.Clear();

            AnsiConsole.MarkupLine($"[darkmagenta]{AppTexts.APP_TITLE}[/]");

            Console.WriteLine();
            chosenOption = AnsiConsole.Prompt(
               new SelectionPrompt<MenuOption>()
               .Title(AppTexts.PROMPT_ACTION)
               .AddChoices(Enum.GetValues<MenuOption>())
               .UseConverter(ConvertMenuOption)
           );

            switch (chosenOption)
            {
                case MenuOption.ManageContacts:
                    new ContactManager().Open();
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