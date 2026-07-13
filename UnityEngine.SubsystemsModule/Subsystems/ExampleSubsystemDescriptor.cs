using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Subsystems
{
	[UsedByNativeCode]
	[NativeType(Header = "Modules/Subsystems/Example/ExampleSubsystemDescriptor.h")]
	public class ExampleSubsystemDescriptor : IntegratedSubsystemDescriptor<ExampleSubsystem>
	{
		public bool supportsEditorMode
		{
			get
			{
				IntPtr intPtr = ExampleSubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ExampleSubsystemDescriptor.get_supportsEditorMode_Injected(intPtr);
			}
		}

		public bool disableBackbufferMSAA
		{
			get
			{
				IntPtr intPtr = ExampleSubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ExampleSubsystemDescriptor.get_disableBackbufferMSAA_Injected(intPtr);
			}
		}

		public bool stereoscopicBackbuffer
		{
			get
			{
				IntPtr intPtr = ExampleSubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ExampleSubsystemDescriptor.get_stereoscopicBackbuffer_Injected(intPtr);
			}
		}

		public bool usePBufferEGL
		{
			get
			{
				IntPtr intPtr = ExampleSubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ExampleSubsystemDescriptor.get_usePBufferEGL_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_supportsEditorMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_disableBackbufferMSAA_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_stereoscopicBackbuffer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_usePBufferEGL_Injected(IntPtr _unity_self);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(ExampleSubsystemDescriptor exampleSubsystemDescriptor)
			{
				return exampleSubsystemDescriptor.m_Ptr;
			}
		}
	}
}
