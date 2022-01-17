using System;
using System.Security.Cryptography.X509Certificates;
using BankDesktop.Core;
using BankDesktop.Model;
using BankLibrary.Clients;

namespace BankDesktop.ViewModel
{
    public class EditClientViewModel : ObservableObject
    {
        private bool isChanged;

        #region Свойства

        private string fullname;
        public string FullName
        {
            get => fullname;
            set => Set(ref fullname, value);
        }

        private int indexClientType = 0;
        public int IndexClientType
        {
            get => indexClientType;
            set => Set(ref indexClientType, value);
        }

        private bool isVip;
        public bool IsVip
        {
            get => isVip;
            set => Set(ref isVip, value);
        }

        #endregion

        #region Команды

        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand ChangeClientCommand { get; set; }

        #endregion

        public Action<object> OnClose;

        public EditClientViewModel(Client client)
        {
            this.fullname = client.FullName;
            CloseWindowCommand = new RelayCommand(o => OnClose(null));
            ChangeClientCommand = new RelayCommand(o =>
            {
                var isChangedType = (int)client.ClientType != indexClientType;
                var isChangedVip = client.IsVip != isVip;

                client.IsVip = isVip;
                var str = string.Empty;
                if (isChangedType)
                {
                    client.ClientType = (ClientTypes)indexClientType;
                    Bank.Instance.ChangeTypeClient(client);
                    str = $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]: Клиент {fullname.Trim()} изменён.";
                    
                }
                else if (!isChangedVip)
                {
                    str = $"[{Bank.Instance.CurrentDate:dd.MM.yyyy}]: Клиент {fullname.Trim()} не изменён.";
                }

                Bank.Instance.UpdateClientDb(client);
                Bank.Instance.OnAddLog(str);

                OnClose?.Invoke(null);
            });
        }
    }
}
