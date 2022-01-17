using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BankDesktop.ViewModel;
using BankLibrary;
using BankLibrary.Clients;

namespace BankDesktop.View
{
    /// <summary>
    /// Логика взаимодействия для TransferToBankAccountView.xaml
    /// </summary>
    public partial class TransferToBankAccountView : Window
    {
        public TransferToBankAccountView(Client client, BankAccount currentBankAccount)
        {
            InitializeComponent();
            TransferToBankAccountViewModel vm = new TransferToBankAccountViewModel(client, currentBankAccount);
            this.DataContext = vm;
            vm.OnClose = o => this.Close();
        }

        private void WindowTransferToBankAccount_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
