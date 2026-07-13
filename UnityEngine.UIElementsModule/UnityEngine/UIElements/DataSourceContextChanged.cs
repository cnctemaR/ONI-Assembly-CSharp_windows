using System;

namespace UnityEngine.UIElements
{
	public readonly struct DataSourceContextChanged
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

		public DataSourceContext previousContext
		{
			get
			{
				return this.m_PreviousContext;
			}
		}

		public DataSourceContext newContext
		{
			get
			{
				return this.m_NewContext;
			}
		}

		internal DataSourceContextChanged(VisualElement element, in BindingId bindingId, in DataSourceContext previousContext, in DataSourceContext newContext)
		{
			this.m_TargetElement = element;
			this.m_BindingId = bindingId;
			this.m_PreviousContext = previousContext;
			this.m_NewContext = newContext;
		}

		private readonly VisualElement m_TargetElement;

		private readonly BindingId m_BindingId;

		private readonly DataSourceContext m_PreviousContext;

		private readonly DataSourceContext m_NewContext;
	}
}
