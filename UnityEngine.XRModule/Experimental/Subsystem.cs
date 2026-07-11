using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	/// <summary>
	///   <para>An Subsystem is initialized from an SubsystemDescriptor for a given Subsystem (Example, Input, Environment, Display, etc.) and provides an interface to interact with that given Subsystem until it is Destroyed. After an Subsystem is created it can be Started or Stopped to turn on and off functionality (and preserve performance). The base type for Subsystem only exposes this functionality; this class is designed to be a base class for derived classes that expose more functionality specific to a given Subsystem.
	///
	///       Note: initializing a second Subsystem from the same SubsystemDescriptor will return a reference to the existing Subsystem as only one Subsystem is currently allowed for a single Subsystem provider.
	///       </para>
	/// </summary>
	[NativeType(Header = "Modules/XR/XRSubsystem.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Subsystem
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetHandle(Subsystem inst);

		/// <summary>
		///   <para>Starts an instance of a subsystem.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Start();

		/// <summary>
		///   <para>Stops an instance of a subsystem.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		/// <summary>
		///   <para>Destroys this instance of a subsystem.</para>
		/// </summary>
		public void Destroy()
		{
			IntPtr ptr = this.m_Ptr;
			Internal_SubsystemInstances.Internal_RemoveInstanceByPtr(this.m_Ptr);
			SubsystemManager.DestroyInstance_Internal(ptr);
		}

		internal IntPtr m_Ptr;

		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
}
