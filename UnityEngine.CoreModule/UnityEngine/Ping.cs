using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Ping any given IP address (given in dot notation).</para>
	/// </summary>
	[NativeHeader("Runtime/Export/Ping.bindings.h")]
	public sealed class Ping
	{
		/// <summary>
		///   <para>Perform a ping to the supplied target IP address.</para>
		/// </summary>
		/// <param name="address"></param>
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

		/// <summary>
		///   <para>Has the ping function completed?</para>
		/// </summary>
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

		/// <summary>
		///   <para>This property contains the ping time result after isDone returns true.</para>
		/// </summary>
		public extern int time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The IP target of the ping.</para>
		/// </summary>
		public extern string ip
		{
			[NativeName("GetIP")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal IntPtr m_Ptr;
	}
}
