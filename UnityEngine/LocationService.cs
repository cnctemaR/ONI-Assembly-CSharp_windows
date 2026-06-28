using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class LocationService
	{
		public extern bool isEnabledByUser
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LocationServiceStatus status
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LocationInfo lastData
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Start([DefaultValue("10f")] float desiredAccuracyInMeters, [DefaultValue("10f")] float updateDistanceInMeters);

		[ExcludeFromDocs]
		public void Start(float desiredAccuracyInMeters)
		{
			float num = 10f;
			this.Start(desiredAccuracyInMeters, num);
		}

		[ExcludeFromDocs]
		public void Start()
		{
			float num = 10f;
			float num2 = 10f;
			this.Start(num2, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();
	}
}
