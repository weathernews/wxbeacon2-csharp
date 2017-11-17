using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Weathernews.Sensor;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WxBeaconApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		private WxBeacon2Watcher wxBeacon2Watcher = new WxBeacon2Watcher();

		public MainPage()
        {
            this.InitializeComponent();
			wxBeacon2Watcher.Received += WxBeacon2Watcher_Found;
        }

		private async void WxBeacon2Watcher_Found(object sender, WxBeacon2 beacon) {
			var latest = await beacon.GetLatestDataAsync();
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			 {
				 textBlock.Text = latest.ToString();
				 beacon.Dispose();
			 });
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			wxBeacon2Watcher.Start();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e) {
			wxBeacon2Watcher.Stop();
		}
	}
}
