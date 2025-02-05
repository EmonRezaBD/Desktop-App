using System;
using System.ComponentModel;
using MultiThreadProj;
namespace MultiThreadingProj
{
    class Program
    {
        //Vars

        //Methods
        public void Add(int a, int b)
        {
            Console.WriteLine("Sum: {0}", a+b);
            //return a+b;
        }
        public void Sub(int a, int b)
        {
            Console.WriteLine("Subtarction: {0}", a - b);
            //return a+b;
        }
        public void Mul(int a, int b, int c)
        {
            Console.WriteLine("Multiplication: {0}", a * b *c);
            //return a+b;
        }
        public void Div()
        {
            Console.WriteLine("Division");
            for (int i = 0; i < 5; i++)
            {
               Console.WriteLine("Inside Div func");
            }
            //return a+b;
        }

        public static void testDelegate(string message)
        {
            Console.WriteLine(message);
        }
        static void Main(string[] args)
        {
            Program p1 = new Program();

            //p1.Add is args delegate function
            //CThread AdditionThread1 = new CThread("Addtion", p1.Add); //if there is parameter then ThreadStart will not accept and we need lamda expression
            CThread AdditionThread2 = new CThread("Addition", () => p1.Add(10, 20)); //Lamda expression
            CThread SubsThread1 = new CThread("Subtraction", () => p1.Sub(10, 20));
            CThread MulThread1 = new CThread("Multiplication", () => p1.Mul(10, 20, 2));
            CThread DivThread1 = new CThread("Division", () => p1.Div());

            //Console.WriteLine("Properties of Thread Class");
            //Task class

            CTask taskObj = new CTask();
            Task taskThread = new Task(() =>
            {
              taskObj.addTask();
            }
             );
            taskThread.Start();

            //Delegate
            DisplayMessage msgDelegate = testDelegate;
            msgDelegate("This is from Delegate"); //invoking delegate method


            Console.ReadKey(); // Prevents the program from exiting immediately

        }
    }
}