using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QRBarcode.Controls
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TestDataTemplate { get; set; }
        public DataTemplate RecoveryDataTemplate { get; set; }
        public DataTemplate VaccinationDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is RecoveryItem)
                return RecoveryDataTemplate;
            if (item is VaccinationItem)
                return VaccinationDataTemplate;
            if (item is TestItem)
                return TestDataTemplate;

            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

    }
}
