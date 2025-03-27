public static class AppTexts
{
    public const string PROMPT_ACTION = "What do you want to do?";
    public const string PROMPT_CATEGORY = "Choose a contact category:";
    public const string PROMPT_RECONFIRM = "Are you REALLY sure?";
    public const string PROMPT_CHOOSECONTACT = "Choose a contact to send your message to:";
    public const string PROMPT_MESSAGE = "Enter message:";
    public const string PROMPT_CONFIRMMESSAGE = "Send message?";
    public const string LABEL_APPTITLE = "===== PHONEBOOK =====";
    public const string LABEL_MESSAGEPREVIEW = "Message preview";
    public const string LOG_MESSAGECANCELLED = "Message discarded.";
    public const string TOOLTIP_CANCEL = "Enter '.' anywhere to cancel.";
    public const string TOOLTIP_KEEP = "Leave field blank to keep it";
    public const string TOOLTIP_KEYTORETURN = "Press any key to return.";
    public const string FIELD_NAME = "Name";
    public const string FIELD_PHONE = "Phone number";
    public const string FIELD_EMAIL = "E-mail";
    public const string FIELD_CATEGORY = "Category";
    public const string FIELD_CONTACT = "Contact";
    public const string FIELD_TO = "To";
    public const string FIELD_MESSAGE = "Message";

    public const string VIEWCONTACTS_LOG_NONE = "No contacts found.";
    public const string VIEWCONTACTS_LABEL_VIEWCATEGORY = "Viewing contacts: {0}";

    public const string CREATECONTACT_PROMPT_NAME = "Name your contact:";
    public const string CREATECONTACT_PROMPT_PHONE = "Enter phone number:";
    public const string CREATECONTACT_PROMPT_EMAIL = "Enter e-mail address [grey](optional)[/]:";
    public const string CREATECONTACT_LOG_ADDED = "Contact added.";

    public const string EDITCONTACT_PROMPT_SELECT = "Which contact do you want to edit?";
    public const string EDITCONTACT_PROMPT_NEWNAME = "Enter new name:";
    public const string EDITCONTACT_PROMPT_NEWPHONE = "Enter new phone number:";
    public const string EDITCONTACT_PROMPT_NEWEMAIL = "Enter new e-mail address:";
    public const string EDITCONTACT_PROMPT_NEWCATEGORY = "Choose a new contact category:";
    public const string EDITCONTACT_LOG_UPDATED = "Contact updated.";

    public const string REMOVECONTACT_PROMPT_SELECT = "Which contact do you want to remove?";
    public const string REMOVECONTACT_PROMPT_REMOVE = "Are you sure you want to remove this contact?";
    public const string REMOVECONTACT_LOG_REMOVED = "Contact removed.";

    public const string MAILER_DISCLAIMER = "This application uses [indianred]Twilio's Sendgrid[/]."
                                            + "\nYou'll need to provide a SendGrid API Key,"
                                            + "\nalong with the sender's name and email.";
    public const string MAILER_DISCLAIMER_HEADER = "Disclaimer";
    public const string MAILER_PROMPT_APIKEY = "Enter SendGrid API Key:";
    public const string MAILER_PROMPT_SENDERNAME = "Enter sender name:";
    public const string MAILER_PROMPT_SENDEREMAIL = "Enter sender email:";
    public const string MAILER_PROMPT_SUBJECT = "Enter email subject:";
    public const string MAILER_LOG_SUCCESS = "Message sent successfully.";
    public const string MAILER_FIELD_APIKEY = "SendGrid API Key";
    public const string MAILER_FIELD_FROM = "From";
    public const string MAILER_FIELD_SUBJECT = "Subject";

    public const string SMS_LOG_SUCCESS = "SMS won't be sent due to API not implemented. Consider success!";

    public const string MENUOPTION_SENDMAIL = "Send mail";
    public const string MENUOPTION_SENDSMS = "Send SMS";
    public const string MENUOPTION_MANAGECONTACTS = "Manage contacts";
    public const string MENUOPTION_EXIT = "[grey]Exit[/]";
    public const string MENUOPTION_VIEWCONTACTS = "View contacts";
    public const string MENUOPTION_CREATECONTACT = "Add new contact";
    public const string MENUOPTION_EDITCONTACT = "Edit contact";
    public const string MENUOPTION_REMOVECONTACT = "Remove contact";
    public const string MENUOPTION_RETURN = "[grey]Return[/]";
}