using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace vcesario.Flashcards;

public class UserInputValidator
{
    public ValidationResult ValidateUniqueNameOrPeriod(string input)
    {
        using (var dbContext = new PhonebookContext())
        {
            try
            {
                var contact = dbContext.Contacts.SingleOrDefault(c => c.Name.Equals(input));
                if (contact != null)
                {
                    return ValidationResult.Error("Name already exists.");
                }

                return ValidationResult.Success();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }

        return ValidationResult.Error();
    }

    public ValidationResult ValidateUniqueNameEmptyOrPeriod(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return ValidationResult.Success();
        }

        return ValidateUniqueNameOrPeriod(input);
    }

    public ValidationResult ValidatePhoneNumberOrPeriod(string input)
    {
        AnsiConsole.MarkupLine("[green]Todo: Validate phone number[/]");
        return ValidationResult.Success();
    }

    public ValidationResult ValidatePhoneNumberEmptyOrPeriod(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return ValidationResult.Success();
        }

        return ValidatePhoneNumberOrPeriod(input);
    }

    public ValidationResult ValidateEmailOrPeriod(string input)
    {
        AnsiConsole.MarkupLine("[green]Todo: Validate email[/]");
        return ValidationResult.Success();
    }

    public ValidationResult ValidateEmailEmptyOrPeriod(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return ValidationResult.Success();
        }

        return ValidateEmailOrPeriod(input);
    }
}