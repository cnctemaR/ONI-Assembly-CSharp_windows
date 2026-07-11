using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/BootConfig.bindings.h")]
	internal class BootConfigData
	{
		private BootConfigData(IntPtr nativeHandle)
		{
			if (nativeHandle == IntPtr.Zero)
			{
				throw new ArgumentException("native handle can not be null");
			}
			this.m_Ptr = nativeHandle;
		}

		public void AddKey(string key)
		{
			this.Append(key, null);
		}

		public string Get(string key)
		{
			return this.GetValue(key, 0);
		}

		public string Get(string key, int index)
		{
			return this.GetValue(key, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Append(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Set(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetValue(string key, int index);

		[RequiredByNativeCode]
		private static BootConfigData WrapBootConfigData(IntPtr nativeHandle)
		{
			return new BootConfigData(nativeHandle);
		}

		private IntPtr m_Ptr;
	}
}
