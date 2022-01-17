using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BankDesktop.Model;
using BankDesktop.Model.EF;
using BankLibrary;
using BankLibrary.Clients;
using Newtonsoft.Json.Linq;

namespace BankDesktop.Services
{
    internal class DataProvider
    {

        private readonly string _connectionString;
        private readonly SkContext _skContext;

        private Random rnd;

        public bool FinishedGenerate { get; set; }
        public bool FinishedLoaded { get; set; }

        public DataProvider()
        {
            rnd = new Random();
            _connectionString = LoadConfigurationConnection();
            try
            {
                _skContext = new SkContext(_connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show($"Для корректного подключения, проверьте файл \"settings_connection.json\"", "Ошибка подключения");
            }
        }

        /// <summary>
        /// Загрузка конфигурации соединения с БД.
        /// </summary>
        /// <returns>Строка подключения.</returns>
        private string LoadConfigurationConnection()
        {
            var fileName = @"./settings_connection.json";
            if (!File.Exists(fileName))
            {
                var js = new JObject();
                var sett = new JObject();
                js["settings_connection"] = sett;
                sett["data_source"] = @"localhost\SQLEXPRESS";
                sett["data_base"] = "SkillboxDB";
                File.WriteAllText("settings_connection.json", js.ToString());
            }

            var settings = JObject.Parse(File.ReadAllText(fileName))["settings_connection"];
            var dataSource = settings?["data_source"]?.ToString();
            var dataBase = settings?["data_base"]?.ToString();

            return $"data source={dataSource};initial catalog={dataBase};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        }

        /// <summary>
        /// Сохранение данных в базе данных.
        /// </summary>
        public async Task SaveData()
        {
            FinishedLoaded = false;
            await CheckBankCredit();
            FinishedLoaded = true;
        }

        public async Task CheckBankCredit()
        {
            var clients = Bank.Instance.GetAllClients().ToList();
            clients.ForEach(c =>
            {
                BankCredits tmpCredit = null;
                foreach (var skBankCredit in _skContext.BankCredits)
                {
                    if (c.BankCredits.Any(bc => bc.Number.Equals(skBankCredit.number)))
                        continue;

                    tmpCredit = skBankCredit;
                }

                if (tmpCredit != null)
                    _skContext.BankCredits.Remove(tmpCredit);
            });
            await _skContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Загрузка клиентов.
        /// </summary>
        public async Task LoadClients()
        {
            await Task.Run(() =>
                MessageBox.Show("Идет загрузка клиентов с базы данных...", "Загрузка клиентов"));
            
            try
            {
                await _skContext.Clients.LoadAsync();
                await _skContext.BankAccounts.LoadAsync();
                await _skContext.BankCredits.LoadAsync();
                int startIndex = _skContext.Clients.Min(c => c.id);

                for (int i = startIndex; i <= startIndex + 100; i++)
                {
                    var bankAccounts = _skContext.BankAccounts
                                        .Where(b => b.clientId == i)
                                        .ToList();
                    var ba = new ObservableCollection<BankAccount>();
                    foreach (var item in bankAccounts)
                    {
                        if (item != null)
                        {
                            ba.Add(new BankAccount(item.number,
                                item.dateOpen,
                                item.balance,
                                item.capitalization,
                                item.numberTimesIncreased)
                            { ClientId = item.clientId });
                        }
                    }

                    var clientDb = _skContext.Clients.Single(c => c.id == i);
                    Client client = new Individual(string.Empty, new ObservableCollection<BankAccount>(), false);

                    if (clientDb.typeId == 1)
                    {
                        client = new Individual(clientDb.fullName, ba, clientDb.privileged) { Id = i };
                    }
                    else
                    {
                        client = new LegalEntity(clientDb.fullName, ba, clientDb.privileged) { Id = i };
                    }

                    var bankCredits = _skContext.BankCredits
                                        .Where(b => b.clientId == i)
                                        .ToList();
                    var bc = new ObservableCollection<BankCredit>();

                    foreach (var item in bankCredits)
                    {
                        if (item != null)
                        {
                            var bankAccount = GetBankAccount(ba, item.numberBankAccount);
                            bc.Add(new BankCredit(item.number,
                                                  item.sumCredit,
                                                  item.dateOpen,
                                                  item.creditTerm,
                                                  item.paidOut,
                                                  bankAccount)
                            { ClientId = item.clientId });
                        }
                    }

                    client.BankCredits = bc;

                    if (client.ClientType == ClientTypes.Individual)
                        Bank.Instance.Individuals.Add(client as Individual);
                    else
                        Bank.Instance.LegalEntities.Add(client as LegalEntity);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Для корректного подключения, проверьте файл \"settings_connection.json\"", "Ошибка подключения");
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.HelpLink);
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.StackTrace);
                return;
            }


            FinishedGenerate = true;
            MessageBox.Show("Загрузка клиентов с базы данных завершена.", "Загрузка клиентов");
        }

        /// <summary>
        /// Обновить данные клиента.
        /// </summary>
        /// <param name="client">Клиент.</param>
        /// <returns></returns>
        public void UpdateClient(Client client)
        {
            var skClient = _skContext.Clients.First(c => c.id == client.Id);
            skClient.privileged = client.IsVip;
            skClient.typeId = (int)client.ClientType;
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Удалить клиента.
        /// </summary>
        /// <param name="client">Текущий клиент.</param>
        /// <returns></returns>
        public void RemoveClient(Client client)
        {
            var skClient = _skContext.Clients.First(c => c.id == client.Id);
            _skContext.Clients.Remove(skClient);
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Добавить клиента.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public void AddClient(Client client)
        {
            Clients skClient = new Clients()
            {
                id = client.Id,
                fullName = client.FullName,
                typeId = (int)client.ClientType,
                privileged = client.IsVip
            };
            _skContext.Clients.Add(skClient);
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Обновить р/с.
        /// </summary>
        /// <param name="bankAccount">Расчетный счет (р/с)</param>
        /// <returns></returns>
        public void UpdateBankAccount(BankAccount bankAccount)
        {
            var skBankAccount = _skContext.BankAccounts.First(ba => ba.number == bankAccount.Number);
            skBankAccount.balance = bankAccount.Balance;
            skBankAccount.numberTimesIncreased = bankAccount.NumberTimesIncreased;
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Удалить р/с.
        /// </summary>
        /// <param name="bankAccount">Расчетный счёт (р/с)</param>
        /// <returns></returns>
        public void RemoveBankAccount(BankAccount bankAccount)
        {
            var skBankAccount = _skContext.BankAccounts.First(ba => ba.number == bankAccount.Number);
            _skContext.BankAccounts.Remove(skBankAccount);
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Добавить р/с.
        /// </summary>
        /// <param name="bankAccount">Расчетный счёт (р/с)</param>
        /// <returns></returns>
        public void AddBankAccount(BankAccount bankAccount)
        {
            var skBankAccount = new BankAccounts()
            {
                balance = bankAccount.Balance,
                capitalization = bankAccount.Capitalization,
                clientId = bankAccount.ClientId,
                dateOpen = bankAccount.DateOpen,
                number = bankAccount.Number,
                numberTimesIncreased = bankAccount.NumberTimesIncreased
            };
            _skContext.BankAccounts.Add(skBankAccount);
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Обновить кредит.
        /// </summary>
        /// <param name="bankCredit">Кредит.</param>
        /// <returns></returns>
        public void UpdateBankCredit(BankCredit bankCredit)
        {
            var skBankCredit = _skContext.BankCredits.First(bc => bc.number == bankCredit.Number);
            skBankCredit.paidOut = bankCredit.PaidOut;
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Обновить кредит.
        /// </summary>
        /// <param name="bankCredit">Кредит.</param>
        /// <returns></returns>
        public void RemoveBankCredit(BankCredit bankCredit)
        {
            var skBankCredit = _skContext.BankCredits.First(bc => bc.number == bankCredit.Number);
            _skContext.BankCredits.Remove(skBankCredit);
            _skContext.SaveChanges();
        }
        
        /// <summary>
        /// Обновить кредит.
        /// </summary>
        /// <param name="bankCredit">Кредит.</param>
        /// <returns></returns>
        public void AddBankCredit(BankCredit bankCredit)
        {
            var skBankCredit = new BankCredits()
            {
                clientId = bankCredit.ClientId,
                dateOpen = bankCredit.DateOpen,
                number = bankCredit.Number,
                paidOut = bankCredit.PaidOut,
                creditTerm = bankCredit.Term,
                numberBankAccount = bankCredit.BankAccount.Number,
                sumCredit = bankCredit.Credit
            };
            _skContext.BankCredits.Add(skBankCredit);
            _skContext.SaveChanges();
        }

        /// <summary>
        /// Генерация клиентов.
        /// </summary>
        public void GenerateClients()
        {
            MessageBox.Show("Идет генерация клиентов...", "Генерация клиентов");
            var rnd = new Random();

            for (int i = 1; i <= 1000; i++)
            {
                var name = GetRandomFullName();
                var typeInt = rnd.Next(1, 3);
                var type = typeInt == 1
                    ? ClientTypes.Individual
                    : ClientTypes.LegalEntity;
                var isVip = rnd.Next(5) == 0;
                var bankAccount = GetGenerateBankAccount();
                bankAccount.ClientId = i;

                Client client = new Individual(name, bankAccount, isVip);



                if (type == ClientTypes.Individual)
                {
                    client = new Individual(name, bankAccount, isVip);
                    Bank.Instance.Individuals.Add(client as Individual);
                }
                else
                {
                    client = new LegalEntity(name, bankAccount, isVip);
                    Bank.Instance.LegalEntities.Add(client as LegalEntity);
                }
                
                client.Id = i;

                AddClient(client);
                AddBankAccount(bankAccount);

                var typeString = client.ClientType == ClientTypes.Individual ? "физическое лицо" : "юридическое лицо";
                var msg = $"Клиент {client.FullName} зарегистрировался как {typeString} от {Bank.Instance.CurrentDate:dd.MM.yyyy}.";

                
                if (rnd.Next(5) == 0)
                {
                    var ba = GetGenerateBankAccount();
                    client.AddBankAccount(ba);
                    AddBankAccount(ba);
                }

                if (rnd.Next(3) != 2)
                {
                    var bc = GetGenerateBankCredit(client, out decimal sum);
                    client.AddBankCredit(bc);
                    //client.Put(bankAccount, sum);
                    //UpdateBankAccount(bankAccount);
                    AddBankCredit(bc);
                }

            }

            Task.Run(() => UploadDataToDb());

            FinishedGenerate = true;
        }

        private async Task UploadDataToDb()
        {
            await _skContext.SaveChangesAsync();
        }

        /// <summary>
        /// Сгенерировать клиенту кредит.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="sum">Сумма взятая в кредит.</param>
        /// <returns></returns>
        private BankCredit GetGenerateBankCredit(Client client, out decimal sum)
        {
            var ba = client.BankAccounts[0];
            sum = rnd.Next(500, (int)ba.Balance);
            var credit = sum + sum * (decimal)(client.IsVip ? 0.2 : 0.3);
            var bk = new BankCredit(credit, Bank.Instance.CurrentDate, 3, ba, sum);
            //AddBankCreditDB(bk);

            return bk;
        }

        /// <summary>
        /// Получить расчётный счёт по номеру р/с.
        /// </summary>
        /// <param name="bankAccounts">Список р/с.</param>
        /// <param name="number">Номер р/с.</param>
        /// <returns></returns>
        private BankAccount GetBankAccount(ObservableCollection<BankAccount> bankAccounts, string number)
        {
            return bankAccounts.FirstOrDefault(c => c.Number == number);
        }

        /// <summary>
        /// Сгенерировать расчётные счета.
        /// </summary>
        /// <returns></returns>
        private BankAccount GetGenerateBankAccount()
        {
            return new BankAccount(
                DateTime.Now,
                rnd.Next(1000, 10001),
                rnd.Next(2) == 0);
        }

        /// <summary>
        /// Сгенерировать полное имя.
        /// </summary>
        /// <returns>Полное имя.</returns>
        private string GetRandomFullName()
        {
            var data = @"Бичурин Алексей Платонович
Царёва Ева Якововна
Бочарова Оксана Виталиевна
Грефа Софья Филипповна
Гусева Роза Мефодиевна
Дёмшина Арина Елизаровна
Архипов Артем Ираклиевич
Цветкова Людмила Павеловна
Ямзин Леонид Филимонович
Плюхина Нина Емельяновна
Григорьева Инна Василиевна
Анисимова Полина Борисовна
Лешев Виктор Богданович
Бессуднов Станислав Евстафиевич
Топоров Анатолий Самсонович
Васильева Владлена Серафимовна
Бикулов Аскольд Капитонович
Головкина Алина Федоровна
Насонова Лада Мироновна
Островерха Ульяна Станиславовна
Шамякин Терентий Тихонович
Кидирбаева Валентина Анатолиевна
Булыгина Диана Никитевна
Беломестнов Фока Никанорович
Гайдученко Тимофей Зиновиевич
Казьмин Агафон Семенович
Соломахина Юлия Михеевна
Хорошилова Ярослава Романовна
Волынкина Валерия Леонидовна
Садовничий Алиса Петровна
Чичерин Кондратий Титович
Рыкова Зинаида Олеговна
Крутой Наталия Брониславовна
Есипов Герман Касьянович
Лачков Аркадий Назарович
Яикбаева Инга Фомевна
Семенов Иосиф Кондратиевич
Курушин Прокл Валериевич
Денисова Анна Кузьмевна
Рыжанов Богдан Моисеевич
Канадина Светлана Данииловна
Никаева Изольда Юлиевна
Кочинян Никон Феликсович
Бурмакина Элеонора Георгиевна
Висенина Ульяна Владиленовна
Валиев Вениамин Яковович
Ярилов Зиновий Епифанович
Гибазов Эдуард Сергеевич
Клокова Антонина Серафимовна
Волобуева Раиса Семеновна
Бабышев Гавриил Феликсович
Задков Филипп Миронович
Варфоломеева Варвара Феликсовна
Селиванов Герман Карлович
Томсин Аскольд Эрнестович
Енотова Евгения Юлиевна
Мандрыкин Владислав Богданович
Голубцов Аскольд Давидович
Рыжов Прокл Всеволодович
Кораблин Иннокентий Наумович
Черенчикова Светлана Несторовна
Арсеньева Римма Виталиевна
Громыко Лука Елизарович
Архаткин Леонид Евграфович
Дубинина Арина Леонидовна
Дуркина Надежда Фомевна
Шкиряк Аким Ипполитович
Солдатов Петр Вячеславович
Иванников Ефрем Григориевич
Липова Пелагея Казимировна
Янкин Модест Ираклиевич
Машлыкин Станислав Евгениевич
Погребной Прохор Сигизмундович
Кетов Лавр Иосифович
Степихова Мирослава Казимировна
Кучава Всеволод Касьянович
Кустов Вадим Назарович
Борзилов Макар Миронович
Блатова Светлана Олеговна
Лапотников Семён Мартьянович
Аронова Клара Никитевна
Кудяшова Розалия Никитевна
Киприянов Антип Вячеславович
Ягунова Дарья Геннадиевна
Ручкина Варвара Юлиевна
Малинина Ярослава Ростиславовна
Завражный Кондратий Эмилевич
Крымов Андрон Матвеевич
Голубов Тимур Андриянович
Клоков Нестор Кондратиевич
Гоминова Роза Евгениевна
Петухов Ефрем Савелиевич
Вьялицына Виктория Несторовна
Игнатенко Эвелина Иосифовна
Фернандес Аким Савелиевич
Блатова Эвелина Якововна
Любимцев Ярослав Мирославович
Уголева Агата Петровна
Саянов Виталий Адрианович
Якунова Зоя Леонидовна
Дорохова Агата Германовна
Журавлёв Евгений Игоревич
Цветков Игнатий Наумович
Дагина Эвелина Мироновна
Гика Алла Яновна
Дубровский Роман Александрович
Касатый Агафья Иларионовна
Березовский Артём Игнатиевич
Чекмарёв Никита Куприянович
Смотров Георгий Демьянович
Кошелева Элеонора Антониновна
Калашников Борислав Кондратиевич
Травкина Ангелина Леонидовна
Кочубей Роза Александровна
Шурдукова Антонина Родионовна
Голованова Полина Всеволодовна
Карчагина Каролина Святославовна
Золотухин Михей Гордеевич
Прокашева Анисья Павеловна
Кулешов Роман Георгиевич
Воронцов Яков Моисеевич
Рунов Марк Ульянович
Солодский Елизар Адамович
Васенин Фока Ерофеевич
Кидина Роза Данииловна
Кологреев Валерий Андреевич
Козариса Василиса Тимуровна
Тупицын Вацлав Святославович
Жариков Петр Модестович
Якубович Платон Иосифович
Нуряев Владилен Миронович
Бебчука Виктория Тимофеевна
Шамякин Гавриил Мартьянович
Елешев Аким Ираклиевич
Лагошина Каролина Яновна
Кантонистов Николай Куприянович
Мусин Михей Анатолиевич
Ямковой Анфиса Данииловна
Ажищенкова Инга Тимуровна
Окрокверцхов Иннокентий Яковович
Зууфина Ника Виталиевна
Янин Алексей Кондратович
Мацовкин Филипп Эдуардович
Камбарова Лада Марковна
Тимофеева Софья Мефодиевна
Зёмина Кристина Андрияновна
Сагунова Яна Яновна
Распутина Мария Геннадиевна
Стегнова Рада Трофимовна
Фанина Жанна Родионовна
Мосякова Инга Иосифовна
Шамякин Артемий Маркович
Драгомирова Агата Ефимовна
Ельченко Валерий Пахомович
Добролюбов Порфирий Севастьянович
Кругликова Елена Ростиславовна
Бабышев Осип Богданович
Дудника Ангелина Евгениевна
Бондарчука Агния Трофимовна
Кобелева Таисия Данииловна
Сапалёва Всеслава Игнатиевна
Дуболазов Всеволод Титович
Яшвили Агап Евграфович
Коллерова Анисья Василиевна
Палюлин Юрий Сигизмундович
Цыгвинцев Дмитрий Филимонович
Большаков Трофим Демьянович
Яндарбиева Софья Алексеевна
Валуев Лаврентий Адамович
Колотушкина Наталья Вячеславовна
Бруевича Жанна Казимировна
Масмеха Кира Несторовна
Меншикова Кира Василиевна
Шаршин Мстислав Сократович
Малафеев Харитон Кириллович
Завражина Виктория Брониславовна
Заболотный Самуил Семенович
Яскунов Фадей Макарович
Зимин Виссарион Григориевич
Крестьянинов Евгений Давидович
Земляков Клавдий Ростиславович
Соловьёв Андриян Прохорович
Якурин Илья Гаврилевич
Мещерякова Алина Кузьмевна
Катаева Бронислава Ираклиевна
Грязнов Август Матвеевич
Никулина Доминика Карповна
Марьин Геннадий Капитонович
Зюлёва Инга Игоревна
Лобана Ярослава Несторовна
Счастливцева Василиса Всеволодовна
Набоко Егор Капитонович
Аничков Всеслав Капитонович
Костюк Венедикт Святославович
Синдеева Ираида Яновна
Москвитина Ефросинья Феликсовна
Арданкина Оксана Мироновна
Казанцева Анастасия Игоревна
Якутин Виталий Брониславович
Безрукова Альбина Владленовна"
                .Split('\n');
            return data[rnd.Next(data.Length)].Trim();
        }


    }
}
