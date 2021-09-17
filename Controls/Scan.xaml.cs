using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Enumeration;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Security.Cryptography;
using Windows.UI.Core;
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
    public sealed partial class Scan : UserControl
    {
        public event Action<string> QRCodeArrived;
        public bool DecodeHC { get; set; }

        public bool SeparateWindowView { get; set; }

        public bool IsInitialised { get; set; }

        public bool BackButtonFromUC { get; set; }

        private Microsoft.UI.Xaml.Controls.NavigationView _menuNavigation;

        private MediaCaptureInitializationSettings _captureInitSettings;
        MediaCapture _mc;
        ClaimedBarcodeScanner _cbs;
        BarcodeScanner _bs;

        public Scan(Microsoft.UI.Xaml.Controls.NavigationView navigationView)
        {
            this.InitializeComponent();
                        
            DecodeHC = true;
            _menuNavigation = navigationView;
             
        }

        public async Task StopForNavigation()
        {
            if(_mc!=null && IsInitialised)
            {
                if (!SeparateWindowView)
                {
                    await _mc.StopPreviewAsync();
                }
                    IsInitialised = false;
                if(_cbs!=null)
                {
                    _cbs.DataReceived -= Cbs_DataReceived;
                    _cbs.ReleaseDeviceRequested -= Cbs_ReleaseDeviceRequested;
                    if(SeparateWindowView)
                    {
                        _cbs.HideVideoPreview();
                    }
                    if(_bs!=null)
                    {
                        _bs.Dispose();
                        _bs = null;
                    }
                }
            }
        }

        public async Task StartForNavigation()
        {
            if(_mc!=null && IsInitialised)
            {
                if (!SeparateWindowView)
                {
                    await _mc.StartPreviewAsync();
                }
                else
                {
                    if(_cbs!=null)
                    {
                        await _cbs.ShowVideoPreviewAsync();
                    }
                }
            }
        }

        public async Task Initialize()
        {
            if(!IsInitialised)
            {
                try
                {
                                        
                    string status = "OK";
                    if(_bs!=null)
                    {
                       status = await _bs.CheckHealthAsync(UnifiedPosHealthCheckLevel.POSInternal);
                    }
                                        
                    if (status == "OK")
                    {
                        _bs = await GetBarcode(App.DeviceId);
                    }
                    else
                    {
                        _bs = await GetBarcode("");
                    }
                    if (_bs != null)
                    {
                        InitCaptureSettings(_bs);

                        if (!BackButtonFromUC) //Nie wiem czemu przy powrocie tak się dzieje, ze claim nie ma podpietego videodeviceid
                        {
                            _cbs = await _bs.ClaimScannerAsync();
                            _cbs.DataReceived += Cbs_DataReceived;
                            _cbs.ReleaseDeviceRequested += Cbs_ReleaseDeviceRequested;
                        }  
                        
                                                
                        if (!SeparateWindowView)
                        {
                            _cbs.HideVideoPreview();
                        }
                                                
                        if (!SeparateWindowView)
                        {
                            await InitMediaCapture();

                            previewControl.Source = _mc;
                            await previewControl.Source.StartPreviewAsync();
                        }
                        else
                        {
                            await _cbs.ShowVideoPreviewAsync();
                        }

                        IsInitialised = true;

                        _cbs.IsDecodeDataEnabled = true;
                        await _cbs.EnableAsync();
                        await _cbs.StartSoftwareTriggerAsync();
                    }
                    else
                    {
                        throw new Exception(ResourceLoader.GetForCurrentView().GetString("NoScannerFoundException"));
                    }

                }
                catch(NullReferenceException nex)
                {
                    MessageDialog md = new MessageDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ErrorNull") + nex.Message);
                    await md.ShowAsync();
                }
                catch (Exception ex)
                {
                    MessageDialog md = new MessageDialog(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Error") + ex.Message);
                    await md.ShowAsync();
                }
                finally
                {
                    if(QRCodeArrived!=null)
                    {
                       
                    }
                    await Task.CompletedTask;
                }
            }
            else
            {
                await Task.CompletedTask;
            }
        }

        private void Cbs_ReleaseDeviceRequested(object sender, ClaimedBarcodeScanner e)
        {
            e.RetainDevice();
        }

        private async void Cbs_DataReceived(ClaimedBarcodeScanner sender, BarcodeScannerDataReceivedEventArgs args)
        {
            uint scanDataType = args.Report.ScanDataType;
            if(scanDataType == BarcodeSymbologies.Upca ||
        scanDataType == BarcodeSymbologies.UpcaAdd2 ||
        scanDataType == BarcodeSymbologies.UpcaAdd5 ||
        scanDataType == BarcodeSymbologies.Upce ||
        scanDataType == BarcodeSymbologies.UpceAdd2 ||
        scanDataType == BarcodeSymbologies.UpceAdd5 ||
        scanDataType == BarcodeSymbologies.Ean8 ||
        scanDataType == BarcodeSymbologies.TfStd || scanDataType == BarcodeSymbologies.Qr)
                {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    string code = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, args.Report.ScanDataLabel);
                    if(DecodeHC)
                    {
                        if (QRCodeArrived != null)
                        {
                            QRCodeArrived(code);
                            _menuNavigation.SelectedItem = App.Kontrolki[2];
                            
                        }
                    }
                    tbUndecoded.Text = code;
                    sndElement.Play();
                });

                
            }
            
        }

        private void InitCaptureSettings(BarcodeScanner bs)
        {
            _captureInitSettings = new MediaCaptureInitializationSettings();
            _captureInitSettings.VideoDeviceId = App.VideoDeviceId;
            _captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.Video;
            _captureInitSettings.PhotoCaptureSource = PhotoCaptureSource.VideoPreview;

            

        }

        public async Task<BarcodeScanner> GetBarcode(string deviceId)
        {
            
            if (!string.IsNullOrEmpty(deviceId))
            {
                BarcodeScanner bs = await BarcodeScanner.FromIdAsync(deviceId);
                if(!string.IsNullOrEmpty(bs.VideoDeviceId))
                {

                    App.VideoDeviceId = bs.VideoDeviceId;
                }
                return bs;
            }
            else
            {
                //string selector = BarcodeScanner.GetDeviceSelector(PosConnectionTypes.Local);
                //DeviceInformationCollection dic = await DeviceInformation.FindAllAsync(selector);
                //BarcodeScanner bs = await BarcodeScanner.FromIdAsync(dic[0].Id);
                BarcodeScanner bs = await BarcodeScanner.GetDefaultAsync();
                App.DeviceId = bs.DeviceId;
                App.VideoDeviceId = bs.VideoDeviceId;
                return bs;
            }
        }

        private async Task InitMediaCapture()
        {
            _mc = new MediaCapture();
            await _mc.InitializeAsync(_captureInitSettings);

            VideoRotation vr = _mc.GetPreviewRotation();
            if(vr == VideoRotation.None)
            {
                //_mc.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            }
            

            await Task.CompletedTask;
        }

        public void ChangeCamera()
        {
            if(IsInitialised)
            {
                
            }
        }

        public void CleanRawCode()
        {
            tbUndecoded.Text = "";
        }

    }
}
