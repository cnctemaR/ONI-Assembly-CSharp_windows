using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Bootstrap/BootConfig.bindings.h")]
	internal class BootConfigData
	{
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

		public unsafe void Append(string key, string value)
		{
			try
			{
				IntPtr intPtr = BootConfigData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = value.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				BootConfigData.Append_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		public unsafe void Set(string key, string value)
		{
			try
			{
				IntPtr intPtr = BootConfigData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = value.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				BootConfigData.Set_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		private unsafe string GetValue(string key, int index)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = BootConfigData.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = key.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				BootConfigData.GetValue_Injected(intPtr, ref managedSpanWrapper, index, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		[RequiredByNativeCode]
		private static BootConfigData WrapBootConfigData(IntPtr nativeHandle)
		{
			return new BootConfigData(nativeHandle);
		}

		private BootConfigData(IntPtr nativeHandle)
		{
			bool flag = nativeHandle == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("native handle can not be null");
			}
			this.m_Ptr = nativeHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Append_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Set_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetValue_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, int index, out ManagedSpanWrapper ret);

		private IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(BootConfigData bootConfig)
			{
				return bootConfig.m_Ptr;
			}
		}
	}
}
