using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public readonly struct BindingContext
	{
		public VisualElement targetElement
		{
			get
			{
				return this.m_TargetElement;
			}
		}

		public BindingId bindingId
		{
			get
			{
				return this.m_BindingId;
			}
		}

		public PropertyPath dataSourcePath
		{
			get
			{
				return this.m_DataSourcePath;
			}
		}

		public object dataSource
		{
			get
			{
				return this.m_DataSource;
			}
		}

		internal BindingContext(VisualElement targetElement, in BindingId bindingId, in PropertyPath resolvedDataSourcePath, object resolvedDataSource)
		{
			this.m_TargetElement = targetElement;
			this.m_BindingId = bindingId;
			this.m_DataSourcePath = resolvedDataSourcePath;
			this.m_DataSource = resolvedDataSource;
		}

		private readonly VisualElement m_TargetElement;

		private readonly BindingId m_BindingId;

		private readonly PropertyPath m_DataSourcePath;

		private readonly object m_DataSource;
	}
}
