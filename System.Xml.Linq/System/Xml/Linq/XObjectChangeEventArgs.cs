using System;

namespace System.Xml.Linq
{
	public class XObjectChangeEventArgs : EventArgs
	{
		public XObjectChangeEventArgs(XObjectChange objectChange)
		{
			this._objectChange = objectChange;
		}

		public XObjectChange ObjectChange
		{
			get
			{
				return this._objectChange;
			}
		}

		private XObjectChange _objectChange;

		public static readonly XObjectChangeEventArgs Add = new XObjectChangeEventArgs(XObjectChange.Add);

		public static readonly XObjectChangeEventArgs Remove = new XObjectChangeEventArgs(XObjectChange.Remove);

		public static readonly XObjectChangeEventArgs Name = new XObjectChangeEventArgs(XObjectChange.Name);

		public static readonly XObjectChangeEventArgs Value = new XObjectChangeEventArgs(XObjectChange.Value);
	}
}
