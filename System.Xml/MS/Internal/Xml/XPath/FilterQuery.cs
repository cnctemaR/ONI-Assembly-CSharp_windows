using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class FilterQuery : BaseAxisQuery
	{
		public FilterQuery(Query qyParent, Query cond, bool noPosition)
			: base(qyParent)
		{
			this.cond = cond;
			this.noPosition = noPosition;
		}

		private FilterQuery(FilterQuery other)
			: base(other)
		{
			this.cond = Query.Clone(other.cond);
			this.noPosition = other.noPosition;
		}

		public override void Reset()
		{
			this.cond.Reset();
			base.Reset();
		}

		public Query Condition
		{
			get
			{
				return this.cond;
			}
		}

		public override void SetXsltContext(XsltContext input)
		{
			base.SetXsltContext(input);
			this.cond.SetXsltContext(input);
			if (this.cond.StaticType != XPathResultType.Number && this.cond.StaticType != XPathResultType.Any && this.noPosition)
			{
				ReversePositionQuery reversePositionQuery = this.qyInput as ReversePositionQuery;
				if (reversePositionQuery != null)
				{
					this.qyInput = reversePositionQuery.input;
				}
			}
		}

		public override XPathNavigator Advance()
		{
			while ((this.currentNode = this.qyInput.Advance()) != null)
			{
				if (this.EvaluatePredicate())
				{
					this.position++;
					return this.currentNode;
				}
			}
			return null;
		}

		internal bool EvaluatePredicate()
		{
			object obj = this.cond.Evaluate(this.qyInput);
			if (obj is XPathNodeIterator)
			{
				return this.cond.Advance() != null;
			}
			if (obj is string)
			{
				return ((string)obj).Length != 0;
			}
			if (obj is double)
			{
				return (double)obj == (double)this.qyInput.CurrentPosition;
			}
			return !(obj is bool) || (bool)obj;
		}

		public override XPathNavigator MatchNode(XPathNavigator current)
		{
			if (current == null)
			{
				return null;
			}
			XPathNavigator xpathNavigator = this.qyInput.MatchNode(current);
			if (xpathNavigator != null)
			{
				switch (this.cond.StaticType)
				{
				case XPathResultType.Number:
				{
					OperandQuery operandQuery = this.cond as OperandQuery;
					if (operandQuery != null)
					{
						double num = (double)operandQuery.val;
						ChildrenQuery childrenQuery = this.qyInput as ChildrenQuery;
						if (childrenQuery != null)
						{
							XPathNavigator xpathNavigator2 = current.Clone();
							xpathNavigator2.MoveToParent();
							int num2 = 0;
							xpathNavigator2.MoveToFirstChild();
							for (;;)
							{
								if (childrenQuery.matches(xpathNavigator2))
								{
									num2++;
									if (current.IsSamePosition(xpathNavigator2))
									{
										break;
									}
								}
								if (!xpathNavigator2.MoveToNext())
								{
									goto Block_9;
								}
							}
							if (num != (double)num2)
							{
								return null;
							}
							return xpathNavigator;
							Block_9:
							return null;
						}
						AttributeQuery attributeQuery = this.qyInput as AttributeQuery;
						if (attributeQuery != null)
						{
							XPathNavigator xpathNavigator3 = current.Clone();
							xpathNavigator3.MoveToParent();
							int num3 = 0;
							xpathNavigator3.MoveToFirstAttribute();
							for (;;)
							{
								if (attributeQuery.matches(xpathNavigator3))
								{
									num3++;
									if (current.IsSamePosition(xpathNavigator3))
									{
										break;
									}
								}
								if (!xpathNavigator3.MoveToNextAttribute())
								{
									goto Block_14;
								}
							}
							if (num != (double)num3)
							{
								return null;
							}
							return xpathNavigator;
							Block_14:
							return null;
						}
					}
					break;
				}
				case XPathResultType.String:
					if (this.noPosition)
					{
						if (((string)this.cond.Evaluate(new XPathSingletonIterator(current, true))).Length == 0)
						{
							return null;
						}
						return xpathNavigator;
					}
					break;
				case XPathResultType.Boolean:
					if (this.noPosition)
					{
						if (!(bool)this.cond.Evaluate(new XPathSingletonIterator(current, true)))
						{
							return null;
						}
						return xpathNavigator;
					}
					break;
				case XPathResultType.NodeSet:
					this.cond.Evaluate(new XPathSingletonIterator(current, true));
					if (this.cond.Advance() == null)
					{
						return null;
					}
					return xpathNavigator;
				case (XPathResultType)4:
					return xpathNavigator;
				default:
					return null;
				}
				this.Evaluate(new XPathSingletonIterator(xpathNavigator, true));
				XPathNavigator xpathNavigator4;
				while ((xpathNavigator4 = this.Advance()) != null)
				{
					if (xpathNavigator4.IsSamePosition(current))
					{
						return xpathNavigator;
					}
				}
			}
			return null;
		}

		public override QueryProps Properties
		{
			get
			{
				return QueryProps.Position | (this.qyInput.Properties & (QueryProps)24);
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new FilterQuery(this);
		}

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			if (!this.noPosition)
			{
				w.WriteAttributeString("position", "yes");
			}
			this.qyInput.PrintQuery(w);
			this.cond.PrintQuery(w);
			w.WriteEndElement();
		}

		private Query cond;

		private bool noPosition;
	}
}
