using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutNodeData
	{
		public bool HasNewLayout
		{
			get
			{
				return (this.Status & LayoutNodeData.FlexStatus.HasNewLayout) == LayoutNodeData.FlexStatus.HasNewLayout;
			}
			set
			{
				this.Status = (value ? (this.Status | LayoutNodeData.FlexStatus.HasNewLayout) : (this.Status & ~LayoutNodeData.FlexStatus.HasNewLayout));
			}
		}

		public bool IsDirty
		{
			get
			{
				return (this.Status & LayoutNodeData.FlexStatus.IsDirty) == LayoutNodeData.FlexStatus.IsDirty;
			}
			set
			{
				this.Status = (value ? (this.Status | LayoutNodeData.FlexStatus.IsDirty) : (this.Status & ~LayoutNodeData.FlexStatus.IsDirty));
			}
		}

		public bool UsesMeasure
		{
			get
			{
				return (this.Status & LayoutNodeData.FlexStatus.UsesMeasure) == LayoutNodeData.FlexStatus.UsesMeasure;
			}
			set
			{
				this.Status = (value ? (this.Status | LayoutNodeData.FlexStatus.UsesMeasure) : (this.Status & ~LayoutNodeData.FlexStatus.UsesMeasure));
			}
		}

		public bool UsesBaseline
		{
			get
			{
				return (this.Status & LayoutNodeData.FlexStatus.UsesBaseline) == LayoutNodeData.FlexStatus.UsesBaseline;
			}
			set
			{
				this.Status = (value ? (this.Status | LayoutNodeData.FlexStatus.UsesBaseline) : (this.Status & ~LayoutNodeData.FlexStatus.UsesBaseline));
			}
		}

		public FixedBuffer2<LayoutValue> ResolvedDimensions;

		private float TargetSize;

		public int ManagedOwnerIndex;

		public int LineIndex;

		public LayoutHandle Config;

		public LayoutHandle Parent;

		public LayoutHandle NextChild;

		public LayoutList<LayoutHandle> Children;

		private LayoutNodeData.FlexStatus Status;

		[Flags]
		internal enum FlexStatus
		{
			IsDirty = 1,
			HasNewLayout = 4,
			DependsOnParentSize = 64,
			UsesMeasure = 128,
			UsesBaseline = 256,
			Fixed = 8,
			MinViolation = 16,
			MaxViolation = 32
		}
	}
}
