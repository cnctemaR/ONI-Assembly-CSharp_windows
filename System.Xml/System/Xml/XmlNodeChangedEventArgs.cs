using System;

namespace System.Xml
{
	public class XmlNodeChangedEventArgs : EventArgs
	{
		public XmlNodeChangedEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			this.node = node;
			this.oldParent = oldParent;
			this.newParent = newParent;
			this.action = action;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public XmlNodeChangedAction Action
		{
			get
			{
				return this.action;
			}
		}

		public XmlNode Node
		{
			get
			{
				return this.node;
			}
		}

		public XmlNode OldParent
		{
			get
			{
				return this.oldParent;
			}
		}

		public XmlNode NewParent
		{
			get
			{
				return this.newParent;
			}
		}

		public string OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public string NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		private XmlNodeChangedAction action;

		private XmlNode node;

		private XmlNode oldParent;

		private XmlNode newParent;

		private string oldValue;

		private string newValue;
	}
}
