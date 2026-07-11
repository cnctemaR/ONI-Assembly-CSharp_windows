using System;

namespace Epic.OnlineServices.Sessions
{
	public class UpdateSessionOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public SessionModification SessionModificationHandle { get; set; }
	}
}
