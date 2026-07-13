using System;

namespace UnityEngine.UIElements.Layout
{
	internal readonly struct LayoutConfig
	{
		public static LayoutConfig Undefined
		{
			get
			{
				return new LayoutConfig(default(LayoutDataAccess), LayoutHandle.Undefined);
			}
		}

		internal LayoutConfig(LayoutDataAccess access, LayoutHandle handle)
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

		public ref float PointScaleFactor
		{
			get
			{
				return ref this.m_Access.GetConfigData(this.m_Handle).PointScaleFactor;
			}
		}

		public ref bool ShouldLog
		{
			get
			{
				return ref this.m_Access.GetConfigData(this.m_Handle).ShouldLog;
			}
		}

		public LayoutMeasureFunction Measure
		{
			get
			{
				return this.m_Access.GetMeasureFunction(this.m_Handle);
			}
			set
			{
				this.m_Access.SetMeasureFunction(this.m_Handle, value);
			}
		}

		public LayoutBaselineFunction Baseline
		{
			get
			{
				return this.m_Access.GetBaselineFunction(this.m_Handle);
			}
			set
			{
				this.m_Access.SetBaselineFunction(this.m_Handle, value);
			}
		}

		private readonly LayoutDataAccess m_Access;

		private readonly LayoutHandle m_Handle;
	}
}
