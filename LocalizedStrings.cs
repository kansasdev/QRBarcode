using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace QRBarcode
{
    public class LocalizedStrings
    {
        public string this[string key]
        {
            get
            {
                ResourceLoader rl = ResourceLoader.GetForViewIndependentUse();
                return rl.GetString(key);
            }
        }
    }
}
