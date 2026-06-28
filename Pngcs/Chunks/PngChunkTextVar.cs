using System;

namespace Hjg.Pngcs.Chunks
{
	public abstract class PngChunkTextVar : PngChunkMultiple
	{
		protected internal PngChunkTextVar(string id, ImageInfo info)
			: base(id, info)
		{
		}

		public override PngChunk.ChunkOrderingConstraint GetOrderingConstraint()
		{
			return PngChunk.ChunkOrderingConstraint.NONE;
		}

		public string GetKey()
		{
			return this.key;
		}

		public string GetVal()
		{
			return this.val;
		}

		public void SetKeyVal(string key, string val)
		{
			this.key = key;
			this.val = val;
		}

		public const string KEY_Title = "Title";

		public const string KEY_Author = "Author";

		public const string KEY_Description = "Description";

		public const string KEY_Copyright = "Copyright";

		public const string KEY_Creation_Time = "Creation Time";

		public const string KEY_Software = "Software";

		public const string KEY_Disclaimer = "Disclaimer";

		public const string KEY_Warning = "Warning";

		public const string KEY_Source = "Source";

		public const string KEY_Comment = "Comment";

		protected internal string key;

		protected internal string val;

		public class PngTxtInfo
		{
			public string title;

			public string author;

			public string description;

			public string creation_time;

			public string software;

			public string disclaimer;

			public string warning;

			public string source;

			public string comment;
		}
	}
}
