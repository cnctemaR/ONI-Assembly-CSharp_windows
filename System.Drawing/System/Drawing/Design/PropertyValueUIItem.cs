using System;

namespace System.Drawing.Design
{
	public class PropertyValueUIItem
	{
		public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
		{
			this.itemImage = uiItemImage;
			this.handler = handler;
			if (this.itemImage == null)
			{
				throw new ArgumentNullException("uiItemImage");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.tooltip = tooltip;
		}

		public virtual Image Image
		{
			get
			{
				return this.itemImage;
			}
		}

		public virtual PropertyValueUIItemInvokeHandler InvokeHandler
		{
			get
			{
				return this.handler;
			}
		}

		public virtual string ToolTip
		{
			get
			{
				return this.tooltip;
			}
		}

		public virtual void Reset()
		{
		}

		private Image itemImage;

		private PropertyValueUIItemInvokeHandler handler;

		private string tooltip;
	}
}
