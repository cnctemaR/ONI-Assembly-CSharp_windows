using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class CommentAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			if (compiler.Recurse())
			{
				base.CompileTemplate(compiler);
				compiler.ToParent();
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 1)
				{
					return;
				}
				if (processor.EndEvent(XPathNodeType.Comment))
				{
					frame.Finished();
				}
			}
			else if (processor.BeginEvent(XPathNodeType.Comment, string.Empty, string.Empty, string.Empty, false))
			{
				processor.PushActionFrame(frame);
				frame.State = 1;
				return;
			}
		}
	}
}
