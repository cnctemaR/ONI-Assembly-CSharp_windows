using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Axis : AstNode
	{
		public Axis(Axis.AxisType axisType, AstNode input, string prefix, string name, XPathNodeType nodetype)
		{
			this._axisType = axisType;
			this._input = input;
			this._prefix = prefix;
			this._name = name;
			this._nodeType = nodetype;
		}

		public Axis(Axis.AxisType axisType, AstNode input)
			: this(axisType, input, string.Empty, string.Empty, XPathNodeType.All)
		{
			this.abbrAxis = true;
		}

		public override AstNode.AstType Type
		{
			get
			{
				return AstNode.AstType.Axis;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		public AstNode Input
		{
			get
			{
				return this._input;
			}
			set
			{
				this._input = value;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public XPathNodeType NodeType
		{
			get
			{
				return this._nodeType;
			}
		}

		public Axis.AxisType TypeOfAxis
		{
			get
			{
				return this._axisType;
			}
		}

		public bool AbbrAxis
		{
			get
			{
				return this.abbrAxis;
			}
		}

		public string Urn
		{
			get
			{
				return this._urn;
			}
			set
			{
				this._urn = value;
			}
		}

		private Axis.AxisType _axisType;

		private AstNode _input;

		private string _prefix;

		private string _name;

		private XPathNodeType _nodeType;

		protected bool abbrAxis;

		private string _urn = string.Empty;

		public enum AxisType
		{
			Ancestor,
			AncestorOrSelf,
			Attribute,
			Child,
			Descendant,
			DescendantOrSelf,
			Following,
			FollowingSibling,
			Namespace,
			Parent,
			Preceding,
			PrecedingSibling,
			Self,
			None
		}
	}
}
