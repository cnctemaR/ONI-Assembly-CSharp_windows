using System;
using System.Xml.XPath;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class CopyOfAction : CompiledAction
	{
		internal override void Compile(Compiler compiler)
		{
			base.CompileAttributes(compiler);
			base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
			base.CheckEmpty(compiler);
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
			{
				Query valueQuery = processor.GetValueQuery(this.selectKey);
				object obj = valueQuery.Evaluate(frame.NodeSet);
				if (obj is XPathNodeIterator)
				{
					processor.PushActionFrame(CopyNodeSetAction.GetAction(), new XPathArrayIterator(valueQuery));
					frame.State = 3;
					return;
				}
				XPathNavigator xpathNavigator = obj as XPathNavigator;
				if (xpathNavigator != null)
				{
					processor.PushActionFrame(CopyNodeSetAction.GetAction(), new XPathSingletonIterator(xpathNavigator));
					frame.State = 3;
					return;
				}
				string text = XmlConvert.ToXPathString(obj);
				if (processor.TextEvent(text))
				{
					frame.Finished();
					return;
				}
				frame.StoredOutput = text;
				frame.State = 2;
				return;
			}
			case 1:
				break;
			case 2:
				processor.TextEvent(frame.StoredOutput);
				frame.Finished();
				return;
			case 3:
				frame.Finished();
				break;
			default:
				return;
			}
		}

		private const int ResultStored = 2;

		private const int NodeSetCopied = 3;

		private int selectKey = -1;
	}
}
