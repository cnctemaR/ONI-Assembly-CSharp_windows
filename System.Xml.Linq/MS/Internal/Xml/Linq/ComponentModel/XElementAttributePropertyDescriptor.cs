using System;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XElementAttributePropertyDescriptor : XPropertyDescriptor<XElement, object>
	{
		public XElementAttributePropertyDescriptor()
			: base("Attribute")
		{
		}

		public override object GetValue(object component)
		{
			return this.value = new XDeferredSingleton<XAttribute>((XElement e, XName n) => e.Attribute(n), component as XElement, null);
		}

		protected override void OnChanged(object sender, XObjectChangeEventArgs args)
		{
			if (this.value == null)
			{
				return;
			}
			XObjectChange objectChange = args.ObjectChange;
			if (objectChange != XObjectChange.Add)
			{
				if (objectChange != XObjectChange.Remove)
				{
					return;
				}
				XAttribute xattribute = sender as XAttribute;
				if (xattribute != null && this.changeState == xattribute)
				{
					this.changeState = null;
					this.OnValueChanged(this.value.element, EventArgs.Empty);
				}
			}
			else
			{
				XAttribute xattribute = sender as XAttribute;
				if (xattribute != null && this.value.element == xattribute.parent && this.value.name == xattribute.Name)
				{
					this.OnValueChanged(this.value.element, EventArgs.Empty);
					return;
				}
			}
		}

		protected override void OnChanging(object sender, XObjectChangeEventArgs args)
		{
			if (this.value == null)
			{
				return;
			}
			XObjectChange objectChange = args.ObjectChange;
			if (objectChange == XObjectChange.Remove)
			{
				XAttribute xattribute = sender as XAttribute;
				this.changeState = ((xattribute != null && this.value.element == xattribute.parent && this.value.name == xattribute.Name) ? xattribute : null);
			}
		}

		private XDeferredSingleton<XAttribute> value;

		private XAttribute changeState;
	}
}
