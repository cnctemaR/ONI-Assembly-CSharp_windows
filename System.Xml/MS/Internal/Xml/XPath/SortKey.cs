using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class SortKey
	{
		public SortKey(int numKeys, int originalPosition, XPathNavigator node)
		{
			this._numKeys = numKeys;
			this._keys = new object[numKeys];
			this._originalPosition = originalPosition;
			this._node = node;
		}

		public object this[int index]
		{
			get
			{
				return this._keys[index];
			}
			set
			{
				this._keys[index] = value;
			}
		}

		public int NumKeys
		{
			get
			{
				return this._numKeys;
			}
		}

		public int OriginalPosition
		{
			get
			{
				return this._originalPosition;
			}
		}

		public XPathNavigator Node
		{
			get
			{
				return this._node;
			}
		}

		private int _numKeys;

		private object[] _keys;

		private int _originalPosition;

		private XPathNavigator _node;
	}
}
