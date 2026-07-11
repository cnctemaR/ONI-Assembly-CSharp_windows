using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class Axis : AstNode
	{
		public Axis(Axis.AxisType axisType, AstNode input, string prefix, string name, XPathNodeType nodetype)
		{
			this.axisType = axisType;
			this.input = input;
			this.prefix = prefix;
			this.name = name;
			this.nodeType = nodetype;
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
				return this.input;
			}
			set
			{
				this.input = value;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public XPathNodeType NodeType
		{
			get
			{
				return this.nodeType;
			}
		}

		public Axis.AxisType TypeOfAxis
		{
			get
			{
				return this.axisType;
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
				return this.urn;
			}
			set
			{
				this.urn = value;
			}
		}

		private Axis.AxisType axisType;

		private AstNode input;

		private string prefix;

		private string name;

		private XPathNodeType nodeType;

		protected bool abbrAxis;

		private string urn = string.Empty;

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
