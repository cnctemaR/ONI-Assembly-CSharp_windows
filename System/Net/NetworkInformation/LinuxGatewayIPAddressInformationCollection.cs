using System;

namespace System.Net.NetworkInformation
{
	internal class LinuxGatewayIPAddressInformationCollection : GatewayIPAddressInformationCollection
	{
		private LinuxGatewayIPAddressInformationCollection(bool isReadOnly)
		{
			this.is_readonly = isReadOnly;
		}

		public LinuxGatewayIPAddressInformationCollection(IPAddressCollection col)
		{
			foreach (IPAddress ipaddress in col)
			{
				this.Add(new GatewayIPAddressInformationImpl(ipaddress));
			}
			this.is_readonly = true;
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.is_readonly;
			}
		}

		public static readonly LinuxGatewayIPAddressInformationCollection Empty = new LinuxGatewayIPAddressInformationCollection(true);

		private bool is_readonly;
	}
}
