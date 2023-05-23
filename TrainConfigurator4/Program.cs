using System.Diagnostics;
using System.Globalization;

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
        private Depot _depot = new Depot();

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
                        _depot.CreateTrain();
                        break;

                    case MenuShowTrainParam:
                        _terminal.ShowTrain();
                        break;

                    case MenuSendTrain:
                        _terminal.SendTrain();
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

    class Terminal //создаёт направление и пассажиров, продаёт билеты. И формирует поезд из вагонов, созданых в депо. (Добавить метод отправки поезда)
    {
        private Random _rand = new Random();
        private Depot _depot = new Depot();

        public string Direction { get; private set; } = "Не задано";
        public int Tickets { get; private set; } = 0;
        public bool IsTrainFormed { get; private set; } = false;

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

        public Train CreateTrain()
        {
            List<Wagon> wagons = _depot.CreateWagons();

            return new Train(wagons);
        }
    }

    class Depot //Добавить метод рассаживания пассажиров и расчёта нужного количества вагонов и сборки поезда.
    {
        private Database _database;

        public Depot()
        {
            _database = new Database();
        }

        public Wagon CreateWagonA(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        public Wagon CreateWagonB(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        public Wagon CreateWagonC(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        public Wagon CreateWagonD(int seats, string type)
        {
            return new Wagon(seats, type);
        }

        public List<Wagon> CreateWagons() // Получаем лист с вагонами всех типов (для теста)
        {
            return new List<Wagon>()
            {
                CreateWagonA(_database.GetWagonSize(WagonTypes.TypeA), _database.GetWagonType(WagonTypes.TypeA)),
                CreateWagonB(_database.GetWagonSize(WagonTypes.TypeB), _database.GetWagonType(WagonTypes.TypeB)),
                CreateWagonC(_database.GetWagonSize(WagonTypes.TypeC), _database.GetWagonType(WagonTypes.TypeC)),
                CreateWagonD(_database.GetWagonSize(WagonTypes.TypeD), _database.GetWagonType(WagonTypes.TypeD)),
            };
        }
    }

    public class Train //Чекнуть что тут вообще такое
    {
        private List<Wagon> _wagons;

        public int Seats => _wagons.Sum(wagon => wagon.Seats); //Вычисляет суммарное количество мест во всех вагонах в листе

        public Train(List<Wagon> wagons)
        {
            _wagons = wagons;
        }

        public void ShowTrain()
        {
            Console.WriteLine("Состав сформирован следующим образом:");

            foreach (Wagon wagon in _wagons)
            {
                _wagon.ShowParameters(wagon);
                checkTotalPax += wagon.Pax;
            }

            Console.WriteLine($"Всего пассажиров в поезде - {checkTotalPax}");

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