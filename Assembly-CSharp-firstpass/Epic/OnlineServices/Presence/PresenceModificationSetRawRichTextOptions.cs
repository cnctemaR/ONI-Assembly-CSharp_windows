using System;

namespace Epic.OnlineServices.Presence
{
	public class PresenceModificationSetRawRichTextOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string RichText { get; set; }
	}
}
