using System;
using System.Runtime.Serialization;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexCompilationInfo
	{
		[OnDeserializing]
		private void InitMatchTimeoutDefaultForOldVersionDeserialization(StreamingContext unusedContext)
		{
			this.matchTimeout = Regex.DefaultMatchTimeout;
		}

		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic)
			: this(pattern, options, name, fullnamespace, ispublic, Regex.DefaultMatchTimeout)
		{
		}

		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic, TimeSpan matchTimeout)
		{
			this.Pattern = pattern;
			this.Name = name;
			this.Namespace = fullnamespace;
			this.options = options;
			this.isPublic = ispublic;
			this.MatchTimeout = matchTimeout;
		}

		public string Pattern
		{
			get
			{
				return this.pattern;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.pattern = value;
			}
		}

		public RegexOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException(global::SR.GetString("Argument {0} cannot be null or zero-length.", new object[] { "value" }), "value");
				}
				this.name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.nspace;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.nspace = value;
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.isPublic;
			}
			set
			{
				this.isPublic = value;
			}
		}

		public TimeSpan MatchTimeout
		{
			get
			{
				return this.matchTimeout;
			}
			set
			{
				Regex.ValidateMatchTimeout(value);
				this.matchTimeout = value;
			}
		}

		private string pattern;

		private RegexOptions options;

		private string name;

		private string nspace;

		private bool isPublic;

		[OptionalField(VersionAdded = 2)]
		private TimeSpan matchTimeout;
	}
}
