using System;

namespace Epic.OnlineServices.UI
{
	public class SetToggleFriendsKeyOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public KeyCombination KeyCombination { get; set; }
	}
}
