using ExRatesLib;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExchangeRates_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            DateTime sDate = new DateTime();
            DateTime eDate = new DateTime();
            try 
            {
                sDate = DateTime.ParseExact(startDateTextBox.Text, "dd-mm-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
               eDate = DateTime.ParseExact(endDateTextBox.Text, "dd-mm-yyyy",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(Exception)
            {
                MessageBox.Show("Invalid Date Format");
            }
            string currency = currencyTextBox.Text;

            GetFiles gf = new GetFiles(currency, sDate, eDate);

            //print additional results
            odchyleniekupno.Content = $"Odchylenie standardowe Kupna: {gf.StandardDeviationBuy().ToString("0.00")}";
            odchylenieSprzedazy.Content = $"Odchylenie standardowe  Sprzedaży: {gf.StandardDeviationSell().ToString("0.00")}";
            avBuy.Content = $"Średnia cena kupna: {gf.AverageBuyCost().ToString("0.00")}";
            avSell.Content = $"Średnia Cena Sprzedaży: {gf.AverageSellCost().ToString("0.00")}";
            MinBuy.Content = $"Minimalna Cena Kupna: {gf.MinBuy().ToString("0.00")}";
            MinSell.Content = $"Minimalna Cena Sprzedaży: {gf.MinSell().ToString("0.00")}";
            MaxBuy.Content = $"Maksymalna Cena Kupna: {gf.MaxBuy().ToString("0.00")}";
            MaxSell.Content = $"Maksymalna Cena Sprzedaży: {gf.MaxSell().ToString("0.00")}";

            //print list of result
            dt.Columns.Add("DATA_NOTOWANIA", typeof(string));
            dt.Columns.Add("NAZWA_KRAJU", typeof(string));
            dt.Columns.Add("WALUTA", typeof(string));
            dt.Columns.Add("KUPNO", typeof(string));
            dt.Columns.Add("SPRZEDAŻ", typeof(string));

            foreach (var er in gf._er)
                dt.Rows.Add(new string[] { er.data_notowania.ToString(), er.nazwa_kraju, er.przelicznik.ToString(), er.kurs_kupna, er.kurs_sprzedazy });

            dataGrid.ItemsSource = dt.DefaultView;
        }
    }
}
