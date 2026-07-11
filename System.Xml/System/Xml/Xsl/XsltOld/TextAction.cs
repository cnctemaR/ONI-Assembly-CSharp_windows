using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class TextAction : CompiledAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			this.CompileContent(compiler);
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.DisableOutputEscaping))
			{
				this.disableOutputEscaping = compiler.GetYesNo(value);
				return true;
			}
			return false;
		}

		private void CompileContent(Compiler compiler)
		{
			if (compiler.Recurse())
			{
				NavigatorInput input = compiler.Input;
				this.text = string.Empty;
				for (;;)
				{
					XPathNodeType nodeType = input.NodeType;
					if (nodeType - XPathNodeType.Text > 2)
					{
						if (nodeType - XPathNodeType.ProcessingInstruction > 1)
						{
							break;
						}
					}
					else
					{
						this.text += input.Value;
					}
					if (!compiler.Advance())
					{
						goto Block_4;
					}
				}
				throw compiler.UnexpectedKeyword();
				Block_4:
				compiler.ToParent();
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			if (frame.State == 0 && processor.TextEvent(this.text, this.disableOutputEscaping))
			{
				frame.Finished();
			}
		}

		private bool disableOutputEscaping;

		private string text;
	}
}
