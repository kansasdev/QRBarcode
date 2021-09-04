using DCC;
using QRBarcode.DigitalKeys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class History : UserControl
    {
        private Microsoft.UI.Xaml.Controls.NavigationView _navi;
        private Scan s;
        private GreenCertificateDecoder _gcd;
        public History(Microsoft.UI.Xaml.Controls.NavigationView navi)
        {
            this.InitializeComponent();
            _navi = navi;
            s = (Scan)App.Kontrolki[1];
            s.QRCodeArrived += S_QRCodeArrived;
            _gcd = new GreenCertificateDecoder();
        }

        private void AddItemsToListViewSource(HCBaseItem hc)
        {
            List<HCBaseItem> lst = null;
            if (lstHCEntries.ItemsSource == null)
            {
                lst = new List<HCBaseItem>();
            }
            else
            {
                lst = (List<HCBaseItem>)lstHCEntries.ItemsSource;
            }

            if(hc is VaccinationItem)
            {
                VaccinationItem vi = (VaccinationItem)hc;
                lst.Add(vi);    
            }

            lstHCEntries.ItemsSource = lst;

        }

        private async void S_QRCodeArrived(string obj)
        {
            if(s!=null && _gcd!=null)
            {
                if(s.DecodeHC)
                {
                    try
                    {
                        CWT hCert = _gcd.Decode(obj);
                        if(hCert!=null)
                        {
                            if(hCert.DGCv1!=null)
                            {
                                if(hCert.DGCv1.Vaccination!=null && hCert.DGCv1.Vaccination.Length>=1)
                                {
                                    VaccinationEntry ve = hCert.DGCv1.Vaccination.LastOrDefault();
                                    if(ve!=null)
                                    {
                                        VaccinationItem vi = new VaccinationItem();
                                        vi.Nazwisko = hCert.DGCv1.Name.SurnameName;
                                        vi.NazwiskoPaszport = hCert.DGCv1.Name.SurameTransliterated;
                                        vi.Imie = hCert.DGCv1.Name.GivenName;
                                        vi.ImiePaszport = hCert.DGCv1.Name.GivenNameTraslitaerated;
                                        vi.DataUrodzenia = hCert.DGCv1.DateOfBirthString;

                                        vi.Choroba = ve.Disease;
                                        vi.DataSzczepienia = ve.VaccinationDate.ToString();
                                        vi.IdentyfikatorCertyfikatuZdrowia = ve.CertificateIdentifier;
                                        vi.NumerDawki = (int)ve.DoseNumber;
                                        vi.Producent = ve.Manufacturer;
                                        vi.Szczepionka = ve.Vaccine;
                                        vi.WymaganaLiczbaDawek = (int)ve.TotalDoses;
                                        vi.WyrobMedyczny = ve.MedicalProduct;
                                        vi.WystawcaCertyfikatuZdrowia = ve.Issuer;
                                        vi.KrajSzczepienia = ve.CountryOfVaccination;

                                        if(App.CertsForVerification!=null && App.CertsForVerification.Count>=1)
                                        {
                                            CertJson cj = App.CertsForVerification.Where(q => q.KID == hCert.CoseMessage.KID).FirstOrDefault();
                                            if(cj!=null)
                                            {                                                
                                                vi.CzyWydanyPrzezZaufanegoWystawce = hCert.CoseMessage.VerifySignature(cj.PubKey);

                                            }
                                        }
                                        else
                                        {
                                            vi.CzyWydanyPrzezZaufanegoWystawce = null;
                                        }

                                        AddItemsToListViewSource(vi);
                                    }

                                }
                            }
                        }

                        await Task.CompletedTask;
                    }
                    catch(Exception ex)
                    {
                        MessageDialog md = new MessageDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Error") + ex.Message);
                        await md.ShowAsync();
                        
                    }
                }
            }
        }

        private async void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (s != null)
            {
                s.BackButtonFromUC = true;
                _navi.SelectedItem = s;
            }
        }
    }
}
