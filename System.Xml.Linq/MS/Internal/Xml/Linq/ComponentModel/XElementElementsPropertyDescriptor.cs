using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XElementElementsPropertyDescriptor : XPropertyDescriptor<XElement, IEnumerable<XElement>>
	{
		public XElementElementsPropertyDescriptor()
			: base("Elements")
		{
		}

		public override object GetValue(object component)
		{
			return this.value = new XDeferredAxis<XElement>(delegate(XElement e, XName n)
			{
				if (!(n != null))
				{
					return e.Elements();
				}
				return e.Elements(n);
			}, component as XElement, null);
		}

		protected override void OnChanged(object sender, XObjectChangeEventArgs args)
		{
			if (this.value == null)
			{
				return;
			}
			switch (args.ObjectChange)
			{
			case XObjectChange.Add:
			{
				XElement xelement = sender as XElement;
				if (xelement != null && this.value.element == xelement.parent && (this.value.name == xelement.Name || this.value.name == null))
				{
					this.OnValueChanged(this.value.element, EventArgs.Empty);
					return;
				}
				break;
			}
			case XObjectChange.Remove:
			{
				XElement xelement = sender as XElement;
				if (xelement != null && this.value.element == this.changeState as XContainer && (this.value.name == xelement.Name || this.value.name == null))
				{
					this.changeState = null;
					this.OnValueChanged(this.value.element, EventArgs.Empty);
					return;
				}
				break;
			}
			case XObjectChange.Name:
			{
				XElement xelement = sender as XElement;
				if (xelement != null && this.value.element == xelement.parent && this.value.name != null && (this.value.name == xelement.Name || this.value.name == this.changeState as XName))
				{
					this.changeState = null;
					this.OnValueChanged(this.value.element, EventArgs.Empty);
				}
				break;
			}
			default:
				return;
			}
		}

		protected override void OnChanging(object sender, XObjectChangeEventArgs args)
		{
			if (this.value == null)
			{
				return;
			}
			XObjectChange objectChange = args.ObjectChange;
			XElement xelement;
			if (objectChange == XObjectChange.Remove)
			{
				xelement = sender as XElement;
				this.changeState = ((xelement != null) ? xelement.parent : null);
				return;
			}
			if (objectChange != XObjectChange.Name)
			{
				return;
			}
			xelement = sender as XElement;
			this.changeState = ((xelement != null) ? xelement.Name : null);
		}

		private XDeferredAxis<XElement> value;

		private object changeState;
	}
}
