using System;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XAttributeValuePropertyDescriptor : XPropertyDescriptor<XAttribute, string>
	{
		public XAttributeValuePropertyDescriptor()
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
			this.attribute = component as XAttribute;
			if (this.attribute == null)
			{
				return string.Empty;
			}
			return this.attribute.Value;
		}

		public override void SetValue(object component, object value)
		{
			this.attribute = component as XAttribute;
			if (this.attribute == null)
			{
				return;
			}
			this.attribute.Value = value as string;
		}

		protected override void OnChanged(object sender, XObjectChangeEventArgs args)
		{
			if (this.attribute == null)
			{
				return;
			}
			if (args.ObjectChange == XObjectChange.Value)
			{
				this.OnValueChanged(this.attribute, EventArgs.Empty);
			}
		}

		private XAttribute attribute;
	}
}
