using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	[UsedByNativeCode("XRSubsystemDescriptorBase")]
	[StructLayout(LayoutKind.Sequential)]
	public class SubsystemDescriptorBase : ISubsystemDescriptor, ISubsystemDescriptorImpl
	{
		public string id
		{
			get
			{
				return Internal_SubsystemDescriptors.GetId(this.m_Ptr);
			}
		}

		IntPtr ISubsystemDescriptorImpl.ptr
		{
			get
			{
				return this.m_Ptr;
			}
			set
			{
				this.m_Ptr = value;
			}
		}

		internal IntPtr m_Ptr;
	}
}
