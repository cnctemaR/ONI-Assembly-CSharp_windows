using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class CopyNamespacesAction : Action
	{
		internal static CopyNamespacesAction GetAction()
		{
			return CopyNamespacesAction.s_Action;
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			while (processor.CanContinue)
			{
				switch (frame.State)
				{
				case 0:
					if (!frame.Node.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
					{
						frame.Finished();
						return;
					}
					frame.State = 2;
					break;
				case 1:
				case 3:
					return;
				case 2:
					break;
				case 4:
					if (processor.EndEvent(XPathNodeType.Namespace))
					{
						frame.State = 5;
						continue;
					}
					return;
				case 5:
					if (frame.Node.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml))
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
				if (!processor.BeginEvent(XPathNodeType.Namespace, null, frame.Node.LocalName, frame.Node.Value, false))
				{
					break;
				}
				frame.State = 4;
			}
		}

		private const int BeginEvent = 2;

		private const int TextEvent = 3;

		private const int EndEvent = 4;

		private const int Advance = 5;

		private static CopyNamespacesAction s_Action = new CopyNamespacesAction();
	}
}
