using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XElementDescendantsPropertyDescriptor : XPropertyDescriptor<XElement, IEnumerable<XElement>>
	{
		public XElementDescendantsPropertyDescriptor()
			: base("Descendants")
		{
		}

		public override object GetValue(object component)
		{
			return this.value = new XDeferredAxis<XElement>(delegate(XElement e, XName n)
			{
				if (!(n != null))
				{
					return e.Descendants();
				}
				return e.Descendants(n);
			}, component as XElement, null);
		}

		protected override void OnChanged(object sender, XObjectChangeEventArgs args)
		{
			if (this.value == null)
			{
				return;
			}
			XObjectChange objectChange = args.ObjectChange;
			if (objectChange > XObjectChange.Remove)
			{
				if (objectChange != XObjectChange.Name)
				{
					return;
				}
				XElement xelement = sender as XElement;
				if (xelement != null && this.value.element != xelement && this.value.name != null && (this.value.name == xelement.Name || this.value.name == this.changeState))
				{
					this.changeState = null;
					this.OnValueChanged(this.value.element, EventArgs.Empty);
				}
			}
			else
			{
				XElement xelement = sender as XElement;
				if (xelement != null && (this.value.name == xelement.Name || this.value.name == null))
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
			if (objectChange == XObjectChange.Name)
			{
				XElement xelement = sender as XElement;
				this.changeState = ((xelement != null) ? xelement.Name : null);
			}
		}

		private XDeferredAxis<XElement> value;

		private XName changeState;
	}
}
