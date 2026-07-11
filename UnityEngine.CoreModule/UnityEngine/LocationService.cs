using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Interface into location functionality.</para>
	/// </summary>
	public sealed class LocationService
	{
		/// <summary>
		///   <para>Specifies whether location service is enabled in user settings.</para>
		/// </summary>
		public extern bool isEnabledByUser
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns location service status.</para>
		/// </summary>
		public extern LocationServiceStatus status
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Last measured device geographical location.</para>
		/// </summary>
		public extern LocationInfo lastData
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Starts location service updates.  Last location coordinates could be.</para>
		/// </summary>
		/// <param name="desiredAccuracyInMeters"></param>
		/// <param name="updateDistanceInMeters"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Start([DefaultValue("10f")] float desiredAccuracyInMeters, [DefaultValue("10f")] float updateDistanceInMeters);

		/// <summary>
		///   <para>Starts location service updates.  Last location coordinates could be.</para>
		/// </summary>
		/// <param name="desiredAccuracyInMeters"></param>
		/// <param name="updateDistanceInMeters"></param>
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

		/// <summary>
		///   <para>Stops location service updates. This could be useful for saving battery life.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();
	}
}
