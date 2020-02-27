using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;

namespace ExRatesLib
{
    public class GetFiles
    {
        private DateTime _startDT;
        private DateTime _endDT;
        private List<string> _urls;
        private List<string> _xmls;
        public List<pozycja> _er;

        public GetFiles(string currencyCode, DateTime _startDT, DateTime _endDT)
        {
            if (_startDT.Year >= 2002) this._startDT = _startDT;
            else this._startDT = new DateTime(2002, 1, 1);

            if (_endDT <= DateTime.Now) this._endDT = _endDT;
            else this._endDT = DateTime.Now;

            _urls = Get_urls();
            _xmls = GetXML();
            _er = Deserializable(currencyCode);

        }

        private List<string> Get_urls()
        {
            List<string> _urls = new List<string>();

            WebClient webClient = new WebClient();
            Stream stream;
            StreamReader streamReader;

            foreach (var year in Enumerable.Range(_startDT.Year, (_endDT.Year - _startDT.Year) + 1))
            {
                string yearStr = "";
                if (year != DateTime.Now.Year) yearStr = $"{year}";
                stream = webClient.OpenRead($"https://www.nbp.pl/kursy/xml/dir{yearStr}.txt");
                streamReader = new StreamReader(stream);
                _urls.AddRange(streamReader.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList());
            }

            _urls = _urls.Where(x => Regex.Match(x, @"c.*").Success).ToList();

            for (int i = 0; i < _urls.Count; i++) _urls[i] = $"http://www.nbp.pl/kursy/xml/{_urls[i]}.xml";

            return _urls;
        }

        private List<string> GetXML()
        {
            List<string> _xmls = new List<string>();

            WebClient webClient = new WebClient();
            Stream stream;
            StreamReader streamReader;


            foreach (var xml in _urls)
            {
                stream = webClient.OpenRead($"{xml}");
                streamReader = new StreamReader(stream);
                _xmls.Add(streamReader.ReadToEnd());
            }

            return _xmls;
        }

        private List<pozycja> Deserializable(string currencyCode)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(tabela_kursow));
            List<pozycja> _er = new List<pozycja>();

            foreach (var xml in _xmls)
            {
                using var textReader = new StringReader(xml);
                var item = (tabela_kursow)serializer.Deserialize(textReader);

                foreach (var position in item.pozycjaList)
                    if (position.kod_waluty == currencyCode)
                    {
                        position.data_notowania = item.data_notowania;
                        _er.Add(position);
                        break;
                    }
            }
            return _er;
        }

        public decimal AverageBuyCost()
        {
            decimal sum = 0m;
            foreach (var cost in _er) sum += decimal.Parse(cost.kurs_kupna);
            if (_er.Count != 0) return sum / _er.Count();
            else return 0;
        }

        public decimal AverageSellCost()
        {
            decimal sum = 0m;
            foreach (var cost in _er) sum += decimal.Parse(cost.kurs_sprzedazy);
            if (_er.Count != 0) return sum / _er.Count();
            else return 0;
        }

        public double StandardDeviationBuy()
        {
            double average = (double)AverageBuyCost();
            double sumOfSquare = 0;
            foreach (var cost in _er) sumOfSquare += Math.Pow(double.Parse(cost.kurs_kupna) - average, 2.0);
            if (_er.Count - 1 != 0) return (Math.Sqrt(sumOfSquare / (_er.Count - 1)));
            else return 0;
        }

        public double StandardDeviationSell()
        {
            double average = (double)AverageSellCost();
            double sumOfSquare = 0;
            foreach (var cost in _er) sumOfSquare += Math.Pow(double.Parse(cost.kurs_sprzedazy) - average, 2d);
            if (_er.Count - 1 != 0) return (Math.Sqrt(sumOfSquare / (_er.Count - 1)));
            else return 0;
        }

        public decimal MinBuy()
        {
            if (_er.Count != 0) return _er.Min(x => decimal.Parse(x.kurs_kupna));
            else return 0;
        }
        public decimal MinSell()
        {
            if (_er.Count != 0) return _er.Min(x => decimal.Parse(x.kurs_sprzedazy));
            else return 0;
        }
        public decimal MaxBuy()
        {
            if (_er.Count != 0) return _er.Max(x => decimal.Parse(x.kurs_kupna));
            else return 0;
        }
        public decimal MaxSell()
        {
            if (_er.Count != 0) return _er.Max(x => decimal.Parse(x.kurs_sprzedazy));
            else return 0;
        }

    }

    public class tabela_kursow
    {
        public string link;
        public string style;
        public string numer_tabeli;
        public DateTime data_notowania;
        public string data_publikacji;
        [XmlElement("pozycja")]
        public List<pozycja> pozycjaList = new List<pozycja>();
    }

    public class pozycja
    {
        public string nazwa_kraju { get; set; }
        public int symbol_waluty { get; set; }
        public int przelicznik { get; set; }
        public string kod_waluty { get; set; }
        public string kurs_kupna { get; set; }
        public string kurs_sprzedazy { get; set; }
        public DateTime data_notowania { get; set; }
    }
}
