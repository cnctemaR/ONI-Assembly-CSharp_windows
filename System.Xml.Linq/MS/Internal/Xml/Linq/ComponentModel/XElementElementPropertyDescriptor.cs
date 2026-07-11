using System;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XElementElementPropertyDescriptor : XPropertyDescriptor<XElement, object>
	{
		public XElementElementPropertyDescriptor()
			: base("Element")
		{
		}

		public override object GetValue(object component)
		{
			return this.value = new XDeferredSingleton<XElement>((XElement e, XName n) => e.Element(n), component as XElement, null);
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
				if (xelement != null && this.value.element == xelement.parent && this.value.name == xelement.Name && this.value.element.Element(this.value.name) == xelement)
				{
					this.OnValueChanged(this.value.element, EventArgs.Empty);
					return;
				}
				break;
			}
			case XObjectChange.Remove:
			{
				XElement xelement = sender as XElement;
				if (xelement != null && this.changeState == xelement)
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
				if (xelement != null)
				{
					if (this.value.element == xelement.parent && this.value.name == xelement.Name && this.value.element.Element(this.value.name) == xelement)
					{
						this.OnValueChanged(this.value.element, EventArgs.Empty);
						return;
					}
					if (this.changeState == xelement)
					{
						this.changeState = null;
						this.OnValueChanged(this.value.element, EventArgs.Empty);
					}
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
			if (objectChange - XObjectChange.Remove <= 1)
			{
				XElement xelement = sender as XElement;
				this.changeState = ((xelement != null && this.value.element == xelement.parent && this.value.name == xelement.Name && this.value.element.Element(this.value.name) == xelement) ? xelement : null);
			}
		}

		private XDeferredSingleton<XElement> value;

		private XElement changeState;
	}
}
