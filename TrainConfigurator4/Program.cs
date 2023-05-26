namespace TrainConfigurator
{
    class Programm
    {
        static void Main()
        {
            Menu menu = new Menu();
            menu.ShowMenu();
        }
    }

    class Menu
    {
        private Terminal _terminal = new Terminal();

        public void ShowMenu()
        {
            const string MenuCreateDirection = "1";
            const string MenuSellTickets = "2";
            const string MenuCompileTrain = "3";
            const string MenuShowTrainParam = "4";
            const string MenuSendTrain = "5";
            const string MenuExit = "0";

            bool isExit = false;

            string userInput;

            while (isExit == false)
            {
                ShowStatus();

                Console.WriteLine("\nМеню:");
                Console.WriteLine(MenuCreateDirection + " - Создать направление");
                Console.WriteLine(MenuSellTickets + " - Продать билеты");
                Console.WriteLine(MenuCompileTrain + " - Сформировать состав");
                Console.WriteLine(MenuShowTrainParam + " - Показать параметры состава");
                Console.WriteLine(MenuSendTrain + " - Отправить поезд");
                Console.WriteLine(MenuExit + " - Выход");

                userInput = Console.ReadLine();
                CleanConsoleBelowLine();

                switch (userInput)
                {
                    case MenuCreateDirection:
                        _terminal.CreateDirection();
                        break;

                    case MenuSellTickets:
                        _terminal.SellTickets();
                        break;

                    case MenuCompileTrain:
                        _terminal.CreateLocomotive();
                        break;

                    case MenuShowTrainParam:
                        _terminal.ShowLocomotive();
                        break;

                    case MenuSendTrain:
                        _terminal.SendLocomotive();
                        break;

                    case MenuExit:
                        isExit = true;
                        break;
                }
            }
        }

        private void ShowStatus()
        {
            int infoPositionY = 0;
            int infoPositionX = 0;
            string readyToGo;
            string trainStatus;

            if (_terminal.IsTrainFormed == true)
            {
                readyToGo = "ГОТОВ!";
                trainStatus = "Сформирован";
            }
            else
            {
                readyToGo = "НЕ ГОТОВ";
                trainStatus = "НЕ сформирован";
            }

            CleanConsoleString();
            Console.SetCursorPosition(infoPositionX, infoPositionY);
            Console.WriteLine($"Направление: {_terminal.Direction} | Билетов продано: {_terminal.Tickets} | Статус поезда: {trainStatus} | К отправке: {readyToGo}");
        }

        private void CleanConsoleString()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, 0);
        }

        private void CleanConsoleBelowLine()
        {
            int currentLineCursor = Console.CursorTop;

            for (int i = currentLineCursor; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLineCursor);
        }
    }

    class Terminal
    {
        private Random _rand = new Random();
        private Database _database = new Database();
        private Depot _depot = new Depot();
        private Train _train = new Train();

        public string Direction { get; private set; } = "Не задано";
        public int Tickets { get; private set; } = 0;
        public bool IsTrainFormed => _train.IsFormed;

        public void CreateDirection()
        {
            string departurePoint;
            string arriavalPoint;

            if (Tickets == 0)
            {
                Console.WriteLine("Введите пункт отправления");
                departurePoint = Console.ReadLine();

                Console.WriteLine("Введите пункт прибытия");
                arriavalPoint = Console.ReadLine();

                Direction = $"{departurePoint} - {arriavalPoint}";

                Console.WriteLine($"Направление \"{departurePoint}\" - \"{arriavalPoint}\" создано");
            }
            else
            {
                Console.WriteLine("Нельзя изменить направление, на которое уже проданы билеты");
            }
        }

        public void SellTickets()
        {
            int minPax = 200;
            int maxPax = 648;

            if (Direction != "Не задано")
            {
                if (Tickets == 0)
                {
                    Tickets = _rand.Next(minPax, maxPax);

                    Console.WriteLine("Продано билетов - " + Tickets);
                }
                else
                {
                    Console.WriteLine("Билеты уже проданы");
                }
            }
            else
            {
                Console.WriteLine("Сначала нужно создать направление");
            }
        }

        private Dictionary<WagonTypes, int> WagonsRequest()
        {
            Dictionary<WagonTypes, int> wagonsRequest = new Dictionary<WagonTypes, int>()
                {
                    { WagonTypes.TypeA, 0},
                    { WagonTypes.TypeB, 0},
                    { WagonTypes.TypeC, 0},
                    { WagonTypes.TypeD, 0}
                };

            int wagonSizeD = _database.GetWagonSize(WagonTypes.TypeD); //54
            int wagonSizeC = _database.GetWagonSize(WagonTypes.TypeC); //36
            int wagonSizeB = _database.GetWagonSize(WagonTypes.TypeB); //18
            int wagonSizeA = _database.GetWagonSize(WagonTypes.TypeA); //6
            double tempTickets = Tickets;

            if (tempTickets > 0)
            {
                if (tempTickets >= wagonSizeD)
                {
                    int CountWagonD = (int)Math.Floor(tempTickets / wagonSizeD);
                    wagonsRequest[WagonTypes.TypeD] = CountWagonD;
                    tempTickets -= CountWagonD * wagonSizeD;
                }
                if (tempTickets >= wagonSizeC)
                {
                    int CountWagonC = (int)Math.Floor(tempTickets / wagonSizeC);
                    wagonsRequest[WagonTypes.TypeC] = CountWagonC;
                    tempTickets -= CountWagonC * wagonSizeC;
                }
                if (tempTickets >= wagonSizeB)
                {
                    int CountWagonB = (int)Math.Floor(tempTickets / wagonSizeB);
                    wagonsRequest[WagonTypes.TypeB] = CountWagonB;
                    tempTickets -= CountWagonB * wagonSizeB;
                }
                if (tempTickets <= wagonSizeB - 1 && tempTickets > 0)
                {
                    int CountWagonA = (int)Math.Ceiling(tempTickets / wagonSizeA);
                    wagonsRequest[WagonTypes.TypeA] = CountWagonA;
                }
            }
            return wagonsRequest;
        }

        public void CreateLocomotive()
        {
            _train.AddWagons(_depot.CreateWagons(WagonsRequest()));
        }

        public void ShowLocomotive()
        {
            _train.ShowTrainInfo();
        }

        public void SendLocomotive()
        {
            if (_train.IsFormed == true)
            {
                Direction = "Не задано";
                Tickets = 0;
                _train.RemoveLocomotive();
                Console.WriteLine("Поезд отправлен");
            }
            else
            {
                Console.WriteLine("Поезд ещё не сформирован");
            }
        }
    }

    class Depot
    {
        private Database _database;

        public Depot()
        {
            _database = new Database();
        }

        private Wagon CreateWagonA(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        private Wagon CreateWagonB(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        private Wagon CreateWagonC(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        private Wagon CreateWagonD(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        public List<Wagon> CreateWagons(Dictionary<WagonTypes, int> wagonsCount)
        {
            List<Wagon> wagons = new List<Wagon>();

            while (wagonsCount[WagonTypes.TypeD] > 0)
            {
                wagons.Add(CreateWagonD(_database.GetWagonSize(WagonTypes.TypeD), _database.GetWagonType(WagonTypes.TypeD)));
                wagonsCount[WagonTypes.TypeD]--;
            }
            while (wagonsCount[WagonTypes.TypeC] > 0)
            {
                wagons.Add(CreateWagonC(_database.GetWagonSize(WagonTypes.TypeC), _database.GetWagonType(WagonTypes.TypeC)));
                wagonsCount[WagonTypes.TypeC]--;
            }
            while (wagonsCount[WagonTypes.TypeB] > 0)
            {
                wagons.Add(CreateWagonB(_database.GetWagonSize(WagonTypes.TypeB), _database.GetWagonType(WagonTypes.TypeB)));
                wagonsCount[WagonTypes.TypeB]--;
            }
            while (wagonsCount[WagonTypes.TypeA] > 0)
            {
                wagons.Add(CreateWagonA(_database.GetWagonSize(WagonTypes.TypeA), _database.GetWagonType(WagonTypes.TypeA)));
                wagonsCount[WagonTypes.TypeA]--;
            }
            return wagons;
        }
    }

    public class Train
    {
        private List<Wagon> _locomotive = new List<Wagon>();
        public bool IsFormed => _locomotive.Count > 0;

        public int Seats => _locomotive.Sum(wagon => wagon.Seats);

        public void AddWagons(List<Wagon> wagons)
        {
            if (wagons.Count() > 0)
            {
                _locomotive = wagons;

                Console.WriteLine("Поезд сформирован и готов к отправке");
            }
            else
            {
                Console.WriteLine("Сначала нужно сформировать состав");
            }
        }

        public void RemoveLocomotive()
        {
            _locomotive.Clear();
        }

        public void ShowTrainInfo()
        {

            if (_locomotive.Count > 0)
            {
                Console.WriteLine("Состав сформирован следующим образом:");

                foreach (Wagon wagon in _locomotive)
                {
                    wagon.ShowWagonInfo();
                }

                Console.WriteLine($"Всего мест в поезде - {Seats}");
            }
            else
            {
                Console.WriteLine("Сначала нужно сформировать состав");
            }
        }
    }

    public class Wagon
    {
        private string _type;

        public Wagon(int seats, string type)
        {
            Seats = seats;
            _type = type;
        }

        public int Seats { get; }

        public void ShowWagonInfo()
        {
            Console.WriteLine($"Тип вагона - {_type}, Количество мест - {Seats}");
        }
    }

    class Database
    {
        private Dictionary<WagonTypes, int> _wagonSizes;
        private Dictionary<WagonTypes, string> _wagonTypes;

        public Database()
        {
            _wagonSizes = new Dictionary<WagonTypes, int>()
            {
                { WagonTypes.TypeA, 6},
                { WagonTypes.TypeB, 18},
                { WagonTypes.TypeC, 36},
                { WagonTypes.TypeD, 54}
            };

            _wagonTypes = new Dictionary<WagonTypes, string>()
            {
                { WagonTypes.TypeA, "Люкс"},
                { WagonTypes.TypeB, "СВ"},
                { WagonTypes.TypeC, "Купе"},
                { WagonTypes.TypeD, "Плацкарт"}
            };
        }

        public int GetWagonSize(WagonTypes size)
        {
            return _wagonSizes[size];
        }

        public string GetWagonType(WagonTypes type)
        {
            return _wagonTypes[type];
        }
    }

    public enum WagonTypes
    {
        TypeA,
        TypeB,
        TypeC,
        TypeD,
    }
}