using System;
using System.Reflection.Emit;

namespace System.Xml.Serialization
{
	internal class ForState
	{
		internal ForState(LocalBuilder indexVar, Label beginLabel, Label testLabel, object end)
		{
			this.indexVar = indexVar;
			this.beginLabel = beginLabel;
			this.testLabel = testLabel;
			this.end = end;
		}

		internal LocalBuilder Index
		{
			get
			{
				return this.indexVar;
			}
		}

		internal Label BeginLabel
		{
			get
			{
				return this.beginLabel;
			}
		}

		internal Label TestLabel
		{
			get
			{
				return this.testLabel;
			}
		}

		internal object End
		{
			get
			{
				return this.end;
			}
		}

		private LocalBuilder indexVar;

		private Label beginLabel;

		private Label testLabel;

		private object end;
	}
}
