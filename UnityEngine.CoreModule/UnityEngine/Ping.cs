using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Ping.bindings.h")]
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
			if (!(this.m_Ptr == IntPtr.Zero))
			{
				Ping.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[FreeFunction("DestroyPing", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[FreeFunction("CreatePing")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create(string address);

		public bool isDone
		{
			get
			{
				return !(this.m_Ptr == IntPtr.Zero) && this.Internal_IsDone();
			}
		}

		[NativeName("GetIsDone")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_IsDone();

		public extern int time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string ip
		{
			[NativeName("GetIP")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal IntPtr m_Ptr;
	}
}
