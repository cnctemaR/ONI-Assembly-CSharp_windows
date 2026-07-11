using System;

namespace Epic.OnlineServices.Lobby
{
	public class Attribute
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData Data { get; set; }

		public LobbyAttributeVisibility Visibility { get; set; }
	}
}
