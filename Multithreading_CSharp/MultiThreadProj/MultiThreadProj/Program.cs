using System;
using System.ComponentModel;
using System.Threading.Tasks;

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
        //static Mutex mutex = new Mutex();
        public static void childThread01()
        {
           // mutex.WaitOne();
            for (int i = 0;i <3 ;i++)
            {
               Console.WriteLine("Child Thread 01 ");
                //Thread.Sleep(500);
            }
            //mutex.ReleaseMutex();
        }

        public static void childThread02()
        {
            for(int i = 0; i<3 ;i++)
            {
                Console.WriteLine("Child Thread 02 ");
                //Thread.Sleep(500);
            }
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

            //Task class

            CTask taskObj = new CTask();
            Task taskThread = new Task(() =>
            {
              taskObj.addTask();
            }
             );
            taskThread.Start();

            Console.WriteLine("Starting task...");

            Task<int> task = Task.Run(() =>
            {
                // Simulating a long-running task
                Task.Delay(2000).Wait();
                return 42; // The answer to everything!
            });

            //Delegate
            DisplayMessage msgDelegate = testDelegate;
            msgDelegate("This is from Delegate"); //invoking delegate method

            //Child Thread, Join operations
            Thread chTh01 = new Thread(()=>childThread01());
            Thread chTh02 = new Thread(() => childThread02());

           // chTh01.Sleep(1000);
            chTh01.Name = "Child Thread 01";
            chTh02.Name = "Child Thread 02";

            chTh02.Start();

            chTh01.Start();
            chTh01.Join();

           
            chTh02.Join();

           

            Console.WriteLine("1. MainThread Started");
            for (int i = 0; i <= 3; i++)
            {
                Console.WriteLine("-> MainThread Executing");
                //Thread.Sleep(2000); // Here 5000 is 5000 Milli Seconds means 5 Seconds
            }



            Console.ReadKey(); // Prevents the program from exiting immediately

        }
    }
}