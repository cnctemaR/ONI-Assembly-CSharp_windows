using System;

namespace Epic.OnlineServices.Connect
{
	public class CreateUserOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public ContinuanceToken ContinuanceToken { get; set; }
	}
}
