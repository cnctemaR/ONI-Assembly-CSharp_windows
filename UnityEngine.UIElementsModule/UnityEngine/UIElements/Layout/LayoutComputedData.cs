using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutComputedData
	{
		public unsafe static LayoutComputedData Default
		{
			get
			{
				LayoutComputedData layoutComputedData = new LayoutComputedData
				{
					Direction = LayoutDirection.Inherit,
					ComputedFlexBasisGeneration = 0U,
					ComputedFlexBasis = float.NaN,
					HadOverflow = false,
					GenerationCount = 0U,
					LastParentDirection = (LayoutDirection)(-1),
					LastPointScaleFactor = 1f
				};
				layoutComputedData.Dimensions.FixedElementField = LayoutDefaults.DimensionValues[0];
				*((ref layoutComputedData.Dimensions.FixedElementField) + 4) = LayoutDefaults.DimensionValues[1];
				layoutComputedData.MeasuredDimensions.FixedElementField = LayoutDefaults.DimensionValues[0];
				*((ref layoutComputedData.MeasuredDimensions.FixedElementField) + 4) = LayoutDefaults.DimensionValues[1];
				return layoutComputedData;
			}
		}

		public unsafe float* MarginBuffer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (float* ptr = &this.Margin.FixedElementField)
				{
					return ptr;
				}
			}
		}

		public unsafe float* BorderBuffer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (float* ptr = &this.Border.FixedElementField)
				{
					return ptr;
				}
			}
		}

		public unsafe float* PaddingBuffer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (float* ptr = &this.Padding.FixedElementField)
				{
					return ptr;
				}
			}
		}

		[FixedBuffer(typeof(float), 4)]
		public LayoutComputedData.<Position>e__FixedBuffer Position;

		[FixedBuffer(typeof(float), 2)]
		public LayoutComputedData.<Dimensions>e__FixedBuffer Dimensions;

		[FixedBuffer(typeof(float), 6)]
		public LayoutComputedData.<Margin>e__FixedBuffer Margin;

		[FixedBuffer(typeof(float), 6)]
		public LayoutComputedData.<Border>e__FixedBuffer Border;

		[FixedBuffer(typeof(float), 6)]
		public LayoutComputedData.<Padding>e__FixedBuffer Padding;

		public LayoutDirection Direction;

		public uint ComputedFlexBasisGeneration;

		public float ComputedFlexBasis;

		public bool HadOverflow;

		public uint GenerationCount;

		public LayoutDirection LastParentDirection;

		public float LastPointScaleFactor;

		[FixedBuffer(typeof(float), 2)]
		public LayoutComputedData.<MeasuredDimensions>e__FixedBuffer MeasuredDimensions;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <Border>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <Dimensions>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <Margin>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <MeasuredDimensions>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <Padding>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Position>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
