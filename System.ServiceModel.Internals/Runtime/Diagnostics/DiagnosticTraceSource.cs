using System;
using System.Diagnostics;

namespace System.Runtime.Diagnostics
{
	internal class DiagnosticTraceSource : TraceSource
	{
		internal DiagnosticTraceSource(string name)
			: base(name)
		{
		}

		protected override string[] GetSupportedAttributes()
		{
			return new string[] { "propagateActivity" };
		}

		internal bool PropagateActivity
		{
			get
			{
				bool flag = false;
				string text = base.Attributes["propagateActivity"];
				if (!string.IsNullOrEmpty(text) && !bool.TryParse(text, out flag))
				{
					flag = false;
				}
				return flag;
			}
			set
			{
				base.Attributes["propagateActivity"] = value.ToString();
			}
		}

		private const string PropagateActivityValue = "propagateActivity";
	}
}
