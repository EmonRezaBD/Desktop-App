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
        public decimal Balance { get; set; }
    }
}
