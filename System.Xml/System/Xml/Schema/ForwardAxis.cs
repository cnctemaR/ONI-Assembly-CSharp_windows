using System;

namespace System.Xml.Schema
{
	internal class ForwardAxis
	{
		internal DoubleLinkAxis RootNode
		{
			get
			{
				return this._rootNode;
			}
		}

		internal DoubleLinkAxis TopNode
		{
			get
			{
				return this._topNode;
			}
		}

		internal bool IsAttribute
		{
			get
			{
				return this._isAttribute;
			}
		}

		internal bool IsDss
		{
			get
			{
				return this._isDss;
			}
		}

		internal bool IsSelfAxis
		{
			get
			{
				return this._isSelfAxis;
			}
		}

		public ForwardAxis(DoubleLinkAxis axis, bool isdesorself)
		{
			this._isDss = isdesorself;
			this._isAttribute = Asttree.IsAttribute(axis);
			this._topNode = axis;
			this._rootNode = axis;
			while (this._rootNode.Input != null)
			{
				this._rootNode = (DoubleLinkAxis)this._rootNode.Input;
			}
			this._isSelfAxis = Asttree.IsSelf(this._topNode);
		}

		private DoubleLinkAxis _topNode;

		private DoubleLinkAxis _rootNode;

		private bool _isAttribute;

		private bool _isDss;

		private bool _isSelfAxis;
	}
}
