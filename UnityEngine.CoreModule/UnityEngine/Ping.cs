using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Networking/Ping.bindings.h")]
	public sealed class Ping
	{
		public Ping(string address)
		{
			this.m_Ptr = Ping.Internal_Create(address);
		}

		~Ping()
		{
			this.DestroyPing();
		}

		[ThreadAndSerializationSafe]
		public void DestroyPing()
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (!flag)
			{
				Ping.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[FreeFunction("DestroyPing", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[FreeFunction("CreatePing")]
		private unsafe static IntPtr Internal_Create(string address)
		{
			IntPtr intPtr;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(address, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = address.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				intPtr = Ping.Internal_Create_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return intPtr;
		}

		public bool isDone
		{
			get
			{
				bool flag = this.m_Ptr == IntPtr.Zero;
				return !flag && this.Internal_IsDone();
			}
		}

		[NativeName("GetIsDone")]
		private bool Internal_IsDone()
		{
			IntPtr intPtr = Ping.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Ping.Internal_IsDone_Injected(intPtr);
		}

		public int time
		{
			get
			{
				IntPtr intPtr = Ping.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Ping.get_time_Injected(intPtr);
			}
		}

		public string ip
		{
			[NativeName("GetIP")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Ping.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					Ping.get_ip_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create_Injected(ref ManagedSpanWrapper address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsDone_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_ip_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(Ping ping)
			{
				return ping.m_Ptr;
			}
		}
	}
}
