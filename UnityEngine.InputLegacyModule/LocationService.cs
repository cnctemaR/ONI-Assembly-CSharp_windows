using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Input/InputBindings.h")]
	[NativeHeader("Runtime/Input/LocationService.h")]
	public class LocationService
	{
		[FreeFunction("LocationService::IsServiceEnabledByUser")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsServiceEnabledByUser();

		[FreeFunction("LocationService::GetLocationStatus")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern LocationServiceStatus GetLocationStatus();

		[FreeFunction("LocationService::GetLastLocation")]
		internal static LocationInfo GetLastLocation()
		{
			LocationInfo locationInfo;
			LocationService.GetLastLocation_Injected(out locationInfo);
			return locationInfo;
		}

		[FreeFunction("LocationService::SetDesiredAccuracy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDesiredAccuracy(float value);

		[FreeFunction("LocationService::SetDistanceFilter")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDistanceFilter(float value);

		[FreeFunction("LocationService::StartUpdatingLocation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StartUpdatingLocation();

		[FreeFunction("LocationService::StopUpdatingLocation")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopUpdatingLocation();

		[FreeFunction("LocationService::GetLastHeading")]
		internal static LocationService.HeadingInfo GetLastHeading()
		{
			LocationService.HeadingInfo headingInfo;
			LocationService.GetLastHeading_Injected(out headingInfo);
			return headingInfo;
		}

		[FreeFunction("LocationService::IsHeadingUpdatesEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsHeadingUpdatesEnabled();

		[FreeFunction("LocationService::SetHeadingUpdatesEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetHeadingUpdatesEnabled(bool value);

		public bool isEnabledByUser
		{
			get
			{
				return LocationService.IsServiceEnabledByUser();
			}
		}

		public LocationServiceStatus status
		{
			get
			{
				return LocationService.GetLocationStatus();
			}
		}

		public LocationInfo lastData
		{
			get
			{
				bool flag = this.status != LocationServiceStatus.Running;
				if (flag)
				{
					Debug.Log("Location service updates are not enabled. Check LocationService.status before querying last location.");
				}
				return LocationService.GetLastLocation();
			}
		}

		public void Start(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			LocationService.SetDesiredAccuracy(desiredAccuracyInMeters);
			LocationService.SetDistanceFilter(updateDistanceInMeters);
			LocationService.StartUpdatingLocation();
		}

		public void Start(float desiredAccuracyInMeters)
		{
			this.Start(desiredAccuracyInMeters, 10f);
		}

		public void Start()
		{
			this.Start(10f, 10f);
		}

		public void Stop()
		{
			LocationService.StopUpdatingLocation();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLastLocation_Injected(out LocationInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLastHeading_Injected(out LocationService.HeadingInfo ret);

		internal struct HeadingInfo
		{
			public float magneticHeading;

			public float trueHeading;

			public float headingAccuracy;

			public Vector3 raw;

			public double timestamp;
		}
	}
}
