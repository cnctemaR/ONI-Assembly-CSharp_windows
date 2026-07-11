using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enumeration for SystemInfo.batteryStatus which represents the current status of the device's battery.</para>
	/// </summary>
	public enum BatteryStatus
	{
		/// <summary>
		///   <para>The device's battery status cannot be determined. If battery status is not available on your target platform, SystemInfo.batteryStatus will return this value.</para>
		/// </summary>
		Unknown,
		/// <summary>
		///   <para>Device is plugged in and charging.</para>
		/// </summary>
		Charging,
		/// <summary>
		///   <para>Device is unplugged and discharging.</para>
		/// </summary>
		Discharging,
		/// <summary>
		///   <para>Device is plugged in, but is not charging.</para>
		/// </summary>
		NotCharging,
		/// <summary>
		///   <para>Device is plugged in and the battery is full.</para>
		/// </summary>
		Full
	}
}
