using System;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IgnoreCommentedLinesAttribute : Attribute
	{
		public string CommentMarker { get; private set; }

		public bool AnyPlace { get; private set; }

		public IgnoreCommentedLinesAttribute(string commentMarker)
			: this(commentMarker, true)
		{
		}

		public IgnoreCommentedLinesAttribute(string commentMarker, bool anyPlace)
		{
			if (commentMarker == null || commentMarker.Trim().Length == 0)
			{
				throw new BadUsageException("The comment string parameter can't be null or empty.");
			}
			this.CommentMarker = commentMarker.Trim();
			this.AnyPlace = anyPlace;
		}
	}
}
