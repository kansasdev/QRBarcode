using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBarcode.Controls
{
    public class RecoveryItem : HCBaseItem
    {
        public string IdentyfikatorCertyfikatuZdrowia { get; set; }
        public string KrajTestu { get; set; }

        public string WaznyOd { get; set; }
        public string WaznyDo { get; set; }

        public string DataPierwszegoTestuPozytywnego { get; set; }

        public string Wystawca { get; set; }

        public string Choroba { get; set; }

    }
}
