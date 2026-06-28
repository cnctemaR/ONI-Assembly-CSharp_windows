using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslVariableInformation
	{
		public XslVariableInformation(Compiler c)
		{
			c.CheckExtraAttributes(c.Input.LocalName, new string[] { "name", "select" });
			c.AssertAttribute("name");
			this.name = c.ParseQNameAttribute("name");
			try
			{
				XmlConvert.VerifyName(this.name.Name);
			}
			catch (XmlException ex)
			{
				throw new XsltCompileException("Variable name is not qualified name", ex, c.Input);
			}
			string attribute = c.GetAttribute("select");
			if (attribute != null && attribute != string.Empty)
			{
				this.select = c.CompileExpression(c.GetAttribute("select"));
			}
			else if (c.Input.MoveToFirstChild())
			{
				this.content = c.CompileTemplateContent();
				c.Input.MoveToParent();
			}
		}

		public object Evaluate(XslTransformProcessor p)
		{
			if (this.select != null)
			{
				object obj = p.Evaluate(this.select);
				if (obj is XPathNodeIterator)
				{
					ArrayList arrayList = new ArrayList();
					XPathNodeIterator xpathNodeIterator = (XPathNodeIterator)obj;
					while (xpathNodeIterator.MoveNext())
					{
						XPathNavigator xpathNavigator = xpathNodeIterator.Current;
						arrayList.Add(xpathNavigator.Clone());
					}
					obj = new ListIterator(arrayList, p.XPathContext);
				}
				return obj;
			}
			if (this.content != null)
			{
				DTMXPathDocumentWriter2 dtmxpathDocumentWriter = new DTMXPathDocumentWriter2(p.Root.NameTable, 200);
				Outputter outputter = new GenericOutputter(dtmxpathDocumentWriter, p.Outputs, null, true);
				p.PushOutput(outputter);
				if (p.CurrentNodeset.CurrentPosition == 0)
				{
					p.NodesetMoveNext();
				}
				this.content.Evaluate(p);
				p.PopOutput();
				return dtmxpathDocumentWriter.CreateDocument().CreateNavigator();
			}
			return string.Empty;
		}

		public XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		internal XPathExpression Select
		{
			get
			{
				return this.select;
			}
		}

		internal XslOperation Content
		{
			get
			{
				return this.content;
			}
		}

		private XmlQualifiedName name;

		private XPathExpression select;

		private XslOperation content;
	}
}
