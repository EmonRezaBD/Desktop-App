using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using MultiThreadProj;

namespace MultiThreadingProj
{
    //class for mutex 
    class Example
    {
       // Create a new Mutex. The creating thread does not own the mutex.
       public static Mutex mut = new Mutex();
       public const int numIterations = 1;
       public const int numThreads = 3;

       public void threadCreation()
       {
            for (int i = 0; i < numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProc));
                newThread.Name = String.Format("Thread{0}", i + 1);
                newThread.Start();
            }

       }

        public static void ThreadProc()
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource();
            }
        }
        public static void UseResource()
        {
            // Wait until it is safe to enter.
            Console.WriteLine("{0} is requesting the mutex",
                              Thread.CurrentThread.Name);
            mut.WaitOne();//mutex call

            Console.WriteLine("{0} has entered the protected area",
                              Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.

            // Simulate some work.
            Thread.Sleep(500);

            Console.WriteLine("{0} is leaving the protected area",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
            Console.WriteLine("{0} has released the mutex",
                Thread.CurrentThread.Name);
        }

    }
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

        static void ChildTask()
        {
            Console.WriteLine("Child Task Started");

            //Child create grand child
            Thread grandChildThread = new Thread(() =>
            {
                Console.WriteLine("Grandchild Thread started");
                Thread.Sleep(1000);
                Console.WriteLine("Grandchild Thread finished");
            });

            grandChildThread.Start();

            grandChildThread.Join();  // Child waits for grandchild
            Console.WriteLine("Child Thread finished.");
        }
        static void Main(string[] args)
        {
            /*  Program p1 = new Program();

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

              //Test Mutex

              Example mutObj = new Example();
              mutObj.threadCreation();*/


            //Parent and Child Task
            //parent creating a child task
            /* Console.WriteLine("Parent Thread started.");
             Thread childThread = new Thread(new ThreadStart(ChildTask));
             childThread.Start();


             Console.WriteLine("Parent Thread continues working...");
             childThread.Join();
             Console.WriteLine("Parent Thread ending.");*/

            //Using Task Class
            Console.WriteLine("Parent Task started.");

            // Create a parent task
            Task parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Parent Task is running...");

                // Create a child task attached to the parent
                Task childTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Child Task started.");

                    // Create a grandchild task attached to the child
                    Task grandChildTask = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Grandchild Task started.");
                        Task.Delay(1000).Wait();  // Simulate work
                        Console.WriteLine("Grandchild Task finished.");
                    }, TaskCreationOptions.AttachedToParent);

                    grandChildTask.Wait(); // Ensure grandchild finishes before moving on
                    Console.WriteLine("Child Task finished.");

                }, TaskCreationOptions.AttachedToParent);
            });

            // Wait for parent, child, and grandchild tasks to complete
            parentTask.Wait();

            Console.WriteLine("Parent, Child, and Grandchild Tasks completed.");

            Console.ReadKey(); // Prevents the program from exiting immediately
        }
    }
}