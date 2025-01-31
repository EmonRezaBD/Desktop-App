namespace BankAccountsApp
{
    public partial class Form1 : Form
    {
        List<BankAccount> bankAccounts = new List<BankAccount>();//declaring here means it is global

        public Form1() //Constructor 
        {
            InitializeComponent();

            //BankAccount bankAccount = new BankAccount("BMW");
            /*  bankAccount.Owner = "BMW";
              bankAccount.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
              bankAccount.Balance = 50000;*/

           // BankAccount bankAccount2 = new BankAccount("Audi");
            /*bankAccount2.Owner = "Audi";
            bankAccount2.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
            bankAccount2.Balance = 999;*/

           // BankAccount bankAccount3 = new BankAccount("Crown");
            /* bankAccount3.Owner = "Crown";
             bankAccount3.AccountNumber = Guid.NewGuid(); //it'll assign a unique identifier
             bankAccount3.Balance = 600;
 */
            //bankAccounts.Add(bankAccount);
            //bankAccounts.Add(bankAccount2);
            //bankAccounts.Add(bankAccount3);

           // BankAccountsGrid.DataSource = bankAccounts; //Data source will be the bank account


        }

        private void CreateAccountBtn_Click(object sender, EventArgs e)
        {

            if(string.IsNullOrEmpty(OwnerTxt.Text))
            {
                return;
            }
            //MessageBox.Show("Hello World");
            BankAccount bankAccount = new BankAccount(OwnerTxt.Text);
            bankAccounts.Add(bankAccount);

            BankAccountsGrid.DataSource=null;
            BankAccountsGrid.DataSource=bankAccounts;

            RefreshGrid();
            Owner.Text = string.Empty;
        }

        private void RefreshGrid()
        { 
            BankAccountsGrid.DataSource = null;
            BankAccountsGrid.DataSource = bankAccounts;
        }

    }
}
