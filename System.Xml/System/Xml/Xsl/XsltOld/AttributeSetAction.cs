using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class AttributeSetAction : ContainerAction
	{
		internal XmlQualifiedName Name
		{
			get
			{
				return this.name;
			}
		}

		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.name, "name");
			this.CompileContent(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Name))
			{
				this.name = compiler.CreateXPathQName(value);
			}
			else
			{
				if (!Ref.Equal(localName, compiler.Atoms.UseAttributeSets))
				{
					return false;
				}
				base.AddAction(compiler.CreateUseAttributeSetsAction());
			}
			return true;
		}

		private void CompileContent(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			if (compiler.Recurse())
			{
				for (;;)
				{
					XPathNodeType nodeType = input.NodeType;
					if (nodeType != XPathNodeType.Element)
					{
						if (nodeType - XPathNodeType.SignificantWhitespace > 3)
						{
							break;
						}
					}
					else
					{
						compiler.PushNamespaceScope();
						string namespaceURI = input.NamespaceURI;
						string localName = input.LocalName;
						if (!Ref.Equal(namespaceURI, input.Atoms.UriXsl) || !Ref.Equal(localName, input.Atoms.Attribute))
						{
							goto IL_006B;
						}
						base.AddAction(compiler.CreateAttributeAction());
						compiler.PopScope();
					}
					if (!compiler.Advance())
					{
						goto Block_5;
					}
				}
				throw XsltException.Create("The contents of '{0}' are invalid.", new string[] { "attribute-set" });
				IL_006B:
				throw compiler.UnexpectedKeyword();
				Block_5:
				compiler.ToParent();
			}
		}

		internal void Merge(AttributeSetAction attributeAction)
		{
			int num = 0;
			Action action;
			while ((action = attributeAction.GetAction(num)) != null)
			{
				base.AddAction(action);
				num++;
			}
		}

		internal XmlQualifiedName name;
	}
}
