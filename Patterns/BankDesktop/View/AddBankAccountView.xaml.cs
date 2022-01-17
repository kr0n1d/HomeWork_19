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
using BankLibrary.Clients;

namespace BankDesktop.View
{
    /// <summary>
    /// Логика взаимодействия для AddBankAccountView.xaml
    /// </summary>
    public partial class AddBankAccountView : Window
    {
        public AddBankAccountView(Client client)
        {
            InitializeComponent();
            AddBankAccountViewModel viewModel = new AddBankAccountViewModel(client);
            viewModel.OnClose = o => Close();
            DataContext = viewModel;
        }

        private void WindowAddBankAccount_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
