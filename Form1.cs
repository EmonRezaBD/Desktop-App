using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NetMQ;//Reza add
using NetMQ.Sockets;//Reza add
using Newtonsoft.Json;//Reza add

namespace zmqCPP
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        //public double Balance { get; set; }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(var requestSocket = new RequestSocket("tcp://127.0.0.1:5555"))
            {
                var Person1 = new Person
                {
                    Name = NametextBox.Text,
                    Age = Int32.Parse(AgetextBox.Text)
                    
                };

                var serializedPerson = JsonConvert.SerializeObject(Person1);
                requestSocket.SendFrame(serializedPerson);
            }
        }
    }
}
