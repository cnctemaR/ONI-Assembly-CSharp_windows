using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode("SubsystemDescriptorBase")]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class IntegratedSubsystemDescriptor : ISubsystemDescriptorImpl, ISubsystemDescriptor
	{
		public string id
		{
			get
			{
				return SubsystemDescriptorBindings.GetId(this.m_Ptr);
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

		ISubsystem ISubsystemDescriptor.Create()
		{
			return this.CreateImpl();
		}

		internal abstract ISubsystem CreateImpl();

		[VisibleToOtherModules(new string[] { "UnityEngine.XRModule" })]
		internal IntPtr m_Ptr;
	}
}
