using QRBarcode.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QRBarcode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("AppSubtitle");
                       
            
            App.Kontrolki.Add(new AboutQR());
            App.Kontrolki.Add(new Scan(menuNavigation));
            App.Kontrolki.Add(new History(menuNavigation));
            App.Kontrolki.Add(new Ustawienia());

            DataContext = App.Kontrolki;
            menuNavigation.SelectedItem = App.Kontrolki[1];

            
        }

        public async void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
                

                if(args.SelectedItem is Scan)
                {
                    Scan s = (Scan)App.Kontrolki[1];
                    
                    s.IsInitialised = false;

                    progressRing.IsActive = true;
                        await s.Initialize();
                    //pierwsze uruchomienie
                    if (args.SelectedItemContainer == null && s.BackButtonFromUC==false)
                    {
                        
                        DGCPubKeys keys = new DGCPubKeys();
                        await keys.GetCertificates();
                        if(keys.LstCerts!=null && keys.LstCerts.Count>=1)
                        {
                            App.CertsForVerification = new List<DigitalKeys.CertJson>(keys.LstCerts);
                        }
                    }

                ccControl.Content = null;
                    ccControl.Content = App.Kontrolki[1];
                    progressRing.IsActive = false;
                }
                else if(args.SelectedItem is AboutQR)
                {
                    
                    AboutQR aboutQR = (AboutQR)App.Kontrolki[0];
                    aboutQR.SetContent();
                    ccControl.Content = null;
                    ccControl.Content = App.Kontrolki[0];
                }
                else if(args.SelectedItem is History)
                {
                    
                    History h = (History)App.Kontrolki[2];
                    ccControl.Content = null;
                    ccControl.Content = App.Kontrolki[2];
                }
                else if(args.SelectedItem is Ustawienia)
                {
                    Ustawienia u = (Ustawienia)App.Kontrolki[3];
                    progressRing.IsActive = true;
                    await u.SetContent();
                    ccControl.Content = null;
                    ccControl.Content = App.Kontrolki[3];
                    progressRing.IsActive = false;
                }
                else
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem nvi = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                    if (nvi.Tag.ToString() == "History")
                    {
                    ccControl.Content = null;
                    ccControl.Content = App.Kontrolki[2];
                    }
                    else if (nvi.Tag.ToString() == "About")
                    {
                        
                        AboutQR aboutQR = (AboutQR)App.Kontrolki[0];
                        aboutQR.SetContent();
                        ccControl.Content = null;
                        ccControl.Content = App.Kontrolki[0];
                    }
                    else if (nvi.Tag.ToString() == "Scan")
                    {
                        Scan s = (Scan)App.Kontrolki[1];
                        
                        s.IsInitialised = false;
                        s.BackButtonFromUC = true;
                        progressRing.IsActive = true;
                            await s.Initialize();
                        
                        ccControl.Content = null;
                        ccControl.Content = App.Kontrolki[1];
                    progressRing.IsActive = false;  
                    }
                    else if(nvi.Tag.ToString()=="Ustawienia")
                    {
                        Ustawienia u = (Ustawienia)App.Kontrolki[3];
                    progressRing.IsActive = true;
                        await u.SetContent();
                        ccControl.Content = null;
                        ccControl.Content = App.Kontrolki[3];
                    progressRing.IsActive = false;
                    }
                }
            
        }
    }
}
