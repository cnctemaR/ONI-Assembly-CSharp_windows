using System;

namespace System.Xml.Xsl.XsltOld
{
	internal abstract class TemplateBaseAction : ContainerAction
	{
		public int AllocateVariableSlot()
		{
			int num = this.variableFreeSlot;
			this.variableFreeSlot++;
			if (this.variableCount < this.variableFreeSlot)
			{
				this.variableCount = this.variableFreeSlot;
			}
			return num;
		}

		public void ReleaseVariableSlots(int n)
		{
		}

		protected int variableCount;

		private int variableFreeSlot;
	}
}
