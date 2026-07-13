using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	public struct MachineLearningOperator : IEquatable<MachineLearningOperator>
	{
		[FreeFunction(Name = "MachineLearning_Bindings::AddInputTensorToOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddInputTensor_Internal(IntPtr self, IntPtr tensor);

		[FreeFunction(Name = "MachineLearning_Bindings::ResetInputTensorsOfOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ResetInputTensors_Internal(IntPtr self);

		[FreeFunction(Name = "MachineLearning_Bindings::AddOutputTensorToOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddOutputTensor_Internal(IntPtr self, IntPtr tensor);

		[FreeFunction(Name = "MachineLearning_Bindings::ResetOutputTensorsOfOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ResetOutputTensors_Internal(IntPtr self);

		[FreeFunction(Name = "MachineLearning_Bindings::DispatchOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Dispatch_Internal(IntPtr self);

		[FreeFunction(Name = "MachineLearning_Bindings::BuildOperator")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Build_Internal(IntPtr self);

		internal unsafe static MachineLearningOperator.ConvAttributes ToConvAttributes(int groups, ReadOnlySpan<int> strides, ReadOnlySpan<int> pads, ReadOnlySpan<int> dilations, MachineLearningOperatorType fusedActivation)
		{
			MachineLearningOperator.ConvAttributes convAttributes = default(MachineLearningOperator.ConvAttributes);
			convAttributes.type = MachineLearningOperatorType.Conv;
			convAttributes.groups = groups;
			for (int i = 0; i < strides.Length; i++)
			{
				*((ref convAttributes.strides.FixedElementField) + (IntPtr)i * 4) = *strides[i];
			}
			for (int j = 0; j < pads.Length; j++)
			{
				*((ref convAttributes.pads.FixedElementField) + (IntPtr)j * 4) = *pads[j];
			}
			for (int k = 0; k < dilations.Length; k++)
			{
				*((ref convAttributes.dilations.FixedElementField) + (IntPtr)k * 4) = *dilations[k];
			}
			convAttributes.fusedActivation = fusedActivation;
			return convAttributes;
		}

		internal unsafe static MachineLearningOperator.ReduceAttributes ToReduceAttributes(ReadOnlySpan<int> axes, int dimensionCount, MachineLearningOperatorType reduceFunc)
		{
			MachineLearningOperator.ReduceAttributes reduceAttributes = default(MachineLearningOperator.ReduceAttributes);
			reduceAttributes.type = reduceFunc;
			reduceAttributes.axes = 0U;
			ReadOnlySpan<int> readOnlySpan = axes;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				int num = *readOnlySpan[i];
				reduceAttributes.axes |= 1U << ((num >= 0) ? num : (num + dimensionCount));
			}
			return reduceAttributes;
		}

		internal static MachineLearningOperator.GemmAttributes ToGemmAttributes(bool transposeA, bool transposeB, float alpha, float beta, MachineLearningOperatorType fusedActivation)
		{
			return new MachineLearningOperator.GemmAttributes
			{
				type = MachineLearningOperatorType.Gemm,
				transposeA = (transposeA ? 1 : 0),
				transposeB = (transposeB ? 1 : 0),
				alpha = alpha,
				beta = beta,
				fusedActivation = fusedActivation
			};
		}

		public bool Equals(MachineLearningOperator other)
		{
			return this.m_Ptr.Equals(other.m_Ptr);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is MachineLearningOperator)
			{
				MachineLearningOperator machineLearningOperator = (MachineLearningOperator)obj;
				flag = this.Equals(machineLearningOperator);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_Ptr.GetHashCode();
		}

		public bool IsValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		private const int kMaxConvolutionRank = 3;

		internal IntPtr m_Ptr;

		internal struct IdentityAttributes
		{
			public MachineLearningOperatorType type;
		}

		internal struct ConvAttributes
		{
			public MachineLearningOperatorType type;

			[FixedBuffer(typeof(int), 6)]
			public MachineLearningOperator.ConvAttributes.<pads>e__FixedBuffer pads;

			[FixedBuffer(typeof(int), 3)]
			public MachineLearningOperator.ConvAttributes.<dilations>e__FixedBuffer dilations;

			[FixedBuffer(typeof(int), 3)]
			public MachineLearningOperator.ConvAttributes.<strides>e__FixedBuffer strides;

			public int groups;

			public MachineLearningOperatorType fusedActivation;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <dilations>e__FixedBuffer
			{
				public int FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 24)]
			public struct <pads>e__FixedBuffer
			{
				public int FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <strides>e__FixedBuffer
			{
				public int FixedElementField;
			}
		}

		internal struct ReduceAttributes
		{
			public MachineLearningOperatorType type;

			public uint axes;
		}

		internal struct GemmAttributes
		{
			public MachineLearningOperatorType type;

			public int transposeA;

			public int transposeB;

			public float alpha;

			public float beta;

			public MachineLearningOperatorType fusedActivation;
		}
	}
}
