using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal abstract class CompiledAction : Action
	{
		internal abstract void Compile(Compiler compiler);

		internal virtual bool CompileAttribute(Compiler compiler)
		{
			return false;
		}

		public void CompileAttributes(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			string localName = input.LocalName;
			if (input.MoveToFirstAttribute())
			{
				do
				{
					if (input.NamespaceURI.Length == 0)
					{
						try
						{
							if (!this.CompileAttribute(compiler))
							{
								throw XsltException.Create("'{0}' is an invalid attribute for the '{1}' element.", new string[] { input.LocalName, localName });
							}
						}
						catch
						{
							if (!compiler.ForwardCompatibility)
							{
								throw;
							}
						}
					}
				}
				while (input.MoveToNextAttribute());
				input.ToParent();
			}
		}

		internal static string PrecalculateAvt(ref Avt avt)
		{
			string text = null;
			if (avt != null && avt.IsConstant)
			{
				text = avt.Evaluate(null, null);
				avt = null;
			}
			return text;
		}

		public void CheckEmpty(Compiler compiler)
		{
			string name = compiler.Input.Name;
			if (compiler.Recurse())
			{
				for (;;)
				{
					XPathNodeType nodeType = compiler.Input.NodeType;
					if (nodeType != XPathNodeType.Whitespace && nodeType != XPathNodeType.Comment && nodeType != XPathNodeType.ProcessingInstruction)
					{
						break;
					}
					if (!compiler.Advance())
					{
						goto Block_4;
					}
				}
				throw XsltException.Create("The contents of '{0}' must be empty.", new string[] { name });
				Block_4:
				compiler.ToParent();
			}
		}

		public void CheckRequiredAttribute(Compiler compiler, object attrValue, string attrName)
		{
			this.CheckRequiredAttribute(compiler, attrValue != null, attrName);
		}

		public void CheckRequiredAttribute(Compiler compiler, bool attr, string attrName)
		{
			if (!attr)
			{
				throw XsltException.Create("Missing mandatory attribute '{0}'.", new string[] { attrName });
			}
		}
	}
}
