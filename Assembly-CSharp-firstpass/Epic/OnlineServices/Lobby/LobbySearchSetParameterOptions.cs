using System;

namespace Epic.OnlineServices.Lobby
{
	public class LobbySearchSetParameterOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public AttributeData Parameter { get; set; }

		public ComparisonOp ComparisonOp { get; set; }
	}
}
