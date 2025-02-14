using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMVVM01.MVVM.Model
{
    public class CustomersModel
    {
        //Fields or attributes
        private string full_name;

        //Contructors
        CustomersModel(string full_name)
        {
            this.full_name = full_name;
        }

        //Property
        public string Full_Name
        {
            get { return full_name; } //getter
            set { full_name = value; }//setter
        }

        //Method

    }
}
