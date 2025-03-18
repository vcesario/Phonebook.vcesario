using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using vcesario.Flashcards;

namespace vcesario.Phonebook;

public class ContactManager
{
    enum CategoryOption
    {
        None,
        Family,
        Friends,
        Work,
        All,
        Return,
    }

    enum MenuOption
    {
        ViewContacts,
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
                case MenuOption.ViewContacts:
                    await ViewContacts();
                    break;
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

    private async Task ViewContacts()
    {
        do
        {
            Console.Clear();
            Console.WriteLine(AppTexts.MENUOPTION_VIEWCONTACTS);

            Console.WriteLine();
            var category = AnsiConsole.Prompt(
                new SelectionPrompt<CategoryOption>()
                .Title(AppTexts.PROMPT_CATEGORY)
                .AddChoices([
                    CategoryOption.All,
                    CategoryOption.None,
                    CategoryOption.Family,
                    CategoryOption.Friends,
                    CategoryOption.Work,
                    CategoryOption.Return,
                ])
                .UseConverter(ConvertCategoryOption)
            );

            if (category == CategoryOption.Return)
            {
                return;
            }

            List<Contact>? contacts = null;
            using (var dbContext = new PhonebookContext())
            {
                try
                {
                    if (category == CategoryOption.All)
                    {
                        contacts = await dbContext.Contacts.OrderBy(c => c.Name).ToListAsync();
                    }
                    else
                    {
                        string categoryStr = category.ToString();
                        contacts = await dbContext.Contacts.Where(c => c.Category.Equals(categoryStr)).OrderBy(c => c.Name).ToListAsync();
                    }
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ReadLine();
                }
            }

            if (contacts == null || contacts.Count == 0)
            {
                Console.WriteLine(AppTexts.VIEWCONTACTS_LOG_NONE);
                Console.ReadLine();
            }
            else
            {
                AnsiConsole.MarkupLine(string.Format(AppTexts.VIEWCONTACTS_LABEL_VIEWCATEGORY, $"[gold3_1]{category}[/]"));

                Console.WriteLine();
                PrintContactTable(contacts);

                Console.WriteLine();
                Console.WriteLine(AppTexts.TOOLTIP_KEYTORETURN);
                Console.ReadLine();
            }
        }
        while (true);
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

        var category = AnsiConsole.Prompt(
            new SelectionPrompt<CategoryOption>()
            .Title(AppTexts.PROMPT_CATEGORY)
            .AddChoices([
                CategoryOption.None,
                CategoryOption.Family,
                CategoryOption.Friends,
                CategoryOption.Work,
                CategoryOption.Return])
            .UseConverter(ConvertCategoryOption));
        if (category == CategoryOption.Return)
        {
            return;
        }
        AnsiConsole.MarkupLine(AppTexts.FIELD_CATEGORY + $": [gold3_1]{category}[/]");

        using (var dbContext = new PhonebookContext())
        {
            try
            {
                dbContext.Add(new Contact()
                {
                    Name = name,
                    PhoneNumber = phone,
                    Email = email,
                    Category = category.ToString(),
                });
                await dbContext.SaveChangesAsync();

                Console.WriteLine();
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
                contacts = await dbContext.Contacts.OrderBy(c => c.Name).ToListAsync();
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

            PrintContactTable([contact]);

            bool changedAnyField = false;
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
            }
            else if (!newName.Equals(contact.Name))
            {
                changedAnyField = true;
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
            }
            else if (!newPhoneNumber.Equals(contact.PhoneNumber))
            {
                changedAnyField = true;
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
            }
            else if (!newEmail.Equals(contact.Email))
            {
                changedAnyField = true;
            }

            var newCategory = AnsiConsole.Prompt(
            new SelectionPrompt<CategoryOption>()
            .Title(AppTexts.EDITCONTACT_PROMPT_NEWCATEGORY)
            .AddChoices([
                CategoryOption.None,
                CategoryOption.Family,
                CategoryOption.Friends,
                CategoryOption.Work,
                CategoryOption.Return])
            .UseConverter(ConvertCategoryOption));

            if (newCategory == CategoryOption.Return)
            {
                continue;
            }

            if (!contact.Category.Equals(newCategory.ToString()))
            {
                changedAnyField = true;
            }

            if (!changedAnyField)
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
                    contact.Category = newCategory.ToString();

                    dbContext.Update(contact);

                    await dbContext.SaveChangesAsync();

                    Console.WriteLine();
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

            PrintContactTable([contact]);

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
            case MenuOption.ViewContacts: return AppTexts.MENUOPTION_VIEWCONTACTS;
            case MenuOption.CreateContact: return AppTexts.MENUOPTION_CREATECONTACT;
            case MenuOption.EditContact: return AppTexts.MENUOPTION_EDITCONTACT;
            case MenuOption.RemoveContact: return AppTexts.MENUOPTION_REMOVECONTACT;
            case MenuOption.Return: return AppTexts.MENUOPTION_RETURN;
            default: return option.ToString();
        }
    }

    private string ConvertCategoryOption(CategoryOption category)
    {
        if (category == CategoryOption.Return)
        {
            return AppTexts.MENUOPTION_RETURN;
        }

        return category.ToString();
    }

    private void PrintContactTable(List<Contact> contacts)
    {
        Table table = new Table();

        table.AddColumns($"[indianred]{AppTexts.FIELD_NAME}[/]",
            $"[indianred]{AppTexts.FIELD_PHONE}[/]",
            $"[indianred]{AppTexts.FIELD_EMAIL}[/]",
            $"[indianred]{AppTexts.FIELD_CATEGORY}[/]");

        foreach (var contact in contacts)
        {
            table.AddRow(contact.Name,
                contact.PhoneNumber,
                string.IsNullOrEmpty(contact.Email) ? "---" : contact.Email,
                GetCategoryDisplayValue(contact.Category));
        }

        table.Border = TableBorder.Rounded;
        AnsiConsole.Write(table);
    }

    private string GetCategoryDisplayValue(string categoryStr)
    {
        if (Enum.TryParse<CategoryOption>(categoryStr, true, out var category) && category != CategoryOption.None)
        {
            return category.ToString();
        }

        return "---";
    }
}