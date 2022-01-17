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
using BankLibrary.Clients;

namespace BankDesktop.View
{
    /// <summary>
    /// Логика взаимодействия для AddBankCreditView.xaml
    /// </summary>
    public partial class AddBankCreditView : Window
    {
        public AddBankCreditView(Client client)
        {
            InitializeComponent();
            AddBankCreditViewModel vm = new AddBankCreditViewModel(client);
            this.DataContext = vm;
            vm.OnClose = o => this.Close();
        }

        private void WindowAddBankCredit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
