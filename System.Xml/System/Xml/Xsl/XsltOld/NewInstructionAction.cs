using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class NewInstructionAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			XPathNavigator xpathNavigator = compiler.Input.Navigator.Clone();
			this.name = xpathNavigator.Name;
			xpathNavigator.MoveToParent();
			this.parent = xpathNavigator.Name;
			if (compiler.Recurse())
			{
				this.CompileSelectiveTemplate(compiler);
				compiler.ToParent();
			}
		}

		internal void CompileSelectiveTemplate(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			do
			{
				if (Ref.Equal(input.NamespaceURI, input.Atoms.UriXsl) && Ref.Equal(input.LocalName, input.Atoms.Fallback))
				{
					this.fallback = true;
					if (compiler.Recurse())
					{
						base.CompileTemplate(compiler);
						compiler.ToParent();
					}
				}
			}
			while (compiler.Advance());
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
			}
			else
			{
				if (!this.fallback)
				{
					throw XsltException.Create("'{0}' is not a recognized extension element.", new string[] { this.name });
				}
				if (this.containedActions != null && this.containedActions.Count > 0)
				{
					processor.PushActionFrame(frame);
					frame.State = 1;
					return;
				}
			}
			frame.Finished();
		}

		private string name;

		private string parent;

		private bool fallback;
	}
}
