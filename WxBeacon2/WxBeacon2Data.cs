using System.Text;

namespace Weathernews.Sensor {
	/// <summary>
	/// WxBeacon2の観測データを格納します
	/// </summary>
	public class WxBeacon2Data {

		/// <summary>
		/// 気温 [℃]
		/// </summary>
		public float Temperature
		{
			get;
			private set;
		}

		/// <summary>
		/// 湿度 [%RH]
		/// </summary>
		public float Humidity
		{
			get;
			private set;
		}

		/// <summary>
		/// 照度 [lx]
		/// </summary>
		public uint Illuminance
		{
			get;
			private set;
		}

		/// <summary>
		/// UVインデックス 
		/// </summary>
		public float UvIndex
		{
			get;
			private set;
		}

		/// <summary>
		/// 気圧 [hPa]
		/// </summary>
		public float Pressure
		{
			get;
			private set;
		}

		/// <summary>
		/// 騒音 [dB]
		/// </summary>
		public float Noise
		{
			get;
			private set;
		}

		/// <summary>
		/// 不快指数
		/// </summary>
		public float DiscomfortIndex
		{
			get;
			private set;
		}

		/// <summary>
		/// 熱中症危険度 WGBT [℃]
		/// </summary>
		public float Wgbt
		{
			get;
			private set;
		}

		/// <summary>
		/// バッテリー電圧 [V]
		/// </summary>
		public float BatteryVoltage
		{
			get;
			private set;
		}

		/// <summary>
		/// WxBeacon2の観測データを初期化します
		/// </summary>
		/// <param name="temperature"></param>
		/// <param name="humidity"></param>
		/// <param name="illuminance"></param>
		/// <param name="uvIndex"></param>
		/// <param name="pressure"></param>
		/// <param name="noise"></param>
		/// <param name="discomfortIndex"></param>
		/// <param name="wgbt"></param>
		/// <param name="battery"></param>
		public WxBeacon2Data(
			float temperature,
			float humidity,
			uint illuminance,
			float uvIndex,
			float pressure,
			float noise,
			float discomfortIndex,
			float wgbt,
			float battery
			) {
			Temperature = temperature;
			Humidity = humidity;
			Illuminance = illuminance;
			UvIndex = uvIndex;
			Pressure = pressure;
			Noise = noise;
			DiscomfortIndex = discomfortIndex;
			Wgbt = wgbt;
			BatteryVoltage = battery;
		}

		public override string ToString() {
			return new StringBuilder()
				.Append("{")
				.Append(" Temperature = ").Append(Temperature).Append(", ")
				.Append(" Humidity = ").Append(Humidity).Append(", ")
				.Append(" Pressure = ").Append(Pressure).Append(", ")
				.Append(" Illuminance = ").Append(Illuminance).Append(", ")
				.Append(" UvIndex = ").Append(UvIndex).Append(", ")
				.Append(" Noise = ").Append(Noise).Append(", ")
				.Append(" DiscomfortIndex = ").Append(DiscomfortIndex).Append(", ")
				.Append(" Wgbt = ").Append(Wgbt).Append(", ")
				.Append(" BatteryVoltage = ").Append(BatteryVoltage)
				.Append("}")
				.ToString();
		}
	}
}
