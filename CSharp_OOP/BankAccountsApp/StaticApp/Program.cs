/*public class Person
{
    public static int PopulationCount { get; set; } = 0; //put static property at the top of the class
    //static property is shared among all the instances of a class 
    public string Name { get; set; }

    public Person(string name)
    {
        Name = name;
        PopulationCount++;
    }   
}*/


public class Calculator
{
    public int Sum(int a, int b) //non static method belongs to the object of that class
    {
        return a + b;
    }

    public static int Multiply(int a, int b) //static method belongs to the class itself
    {
        return a * b;
    }

}

public class WelcomeBot
{
    public static void SayHello(string name) //it can be use for global access
    {
        Console.WriteLine("Hello "+name+"!");
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
       /* Person p1 = new Person("Reza");
        Person p2 = new Person("Tom");
        Person p3 = new Person("Jerry");

        Console.WriteLine(p1.Name);
        Console.WriteLine(p2.Name);
        Console.WriteLine(p3.Name);

        Console.WriteLine(Person.PopulationCount); //classname.static property name */


       //Static method
       Calculator cal1 = new Calculator();
       cal1.Sum(1, 2);
       Calculator.Multiply(1,3);

        //
        WelcomeBot.SayHello("John");
        WelcomeBot.SayHello("Anna");
    }

}
