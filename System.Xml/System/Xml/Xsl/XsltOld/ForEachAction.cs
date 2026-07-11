using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class ForEachAction : ContainerAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
			compiler.CanHaveApplyImports = false;
			if (compiler.Recurse())
			{
				this.CompileSortElements(compiler);
				base.CompileTemplate(compiler);
				compiler.ToParent();
			}
		}

		internal override bool CompileAttribute(Compiler compiler)
		{
			string localName = compiler.Input.LocalName;
			string value = compiler.Input.Value;
			if (Ref.Equal(localName, compiler.Atoms.Select))
			{
				this.selectKey = compiler.AddQuery(value);
				return true;
			}
			return false;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			switch (frame.State)
			{
			case 0:
				if (this.sortContainer != null)
				{
					processor.InitSortArray();
					processor.PushActionFrame(this.sortContainer, frame.NodeSet);
					frame.State = 2;
					return;
				}
				break;
			case 1:
				return;
			case 2:
				break;
			case 3:
				goto IL_0082;
			case 4:
				goto IL_009B;
			case 5:
				frame.State = 3;
				goto IL_0082;
			default:
				return;
			}
			frame.InitNewNodeSet(processor.StartQuery(frame.NodeSet, this.selectKey));
			if (this.sortContainer != null)
			{
				frame.SortNewNodeSet(processor, processor.SortArray);
			}
			frame.State = 3;
			IL_0082:
			if (!frame.NewNextNode(processor))
			{
				frame.Finished();
				return;
			}
			frame.State = 4;
			IL_009B:
			processor.PushActionFrame(frame, frame.NewNodeSet);
			frame.State = 5;
		}

		protected void CompileSortElements(Compiler compiler)
		{
			NavigatorInput input = compiler.Input;
			for (;;)
			{
				switch (input.NodeType)
				{
				case XPathNodeType.Element:
					if (!Ref.Equal(input.NamespaceURI, input.Atoms.UriXsl) || !Ref.Equal(input.LocalName, input.Atoms.Sort))
					{
						return;
					}
					if (this.sortContainer == null)
					{
						this.sortContainer = new ContainerAction();
					}
					this.sortContainer.AddAction(compiler.CreateSortAction());
					break;
				case XPathNodeType.Text:
					return;
				case XPathNodeType.SignificantWhitespace:
					base.AddEvent(compiler.CreateTextEvent());
					break;
				}
				if (!input.Advance())
				{
					return;
				}
			}
		}

		private const int ProcessedSort = 2;

		private const int ProcessNextNode = 3;

		private const int PositionAdvanced = 4;

		private const int ContentsProcessed = 5;

		private int selectKey = -1;

		private ContainerAction sortContainer;
	}
}
