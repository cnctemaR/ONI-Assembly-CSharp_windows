using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The platform application is running. Returned by Application.platform.</para>
	/// </summary>
	public enum RuntimePlatform
	{
		/// <summary>
		///   <para>In the Unity editor on macOS.</para>
		/// </summary>
		OSXEditor,
		/// <summary>
		///   <para>In the player on macOS.</para>
		/// </summary>
		OSXPlayer,
		/// <summary>
		///   <para>In the player on Windows.</para>
		/// </summary>
		WindowsPlayer,
		/// <summary>
		///   <para>In the web player on macOS.</para>
		/// </summary>
		[Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
		OSXWebPlayer,
		/// <summary>
		///   <para>In the Dashboard widget on macOS.</para>
		/// </summary>
		[Obsolete("Dashboard widget on Mac OS X export is no longer supported in Unity 5.4+.", true)]
		OSXDashboardPlayer,
		/// <summary>
		///   <para>In the web player on Windows.</para>
		/// </summary>
		[Obsolete("WebPlayer export is no longer supported in Unity 5.4+.", true)]
		WindowsWebPlayer,
		/// <summary>
		///   <para>In the Unity editor on Windows.</para>
		/// </summary>
		WindowsEditor = 7,
		/// <summary>
		///   <para>In the player on the iPhone.</para>
		/// </summary>
		IPhonePlayer,
		[Obsolete("Xbox360 export is no longer supported in Unity 5.5+.")]
		XBOX360 = 10,
		[Obsolete("PS3 export is no longer supported in Unity >=5.5.")]
		PS3 = 9,
		/// <summary>
		///   <para>In the player on Android devices.</para>
		/// </summary>
		Android = 11,
		[Obsolete("NaCl export is no longer supported in Unity 5.0+.")]
		NaCl,
		[Obsolete("FlashPlayer export is no longer supported in Unity 5.0+.")]
		FlashPlayer = 15,
		/// <summary>
		///   <para>In the player on Linux.</para>
		/// </summary>
		LinuxPlayer = 13,
		/// <summary>
		///   <para>In the Unity editor on Linux.</para>
		/// </summary>
		LinuxEditor = 16,
		/// <summary>
		///   <para>In the player on WebGL</para>
		/// </summary>
		WebGLPlayer,
		[Obsolete("Use WSAPlayerX86 instead")]
		MetroPlayerX86,
		/// <summary>
		///   <para>In the player on Windows Store Apps when CPU architecture is X86.</para>
		/// </summary>
		WSAPlayerX86 = 18,
		[Obsolete("Use WSAPlayerX64 instead")]
		MetroPlayerX64,
		/// <summary>
		///   <para>In the player on Windows Store Apps when CPU architecture is X64.</para>
		/// </summary>
		WSAPlayerX64 = 19,
		[Obsolete("Use WSAPlayerARM instead")]
		MetroPlayerARM,
		/// <summary>
		///   <para>In the player on Windows Store Apps when CPU architecture is ARM.</para>
		/// </summary>
		WSAPlayerARM = 20,
		[Obsolete("Windows Phone 8 was removed in 5.3")]
		WP8Player,
		[Obsolete("BlackBerryPlayer export is no longer supported in Unity 5.4+.")]
		BlackBerryPlayer,
		[Obsolete("TizenPlayer export is no longer supported in Unity 2017.3+.")]
		TizenPlayer,
		/// <summary>
		///   <para>In the player on the PS Vita.</para>
		/// </summary>
		PSP2,
		/// <summary>
		///   <para>In the player on the Playstation 4.</para>
		/// </summary>
		PS4,
		[Obsolete("PSM export is no longer supported in Unity >= 5.3")]
		PSM,
		/// <summary>
		///   <para>In the player on Xbox One.</para>
		/// </summary>
		XboxOne,
		[Obsolete("SamsungTVPlayer export is no longer supported in Unity 2017.3+.")]
		SamsungTVPlayer,
		[Obsolete("Wii U is no longer supported in Unity 2018.1+.")]
		WiiU = 30,
		/// <summary>
		///   <para>In the player on the Apple's tvOS.</para>
		/// </summary>
		tvOS,
		/// <summary>
		///   <para>In the player on Nintendo Switch.</para>
		/// </summary>
		Switch
	}
}
