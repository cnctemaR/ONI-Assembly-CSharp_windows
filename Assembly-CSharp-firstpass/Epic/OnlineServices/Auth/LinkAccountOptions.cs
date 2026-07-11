using System;

namespace Epic.OnlineServices.Auth
{
	public class LinkAccountOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public LinkAccountFlags LinkAccountFlags { get; set; }

		public ContinuanceToken ContinuanceToken { get; set; }

		public EpicAccountId LocalUserId { get; set; }
	}
}
