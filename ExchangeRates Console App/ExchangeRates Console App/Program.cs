using ExRatesLib;
using System;
using System.Text.RegularExpressions;

namespace ExchangeRates_Console_App
{
    class Program
    {
        public static DateTime _Startdt;
        public static DateTime _Enddt;
        static void Main(string[] args)
        {
            Console.Title = "ExchangeRates";
            Menu(args);
            var gf = new GetFiles(args[0], _Startdt, _Enddt);
            Display(gf);
            Console.ReadKey();
        }

        static void Menu(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Prawidłowy format zapytania to :");
                Console.WriteLine("nazwa_programu.exe kod_waluty(USD, EUR, CHF, GBP) data_poczatkowa(dd-mm-yyyy) data_koncowa(dd-mm-yyyy)");
                Console.ReadKey();
                Menu(args);
            }

            string waluta = "";
            switch (args[0])
            {
                case "USD":
                case "EUR":
                case "CHF":
                case "GBP":
                    waluta = args[0];
                    break;
                default:
                    Console.WriteLine("Wybrano złą walutę!");
                    Console.ReadKey();
                    Menu(args);
                    break;
            }
            try
            {
                if (Regex.IsMatch(args[1], @"[0-3]\d-[0-1]\d-20\d\d") && Regex.IsMatch(args[2], @"[0-3]\d-[0-1]\d-20\d\d"))
                {
                    _Startdt = DateTime.ParseExact(args[1], "dd-mm-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);

                    _Enddt = DateTime.ParseExact(args[2], "dd-mm-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);
                }
                else throw new ArgumentOutOfRangeException();
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Wybrano zły format daty!");
                Console.ReadKey();
                Menu(args);
            }
        }

        public static void Display(GetFiles gf)
        {

            foreach (var er in gf._er)
                Console.WriteLine($"Data notowania: {er.data_notowania}; Nazwa kraju: {er.nazwa_kraju}; Waluta: {er.kod_waluty}; Kupno: {er.kurs_kupna}; Sprzedaż: {er.kurs_sprzedazy}");

            Console.WriteLine($"Odchylenie standardowe: {gf.StandardDeviationBuy().ToString("0.00")} / {gf.StandardDeviationSell().ToString("0.00")}");
            Console.WriteLine($"Średnia: {gf.AverageBuyCost().ToString("0.00")} / {gf.AverageSellCost().ToString("0.00")}");
            Console.WriteLine($"Minimum: {gf.MinBuy().ToString("0.00")} / {gf.MinSell().ToString("0.00")}");
            Console.WriteLine($"Maximum: {gf.MaxBuy().ToString("0.00")} / {gf.MaxSell().ToString("0.00")}");
        }
    }
}
