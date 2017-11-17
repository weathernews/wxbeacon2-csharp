using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Weathernews.Sensor {
	/// <summary>
	/// WxBeacon2を格納します
	/// </summary>
	public class WxBeacon2: IDisposable {
		/// <summary>
		/// Unix時刻 (time_t)との変換用基準時刻
		/// </summary>
		private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly Guid SERVICE_SENSOR = Guid.Parse("0c4c3000-7700-46f4-aa96-d5e974e32a54");
		private static readonly Guid CHARACTERISTIC_LATEST_DATA = Guid.Parse("0c4c3001-7700-46f4-aa96-d5e974e32a54");
		private static readonly Guid SERVICE_SENSOR_SETTING = Guid.Parse("0c4c3010-7700-46f4-aa96-d5e974e32a54");
		private static readonly Guid CHARACTERISTIC_MEASUREMENT_INTERVAL = Guid.Parse("0c4c3011-7700-46f4-aa96-d5e974e32a54");
		private static readonly Guid SERVICE_CONTROL = Guid.Parse("0c4c3030-7700-46f4-aa96-d5e974e32a54");
		private static readonly Guid CHARACTERISTIC_TIME_INFORMATION = Guid.Parse("0c4c3030-7700-46f4-aa96-d5e974e32a54");

		/// <summary>
		/// WxBeacon2のBLEデバイスインスタンスを取得します
		/// </summary>
		public BluetoothLEDevice Device
		{
			private set;
			get;
		}

		/// <summary>
		/// WxBeacon2のインスタンスを初期化します
		/// </summary>
		/// <param name="device"></param>
		public WxBeacon2(BluetoothLEDevice device) {
			Device = device;
		}

		/// <summary>
		/// WxBeacon2で測定した最新の測定値を取得します
		/// </summary>
		/// <returns></returns>
		public async Task<WxBeacon2Data> GetLatestDataAsync()
		{
			Debug.WriteLine("測定値を取得しています...");
			byte[] data = await ReadCharacteristicAsync(Device, SERVICE_SENSOR, CHARACTERISTIC_LATEST_DATA);
			if (data != null)
			{
				Debug.WriteLine("完了");
				return new WxBeacon2Data(
						BitConverter.ToInt16(data, 1) * 0.01f,
						BitConverter.ToInt16(data, 3) * 0.01f,
						BitConverter.ToUInt16(data, 5),
						BitConverter.ToInt16(data, 7) * 0.01f,
						BitConverter.ToInt16(data, 9) * 0.1f,
						BitConverter.ToInt16(data, 11) * 0.01f,
						BitConverter.ToInt16(data, 13) * 0.01f,
						BitConverter.ToInt16(data, 15) * 0.01f,
						BitConverter.ToUInt16(data, 17) * 0.001f
				);
			}
			else
			{
				Debug.WriteLine("失敗");
				return null;
			}
		}

		/// <summary>
		/// WxBeacon2の現在時刻を取得します
		/// </summary>
		/// <returns></returns>
		public async Task<DateTime?> GetDateTimeAsync()
		{
			Debug.WriteLine("現在時刻を取得します...");
			byte[] data = await ReadCharacteristicAsync(Device, SERVICE_CONTROL, CHARACTERISTIC_TIME_INFORMATION);
			Debug.WriteLine("完了");
			return UNIX_EPOCH.AddSeconds(BitConverter.ToUInt32(data, 0)).ToLocalTime();
		}

		/// <summary>
		/// WxBeacon2の現在時刻を設定します
		/// </summary>
		/// <param name="time">現在時刻</param>
		/// <returns></returns>
		public async Task SetDateTimeAsync(DateTime time)
		{
			Debug.WriteLine("現在時刻を設定します...");
			var unixtime = (UInt32) ((time.ToUniversalTime() - UNIX_EPOCH).TotalSeconds);
			Debug.WriteLine("完了");
			byte[] data = BitConverter.GetBytes(unixtime);
			await WriteCharacteristicAsync(Device, SERVICE_CONTROL, CHARACTERISTIC_TIME_INFORMATION, data);
		}

		/// <summary>
		/// WxBeacon2の計測間隔を取得します
		/// </summary>
		/// <returns></returns>
		public async Task<TimeSpan> GetMeasureSpanAsync()
		{
			Debug.WriteLine("測定間隔を取得します...");
			byte[] data = await ReadCharacteristicAsync(Device, SERVICE_SENSOR_SETTING, CHARACTERISTIC_MEASUREMENT_INTERVAL);
			Debug.WriteLine("完了");
			return TimeSpan.FromSeconds(BitConverter.ToUInt16(data, 0));
		}

		/// <summary>
		/// WxBeacon2の計測間隔を設定します
		/// </summary>
		/// <param name="span">計測間隔</param>
		/// <returns></returns>
		public async Task SetMeasureSpanAsync(TimeSpan span)
		{
			Debug.WriteLine("測定間隔を設定します...");
			byte[] data = BitConverter.GetBytes((UInt16)span.TotalSeconds);
			Debug.WriteLine("完了");
			await WriteCharacteristicAsync(Device, SERVICE_SENSOR_SETTING, CHARACTERISTIC_MEASUREMENT_INTERVAL, data);
		}

		public void Dispose()
		{
			Device.Dispose();
		}

		/// <summary>
		/// 指定されたBluetoothLEDeviceからキャラクタリスティックの値を読み込みます
		/// </summary>
		/// <param name="device">デバイス</param>
		/// <param name="serviceUuid">サービスUUID</param>
		/// <param name="characteristicUuid">キャラクタリスティックUUID</param>
		/// <returns></returns>
		private static async Task<byte[]> ReadCharacteristicAsync(BluetoothLEDevice device, Guid serviceUuid, Guid characteristicUuid)
		{
			var serviceFinder = await device.GetGattServicesAsync();
			if (serviceFinder.Status != GattCommunicationStatus.Success)
			{
				throw new Exception("サービスのスキャンに失敗しました");
			}
			var service = serviceFinder.Services.Single(s => s.Uuid == serviceUuid);
			if (service == null)
			{
				throw new Exception("サービスが見つかりませんでした");
			}
			var characteristicFinder = await service.GetCharacteristicsAsync();
			if (characteristicFinder.Status != GattCommunicationStatus.Success)
			{
				throw new Exception("キャラクタリスティックのスキャンに失敗しました");
			}
			var characteristic = characteristicFinder.Characteristics.Single(c => c.Uuid == characteristicUuid);
			if (characteristic == null)
			{
				throw new Exception("キャラクタリスティックが見つかりませんでした");
			}
			var readResult = await characteristic.ReadValueAsync();
			if (readResult.Status != GattCommunicationStatus.Success)
			{
				throw new Exception("キャラクタリスティックの読み込みに失敗しました");
			}
			return readResult.Value.ToArray();
		}

		/// <summary>
		/// 指定されたBluetoothLEDeviceにキャラクタリスティックの値を書き込みます
		/// </summary>
		/// <param name="device">デバイス</param>
		/// <param name="serviceUuid">サービスUUID</param>
		/// <param name="characteristicUuid">キャラクタリスティックUUID</param>
		/// <param name="data">書き込む値</param>
		/// <returns></returns>
		private static async Task WriteCharacteristicAsync(BluetoothLEDevice device, Guid serviceUuid, Guid characteristicUuid, byte[] data)
		{
			var serviceFinder = await device.GetGattServicesAsync();
			if (serviceFinder.Status != GattCommunicationStatus.Success)
			{
				throw new Exception("サービスのスキャンに失敗しました");
			}
			var service = serviceFinder.Services.Single(s => s.Uuid == serviceUuid);
			if (service == null)
			{
				throw new Exception("サービスが見つかりませんでした");
			}
			var characteristicFinder = await service.GetCharacteristicsAsync();
			if (characteristicFinder.Status != GattCommunicationStatus.Success)
			{
				throw new Exception("キャラクタリスティックのスキャンに失敗しました");
			}
			var characteristic = characteristicFinder.Characteristics.Single(c => c.Uuid == characteristicUuid);
			if (characteristic == null)
			{
				throw new Exception("キャラクタリスティックが見つかりませんでした");
			}
			await characteristic.WriteValueAsync(data.AsBuffer());
		}

		/// <summary>
		/// WxBeacon2の検索用アドバタイズフィルタを作成して返します
		/// </summary>
		/// <returns>WxBeacon2検索用のBluetoothLEAdvertisementFilterインスタンス</returns>
		public static BluetoothLEAdvertisementFilter CreateAdvertisementFilter() {
			var filter = new BluetoothLEAdvertisementFilter();
			filter.Advertisement.LocalName = "Env";
			return filter;
		}
	}
}
