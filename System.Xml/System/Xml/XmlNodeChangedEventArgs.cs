using System;

namespace System.Xml
{
	public class XmlNodeChangedEventArgs : EventArgs
	{
		public XmlNodeChangedEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			this._node = node;
			this._oldParent = oldParent;
			this._newParent = newParent;
			this._oldValue = oldValue;
			this._newValue = newValue;
			this._action = action;
		}

		public XmlNodeChangedAction Action
		{
			get
			{
				return this._action;
			}
		}

		public XmlNode Node
		{
			get
			{
				return this._node;
			}
		}

		public XmlNode OldParent
		{
			get
			{
				return this._oldParent;
			}
		}

		public XmlNode NewParent
		{
			get
			{
				return this._newParent;
			}
		}

		public string OldValue
		{
			get
			{
				return (this._oldValue == null) ? this._node.Value : this._oldValue;
			}
		}

		public string NewValue
		{
			get
			{
				return (this._newValue == null) ? this._node.Value : this._newValue;
			}
		}

		private XmlNode _oldParent;

		private XmlNode _newParent;

		private XmlNodeChangedAction _action;

		private XmlNode _node;

		private string _oldValue;

		private string _newValue;
	}
}
