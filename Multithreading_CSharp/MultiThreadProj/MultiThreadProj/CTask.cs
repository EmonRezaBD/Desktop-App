using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiThreadingProj;

namespace MultiThreadProj
{
    public class CTask
    {
        //Method
        private Program pr = new Program();

        public void addTask()
        {
            Console.WriteLine("Task class Thread start");
            pr.Add(2, 40);
            Console.WriteLine("Task class Thread end");
        }
    }
}
