using System;
using System.Text;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlCharacterData : XmlLinkedNode
	{
		protected internal XmlCharacterData(string data, XmlDocument doc)
			: base(doc)
		{
			this.data = data;
		}

		public override string Value
		{
			get
			{
				return this.Data;
			}
			set
			{
				this.Data = value;
			}
		}

		public override string InnerText
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		public virtual string Data
		{
			get
			{
				if (this.data != null)
				{
					return this.data;
				}
				return string.Empty;
			}
			set
			{
				XmlNode parentNode = this.ParentNode;
				XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, value, XmlNodeChangedAction.Change);
				if (eventArgs != null)
				{
					this.BeforeEvent(eventArgs);
				}
				this.data = value;
				if (eventArgs != null)
				{
					this.AfterEvent(eventArgs);
				}
			}
		}

		public virtual int Length
		{
			get
			{
				if (this.data != null)
				{
					return this.data.Length;
				}
				return 0;
			}
		}

		public virtual string Substring(int offset, int count)
		{
			int num = ((this.data != null) ? this.data.Length : 0);
			if (num > 0)
			{
				if (num < offset + count)
				{
					count = num - offset;
				}
				return this.data.Substring(offset, count);
			}
			return string.Empty;
		}

		public virtual void AppendData(string strData)
		{
			XmlNode parentNode = this.ParentNode;
			int num = ((this.data != null) ? this.data.Length : 0);
			if (strData != null)
			{
				num += strData.Length;
			}
			string text = new StringBuilder(num).Append(this.data).Append(strData).ToString();
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, text, XmlNodeChangedAction.Change);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			this.data = text;
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
		}

		public virtual void InsertData(int offset, string strData)
		{
			XmlNode parentNode = this.ParentNode;
			int num = ((this.data != null) ? this.data.Length : 0);
			if (strData != null)
			{
				num += strData.Length;
			}
			string text = new StringBuilder(num).Append(this.data).Insert(offset, strData).ToString();
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, text, XmlNodeChangedAction.Change);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			this.data = text;
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
		}

		public virtual void DeleteData(int offset, int count)
		{
			int num = ((this.data != null) ? this.data.Length : 0);
			if (num > 0 && num < offset + count)
			{
				count = Math.Max(num - offset, 0);
			}
			string text = new StringBuilder(this.data).Remove(offset, count).ToString();
			XmlNode parentNode = this.ParentNode;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, text, XmlNodeChangedAction.Change);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			this.data = text;
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
		}

		public virtual void ReplaceData(int offset, int count, string strData)
		{
			int num = ((this.data != null) ? this.data.Length : 0);
			if (num > 0 && num < offset + count)
			{
				count = Math.Max(num - offset, 0);
			}
			string text = new StringBuilder(this.data).Remove(offset, count).Insert(offset, strData).ToString();
			XmlNode parentNode = this.ParentNode;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, text, XmlNodeChangedAction.Change);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			this.data = text;
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
		}

		internal bool CheckOnData(string data)
		{
			return XmlCharType.Instance.IsOnlyWhitespace(data);
		}

		internal bool DecideXPNodeTypeForTextNodes(XmlNode node, ref XPathNodeType xnt)
		{
			while (node != null)
			{
				XmlNodeType nodeType = node.NodeType;
				if (nodeType <= XmlNodeType.EntityReference)
				{
					if (nodeType - XmlNodeType.Text <= 1)
					{
						xnt = XPathNodeType.Text;
						return false;
					}
					if (nodeType != XmlNodeType.EntityReference)
					{
						return false;
					}
					if (!this.DecideXPNodeTypeForTextNodes(node.FirstChild, ref xnt))
					{
						return false;
					}
				}
				else if (nodeType != XmlNodeType.Whitespace)
				{
					if (nodeType != XmlNodeType.SignificantWhitespace)
					{
						return false;
					}
					xnt = XPathNodeType.SignificantWhitespace;
				}
				node = node.NextSibling;
			}
			return true;
		}

		private string data;
	}
}
