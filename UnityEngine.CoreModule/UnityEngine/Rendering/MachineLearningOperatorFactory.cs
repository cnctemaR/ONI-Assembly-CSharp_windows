using System;

namespace UnityEngine.Rendering
{
	public static class MachineLearningOperatorFactory
	{
		private unsafe static MachineLearningOperator Identity_Internal(MachineLearningContext context, in MachineLearningOperatorFactory.IdentityDescriptor desc)
		{
			checked
			{
				IntPtr intPtr = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr = desc.X;
				Span<MachineLearningTensorDescriptor> span = new Span<MachineLearningTensorDescriptor>(intPtr, 1);
				Span<MachineLearningTensorDescriptor> span2 = span;
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr2 = desc.O;
				span = new Span<MachineLearningTensorDescriptor>(intPtr2, 1);
				Span<MachineLearningTensorDescriptor> span3 = span;
				MachineLearningOperator.IdentityAttributes identityAttributes = default(MachineLearningOperator.IdentityAttributes);
				identityAttributes.type = MachineLearningOperatorType.Identity;
				return context.BuildIdentity_Internal(span2, span3, identityAttributes);
			}
		}

		private unsafe static MachineLearningOperator Gemm_Internal(MachineLearningContext context, in MachineLearningOperatorFactory.GemmDescriptor desc)
		{
			IntPtr intPtr;
			checked
			{
				intPtr = stackalloc byte[unchecked((UIntPtr)3) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr = desc.X;
			}
			*(intPtr + (IntPtr)sizeof(MachineLearningTensorDescriptor)) = desc.Y;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(MachineLearningTensorDescriptor)) = desc.Z;
			Span<MachineLearningTensorDescriptor> span = new Span<MachineLearningTensorDescriptor>(intPtr, 3);
			Span<MachineLearningTensorDescriptor> span2 = span;
			checked
			{
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr2 = desc.O;
				span = new Span<MachineLearningTensorDescriptor>(intPtr2, 1);
				Span<MachineLearningTensorDescriptor> span3 = span;
				MachineLearningOperator.GemmAttributes gemmAttributes = MachineLearningOperator.ToGemmAttributes(desc.transposeX, desc.transposeY, desc.alpha, desc.beta, desc.fusedActivation);
				return context.BuildGemm_Internal(span2, span3, gemmAttributes);
			}
		}

		private unsafe static MachineLearningOperator Conv_Internal(MachineLearningContext context, in MachineLearningOperatorFactory.ConvDescriptor desc)
		{
			IntPtr intPtr;
			checked
			{
				intPtr = stackalloc byte[unchecked((UIntPtr)3) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr = desc.X;
			}
			*(intPtr + (IntPtr)sizeof(MachineLearningTensorDescriptor)) = desc.K;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(MachineLearningTensorDescriptor)) = desc.B;
			Span<MachineLearningTensorDescriptor> span = new Span<MachineLearningTensorDescriptor>(intPtr, 3);
			Span<MachineLearningTensorDescriptor> span2 = span;
			checked
			{
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr2 = desc.O;
				span = new Span<MachineLearningTensorDescriptor>(intPtr2, 1);
				Span<MachineLearningTensorDescriptor> span3 = span;
				MachineLearningOperator.ConvAttributes convAttributes = MachineLearningOperator.ToConvAttributes(desc.groups, desc.strides, desc.pads, desc.dilations, desc.fusedActivation);
				return context.BuildConv_Internal(span2, span3, convAttributes);
			}
		}

		private unsafe static MachineLearningOperator Reduce_Internal(MachineLearningContext context, in MachineLearningOperatorFactory.ReduceDescriptor desc)
		{
			checked
			{
				IntPtr intPtr = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr = desc.X;
				Span<MachineLearningTensorDescriptor> span = new Span<MachineLearningTensorDescriptor>(intPtr, 1);
				Span<MachineLearningTensorDescriptor> span2 = span;
				IntPtr intPtr2 = stackalloc byte[unchecked((UIntPtr)1) * (UIntPtr)sizeof(MachineLearningTensorDescriptor)];
				*intPtr2 = desc.O;
				span = new Span<MachineLearningTensorDescriptor>(intPtr2, 1);
				Span<MachineLearningTensorDescriptor> span3 = span;
				MachineLearningOperator.ReduceAttributes reduceAttributes = MachineLearningOperator.ToReduceAttributes(desc.axes, (int)desc.X.shape.rank, desc.reduceFunc);
				return context.BuildReduce_Internal(span2, span3, reduceAttributes);
			}
		}

		public static MachineLearningOperator Identity(MachineLearningContext context, in MachineLearningOperatorFactory.IdentityDescriptor desc)
		{
			return MachineLearningOperatorFactory.Identity_Internal(context, in desc);
		}

		public static MachineLearningOperator Gemm(MachineLearningContext context, in MachineLearningOperatorFactory.GemmDescriptor desc)
		{
			return MachineLearningOperatorFactory.Gemm_Internal(context, in desc);
		}

		public static MachineLearningOperator Reduce(MachineLearningContext context, in MachineLearningOperatorFactory.ReduceDescriptor desc)
		{
			return MachineLearningOperatorFactory.Reduce_Internal(context, in desc);
		}

		public static MachineLearningOperator Conv(MachineLearningContext context, in MachineLearningOperatorFactory.ConvDescriptor desc)
		{
			return MachineLearningOperatorFactory.Conv_Internal(context, in desc);
		}

		[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
		public ref struct IdentityDescriptor
		{
			public MachineLearningTensorDescriptor X;

			public MachineLearningTensorDescriptor O;
		}

		[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
		public ref struct GemmDescriptor
		{
			public MachineLearningTensorDescriptor X;

			public MachineLearningTensorDescriptor Y;

			public MachineLearningTensorDescriptor Z;

			public MachineLearningTensorDescriptor O;

			public bool transposeX;

			public bool transposeY;

			public float alpha;

			public float beta;

			public MachineLearningOperatorType fusedActivation;
		}

		[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
		public ref struct ConvDescriptor
		{
			public MachineLearningTensorDescriptor X;

			public MachineLearningTensorDescriptor K;

			public MachineLearningTensorDescriptor B;

			public MachineLearningTensorDescriptor O;

			public int groups;

			public ReadOnlySpan<int> strides;

			public ReadOnlySpan<int> pads;

			public ReadOnlySpan<int> dilations;

			public MachineLearningOperatorType fusedActivation;
		}

		public ref struct ReduceDescriptor
		{
			public MachineLearningTensorDescriptor X;

			public MachineLearningTensorDescriptor O;

			public MachineLearningOperatorType reduceFunc;

			public ReadOnlySpan<int> axes;
		}
	}
}
