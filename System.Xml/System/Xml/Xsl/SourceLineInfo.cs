using System;
using System.Diagnostics;

namespace System.Xml.Xsl
{
	[DebuggerDisplay("{Uri} [{StartLine},{StartPos} -- {EndLine},{EndPos}]")]
	internal class SourceLineInfo : ISourceLineInfo
	{
		public SourceLineInfo(string uriString, int startLine, int startPos, int endLine, int endPos)
			: this(uriString, new Location(startLine, startPos), new Location(endLine, endPos))
		{
		}

		public SourceLineInfo(string uriString, Location start, Location end)
		{
			this.uriString = uriString;
			this.start = start;
			this.end = end;
		}

		public string Uri
		{
			get
			{
				return this.uriString;
			}
		}

		public int StartLine
		{
			get
			{
				return this.start.Line;
			}
		}

		public int StartPos
		{
			get
			{
				return this.start.Pos;
			}
		}

		public int EndLine
		{
			get
			{
				return this.end.Line;
			}
		}

		public int EndPos
		{
			get
			{
				return this.end.Pos;
			}
		}

		public Location End
		{
			get
			{
				return this.end;
			}
		}

		public Location Start
		{
			get
			{
				return this.start;
			}
		}

		public bool IsNoSource
		{
			get
			{
				return this.StartLine == 16707566;
			}
		}

		[Conditional("DEBUG")]
		public static void Validate(ISourceLineInfo lineInfo)
		{
			if (lineInfo.Start.Line != 0)
			{
				int line = lineInfo.Start.Line;
			}
		}

		public static string GetFileName(string uriString)
		{
			Uri uri;
			if (uriString.Length != 0 && global::System.Uri.TryCreate(uriString, UriKind.Absolute, out uri) && uri.IsFile)
			{
				return uri.LocalPath;
			}
			return uriString;
		}

		protected string uriString;

		protected Location start;

		protected Location end;

		protected const int NoSourceMagicNumber = 16707566;

		public static SourceLineInfo NoSource = new SourceLineInfo(string.Empty, 16707566, 0, 16707566, 0);
	}
}
