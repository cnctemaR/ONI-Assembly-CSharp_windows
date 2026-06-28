using System;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class PropertyValueUIItem
	{
		public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string tooltip)
		{
			if (uiItemImage == null)
			{
				throw new ArgumentNullException("uiItemImage");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.uiItemImage = uiItemImage;
			this.handler = handler;
			this.tooltip = tooltip;
		}

		public virtual Image Image
		{
			get
			{
				return this.uiItemImage;
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

		private Image uiItemImage;

		private PropertyValueUIItemInvokeHandler handler;

		private string tooltip;
	}
}
