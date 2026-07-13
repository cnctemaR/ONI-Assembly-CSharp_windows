using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningContext.h")]
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperator.h")]
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperatorAttributes.h")]
	public class MachineLearningContext : IDisposable
	{
		[FreeFunction(Name = "MachineLearning_Bindings::CreateContext")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateContext();

		[FreeFunction(Name = "MachineLearning_Bindings::DestroyContext")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyContext(IntPtr op);

		[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<IdentityAttributes>", HasExplicitThis = true)]
		internal unsafe MachineLearningOperator BuildIdentity_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.IdentityAttributes attributes)
		{
			IntPtr intPtr = MachineLearningContext.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
			fixed (MachineLearningTensorDescriptor* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
				MachineLearningOperator machineLearningOperator;
				fixed (MachineLearningTensorDescriptor* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					MachineLearningContext.BuildIdentity_Internal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2, ref attributes, out machineLearningOperator);
					ptr = null;
				}
				return machineLearningOperator;
			}
		}

		[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<ConvAttributes>", HasExplicitThis = true)]
		internal unsafe MachineLearningOperator BuildConv_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.ConvAttributes attributes)
		{
			IntPtr intPtr = MachineLearningContext.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
			fixed (MachineLearningTensorDescriptor* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
				MachineLearningOperator machineLearningOperator;
				fixed (MachineLearningTensorDescriptor* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					MachineLearningContext.BuildConv_Internal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2, ref attributes, out machineLearningOperator);
					ptr = null;
				}
				return machineLearningOperator;
			}
		}

		[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<ReduceAttributes>", HasExplicitThis = true)]
		internal unsafe MachineLearningOperator BuildReduce_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.ReduceAttributes attributes)
		{
			IntPtr intPtr = MachineLearningContext.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
			fixed (MachineLearningTensorDescriptor* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
				MachineLearningOperator machineLearningOperator;
				fixed (MachineLearningTensorDescriptor* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					MachineLearningContext.BuildReduce_Internal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2, ref attributes, out machineLearningOperator);
					ptr = null;
				}
				return machineLearningOperator;
			}
		}

		[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<GemmAttributes>", HasExplicitThis = true)]
		internal unsafe MachineLearningOperator BuildGemm_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.GemmAttributes attributes)
		{
			IntPtr intPtr = MachineLearningContext.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
			fixed (MachineLearningTensorDescriptor* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
				MachineLearningOperator machineLearningOperator;
				fixed (MachineLearningTensorDescriptor* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					MachineLearningContext.BuildGemm_Internal_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2, ref attributes, out machineLearningOperator);
					ptr = null;
				}
				return machineLearningOperator;
			}
		}

		public MachineLearningContext()
		{
			this.m_Ptr = MachineLearningContext.CreateContext();
		}

		public void Dispose()
		{
			MachineLearningContext.DestroyContext(this.m_Ptr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BuildIdentity_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.IdentityAttributes attributes, out MachineLearningOperator ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BuildConv_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.ConvAttributes attributes, out MachineLearningOperator ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BuildReduce_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.ReduceAttributes attributes, out MachineLearningOperator ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BuildGemm_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.GemmAttributes attributes, out MachineLearningOperator ret);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(MachineLearningContext obj)
			{
				return obj.m_Ptr;
			}
		}
	}
}
