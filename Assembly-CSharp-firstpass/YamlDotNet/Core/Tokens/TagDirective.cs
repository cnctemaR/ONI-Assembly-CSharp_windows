using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class TagDirective : Token
	{
		public TagDirective(string handle, string prefix)
			: this(handle, prefix, Mark.Empty, Mark.Empty)
		{
		}

		public TagDirective(string handle, string prefix, Mark start, Mark end)
			: base(start, end)
		{
			if (string.IsNullOrEmpty(handle))
			{
				throw new ArgumentNullException("handle", "Tag handle must not be empty.");
			}
			if (!TagDirective.tagHandleValidator.IsMatch(handle))
			{
				throw new ArgumentException("Tag handle must start and end with '!' and contain alphanumerical characters only.", "handle");
			}
			this.handle = handle;
			if (string.IsNullOrEmpty(prefix))
			{
				throw new ArgumentNullException("prefix", "Tag prefix must not be empty.");
			}
			this.prefix = prefix;
		}

		public string Handle
		{
			get
			{
				return this.handle;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public override bool Equals(object obj)
		{
			TagDirective tagDirective = obj as TagDirective;
			return tagDirective != null && this.handle.Equals(tagDirective.handle) && this.prefix.Equals(tagDirective.prefix);
		}

		public override int GetHashCode()
		{
			return this.handle.GetHashCode() ^ this.prefix.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} => {1}", this.handle, this.prefix);
		}

		private readonly string handle;

		private readonly string prefix;

		private static readonly Regex tagHandleValidator = new Regex("^!([0-9A-Za-z_\\-]*!)?$", RegexOptions.Compiled);
	}
}
