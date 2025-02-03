using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountsApp
{
    public class BankAccount
    {
        public string Owner {  get; set; }
        public Guid AccountNumber { get; set; } //Global unique variable
        public decimal Balance { get; protected set; } //encapsulating using setter as private
                                //protected : accessible inside own class and inside derived classes
        //Constructor
        public BankAccount(string owner) 
        { 
            Owner = owner;
            AccountNumber = Guid.NewGuid();
            Balance = 0;
        }

        public virtual string Deposit(decimal amount)//this method will allow to access the balance to other classes, but here all the rules will be there and we need to follow those
        {
            if(amount <= 0)
                return "You can't deposit $"+amount;
            if (amount > 20000)
                return "Limit reached";

            Balance += amount;
            return "Deposit completed successfully";

        }

        public string Withdraw(decimal amount)//this method will allow to access the balance to other classes, but here all the rules will be there and we need to follow those
        {
            if (amount <= 0)
                return "You can't withdraw $" + amount;
            if (amount > Balance)
                return "Not have enough balance";

            Balance -= amount;
            return "Withdraw completed successfully";

        }
    }
}
