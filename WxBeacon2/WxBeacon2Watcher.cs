using System;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace Weathernews.Sensor {
	/// <summary>
	/// WxBeacon2を検索します
	/// </summary>
	public class WxBeacon2Watcher: IDisposable {
		/// <summary>
		/// BluetoothLEAdvertisementWatcherのインスタンス
		/// </summary>
		private BluetoothLEAdvertisementWatcher bluetoothLEAdvertisementWatcher;

		/// <summary>
		/// WxBeacon2FoundEventのハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="beacon"></param>
		public delegate void WxBeacon2FoundEventHandler(object sender, WxBeacon2 beacon);

		/// <summary>
		/// WxBeacon2が発見されたときのイベントを取得または設定します
		/// </summary>
		public event WxBeacon2FoundEventHandler Received;

		/// <summary>
		/// WxBeacon2Watcherが開始状態かどうかを取得します
		/// </summary>
		public bool Started
		{
			get
			{
				return bluetoothLEAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
			}
		}

		/// <summary>
		/// WxBeacon2Watcherのインスタンスを初期化します
		/// </summary>
		public WxBeacon2Watcher() {
			//Initializes iBeacon Watcher
			bluetoothLEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher
			{
				AdvertisementFilter = WxBeacon2.CreateAdvertisementFilter(),
				ScanningMode = BluetoothLEScanningMode.Active
			};
			bluetoothLEAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
		}

		/// <summary>
		/// WxBeacon2のアドバタイズを受信したときに呼ばれます
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private async void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
			Debug.WriteLine("WxBeacon2 " + args.BluetoothAddress + " を発見しました。接続しています...");
			var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
			Debug.WriteLine(args.BluetoothAddress + "に接続完了。");
			Received?.Invoke(this, new WxBeacon2(device));
		}

		/// <summary>
		/// WxBeacon2の検索を開始します
		/// </summary>
		public void Start() {
			if (bluetoothLEAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started) {
				return;
			}
			bluetoothLEAdvertisementWatcher.Start();
		}

		/// <summary>
		/// WxBeacon2の検索を終了します
		/// </summary>
		public void Stop() {
			if (bluetoothLEAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Stopped) {
				return;
			}
			bluetoothLEAdvertisementWatcher.Stop();
		}

		/// <summary>
		/// WxBeacon2Watcherのインスタンスを破棄します
		/// </summary>
		public void Dispose() {
			Stop();
		}

	}
}
