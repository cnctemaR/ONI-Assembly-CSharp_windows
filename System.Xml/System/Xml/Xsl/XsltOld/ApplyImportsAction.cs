using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class ApplyImportsAction : CompiledAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CheckEmpty(compiler);
			if (!compiler.CanHaveApplyImports)
			{
				throw XsltException.Create("The 'xsl:apply-imports' instruction cannot be included within the content of an 'xsl:for-each' instruction or within an 'xsl:template' instruction without the 'match' attribute.", Array.Empty<string>());
			}
			this.mode = compiler.CurrentMode;
			this.stylesheet = compiler.CompiledStylesheet;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state == 0)
			{
				processor.PushTemplateLookup(frame.NodeSet, this.mode, this.stylesheet);
				frame.State = 2;
				return;
			}
			if (state != 2)
			{
				return;
			}
			frame.Finished();
		}

		private XmlQualifiedName mode;

		private Stylesheet stylesheet;

		private const int TemplateProcessed = 2;
	}
}
