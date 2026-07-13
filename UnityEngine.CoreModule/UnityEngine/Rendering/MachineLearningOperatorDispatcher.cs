using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	[global::System.Runtime.CompilerServices.NullableContext(1)]
	[global::System.Runtime.CompilerServices.Nullable(0)]
	public static class MachineLearningOperatorDispatcher
	{
		internal unsafe static void Identity_Internal([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
		{
			checked
			{
				IntPtr intPtr = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr = X.m_Ptr;
				Span<IntPtr> span = new Span<IntPtr>(intPtr, 1);
				Span<IntPtr> span2 = span;
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr2 = O.m_Ptr;
				span = new Span<IntPtr>(intPtr2, 1);
				Span<IntPtr> span3 = span;
				MachineLearningOperatorDispatcher.RecordDispatch(cb, op, span2, span3);
			}
		}

		internal unsafe static void Gemm_Internal([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer Y, [global::System.Runtime.CompilerServices.Nullable(2)] ComputeBuffer Z, ComputeBuffer O)
		{
			IntPtr intPtr;
			checked
			{
				intPtr = stackalloc byte[unchecked((UIntPtr)3) * (UIntPtr)sizeof(IntPtr)];
				*intPtr = X.m_Ptr;
			}
			*(intPtr + (IntPtr)sizeof(IntPtr)) = Y.m_Ptr;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(IntPtr)) = ((Z != null) ? Z.m_Ptr : IntPtr.Zero);
			Span<IntPtr> span = new Span<IntPtr>(intPtr, 3);
			Span<IntPtr> span2 = span;
			checked
			{
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr2 = O.m_Ptr;
				span = new Span<IntPtr>(intPtr2, 1);
				Span<IntPtr> span3 = span;
				MachineLearningOperatorDispatcher.RecordDispatch(cb, op, span2, span3);
			}
		}

		internal unsafe static void Conv_Internal([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer K, [global::System.Runtime.CompilerServices.Nullable(2)] ComputeBuffer B, ComputeBuffer O)
		{
			IntPtr intPtr;
			checked
			{
				intPtr = stackalloc byte[unchecked((UIntPtr)3) * (UIntPtr)sizeof(IntPtr)];
				*intPtr = X.m_Ptr;
			}
			*(intPtr + (IntPtr)sizeof(IntPtr)) = K.m_Ptr;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(IntPtr)) = ((B != null) ? B.m_Ptr : IntPtr.Zero);
			Span<IntPtr> span = new Span<IntPtr>(intPtr, 3);
			Span<IntPtr> span2 = span;
			checked
			{
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr2 = O.m_Ptr;
				span = new Span<IntPtr>(intPtr2, 1);
				Span<IntPtr> span3 = span;
				MachineLearningOperatorDispatcher.RecordDispatch(cb, op, span2, span3);
			}
		}

		internal unsafe static void Reduce_Internal([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
		{
			checked
			{
				IntPtr intPtr = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr = X.m_Ptr;
				Span<IntPtr> span = new Span<IntPtr>(intPtr, 1);
				Span<IntPtr> span2 = span;
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				*intPtr2 = O.m_Ptr;
				span = new Span<IntPtr>(intPtr2, 1);
				Span<IntPtr> span3 = span;
				MachineLearningOperatorDispatcher.RecordDispatch(cb, op, span2, span3);
			}
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static void RecordDispatch(CommandBuffer cb, MachineLearningOperator op, ReadOnlySpan<IntPtr> inputs, ReadOnlySpan<IntPtr> outputs)
		{
			bool flag = cb != null;
			if (flag)
			{
				cb.SetMachineLearningOperatorTensors(op, inputs, outputs);
				cb.DispatchMachineLearningOperator(op);
			}
			else
			{
				MachineLearningOperator.ResetInputTensors_Internal(op.m_Ptr);
				ReadOnlySpan<IntPtr> readOnlySpan = inputs;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					IntPtr intPtr = *readOnlySpan[i];
					MachineLearningOperator.AddInputTensor_Internal(op.m_Ptr, intPtr);
				}
				MachineLearningOperator.ResetOutputTensors_Internal(op.m_Ptr);
				ReadOnlySpan<IntPtr> readOnlySpan2 = outputs;
				for (int j = 0; j < readOnlySpan2.Length; j++)
				{
					IntPtr intPtr2 = *readOnlySpan2[j];
					MachineLearningOperator.AddOutputTensor_Internal(op.m_Ptr, intPtr2);
				}
				MachineLearningOperator.Dispatch_Internal(op.m_Ptr);
			}
		}

		public static void Identity([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
		{
			MachineLearningOperatorDispatcher.Identity_Internal(cb, op, X, O);
		}

		public static void Gemm([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer Y, [global::System.Runtime.CompilerServices.Nullable(2)] ComputeBuffer Z, ComputeBuffer O)
		{
			MachineLearningOperatorDispatcher.Gemm_Internal(cb, op, X, Y, Z, O);
		}

		public static void Conv([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer K, [global::System.Runtime.CompilerServices.Nullable(2)] ComputeBuffer B, ComputeBuffer O)
		{
			MachineLearningOperatorDispatcher.Conv_Internal(cb, op, X, K, B, O);
		}

		public static void Reduce([global::System.Runtime.CompilerServices.Nullable(2)] CommandBuffer cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
		{
			MachineLearningOperatorDispatcher.Reduce_Internal(cb, op, X, O);
		}
	}
}
