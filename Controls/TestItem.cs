using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBarcode.Controls
{
    public class TestItem : HCBaseItem
    {
        public string IdentyfikatorCertyfikatuZdrowia { get; set; }
        public string KrajTestu { get; set; }

        public string RodzajTestu { get; set; }

        public string DataTestu { get; set; }

        public string Choroba { get; set; }

        public string CentrumTestowe { get; set; }

        public string WynikTestu { get; set; }

    }
}
