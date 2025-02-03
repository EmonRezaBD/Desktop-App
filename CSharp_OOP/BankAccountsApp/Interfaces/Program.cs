// See https://aka.ms/new-console-template for more information


public interface IEnemy
{
    //we will introduce the rules our enemy need to follow
    //If one can impelemnet these rules then it can be an enemy otherwise not

    void Attack();
    void Defend();
}

public class Dragon : IEnemy
{
    public void Attack()
    {
        Console.WriteLine("Dragon breaths fire!");
    }

    public void Defend()
    {
        Console.WriteLine("Dragon spreads its wings to sheild itself");
    }

    public int Eat()
    {
        return 0;
    }
}

public class Vampire : IEnemy
{
    public void Attack()
    {
        Console.WriteLine("With fangs");
    }

    public void Defend()
    {
        Console.WriteLine("Leg");
    }

}

internal class Program
{
    //interface is a contract. Classes need to sign the contarct to use the interface. Just like USB devices to interface with PC. Different peripheral devices need to comply with those contracts
    //
    private static void Main(string[] args)
    {
        IEnemy e1 = new Dragon(); //Creating object of interface in C#
        IEnemy e2 = new Vampire(); 

       /* e1.Attack();
        e2.Attack();

        e1.Defend();
        e2.Defend();*/

        List<IEnemy> list = new List<IEnemy>();
        list.Add(e1);
        list.Add(e2);

        foreach (IEnemy e in list)
        {
            e.Attack();
        }

    }

}
