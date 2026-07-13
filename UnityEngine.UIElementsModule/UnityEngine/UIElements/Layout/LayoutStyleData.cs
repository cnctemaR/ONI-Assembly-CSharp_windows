using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.Layout
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/UIElements/Core/Layout/Native/LayoutModel.h")]
	internal struct LayoutStyleData
	{
		public static LayoutStyleData Default = new LayoutStyleData
		{
			Direction = LayoutDirection.Inherit,
			FlexDirection = LayoutFlexDirection.Column,
			JustifyContent = LayoutJustify.FlexStart,
			AlignContent = LayoutAlign.Auto,
			AlignItems = LayoutAlign.Stretch,
			AlignSelf = LayoutAlign.Auto,
			PositionType = LayoutPositionType.Relative,
			AspectRatio = float.NaN,
			FlexWrap = LayoutWrap.NoWrap,
			Overflow = LayoutOverflow.Visible,
			Display = LayoutDisplay.Flex,
			FlexGrow = float.NaN,
			FlexShrink = float.NaN,
			FlexBasis = LayoutValue.Auto(),
			border = LayoutDefaults.EdgeValuesUnit,
			position = LayoutDefaults.EdgeValuesUnit,
			margin = LayoutDefaults.EdgeValuesUnit,
			padding = LayoutDefaults.EdgeValuesUnit,
			dimensions = LayoutDefaults.DimensionValuesAutoUnit,
			minDimensions = LayoutDefaults.DimensionValuesUnit
		};

		public LayoutDirection Direction;

		public LayoutFlexDirection FlexDirection;

		public LayoutJustify JustifyContent;

		public LayoutAlign AlignContent;

		public LayoutAlign AlignItems;

		public LayoutAlign AlignSelf;

		public LayoutPositionType PositionType;

		public float AspectRatio;

		public LayoutWrap FlexWrap;

		public LayoutOverflow Overflow;

		public LayoutDisplay Display;

		public float FlexGrow;

		public float FlexShrink;

		public LayoutValue FlexBasis;

		public FixedBuffer9<LayoutValue> border;

		public FixedBuffer9<LayoutValue> position;

		public FixedBuffer9<LayoutValue> margin;

		public FixedBuffer9<LayoutValue> padding;

		public FixedBuffer2<LayoutValue> maxDimensions;

		public FixedBuffer2<LayoutValue> minDimensions;

		public FixedBuffer2<LayoutValue> dimensions;
	}
}
