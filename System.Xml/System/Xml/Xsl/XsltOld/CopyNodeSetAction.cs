using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class CopyNodeSetAction : Action
	{
		internal static CopyNodeSetAction GetAction()
		{
			return CopyNodeSetAction.s_Action;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			while (processor.CanContinue)
			{
				switch (frame.State)
				{
				case 0:
					if (!frame.NextNode(processor))
					{
						frame.Finished();
						return;
					}
					frame.State = 2;
					break;
				case 1:
					return;
				case 2:
					break;
				case 3:
				{
					XPathNodeType nodeType = frame.Node.NodeType;
					if (nodeType == XPathNodeType.Element || nodeType == XPathNodeType.Root)
					{
						processor.PushActionFrame(CopyNamespacesAction.GetAction(), frame.NodeSet);
						frame.State = 4;
						return;
					}
					if (CopyNodeSetAction.SendTextEvent(processor, frame.Node))
					{
						frame.State = 7;
						continue;
					}
					return;
				}
				case 4:
					processor.PushActionFrame(CopyAttributesAction.GetAction(), frame.NodeSet);
					frame.State = 5;
					return;
				case 5:
					if (frame.Node.HasChildren)
					{
						processor.PushActionFrame(CopyNodeSetAction.GetAction(), frame.Node.SelectChildren(XPathNodeType.All));
						frame.State = 6;
						return;
					}
					frame.State = 7;
					goto IL_0107;
				case 6:
					frame.State = 7;
					continue;
				case 7:
					goto IL_0107;
				default:
					return;
				}
				if (CopyNodeSetAction.SendBeginEvent(processor, frame.Node))
				{
					frame.State = 3;
					continue;
				}
				break;
				IL_0107:
				if (!CopyNodeSetAction.SendEndEvent(processor, frame.Node))
				{
					break;
				}
				frame.State = 0;
			}
		}

		private static bool SendBeginEvent(Processor processor, XPathNavigator node)
		{
			return processor.CopyBeginEvent(node, node.IsEmptyElement);
		}

		private static bool SendTextEvent(Processor processor, XPathNavigator node)
		{
			return processor.CopyTextEvent(node);
		}

		private static bool SendEndEvent(Processor processor, XPathNavigator node)
		{
			return processor.CopyEndEvent(node);
		}

		private const int BeginEvent = 2;

		private const int Contents = 3;

		private const int Namespaces = 4;

		private const int Attributes = 5;

		private const int Subtree = 6;

		private const int EndEvent = 7;

		private static CopyNodeSetAction s_Action = new CopyNodeSetAction();
	}
}
