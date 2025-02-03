public partial class Book
{
    public string Title {  get; set; }
    public string Author {  get; set; }
    public int Year {  get; set; }
}
//We can split a class into 02 parts but compiler treats it as a single class
public partial class Book
{
    public void showInfo()
    {
        Console.WriteLine("Title: " + Title);
        Console.WriteLine("Author: " + Author);
        Console.WriteLine("Year: " + Year);

    }
}
public partial class Book
{ 
    public void updatedate()
    {
        Year = 2001;
    }
}



namespace PartialClasses
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            Book book1 = new Book();
            book1.Title = "Times";
            book1.Author = "Reza";
            book1.Year = 2000;
            book1.updatedate();

            book1.showInfo();

            //Benefits
            //1. We can separate different parts upon our purpose
            //2. Multiple developer working on a same class
        }
    }
}