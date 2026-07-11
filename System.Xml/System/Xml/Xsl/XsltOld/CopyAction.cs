using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class CopyAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			if (compiler.Recurse())
			{
				base.CompileTemplate(compiler);
				compiler.ToParent();
			}
			if (this.containedActions == null)
			{
				this.empty = true;
			}
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.UseAttributeSets))
			{
				this.useAttributeSets = value;
				base.AddAction(compiler.CreateUseAttributeSetsAction());
				return true;
			}
			return false;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			while (processor.CanContinue)
			{
				switch (frame.State)
				{
				case 0:
					if (Processor.IsRoot(frame.Node))
					{
						processor.PushActionFrame(frame);
						frame.State = 8;
						return;
					}
					if (!processor.CopyBeginEvent(frame.Node, this.empty))
					{
						return;
					}
					frame.State = 5;
					break;
				case 1:
				case 2:
				case 3:
				case 4:
					return;
				case 5:
					frame.State = 6;
					if (frame.Node.NodeType == XPathNodeType.Element)
					{
						processor.PushActionFrame(CopyNamespacesAction.GetAction(), frame.NodeSet);
						return;
					}
					break;
				case 6:
					if (frame.Node.NodeType == XPathNodeType.Element && !this.empty)
					{
						processor.PushActionFrame(frame);
						frame.State = 7;
						return;
					}
					if (!processor.CopyTextEvent(frame.Node))
					{
						return;
					}
					frame.State = 7;
					break;
				case 7:
					if (processor.CopyEndEvent(frame.Node))
					{
						frame.Finished();
						return;
					}
					return;
				case 8:
					frame.Finished();
					return;
				default:
					return;
				}
			}
		}

		private const int CopyText = 4;

		private const int NamespaceCopy = 5;

		private const int ContentsCopy = 6;

		private const int ProcessChildren = 7;

		private const int ChildrenOnly = 8;

		private string useAttributeSets;

		private bool empty;
	}
}
