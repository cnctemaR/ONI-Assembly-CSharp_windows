using System;
using System.Runtime.Serialization;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexMatchTimeoutException : TimeoutException, ISerializable
	{
		public RegexMatchTimeoutException(string regexInput, string regexPattern, TimeSpan matchTimeout)
			: base("The RegEx engine has timed out while trying to match a pattern to an input string. This can occur for many reasons, including very large inputs or excessive backtracking caused by nested quantifiers, back-references and other factors.")
		{
			this.Input = regexInput;
			this.Pattern = regexPattern;
			this.MatchTimeout = matchTimeout;
		}

		public RegexMatchTimeoutException()
		{
		}

		public RegexMatchTimeoutException(string message)
			: base(message)
		{
		}

		public RegexMatchTimeoutException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected RegexMatchTimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Input = info.GetString("regexInput");
			this.Pattern = info.GetString("regexPattern");
			this.MatchTimeout = new TimeSpan(info.GetInt64("timeoutTicks"));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("regexInput", this.Input);
			info.AddValue("regexPattern", this.Pattern);
			info.AddValue("timeoutTicks", this.MatchTimeout.Ticks);
		}

		public string Input { get; } = string.Empty;

		public string Pattern { get; } = string.Empty;

		public TimeSpan MatchTimeout { get; } = TimeSpan.FromTicks(-1L);
	}
}
