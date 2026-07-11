using System;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexCompilationInfo
	{
		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic)
		{
			this.Pattern = pattern;
			this.Options = options;
			this.Name = name;
			this.Namespace = fullnamespace;
			this.IsPublic = ispublic;
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
					throw new ArgumentNullException("Name");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Name");
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
					throw new ArgumentNullException("Namespace");
				}
				this.nspace = value;
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
					throw new ArgumentNullException("pattern");
				}
				this.pattern = value;
			}
		}

		private string pattern;

		private string name;

		private string nspace;

		private RegexOptions options;

		private bool isPublic;
	}
}
