public static class DataService
{
    public static void Initialize()
    {
        using (var db = new PhonebookContext())
        {
            db.Database.EnsureCreated();
        }
    }
}