public class student
{
    public string name { get; set; }
    public int age { get; set; }
    public double averageGrade { get; set; }

    public override string ToString()
    {
        return name +" "+age+" "+"("+averageGrade+")";
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        int num = 5;
        student s = new student();
        s.name = "Sladina";
        s.age = 23;
        s.averageGrade = 10;


        Console.WriteLine("Your num: " + num.ToString());
        Console.WriteLine(s.name);

        Console.WriteLine(s.ToString());
        Console.WriteLine(s);
    }

}
