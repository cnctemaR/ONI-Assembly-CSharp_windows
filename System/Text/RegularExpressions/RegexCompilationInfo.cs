using System;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexCompilationInfo
	{
		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic)
			: this(pattern, options, name, fullnamespace, ispublic, Regex.s_defaultMatchTimeout)
		{
		}

		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic, TimeSpan matchTimeout)
		{
			this.Pattern = pattern;
			this.Name = name;
			this.Namespace = fullnamespace;
			this.Options = options;
			this.IsPublic = ispublic;
			this.MatchTimeout = matchTimeout;
		}

		public bool IsPublic { get; set; }

		public TimeSpan MatchTimeout
		{
			get
			{
				return this._matchTimeout;
			}
			set
			{
				Regex.ValidateMatchTimeout(value);
				this._matchTimeout = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Name");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException(SR.Format("Argument {0} cannot be zero-length.", "Name"), "Name");
				}
				this._name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this._nspace;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Namespace");
				}
				this._nspace = value;
			}
		}

		public RegexOptions Options { get; set; }

		public string Pattern
		{
			get
			{
				return this._pattern;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Pattern");
				}
				this._pattern = value;
			}
		}

		private string _pattern;

		private string _name;

		private string _nspace;

		private TimeSpan _matchTimeout;
	}
}
