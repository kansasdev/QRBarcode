using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace QRBarcode.Controls
{
    public sealed partial class Ustawienia : UserControl
    {

        private Scan s;

        public Ustawienia()
        {
            this.InitializeComponent();
            s = (Scan)App.Kontrolki[1];
        }

        public async Task SetContent()
        {
            cbScanners.Items.Clear();
            int index = 0;
            string selector = BarcodeScanner.GetDeviceSelector(PosConnectionTypes.Local);
            App.DIC = await DeviceInformation.FindAllAsync(selector);

            App.DICCameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if(App.DIC!=null && App.DIC.Count>0)
            {
                foreach(DeviceInformation di in App.DIC)
                {
                    cbScanners.Items.Add(di.Name);
                    if(s!=null && !string.IsNullOrEmpty(App.DeviceId))
                    {
                        List<int> lstPos = AllIndexesOf(di.Id, "#");
                        string idCleanded = di.Id;
                        if(lstPos.Count()==2)
                        {
                            idCleanded = idCleanded.Substring(0, lstPos[1]);
                        }

                        List<int> lstPosScanner = AllIndexesOf(App.DeviceId, "#");
                        string idCleanedScanner = App.DeviceId;
                        if(lstPosScanner.Count()==2)
                        {
                            idCleanedScanner = idCleanedScanner.Substring(0, lstPosScanner[1]);
                        }

                        if(idCleanedScanner==idCleanded)
                        {
                            cbScanners.SelectedItem = di.Name;
                            cbScanners.SelectedIndex = index;
                        }
                    }
                    index++;
                }
                                
            }
            if(s!=null)
            {
                cbxHCDecode.IsChecked = s.DecodeHC;
            }
            
            await Task.CompletedTask;

        }

        private List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private void cbScanners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(App.DIC!=null && App.DIC.Count>1&&App.DICCameras!=null&&App.DICCameras.Count>=1)
            {
                if(s!=null && e.AddedItems!=null && e.AddedItems.Count()>=1)
                {
                    App.DeviceId = App.DIC[cbScanners.SelectedIndex].Id;
                    if(App.DICCameras.Count()==App.DIC.Count())
                    {
                        App.VideoDeviceId = App.DICCameras[cbScanners.SelectedIndex].Id;
                    }
                }
            }
            if(s!=null)
            {
                s.BackButtonFromUC = false;
            }
        }

        private void cbxHCDecode_Checked(object sender, RoutedEventArgs e)
        {
            if(s!=null)
            {
                s.DecodeHC = true;
            }
        }

        private void cbxHCDecode_Unchecked(object sender, RoutedEventArgs e)
        {
            if(s!=null)
            {
                s.DecodeHC = false;
            }
        }

        private void cbxSeparateWindow_Checked(object sender, RoutedEventArgs e)
        {
            if(s!=null)
            {
                s.SeparateWindowView = true;
            }    
        }

        private void cbxSeparateWindow_Unchecked(object sender, RoutedEventArgs e)
        {
            if(s!=null)
            {
                s.SeparateWindowView = false;
            }    
        }
    }
}
