using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class TemplateLookupActionDbg : TemplateLookupAction
	{
		internal override void Execute(Processor processor, ActionFrame frame)
		{
			if (this.mode == Compiler.BuiltInMode)
			{
				this.mode = processor.GetPrevioseMode();
			}
			processor.SetCurrentMode(this.mode);
			Action action;
			if (this.mode != null)
			{
				action = ((this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node, this.mode) : this.importsOf.FindTemplateImports(processor, frame.Node, this.mode));
			}
			else
			{
				action = ((this.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node) : this.importsOf.FindTemplateImports(processor, frame.Node));
			}
			if (action == null && processor.RootAction.builtInSheet != null)
			{
				action = processor.RootAction.builtInSheet.FindTemplate(processor, frame.Node, Compiler.BuiltInMode);
			}
			if (action == null)
			{
				action = base.BuiltInTemplate(frame.Node);
			}
			if (action != null)
			{
				frame.SetAction(action);
				return;
			}
			frame.Finished();
		}
	}
}
