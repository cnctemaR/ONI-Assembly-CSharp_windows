using System;

namespace UnityEngine.UIElements
{
	public readonly struct BindablePropertyChangedEventArgs
	{
		public BindablePropertyChangedEventArgs(in BindingId propertyName)
		{
			this.m_PropertyName = propertyName;
		}

		public BindingId propertyName
		{
			get
			{
				return this.m_PropertyName;
			}
		}

		private readonly BindingId m_PropertyName;
	}
}
