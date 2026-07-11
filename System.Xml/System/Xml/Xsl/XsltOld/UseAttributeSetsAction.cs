using System;

namespace System.Xml.Xsl.XsltOld
{
	internal class UseAttributeSetsAction : CompiledAction
	{
		internal XmlQualifiedName[] UsedSets
		{
			get
			{
				return this.useAttributeSets;
			}
		}

		internal override void Compile(Compiler compiler)
		{
			this.useString = compiler.Input.Value;
			if (this.useString.Length == 0)
			{
				this.useAttributeSets = new XmlQualifiedName[0];
				return;
			}
			string[] array = XmlConvert.SplitString(this.useString);
			try
			{
				this.useAttributeSets = new XmlQualifiedName[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.useAttributeSets[i] = compiler.CreateXPathQName(array[i]);
				}
			}
			catch (XsltException)
			{
				if (!compiler.ForwardCompatibility)
				{
					throw;
				}
				this.useAttributeSets = new XmlQualifiedName[0];
			}
		}

		internal override void Execute(Processor processor, ActionFrame frame)
		{
			int state = frame.State;
			if (state != 0)
			{
				if (state != 2)
				{
					return;
				}
			}
			else
			{
				frame.Counter = 0;
				frame.State = 2;
			}
			if (frame.Counter < this.useAttributeSets.Length)
			{
				AttributeSetAction attributeSet = processor.RootAction.GetAttributeSet(this.useAttributeSets[frame.Counter]);
				frame.IncrementCounter();
				processor.PushActionFrame(attributeSet, frame.NodeSet);
				return;
			}
			frame.Finished();
		}

		private XmlQualifiedName[] useAttributeSets;

		private string useString;

		private const int ProcessingSets = 2;
	}
}
