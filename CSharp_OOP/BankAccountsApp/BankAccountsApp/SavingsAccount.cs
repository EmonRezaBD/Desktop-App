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

        public override string Deposit(decimal amount)//this method will allow to access the balance to other classes, but here all the rules will be there and we need to follow those
        {
            if (amount <= 0)
                return "You can't deposit $" + amount;
            if (amount > 20000)
                return "Limit reached";
            decimal interestaAmount = (InterestRate/100)*amount;

            Balance += amount + interestaAmount;

            return "Deposit completed successfully";

        }
    }
}
