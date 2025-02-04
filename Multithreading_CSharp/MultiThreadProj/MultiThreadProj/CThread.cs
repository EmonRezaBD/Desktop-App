using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadProj
{
    public class CThread
    {
        //Properties
        public string ThreadName { get; set; }
        /*private bool isRunning = true;
        private object lockObject = new object();*/

        public CThread(string name, ThreadStart method) //ThreadStart is a delegate method
        {
            Thread t1 = new Thread(method);
            t1.Name = name;
            t1.Start();
        }


    }
}
