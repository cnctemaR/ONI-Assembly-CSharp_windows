using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AssertionFailedException : Exception
	{
		public AssertionFailedException()
		{
			this.Message = "";
		}

		[NullableContext(2)]
		public AssertionFailedException(string message)
			: base("Assertion failed! " + message)
		{
			this.Message = message ?? "";
		}

		public AssertionFailedException([Nullable(2)] string message, Exception innerException)
			: base("Assertion failed! " + message, innerException)
		{
			this.Message = message ?? "";
		}

		public AssertionFailedException([Nullable(2)] string message, string expression)
			: base("Assertion failed! " + expression + " " + message)
		{
			this.Message = message ?? "";
			this.Expression = expression;
		}

		public string Expression { get; } = "";

		public new string Message { get; }

		private const string AssertFailed = "Assertion failed! ";
	}
}
