using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace QRBarcode.Controls
{
    public class HCBaseItem
    {
        public string Nazwisko { get; set; }
        public string Imie { get; set; }
        public string NazwiskoPaszport { get; set; }

        public string ImiePaszport { get; set; }

        public string DataUrodzenia { get; set; }

        public bool? CzyWydanyPrzezZaufanegoWystawce { get; set; }

        public string UrzadOdpowiedzialny { get; set; }

        public SolidColorBrush Kolor { get; set; }
    }
}
