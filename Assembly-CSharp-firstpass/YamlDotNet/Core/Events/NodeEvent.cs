using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Events
{
	public abstract class NodeEvent : ParsingEvent
	{
		protected NodeEvent(string anchor, string tag, Mark start, Mark end)
			: base(start, end)
		{
			if (anchor != null)
			{
				if (anchor.Length == 0)
				{
					throw new ArgumentException("Anchor value must not be empty.", "anchor");
				}
				if (!NodeEvent.anchorValidator.IsMatch(anchor))
				{
					throw new ArgumentException("Anchor value must contain alphanumerical characters only.", "anchor");
				}
			}
			if (tag != null && tag.Length == 0)
			{
				throw new ArgumentException("Tag value must not be empty.", "tag");
			}
			this.anchor = anchor;
			this.tag = tag;
		}

		protected NodeEvent(string anchor, string tag)
			: this(anchor, tag, Mark.Empty, Mark.Empty)
		{
		}

		public string Anchor
		{
			get
			{
				return this.anchor;
			}
		}

		public string Tag
		{
			get
			{
				return this.tag;
			}
		}

		public abstract bool IsCanonical { get; }

		internal static readonly Regex anchorValidator = new Regex("^[0-9a-zA-Z_\\-]+$", RegexOptions.Compiled);

		private readonly string anchor;

		private readonly string tag;
	}
}
