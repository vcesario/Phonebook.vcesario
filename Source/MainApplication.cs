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
}