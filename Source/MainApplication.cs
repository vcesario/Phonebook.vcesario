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
        TestConnection,
        Exit,
    }

    public static async Task Run()
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
           );

            switch (chosenOption)
            {
                case MenuOption.TestConnection:
                    await TestConnection();
                    Console.ReadLine();
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

    private static async Task TestConnection()
    {
        using (var db = new PhonebookContext())
        {
            var any = await db.Contacts.OrderBy(c => c.Id).FirstAsync();
            Console.WriteLine(any);
        }
    }
}