namespace BankAccountsApp
{
    public partial class Form1 : Form
    {
        public Form1() //Constructor 
        {
            InitializeComponent();

            BankAccount bankAccount = new BankAccount();
            bankAccount.Owner = "BMW";
            bankAccount.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
            bankAccount.Balance = 50000;

            BankAccount bankAccount2 = new BankAccount();
            bankAccount2.Owner = "Audi";
            bankAccount2.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
            bankAccount2.Balance = 999;

            BankAccount bankAccount3 = new BankAccount();
            bankAccount3.Owner = "Crown";
            bankAccount3.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
            bankAccount3.Balance = 600;

            List<BankAccount> bankAccounts = new List<BankAccount>();
            bankAccounts.Add(bankAccount);
            bankAccounts.Add(bankAccount2);
            bankAccounts.Add(bankAccount3);

            BankAccountsGrid.DataSource = bankAccounts; //Data source will be the bank account


        }


    }
}
