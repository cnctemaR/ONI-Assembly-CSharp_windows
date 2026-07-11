using System;

namespace Epic.OnlineServices.Sessions
{
	public class CreateSessionSearchOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public uint MaxSearchResults { get; set; }
	}
}
