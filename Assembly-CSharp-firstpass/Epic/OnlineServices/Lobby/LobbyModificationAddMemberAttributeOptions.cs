using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbyModificationAddMemberAttributeOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData Attribute { get; set; }

		public LobbyAttributeVisibility Visibility { get; set; }
	}
}
