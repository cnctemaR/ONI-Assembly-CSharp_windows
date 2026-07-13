using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutNode : IEquatable<LayoutNode>
	{
		public LayoutDirection LayoutDirection
		{
			get
			{
				return this.Layout.Direction;
			}
		}

		public float LayoutX
		{
			get
			{
				return this.Layout.Position.FixedElementField;
			}
		}

		public unsafe float LayoutY
		{
			get
			{
				return *((ref this.Layout.Position.FixedElementField) + 4);
			}
		}

		public unsafe float LayoutRight
		{
			get
			{
				return *((ref this.Layout.Position.FixedElementField) + (IntPtr)2 * 4);
			}
		}

		public unsafe float LayoutBottom
		{
			get
			{
				return *((ref this.Layout.Position.FixedElementField) + (IntPtr)3 * 4);
			}
		}

		public float LayoutWidth
		{
			get
			{
				return this.Layout.Dimensions.FixedElementField;
			}
		}

		public unsafe float LayoutHeight
		{
			get
			{
				return *((ref this.Layout.Dimensions.FixedElementField) + 4);
			}
		}

		public float LayoutMarginLeft
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.Left);
			}
		}

		public float LayoutMarginTop
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.Top);
			}
		}

		public float LayoutMarginRight
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.Right);
			}
		}

		public float LayoutMarginBottom
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.Bottom);
			}
		}

		public float LayoutMarginStart
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.Start);
			}
		}

		public float LayoutMarginEnd
		{
			get
			{
				return this.GetLayoutValue(this.Layout.MarginBuffer, LayoutEdge.End);
			}
		}

		public float LayoutPaddingLeft
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.Left);
			}
		}

		public float LayoutPaddingTop
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.Top);
			}
		}

		public float LayoutPaddingRight
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.Right);
			}
		}

		public float LayoutPaddingBottom
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.Bottom);
			}
		}

		public float LayoutPaddingStart
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.Start);
			}
		}

		public float LayoutPaddingEnd
		{
			get
			{
				return this.GetLayoutValue(this.Layout.PaddingBuffer, LayoutEdge.End);
			}
		}

		public float LayoutBorderLeft
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.Left);
			}
		}

		public float LayoutBorderTop
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.Top);
			}
		}

		public float LayoutBorderRight
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.Right);
			}
		}

		public float LayoutBorderBottom
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.Bottom);
			}
		}

		public float LayoutBorderStart
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.Start);
			}
		}

		public float LayoutBorderEnd
		{
			get
			{
				return this.GetLayoutValue(this.Layout.BorderBuffer, LayoutEdge.End);
			}
		}

		public float ComputedFlexBasis
		{
			get
			{
				return this.Layout.ComputedFlexBasis;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe float GetLayoutValue(float* buffer, LayoutEdge edge)
		{
			if (!true)
			{
			}
			float num;
			if (edge != LayoutEdge.Left)
			{
				if (edge != LayoutEdge.Right)
				{
					num = buffer[(IntPtr)edge];
				}
				else
				{
					num = ((this.Layout.Direction == LayoutDirection.RTL) ? buffer[4] : buffer[5]);
				}
			}
			else
			{
				num = ((this.Layout.Direction == LayoutDirection.RTL) ? buffer[5] : buffer[4]);
			}
			if (!true)
			{
			}
			return num;
		}

		public LayoutNode Parent
		{
			get
			{
				return new LayoutNode(this.m_Access, this.m_Access.GetNodeData(this.m_Handle).Parent);
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).Parent = value.m_Handle;
			}
		}

		public LayoutNode NextChild
		{
			get
			{
				return new LayoutNode(this.m_Access, this.m_Access.GetNodeData(this.m_Handle).NextChild);
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).NextChild = value.m_Handle;
			}
		}

		private LayoutList<LayoutHandle> Children
		{
			get
			{
				return this.m_Access.GetNodeData(this.m_Handle).Children;
			}
		}

		public int Count
		{
			get
			{
				return this.Children.IsCreated ? this.Children.Count : 0;
			}
		}

		public unsafe LayoutNode this[int index]
		{
			get
			{
				return new LayoutNode(this.m_Access, *this.Children[index]);
			}
			set
			{
				*this.Children[index] = value.Handle;
			}
		}

		public void AddChild(LayoutNode child)
		{
			this.Insert(this.Count, child);
		}

		public void RemoveChild(LayoutNode child)
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			Assert.IsTrue(nodeData.Children.IsCreated);
			int num = nodeData.Children.IndexOf(child.m_Handle);
			bool flag = num >= 0;
			if (flag)
			{
				this.RemoveAt(num);
			}
		}

		public int IndexOf(LayoutNode child)
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			bool isCreated = nodeData.Children.IsCreated;
			int num;
			if (isCreated)
			{
				num = nodeData.Children.IndexOf(child.m_Handle);
			}
			else
			{
				num = -1;
			}
			return num;
		}

		public void Insert(int index, LayoutNode child)
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			bool flag = !nodeData.Children.IsCreated;
			if (flag)
			{
				nodeData.Children = new LayoutList<LayoutHandle>(4);
			}
			nodeData.Children.Insert(index, child.Handle);
			child.Parent = this;
			this.MarkDirty();
		}

		public unsafe void RemoveAt(int index)
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			Assert.IsTrue(nodeData.Children.IsCreated);
			bool flag = (ulong)index >= (ulong)((long)nodeData.Children.Count);
			if (flag)
			{
				throw new ArgumentOutOfRangeException();
			}
			LayoutHandle layoutHandle = *nodeData.Children[index];
			ref LayoutNodeData nodeData2 = ref this.m_Access.GetNodeData(layoutHandle);
			bool flag2 = nodeData2.Parent.Equals(this.m_Handle);
			nodeData2.Parent = LayoutHandle.Undefined;
			nodeData.Children.RemoveAt(index);
			bool flag3 = flag2;
			if (flag3)
			{
				this.MarkDirty();
			}
		}

		public void Clear()
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			bool flag = !nodeData.Children.IsCreated;
			if (!flag)
			{
				while (nodeData.Children.Count > 0)
				{
					this.RemoveAt(nodeData.Children.Count - 1);
				}
			}
		}

		public LayoutNode.Enumerator GetEnumerator()
		{
			return new LayoutNode.Enumerator(this.m_Access, this.Children);
		}

		public LayoutDirection StyleDirection
		{
			get
			{
				return this.Style.Direction;
			}
			set
			{
				bool flag = this.Style.Direction == value;
				if (!flag)
				{
					this.Style.Direction = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutFlexDirection FlexDirection
		{
			get
			{
				return this.Style.FlexDirection;
			}
			set
			{
				bool flag = this.Style.FlexDirection == value;
				if (!flag)
				{
					this.Style.FlexDirection = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutJustify JustifyContent
		{
			get
			{
				return this.Style.JustifyContent;
			}
			set
			{
				bool flag = this.Style.JustifyContent == value;
				if (!flag)
				{
					this.Style.JustifyContent = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutDisplay Display
		{
			get
			{
				return this.Style.Display;
			}
			set
			{
				bool flag = this.Style.Display == value;
				if (!flag)
				{
					this.Style.Display = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutAlign AlignItems
		{
			get
			{
				return this.Style.AlignItems;
			}
			set
			{
				bool flag = this.Style.AlignItems == value;
				if (!flag)
				{
					this.Style.AlignItems = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutAlign AlignSelf
		{
			get
			{
				return this.Style.AlignSelf;
			}
			set
			{
				bool flag = this.Style.AlignSelf == value;
				if (!flag)
				{
					this.Style.AlignSelf = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutAlign AlignContent
		{
			get
			{
				return this.Style.AlignContent;
			}
			set
			{
				bool flag = this.Style.AlignContent == value;
				if (!flag)
				{
					this.Style.AlignContent = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutPositionType PositionType
		{
			get
			{
				return this.Style.PositionType;
			}
			set
			{
				bool flag = this.Style.PositionType == value;
				if (!flag)
				{
					this.Style.PositionType = value;
					this.MarkDirty();
				}
			}
		}

		public LayoutWrap Wrap
		{
			get
			{
				return this.Style.FlexWrap;
			}
			set
			{
				bool flag = this.Style.FlexWrap == value;
				if (!flag)
				{
					this.Style.FlexWrap = value;
					this.MarkDirty();
				}
			}
		}

		public float FlexGrow
		{
			get
			{
				return this.Style.FlexGrow;
			}
			set
			{
				this.SetValue(ref this.Style.FlexGrow, value);
			}
		}

		public float FlexShrink
		{
			get
			{
				return this.Style.FlexShrink;
			}
			set
			{
				this.SetValue(ref this.Style.FlexShrink, value);
			}
		}

		public LayoutValue FlexBasis
		{
			get
			{
				return this.Style.FlexBasis;
			}
			set
			{
				this.SetStyleValueUnit(ref this.Style.FlexBasis, value);
			}
		}

		public unsafe LayoutValue Width
		{
			get
			{
				return *this.Style.dimensions[0];
			}
			set
			{
				this.SetStyleValueUnit(this.Style.dimensions[0], value);
			}
		}

		public unsafe LayoutValue Height
		{
			get
			{
				return *this.Style.dimensions[1];
			}
			set
			{
				this.SetStyleValueUnit(this.Style.dimensions[1], value);
			}
		}

		public unsafe LayoutValue MaxWidth
		{
			get
			{
				return *this.Style.maxDimensions[0];
			}
			set
			{
				this.SetStyleValue(this.Style.maxDimensions[0], value);
			}
		}

		public unsafe LayoutValue MaxHeight
		{
			get
			{
				return *this.Style.maxDimensions[1];
			}
			set
			{
				this.SetStyleValue(this.Style.maxDimensions[1], value);
			}
		}

		public unsafe LayoutValue MinWidth
		{
			get
			{
				return *this.Style.minDimensions[0];
			}
			set
			{
				this.SetStyleValue(this.Style.minDimensions[0], value);
			}
		}

		public unsafe LayoutValue MinHeight
		{
			get
			{
				return *this.Style.minDimensions[1];
			}
			set
			{
				this.SetStyleValue(this.Style.minDimensions[1], value);
			}
		}

		public float AspectRatio
		{
			get
			{
				return this.Style.AspectRatio;
			}
			set
			{
				this.SetValue(ref this.Style.AspectRatio, value);
			}
		}

		public LayoutOverflow Overflow
		{
			get
			{
				return this.Style.Overflow;
			}
			set
			{
				bool flag = this.Style.Overflow == value;
				if (!flag)
				{
					this.Style.Overflow = value;
					this.MarkDirty();
				}
			}
		}

		public unsafe LayoutValue Left
		{
			get
			{
				return *this.Style.position[0];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.Left, value);
			}
		}

		public unsafe LayoutValue Top
		{
			get
			{
				return *this.Style.position[1];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.Top, value);
			}
		}

		public unsafe LayoutValue Right
		{
			get
			{
				return *this.Style.position[2];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.Right, value);
			}
		}

		public unsafe LayoutValue Bottom
		{
			get
			{
				return *this.Style.position[3];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.Bottom, value);
			}
		}

		public unsafe LayoutValue Start
		{
			get
			{
				return *this.Style.position[4];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.Start, value);
			}
		}

		public unsafe LayoutValue End
		{
			get
			{
				return *this.Style.position[5];
			}
			set
			{
				this.SetStyleEdgePosition(LayoutEdge.End, value);
			}
		}

		public unsafe LayoutValue MarginLeft
		{
			get
			{
				return *this.Style.margin[0];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Left, value);
			}
		}

		public unsafe LayoutValue MarginTop
		{
			get
			{
				return *this.Style.margin[1];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Top, value);
			}
		}

		public unsafe LayoutValue MarginRight
		{
			get
			{
				return *this.Style.margin[2];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Right, value);
			}
		}

		public unsafe LayoutValue MarginBottom
		{
			get
			{
				return *this.Style.margin[3];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Bottom, value);
			}
		}

		public unsafe LayoutValue MarginStart
		{
			get
			{
				return *this.Style.margin[4];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Start, value);
			}
		}

		public unsafe LayoutValue MarginEnd
		{
			get
			{
				return *this.Style.margin[5];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.End, value);
			}
		}

		public unsafe LayoutValue MarginHorizontal
		{
			get
			{
				return *this.Style.margin[6];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Horizontal, value);
			}
		}

		public unsafe LayoutValue MarginVertical
		{
			get
			{
				return *this.Style.margin[7];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.Vertical, value);
			}
		}

		public unsafe LayoutValue Margin
		{
			get
			{
				return *this.Style.margin[8];
			}
			set
			{
				this.SetStyleEdgeMargin(LayoutEdge.All, value);
			}
		}

		public unsafe LayoutValue PaddingLeft
		{
			get
			{
				return *this.Style.padding[0];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Left, value);
			}
		}

		public unsafe LayoutValue PaddingTop
		{
			get
			{
				return *this.Style.padding[1];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Top, value);
			}
		}

		public unsafe LayoutValue PaddingRight
		{
			get
			{
				return *this.Style.padding[2];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Right, value);
			}
		}

		public unsafe LayoutValue PaddingBottom
		{
			get
			{
				return *this.Style.padding[3];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Bottom, value);
			}
		}

		public unsafe LayoutValue PaddingStart
		{
			get
			{
				return *this.Style.padding[4];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Start, value);
			}
		}

		public unsafe LayoutValue PaddingEnd
		{
			get
			{
				return *this.Style.padding[5];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.End, value);
			}
		}

		public unsafe LayoutValue PaddingHorizontal
		{
			get
			{
				return *this.Style.padding[6];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Horizontal, value);
			}
		}

		public unsafe LayoutValue PaddingVertical
		{
			get
			{
				return *this.Style.padding[7];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.Vertical, value);
			}
		}

		public unsafe LayoutValue Padding
		{
			get
			{
				return *this.Style.padding[8];
			}
			set
			{
				this.SetStyleEdgePadding(LayoutEdge.All, value);
			}
		}

		public float BorderLeftWidth
		{
			get
			{
				return this.Style.border[0].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[0], value);
			}
		}

		public float BorderTopWidth
		{
			get
			{
				return this.Style.border[1].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[1], value);
			}
		}

		public float BorderRightWidth
		{
			get
			{
				return this.Style.border[2].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[2], value);
			}
		}

		public float BorderBottomWidth
		{
			get
			{
				return this.Style.border[3].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[3], value);
			}
		}

		public float BorderStartWidth
		{
			get
			{
				return this.Style.border[4].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[4], value);
			}
		}

		public float BorderEndWidth
		{
			get
			{
				return this.Style.border[5].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[5], value);
			}
		}

		public float BorderWidth
		{
			get
			{
				return this.Style.border[8].Value;
			}
			set
			{
				this.StyleEdgeSetPoint(this.Style.border[8], value);
			}
		}

		private void SetValue(ref float currentValue, float newValue)
		{
			bool flag = currentValue.Equals(newValue);
			if (!flag)
			{
				currentValue = newValue;
				this.MarkDirty();
			}
		}

		private void SetStyleValue(ref LayoutValue currentValue, LayoutValue newValue)
		{
			bool flag = newValue.Unit == LayoutUnit.Percent;
			if (flag)
			{
				this.SetStyleValuePercent(ref currentValue, newValue);
			}
			else
			{
				this.SetStyleValuePoint(ref currentValue, newValue);
			}
		}

		private void SetStyleValueUnit(ref LayoutValue currentValue, LayoutValue newValue)
		{
			bool flag = newValue.Unit == LayoutUnit.Percent;
			if (flag)
			{
				this.SetStyleValuePercent(ref currentValue, newValue);
			}
			else
			{
				bool flag2 = newValue.Unit == LayoutUnit.Auto;
				if (flag2)
				{
					this.SetStyleValueAuto(ref currentValue);
				}
				else
				{
					this.SetStyleValuePoint(ref currentValue, newValue);
				}
			}
		}

		private void SetStyleValuePoint(ref LayoutValue currentValue, LayoutValue newValue)
		{
			bool flag = float.IsNaN(currentValue.Value) && float.IsNaN(newValue.Value) && newValue.Unit == currentValue.Unit;
			if (!flag)
			{
				bool flag2 = currentValue.Value != newValue.Value || currentValue.Unit != LayoutUnit.Point;
				if (flag2)
				{
					bool flag3 = float.IsNaN(newValue.Value);
					if (flag3)
					{
						currentValue = LayoutValue.Auto();
					}
					else
					{
						currentValue = LayoutValue.Point(newValue.Value);
					}
					this.MarkDirty();
				}
			}
		}

		private void SetStyleValuePercent(ref LayoutValue currentValue, LayoutValue newValue)
		{
			bool flag = currentValue.Value != newValue.Value || currentValue.Unit != LayoutUnit.Percent;
			if (flag)
			{
				bool flag2 = float.IsNaN(newValue.Value);
				if (flag2)
				{
					currentValue = LayoutValue.Auto();
				}
				else
				{
					currentValue = newValue;
				}
				this.MarkDirty();
			}
		}

		private void SetStyleValueAuto(ref LayoutValue currentValue)
		{
			bool flag = currentValue.Unit != LayoutUnit.Auto;
			if (flag)
			{
				currentValue = LayoutValue.Auto();
				this.MarkDirty();
			}
		}

		private void SetStyleEdgePosition(LayoutEdge edge, LayoutValue value)
		{
			bool flag = value.Unit == LayoutUnit.Percent;
			if (flag)
			{
				this.StyleEdgeSetPercent(this.Style.position[(int)edge], value.Value);
			}
			else
			{
				this.StyleEdgeSetPoint(this.Style.position[(int)edge], value.Value);
			}
		}

		private void SetStyleEdgeMargin(LayoutEdge edge, LayoutValue value)
		{
			bool flag = value.Unit == LayoutUnit.Percent;
			if (flag)
			{
				this.StyleEdgeSetPercent(this.Style.margin[(int)edge], value.Value);
			}
			else
			{
				bool flag2 = value.Unit == LayoutUnit.Auto;
				if (flag2)
				{
					this.StyleEdgeSetAuto(this.Style.margin[(int)edge]);
				}
				else
				{
					this.StyleEdgeSetPoint(this.Style.margin[(int)edge], value.Value);
				}
			}
		}

		private void SetStyleEdgePadding(LayoutEdge edge, LayoutValue value)
		{
			bool flag = value.Unit == LayoutUnit.Percent;
			if (flag)
			{
				this.StyleEdgeSetPercent(this.Style.padding[(int)edge], value.Value);
			}
			else
			{
				this.StyleEdgeSetPoint(this.Style.padding[(int)edge], value.Value);
			}
		}

		private void StyleEdgeSetPercent(ref LayoutValue value, float newValue)
		{
			bool flag = value.Value != newValue || value.Unit != LayoutUnit.Percent;
			if (flag)
			{
				value = (float.IsNaN(newValue) ? LayoutValue.Undefined() : LayoutValue.Percent(newValue));
				this.MarkDirty();
			}
		}

		private void StyleEdgeSetAuto(ref LayoutValue value)
		{
			bool flag = value.Unit != LayoutUnit.Auto;
			if (flag)
			{
				value = LayoutValue.Auto();
				this.MarkDirty();
			}
		}

		private void StyleEdgeSetPoint(ref LayoutValue value, float newValue)
		{
			bool flag = float.IsNaN(value.Value) && float.IsNaN(newValue);
			if (!flag)
			{
				bool flag2 = value.Value != newValue || value.Unit != LayoutUnit.Point;
				if (flag2)
				{
					value = (float.IsNaN(newValue) ? LayoutValue.Undefined() : LayoutValue.Point(newValue));
					this.MarkDirty();
				}
			}
		}

		public static LayoutNode Undefined
		{
			get
			{
				return new LayoutNode(default(LayoutDataAccess), LayoutHandle.Undefined);
			}
		}

		internal LayoutNode(LayoutDataAccess access, LayoutHandle handle)
		{
			this.m_Access = access;
			this.m_Handle = handle;
		}

		public bool IsUndefined
		{
			get
			{
				return this.m_Handle.Equals(LayoutHandle.Undefined);
			}
		}

		public LayoutHandle Handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public ref LayoutComputedData Layout
		{
			get
			{
				return this.m_Access.GetComputedData(this.m_Handle);
			}
		}

		public ref LayoutStyleData Style
		{
			get
			{
				return this.m_Access.GetStyleData(this.m_Handle);
			}
		}

		internal ref LayoutCacheData Cache
		{
			get
			{
				return this.m_Access.GetCacheData(this.m_Handle);
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.m_Access.GetNodeData(this.m_Handle).IsDirty;
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).IsDirty = value;
			}
		}

		public bool HasNewLayout
		{
			get
			{
				return this.m_Access.GetNodeData(this.m_Handle).HasNewLayout;
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).HasNewLayout = value;
			}
		}

		public bool UsesMeasure
		{
			get
			{
				return this.m_Access.GetNodeData(this.m_Handle).UsesMeasure;
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).UsesMeasure = value;
			}
		}

		public bool UsesBaseline
		{
			get
			{
				return this.m_Access.GetNodeData(this.m_Handle).UsesBaseline;
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).UsesBaseline = value;
			}
		}

		public void SetOwner(VisualElement func)
		{
			this.m_Access.SetOwner(this.m_Handle, func);
		}

		public VisualElement GetOwner()
		{
			return this.m_Access.GetOwner(this.m_Handle);
		}

		public ref int LineIndex
		{
			get
			{
				return ref this.m_Access.GetNodeData(this.m_Handle).LineIndex;
			}
		}

		public LayoutConfig Config
		{
			get
			{
				return new LayoutConfig(this.m_Access, this.m_Access.GetNodeData(this.m_Handle).Config);
			}
			set
			{
				this.m_Access.GetNodeData(this.m_Handle).Config = value.Handle;
			}
		}

		public void MarkDirty()
		{
			bool isDirty = this.IsDirty;
			if (!isDirty)
			{
				this.IsDirty = true;
				this.Layout.ComputedFlexBasis = float.NaN;
				bool flag = !this.Parent.IsUndefined;
				if (flag)
				{
					this.Parent.MarkDirty();
				}
			}
		}

		public void MarkLayoutSeen()
		{
			this.HasNewLayout = false;
		}

		public void CopyFromComputedStyle(ComputedStyle style)
		{
			this.FlexGrow = style.flexGrow;
			this.FlexShrink = style.flexShrink;
			this.FlexBasis = style.flexBasis.ToLayoutValue();
			this.Left = style.left.ToLayoutValue();
			this.Top = style.top.ToLayoutValue();
			this.Right = style.right.ToLayoutValue();
			this.Bottom = style.bottom.ToLayoutValue();
			this.MarginLeft = style.marginLeft.ToLayoutValue();
			this.MarginTop = style.marginTop.ToLayoutValue();
			this.MarginRight = style.marginRight.ToLayoutValue();
			this.MarginBottom = style.marginBottom.ToLayoutValue();
			this.PaddingLeft = style.paddingLeft.ToLayoutValue();
			this.PaddingTop = style.paddingTop.ToLayoutValue();
			this.PaddingRight = style.paddingRight.ToLayoutValue();
			this.PaddingBottom = style.paddingBottom.ToLayoutValue();
			this.BorderLeftWidth = style.borderLeftWidth;
			this.BorderTopWidth = style.borderTopWidth;
			this.BorderRightWidth = style.borderRightWidth;
			this.BorderBottomWidth = style.borderBottomWidth;
			this.Width = style.width.ToLayoutValue();
			this.Height = style.height.ToLayoutValue();
			this.PositionType = (LayoutPositionType)style.position;
			this.Overflow = (LayoutOverflow)style.overflow;
			this.AlignSelf = (LayoutAlign)style.alignSelf;
			this.MaxWidth = style.maxWidth.ToLayoutValue();
			this.MaxHeight = style.maxHeight.ToLayoutValue();
			this.MinWidth = style.minWidth.ToLayoutValue();
			this.MinHeight = style.minHeight.ToLayoutValue();
			this.FlexDirection = (LayoutFlexDirection)style.flexDirection;
			this.AlignContent = (LayoutAlign)style.alignContent;
			this.AlignItems = (LayoutAlign)style.alignItems;
			this.JustifyContent = (LayoutJustify)style.justifyContent;
			this.Wrap = (LayoutWrap)style.flexWrap;
			this.Display = (LayoutDisplay)style.display;
			this.AspectRatio = style.aspectRatio.value;
		}

		public unsafe void CopyStyle(LayoutNode node)
		{
			bool flag = false;
			fixed (LayoutStyleData* style = this.Style)
			{
				LayoutStyleData* ptr = style;
				fixed (LayoutStyleData* style2 = node.Style)
				{
					LayoutStyleData* ptr2 = style2;
					bool flag2 = UnsafeUtility.MemCmp((void*)ptr, (void*)ptr2, (long)UnsafeUtility.SizeOf<LayoutStyleData>()) != 0;
					if (flag2)
					{
						*this.Style = *node.Style;
						flag = true;
					}
				}
			}
			bool flag3 = flag;
			if (flag3)
			{
				this.MarkDirty();
			}
		}

		public void SoftReset()
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			nodeData.HasNewLayout = true;
			ref LayoutCacheData cache = ref this.Cache;
			bool flag = cache.CachedLayout.NextMeasurementCache != null;
			if (flag)
			{
				cache.ClearCachedMeasurements();
			}
		}

		public unsafe void Reset()
		{
			ref LayoutNodeData nodeData = ref this.m_Access.GetNodeData(this.m_Handle);
			Assert.IsTrue(!nodeData.Children.IsCreated || nodeData.Children.Count == 0, "Cannot reset a node which still has children attached");
			nodeData.Parent = default(LayoutHandle);
			nodeData.HasNewLayout = true;
			ref LayoutNodeData ptr = ref nodeData;
			FixedBuffer2<LayoutValue> fixedBuffer = default(FixedBuffer2<LayoutValue>);
			*fixedBuffer[0] = LayoutValue.Undefined();
			*fixedBuffer[1] = LayoutValue.Undefined();
			ptr.ResolvedDimensions = fixedBuffer;
			nodeData.UsesMeasure = false;
			nodeData.UsesBaseline = false;
			this.SetOwner(null);
			*this.Layout = LayoutComputedData.Default;
			*this.Style = LayoutStyleData.Default;
		}

		public bool Equals(LayoutNode other)
		{
			return this.m_Handle.Equals(other.m_Handle);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is LayoutNode)
			{
				LayoutNode layoutNode = (LayoutNode)obj;
				flag = this.Equals(layoutNode);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode();
		}

		public static bool operator ==(LayoutNode lhs, LayoutNode rhs)
		{
			bool isUndefined = lhs.IsUndefined;
			bool flag;
			if (isUndefined)
			{
				bool isUndefined2 = rhs.IsUndefined;
				flag = isUndefined2;
			}
			else
			{
				flag = lhs.Equals(rhs);
			}
			return flag;
		}

		public static bool operator !=(LayoutNode lhs, LayoutNode rhs)
		{
			return !(lhs == rhs);
		}

		public void CalculateLayout(float width = float.NaN, float height = float.NaN)
		{
			LayoutProcessor.CalculateLayout(this, width, height, this.Style.Direction);
		}

		private const int k_DefaultChildCapacity = 4;

		private readonly LayoutDataAccess m_Access;

		private readonly LayoutHandle m_Handle;

		public struct Enumerator : IEnumerator<LayoutNode>, IEnumerator, IDisposable
		{
			public Enumerator(LayoutDataAccess access, LayoutList<LayoutHandle> children)
			{
				this.m_Access = access;
				this.m_Enumerator = children.GetEnumerator();
			}

			public LayoutNode Current
			{
				get
				{
					return new LayoutNode(this.m_Access, this.m_Enumerator.Current);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			public void Reset()
			{
				this.m_Enumerator.Reset();
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			private readonly LayoutDataAccess m_Access;

			private LayoutList<LayoutHandle>.Enumerator m_Enumerator;
		}
	}
}
