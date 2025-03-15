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
                case MenuOption.EditContact:
                    await EditContact();
                    break;
                case MenuOption.RemoveContact:
                    await RemoveContact();
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

                Console.WriteLine(AppTexts.CREATECONTACT_LOG_ADDED);
                Console.ReadLine();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }
    }

    private async Task EditContact()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_EDITCONTACT);

        List<Contact> contacts;
        using (var dbContext = new PhonebookContext())
        {
            try
            {
                contacts = await dbContext.Contacts.OrderBy(c => c.Id).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
                return;
            }
        }

        do
        {
            Console.Clear();
            Console.WriteLine(AppTexts.MENUOPTION_EDITCONTACT);
            AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_CANCEL}[/]");
            AnsiConsole.MarkupLine($"[grey]{AppTexts.TOOLTIP_KEEP}[/]");

            var prompt = new SelectionPrompt<Contact>()
                .Title(AppTexts.EDITCONTACT_PROMPT_SELECT)
                .AddChoices(contacts)
                .UseConverter(c => c.Name);
            prompt.AddChoice(new Contact() { Id = -1, Name = AppTexts.MENUOPTION_RETURN });

            Console.WriteLine();
            var contact = AnsiConsole.Prompt(prompt);
            if (contact.Id == -1)
            {
                return;
            }

            AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_NAME, $"[gold3_1]{contact.Name}[/]"));
            AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_PHONE, $"[gold3_1]{contact.PhoneNumber}[/]"));
            AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_EMAIL, $"[gold3_1]{(string.IsNullOrEmpty(contact.Email) ? "---" : contact.Email)}[/]"));

            int unalteredFields = 0;
            var validator = new UserInputValidator();

            Console.WriteLine();
            var newName = AnsiConsole.Prompt(
                new TextPrompt<string>(AppTexts.EDITCONTACT_PROMPT_NEWNAME)
                {
                    AllowEmpty = true
                }
                .Validate(validator.ValidateUniqueNameEmptyOrPeriod));

            if (newName.Equals("."))
            {
                continue;
            }

            if (string.IsNullOrEmpty(newName))
            {
                newName = contact.Name;
                unalteredFields++;
            }

            var newPhoneNumber = AnsiConsole.Prompt(
                new TextPrompt<string>(AppTexts.EDITCONTACT_PROMPT_NEWPHONE)
                {
                    AllowEmpty = true
                }
                .Validate(validator.ValidatePhoneNumberEmptyOrPeriod));

            if (newPhoneNumber.Equals("."))
            {
                continue;
            }

            if (string.IsNullOrEmpty(newPhoneNumber))
            {
                newPhoneNumber = contact.PhoneNumber;
                unalteredFields++;
            }

            var newEmail = AnsiConsole.Prompt(
                            new TextPrompt<string>(AppTexts.EDITCONTACT_PROMPT_NEWEMAIL)
                            {
                                AllowEmpty = true
                            }
                            .Validate(validator.ValidateEmailEmptyOrPeriod));

            if (newEmail.Equals("."))
            {
                continue;
            }

            if (string.IsNullOrEmpty(newEmail))
            {
                newEmail = contact.Email;
                unalteredFields++;
            }

            if (unalteredFields == 3)
            {
                continue;
            }

            using (var dbContext = new PhonebookContext())
            {
                try
                {
                    contact.Name = newName;
                    contact.PhoneNumber = newPhoneNumber;
                    contact.Email = newEmail;

                    dbContext.Update(contact);

                    await dbContext.SaveChangesAsync();

                    Console.WriteLine(AppTexts.EDITCONTACT_LOG_UPDATED);
                    Console.ReadLine();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ReadLine();
                }
            }
        }
        while (true);
    }

    private async Task RemoveContact()
    {
        Console.Clear();
        Console.WriteLine(AppTexts.MENUOPTION_REMOVECONTACT);

        List<Contact> contacts;
        using (var dbContext = new PhonebookContext())
        {
            try
            {
                contacts = await dbContext.Contacts.OrderBy(c => c.Id).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
                return;
            }
        }

        do
        {
            Console.Clear();
            Console.WriteLine(AppTexts.MENUOPTION_REMOVECONTACT);

            var prompt = new SelectionPrompt<Contact>()
                .Title(AppTexts.REMOVECONTACT_PROMPT_SELECT)
                .AddChoices(contacts)
                .UseConverter(c => c.Name);
            prompt.AddChoice(new Contact() { Id = -1, Name = AppTexts.MENUOPTION_RETURN });

            Console.WriteLine();
            var contact = AnsiConsole.Prompt(prompt);
            if (contact.Id == -1)
            {
                return;
            }

            AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_NAME, $"[gold3_1]{contact.Name}[/]"));
            AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_PHONE, $"[gold3_1]{contact.PhoneNumber}[/]"));
            if (!string.IsNullOrEmpty(contact.Email))
            {
                AnsiConsole.MarkupLine(string.Format(AppTexts.FIELD_EMAIL, $"[gold3_1]{contact.Email}[/]"));
            }

            Console.WriteLine();
            var confirm = AnsiConsole.Prompt(
                new ConfirmationPrompt(AppTexts.REMOVECONTACT_PROMPT_REMOVE)
                {
                    DefaultValue = false
                });

            if (!confirm)
            {
                continue;
            }

            confirm = AnsiConsole.Prompt(
                new ConfirmationPrompt(AppTexts.PROMPT_RECONFIRM)
                {
                    DefaultValue = false
                });

            if (!confirm)
            {
                continue;
            }

            using (var dbContext = new PhonebookContext())
            {
                try
                {
                    dbContext.Remove(contact);
                    await dbContext.SaveChangesAsync();

                    contacts.Remove(contact);

                    Console.WriteLine();
                    Console.WriteLine(AppTexts.REMOVECONTACT_LOG_REMOVED);
                    Console.ReadLine();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ReadLine();
                }
            }
        }
        while (true);
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