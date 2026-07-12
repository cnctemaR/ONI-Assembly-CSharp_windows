using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;

namespace Unity.Profiling
{
	internal struct ProfilerMarkerWithStringData
	{
		public static ProfilerMarkerWithStringData Create(string name, string parameterName)
		{
			IntPtr intPtr = ProfilerUnsafeUtility.CreateMarker(name, 16, MarkerFlags.Default, 1);
			ProfilerUnsafeUtility.SetMarkerMetadata(intPtr, 0, parameterName, 9, 0);
			return new ProfilerMarkerWithStringData
			{
				_marker = intPtr
			};
		}

		[Pure]
		[MethodImpl((MethodImplOptions)256)]
		public ProfilerMarkerWithStringData.AutoScope Auto(bool enabled, Func<string> parameterValue)
		{
			ProfilerMarkerWithStringData.AutoScope autoScope;
			if (enabled)
			{
				autoScope = this.Auto(parameterValue());
			}
			else
			{
				autoScope = new ProfilerMarkerWithStringData.AutoScope(IntPtr.Zero);
			}
			return autoScope;
		}

		[Pure]
		[MethodImpl((MethodImplOptions)256)]
		public unsafe ProfilerMarkerWithStringData.AutoScope Auto(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				ProfilerMarkerData profilerMarkerData = new ProfilerMarkerData
				{
					Type = 9,
					Size = (uint)(value.Length * 2 + 2)
				};
				profilerMarkerData.Ptr = (void*)ptr;
				ProfilerUnsafeUtility.BeginSampleWithMetadata(this._marker, 1, (void*)(&profilerMarkerData));
			}
			return new ProfilerMarkerWithStringData.AutoScope(this._marker);
		}

		private const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;

		private IntPtr _marker;

		public struct AutoScope : IDisposable
		{
			[MethodImpl((MethodImplOptions)256)]
			internal AutoScope(IntPtr marker)
			{
				this._marker = marker;
			}

			[Pure]
			[MethodImpl((MethodImplOptions)256)]
			public void Dispose()
			{
				bool flag = this._marker != IntPtr.Zero;
				if (flag)
				{
					ProfilerUnsafeUtility.EndSample(this._marker);
				}
			}

			private IntPtr _marker;
		}
	}
}
