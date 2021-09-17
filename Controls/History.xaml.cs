using DCC;
using QRBarcode.DigitalKeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
            ObservableCollection<HCBaseItem> lst = null;
            if (lstHCEntries.ItemsSource == null)
            {
                lst = new ObservableCollection<HCBaseItem>();
            }
            else
            {
                lst = (ObservableCollection<HCBaseItem>)lstHCEntries.ItemsSource;
            }

            if (hc is VaccinationItem)
            {
                VaccinationItem vi = (VaccinationItem)hc;
                lst.Add(vi);
            }
            else if (hc is TestItem)
            {
                TestItem ti = (TestItem)hc;
                lst.Add(ti);
            }
            else if (hc is RecoveryItem)
            {
                RecoveryItem ri = (RecoveryItem)hc;
                lst.Add(ri);
            }
            
            lstHCEntries.ItemsSource = lst.Reverse();

        }

        private async void S_QRCodeArrived(string obj)
        {
            if (s != null && _gcd != null)
            {
                if (s.DecodeHC)
                {
                    try
                    {
                        CWT hCert = _gcd.Decode(obj);
                        if (hCert != null)
                        {
                            if (hCert.DGCv1 != null)
                            {
                                VaccinationItem vi = null;
                                TestItem ti = null;
                                RecoveryItem ri = null;
                                if (hCert.DGCv1.Vaccination != null && hCert.DGCv1.Vaccination.Length >= 1)
                                {
                                    VaccinationEntry ve = hCert.DGCv1.Vaccination.LastOrDefault();
                                    if (ve != null)
                                    {
                                        vi = new VaccinationItem();
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


                                    }
                                }
                                else if (hCert.DGCv1.Test != null && hCert.DGCv1.Test.Length >= 1)
                                {
                                    TestEntry te = hCert.DGCv1.Test.LastOrDefault();
                                    if (te != null)
                                    {
                                        ti = new TestItem();
                                        ti.Nazwisko = hCert.DGCv1.Name.SurnameName;
                                        ti.NazwiskoPaszport = hCert.DGCv1.Name.SurameTransliterated;
                                        ti.Imie = hCert.DGCv1.Name.GivenName;
                                        ti.ImiePaszport = hCert.DGCv1.Name.GivenNameTraslitaerated;
                                        ti.DataUrodzenia = hCert.DGCv1.DateOfBirthString;

                                        ti.CentrumTestowe = te.TestingCenter;
                                        ti.Choroba = te.Disease;
                                        ti.DataTestu = te.SampleTakenDate.ToString();
                                        ti.IdentyfikatorCertyfikatuZdrowia = te.CertificateIdentifier;
                                        ti.KrajTestu = te.CountryOfTest;
                                        ti.RodzajTestu = te.TestType;
                                        ti.WynikTestu = te.TestResult;
                                    }


                                }
                                else if (hCert.DGCv1.Recovery != null && hCert.DGCv1.Recovery.Length >= 1)
                                {
                                    RecoveryElement re = hCert.DGCv1.Recovery.LastOrDefault();
                                    if (re != null)
                                    {
                                        ri = new RecoveryItem();
                                        ri.Nazwisko = hCert.DGCv1.Name.SurnameName;
                                        ri.NazwiskoPaszport = hCert.DGCv1.Name.SurameTransliterated;
                                        ri.Imie = hCert.DGCv1.Name.GivenName;
                                        ri.ImiePaszport = hCert.DGCv1.Name.GivenNameTraslitaerated;
                                        ri.DataUrodzenia = hCert.DGCv1.DateOfBirthString;

                                        ri.Choroba = re.Disease;
                                        ri.DataPierwszegoTestuPozytywnego = re.FirstPositiveTestResult.ToString("yyyy-MM-dd");
                                        ri.IdentyfikatorCertyfikatuZdrowia = re.CertificateIdentifier;
                                        ri.KrajTestu = re.CountryOfTest;
                                        ri.WaznyDo = re.ValidUntil.ToString("yyyy-MM-dd");
                                        ri.WaznyOd = re.ValidFrom.ToString("yyyy-MM-dd");
                                        ri.Wystawca = re.Issuer;

                                    }
                                }
                                if (vi != null)
                                {
                                    if (App.CertsForVerification != null && App.CertsForVerification.Count >= 1)
                                    {

                                        CertJson cj = App.CertsForVerification.Where(q => q.KID == hCert.CoseMessage.KID).FirstOrDefault();
                                        if (cj != null)
                                        {

                                            vi.CzyWydanyPrzezZaufanegoWystawce = hCert.CoseMessage.VerifySignature(cj.PubKey);
                                            if ((bool)vi.CzyWydanyPrzezZaufanegoWystawce)
                                            {
                                                vi.Kolor = new SolidColorBrush(Colors.Green);
                                            }
                                            else
                                            {
                                                vi.Kolor = new SolidColorBrush(Colors.Red);
                                            }
                                            vi.UrzadOdpowiedzialny = cj.Subject;
                                        }
                                        else
                                        {
                                            vi.Kolor = new SolidColorBrush(Colors.Yellow);
                                        }
                                    }
                                    else
                                    {
                                        vi.CzyWydanyPrzezZaufanegoWystawce = null;
                                        vi.Kolor = new SolidColorBrush(Colors.Yellow);
                                    }

                                    AddItemsToListViewSource(vi);
                                }
                                else if(ti!=null)
                                {
                                    if (App.CertsForVerification != null && App.CertsForVerification.Count >= 1)
                                    {

                                        CertJson cj = App.CertsForVerification.Where(q => q.KID == hCert.CoseMessage.KID).FirstOrDefault();
                                        if (cj != null)
                                        {

                                            ti.CzyWydanyPrzezZaufanegoWystawce = hCert.CoseMessage.VerifySignature(cj.PubKey);
                                            if ((bool)ti.CzyWydanyPrzezZaufanegoWystawce)
                                            {
                                                ti.Kolor = new SolidColorBrush(Colors.Green);
                                            }
                                            else
                                            {
                                                ti.Kolor = new SolidColorBrush(Colors.Red);
                                            }
                                            ti.UrzadOdpowiedzialny = cj.Subject;
                                        }
                                        else
                                        {
                                            ti.Kolor = new SolidColorBrush(Colors.Yellow);
                                        }
                                    }
                                    else
                                    {
                                        ti.CzyWydanyPrzezZaufanegoWystawce = null;
                                        ti.Kolor = new SolidColorBrush(Colors.Yellow);
                                    }

                                    AddItemsToListViewSource(ti);
                                }
                                else if(ri!=null)
                                {
                                    if (App.CertsForVerification != null && App.CertsForVerification.Count >= 1)
                                    {

                                        CertJson cj = App.CertsForVerification.Where(q => q.KID == hCert.CoseMessage.KID).FirstOrDefault();
                                        if (cj != null)
                                        {

                                            ri.CzyWydanyPrzezZaufanegoWystawce = hCert.CoseMessage.VerifySignature(cj.PubKey);
                                            if ((bool)ri.CzyWydanyPrzezZaufanegoWystawce)
                                            {
                                                ri.Kolor = new SolidColorBrush(Colors.Green);
                                            }
                                            else
                                            {
                                                ri.Kolor = new SolidColorBrush(Colors.Red);
                                            }
                                            ri.UrzadOdpowiedzialny = cj.Subject;
                                        }
                                        else
                                        {
                                            ri.Kolor = new SolidColorBrush(Colors.Yellow);
                                        }
                                    }
                                    else
                                    {
                                        ri.CzyWydanyPrzezZaufanegoWystawce = null;
                                        ri.Kolor = new SolidColorBrush(Colors.Yellow);
                                    }

                                    AddItemsToListViewSource(ri);
                                }
                            }
                        }

                        await Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        MessageDialog md = new MessageDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Error") + ex.Message);
                        await md.ShowAsync();

                    }
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (s != null)
            {
                s.BackButtonFromUC = true;

                _navi.SelectedItem = s;
            }
        }

        private void czyscButton_Click(object sender, RoutedEventArgs e)
        {
            if(lstHCEntries.ItemsSource!=null)
            {
                lstHCEntries.ItemsSource = null;
            }
            if(s!=null)
            {
                s.CleanRawCode();
            }
        }
    }
}
