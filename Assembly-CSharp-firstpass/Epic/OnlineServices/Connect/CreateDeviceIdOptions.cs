using System;

namespace Epic.OnlineServices.Connect
{
	public class CreateDeviceIdOptions
	{
		public int ApiVersion
		{
			get
			{
				return 1;
			}
		}

		public string DeviceModel { get; set; }
	}
}
