using System;
using System.Collections;
using System.Text;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class Avt
	{
		private Avt(string constAvt)
		{
			this.constAvt = constAvt;
		}

		private Avt(ArrayList eventList)
		{
			this.events = new TextEvent[eventList.Count];
			for (int i = 0; i < eventList.Count; i++)
			{
				this.events[i] = (TextEvent)eventList[i];
			}
		}

		public bool IsConstant
		{
			get
			{
				return this.events == null;
			}
		}

		internal string Evaluate(Processor processor, ActionFrame frame)
		{
			if (this.IsConstant)
			{
				return this.constAvt;
			}
			StringBuilder sharedStringBuilder = processor.GetSharedStringBuilder();
			for (int i = 0; i < this.events.Length; i++)
			{
				sharedStringBuilder.Append(this.events[i].Evaluate(processor, frame));
			}
			processor.ReleaseSharedStringBuilder();
			return sharedStringBuilder.ToString();
		}

		internal static Avt CompileAvt(Compiler compiler, string avtText)
		{
			bool flag;
			ArrayList arrayList = compiler.CompileAvt(avtText, out flag);
			if (!flag)
			{
				return new Avt(arrayList);
			}
			return new Avt(avtText);
		}

		private string constAvt;

		private TextEvent[] events;
	}
}
