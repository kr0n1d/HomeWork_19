using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для PutBankAccountView.xaml
    /// </summary>
    public partial class PutBankAccountView : Window
    {
        public PutBankAccountView(Client client, BankAccount bankAccount)
        {
            InitializeComponent();
            PutBankAccountViewModel vm = new PutBankAccountViewModel(client, bankAccount);
            this.DataContext = vm;
            vm.OnClose = o => this.Close();
        }

        private void WindowPutClient_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
