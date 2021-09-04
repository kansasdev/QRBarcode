using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
