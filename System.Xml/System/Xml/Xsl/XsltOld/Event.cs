using System;

namespace System.Xml.Xsl.XsltOld
{
	internal abstract class Event
	{
		public virtual void ReplaceNamespaceAlias(Compiler compiler)
		{
		}

		public abstract bool Output(Processor processor, ActionFrame frame);

		internal void OnInstructionExecute(Processor processor)
		{
			processor.OnInstructionExecute();
		}

		internal virtual DbgData DbgData
		{
			get
			{
				return DbgData.Empty;
			}
		}
	}
}
