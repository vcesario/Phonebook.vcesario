using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using vcesario.Flashcards;

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

    public async Task Open()
    {
        MenuOption chosenOption;
        do
        {
            Console.Clear();

            Console.WriteLine(AppTexts.MENUOPTION_MANAGECONTACTS);

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
                    await CreateContact();
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

    private async Task CreateContact()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_CREATECONTACT);
        AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_CANCEL}[/]");

        var validator = new UserInputValidator();

        Console.WriteLine();
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>(AppTexts.CREATECONTACT_PROMPT_NAME)
            .Validate(validator.ValidateUniqueNameOrPeriod));
        if (name.Equals("."))
        {
            return;
        }

        var phone = AnsiConsole.Prompt(
            new TextPrompt<string>(AppTexts.CREATECONTACT_PROMPT_PHONE)
            .Validate(validator.ValidatePhoneNumberOrPeriod));
        if (phone.Equals("."))
        {
            return;
        }

        var email = AnsiConsole.Prompt(
            new TextPrompt<string>(AppTexts.CREATECONTACT_PROMPT_EMAIL)
            {
                AllowEmpty = true
            }
            .Validate(validator.ValidateEmailEmptyOrPeriod));
        if (email.Equals("."))
        {
            return;
        }

        using (var dbContext = new PhonebookContext())
        {
            try
            {
                dbContext.Add(new Contact()
                {
                    Name = name,
                    PhoneNumber = phone,
                    Email = email
                });
                await dbContext.SaveChangesAsync();

                Console.WriteLine(AppTexts.CREATECONTACT_LOG_CONTACTADDED);
                Console.ReadLine();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }
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