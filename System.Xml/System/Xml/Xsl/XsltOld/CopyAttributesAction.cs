using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class CopyAttributesAction : Action
	{
		internal static CopyAttributesAction GetAction()
		{
			return CopyAttributesAction.s_Action;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			while (processor.CanContinue)
			{
				switch (frame.State)
				{
				case 0:
					if (!frame.Node.HasAttributes || !frame.Node.MoveToFirstAttribute())
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
					if (CopyAttributesAction.SendTextEvent(processor, frame.Node))
					{
						frame.State = 4;
						continue;
					}
					return;
				case 4:
					if (CopyAttributesAction.SendEndEvent(processor, frame.Node))
					{
						frame.State = 5;
						continue;
					}
					return;
				case 5:
					if (frame.Node.MoveToNextAttribute())
					{
						frame.State = 2;
						continue;
					}
					frame.Node.MoveToParent();
					frame.Finished();
					return;
				default:
					return;
				}
				if (!CopyAttributesAction.SendBeginEvent(processor, frame.Node))
				{
					break;
				}
				frame.State = 3;
			}
		}

		private static bool SendBeginEvent(Processor processor, XPathNavigator node)
		{
			return processor.BeginEvent(XPathNodeType.Attribute, node.Prefix, node.LocalName, node.NamespaceURI, false);
		}

		private static bool SendTextEvent(Processor processor, XPathNavigator node)
		{
			return processor.TextEvent(node.Value);
		}

		private static bool SendEndEvent(Processor processor, XPathNavigator node)
		{
			return processor.EndEvent(XPathNodeType.Attribute);
		}

		private const int BeginEvent = 2;

		private const int TextEvent = 3;

		private const int EndEvent = 4;

		private const int Advance = 5;

		private static CopyAttributesAction s_Action = new CopyAttributesAction();
	}
}
