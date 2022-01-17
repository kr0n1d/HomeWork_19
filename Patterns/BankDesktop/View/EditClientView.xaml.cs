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
    /// Логика взаимодействия для EditClientView.xaml
    /// </summary>
    public partial class EditClientView : Window
    {
        public EditClientView(Client client)
        {
            InitializeComponent();
            EditClientViewModel viewModel = new EditClientViewModel(client);
            DataContext = viewModel;
            viewModel.OnClose = o =>  this.Close();
        }

        private void WindowEditClient_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
