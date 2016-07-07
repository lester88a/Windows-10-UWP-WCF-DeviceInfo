using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            //get ip address
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        TextBlock.Text = "IPv4 Address: " + localHostName.ToString();
                        TextBlock.Text += "\nLocal IP:  " + GetLocalIp();
                        break;
                    }
                }
            }

            //get device info
            TextBlock.Text += "\nDeviceForm: " + AnalyticsInfo.DeviceForm;
            TextBlock.Text += "\nDeviceFamily: " + AnalyticsInfo.VersionInfo.DeviceFamily;

            // get the system version number
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            TextBlock.Text += "\nSystem Version: " + $"{v1}.{v2}.{v3}.{v4}";

            // get the package architecure
            Package package = Package.Current;
            TextBlock.Text += "\nSystem Architecure: " + package.Id.Architecture.ToString();

            // get the user friendly app name
            TextBlock.Text += "\nApp Name: " + package.DisplayName;

            // get the device manufacturer and model name
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            TextBlock.Text += "\nOperatingSystem: " + eas.OperatingSystem;
            TextBlock.Text += "\nSystemFirmwareVersion: " + eas.SystemFirmwareVersion;
            TextBlock.Text += "\nSystemHardwareVersion: " + eas.SystemHardwareVersion;
            TextBlock.Text += "\nSystemManufacturer: " + eas.SystemManufacturer;
            TextBlock.Text += "\nSystemProductName: " + eas.SystemProductName;
            



        }
        
        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }
        private async void GetButton_Click(object sender, RoutedEventArgs e)
        {
            // get the package architecure
            Package package = Package.Current;
            if (!package.Id.Architecture.ToString().Contains("Arm"))
            {
                await GetDateFromServer();
            }
            

            if (NetAddress.Text != "" && NetAddress.Text != "http://" && NetAddress.Text != "https://")
            {
                BroswerWebView.Source = new Uri(NetAddress.Text);
            }





        }

        private async System.Threading.Tasks.Task GetDateFromServer()
        {
            try
            {
                // Create proxy instance 
                AccessSQLService.ServiceClient serviceClient = new AccessSQLService.ServiceClient();

                // async call WCF method to get returned data 
                AccessSQLService.querySqlRequest request = new AccessSQLService.querySqlRequest();
                AccessSQLService.querySqlResponse ds = await serviceClient.querySqlAsync(request);

                if (ds.queryParam)
                {
                    // Convert Xml to List<Article> 
                    XDocument xdoc = XDocument.Parse(ds.querySqlResult.Nodes[1].ToString(), LoadOptions.None);
                    var data = from query in xdoc.Descendants("Table")
                               select new Article
                               {
                                   Manufacturer = query.Element("Manufacturer").Value,
                                   //Text = query.Element("Text").Value
                               };

                    // Set ItemsSource of ListView control 
                    lvDataTemplates.ItemsSource = data;
                }
                else
                {
                    //Debug.WriteLine("Error occurs. Please make sure the database has been attached to SQL Server!");
                    var dialog = new MessageDialog("Error");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog("Error:\n" + e);
                await dialog.ShowAsync();
            }
            
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
        }
    }
}
