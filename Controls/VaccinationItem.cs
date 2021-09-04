using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBarcode.Controls
{
    public class VaccinationItem : HCBaseItem
    {
        public string IdentyfikatorCertyfikatuZdrowia { get; set; }
        public string KrajSzczepienia { get; set; }
        public int NumerDawki { get; set; }
        public string DataSzczepienia { get; set;  }

        public string WystawcaCertyfikatuZdrowia { get; set; }

        public string Producent { get; set; }
        public string WyrobMedyczny { get; set; }

        public int WymaganaLiczbaDawek { get; set; }

        public string Choroba { get; set; }
        public string Szczepionka { get; set; }
    }
}
