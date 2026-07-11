using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexMatchTimeoutException : TimeoutException, ISerializable
	{
		public RegexMatchTimeoutException(string regexInput, string regexPattern, TimeSpan matchTimeout)
			: base(global::SR.GetString("The RegEx engine has timed out while trying to match a pattern to an input string. This can occur for many reasons, including very large inputs or excessive backtracking caused by nested quantifiers, back-references and other factors."))
		{
			this.Init(regexInput, regexPattern, matchTimeout);
		}

		public RegexMatchTimeoutException()
		{
			this.Init();
		}

		public RegexMatchTimeoutException(string message)
			: base(message)
		{
			this.Init();
		}

		public RegexMatchTimeoutException(string message, Exception inner)
			: base(message, inner)
		{
			this.Init();
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		protected RegexMatchTimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			string @string = info.GetString("regexInput");
			string string2 = info.GetString("regexPattern");
			TimeSpan timeSpan = TimeSpan.FromTicks(info.GetInt64("timeoutTicks"));
			this.Init(@string, string2, timeSpan);
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			base.GetObjectData(si, context);
			si.AddValue("regexInput", this.regexInput);
			si.AddValue("regexPattern", this.regexPattern);
			si.AddValue("timeoutTicks", this.matchTimeout.Ticks);
		}

		private void Init()
		{
			this.Init("", "", TimeSpan.FromTicks(-1L));
		}

		private void Init(string input, string pattern, TimeSpan timeout)
		{
			this.regexInput = input;
			this.regexPattern = pattern;
			this.matchTimeout = timeout;
		}

		public string Pattern
		{
			[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
			get
			{
				return this.regexPattern;
			}
		}

		public string Input
		{
			[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
			get
			{
				return this.regexInput;
			}
		}

		public TimeSpan MatchTimeout
		{
			[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
			get
			{
				return this.matchTimeout;
			}
		}

		private string regexInput;

		private string regexPattern;

		private TimeSpan matchTimeout = TimeSpan.FromTicks(-1L);
	}
}
