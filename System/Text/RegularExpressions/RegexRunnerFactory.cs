using System;
using System.ComponentModel;

namespace System.Text.RegularExpressions
{
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class RegexRunnerFactory
	{
		protected internal abstract RegexRunner CreateInstance();
	}
}
