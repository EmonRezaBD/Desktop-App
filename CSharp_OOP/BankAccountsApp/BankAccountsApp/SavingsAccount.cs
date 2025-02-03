using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountsApp
{
    //internal class SavingsAccount
    public class SavingsAccount:BankAccount //child class: base class
    {
        public decimal InterestRate {  get; set; }

        public SavingsAccount(string owner, decimal interestRate):base(owner+" ("+interestRate+"%)")//passing S with the default parameter
            //each class should initiazlize its own properties
        {   //passing the parameter received in the child class to the base class
           InterestRate = interestRate;
        }
    }
}
