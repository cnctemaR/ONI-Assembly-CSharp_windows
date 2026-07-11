using System;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XElementValuePropertyDescriptor : XPropertyDescriptor<XElement, string>
	{
		public XElementValuePropertyDescriptor()
			: base("Value")
		{
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override object GetValue(object component)
		{
			this.element = component as XElement;
			if (this.element == null)
			{
				return string.Empty;
			}
			return this.element.Value;
		}

		public override void SetValue(object component, object value)
		{
			this.element = component as XElement;
			if (this.element == null)
			{
				return;
			}
			this.element.Value = value as string;
		}

		protected override void OnChanged(object sender, XObjectChangeEventArgs args)
		{
			if (this.element == null)
			{
				return;
			}
			XObjectChange objectChange = args.ObjectChange;
			if (objectChange > XObjectChange.Remove)
			{
				if (objectChange != XObjectChange.Value)
				{
					return;
				}
				if (sender is XText)
				{
					this.OnValueChanged(this.element, EventArgs.Empty);
				}
			}
			else if (sender is XElement || sender is XText)
			{
				this.OnValueChanged(this.element, EventArgs.Empty);
				return;
			}
		}

		private XElement element;
	}
}
