using System;

namespace System.Net
{
	public class IPHostEntry
	{
		public string HostName
		{
			get
			{
				return this.hostName;
			}
			set
			{
				this.hostName = value;
			}
		}

		public string[] Aliases
		{
			get
			{
				return this.aliases;
			}
			set
			{
				this.aliases = value;
			}
		}

		public IPAddress[] AddressList
		{
			get
			{
				return this.addressList;
			}
			set
			{
				this.addressList = value;
			}
		}

		private string hostName;

		private string[] aliases;

		private IPAddress[] addressList;

		internal bool isTrustedHost = true;
	}
}
