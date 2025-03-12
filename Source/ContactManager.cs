using Spectre.Console;

namespace vcesario.Phonebook;

public class ContactManager
{
    enum MenuOption
    {
        CreateContact,
        EditContact,
        RemoveContact,
        Return,
    }

    public void Open()
    {
        MenuOption chosenOption;
        do
        {
            Console.Clear();

            Console.WriteLine("Manage contacts");

            Console.WriteLine();
            chosenOption = AnsiConsole.Prompt(
               new SelectionPrompt<MenuOption>()
               .Title(AppTexts.PROMPT_ACTION)
               .AddChoices(Enum.GetValues<MenuOption>())
               .UseConverter(ConvertMenuOption)
           );

            switch (chosenOption)
            {
                case MenuOption.CreateContact:
                    CreateContact();
                    break;
                case MenuOption.Return:
                    break;
                default:
                    Console.WriteLine("Implement option: " + chosenOption);
                    Console.ReadLine();
                    break;
            }
        }
        while (chosenOption != MenuOption.Return);
    }

    private void CreateContact()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_CREATECONTACT);
        AnsiConsole.MarkupLine("[grey]Enter '.' anywhere to cancel.[/]");
        Console.WriteLine();
        var name = AnsiConsole.Prompt(new TextPrompt<string>("Name your contact:"));

        if (name.Equals("."))
        {
            return;
        }

        Console.WriteLine("Contact added.");
        Console.ReadLine();
    }

    private string ConvertMenuOption(MenuOption option)
    {
        switch (option)
        {
            case MenuOption.CreateContact: return AppTexts.MENUOPTION_CREATECONTACT;
            case MenuOption.EditContact: return AppTexts.MENUOPTION_EDITCONTACT;
            case MenuOption.RemoveContact: return AppTexts.MENUOPTION_REMOVECONTACT;
            case MenuOption.Return: return AppTexts.MENUOPTION_RETURN;
            default: return option.ToString();
        }
    }
}