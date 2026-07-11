using System;
using System.IO;

namespace System.Net.NetworkInformation
{
	internal sealed class LinuxIPv4InterfaceProperties : UnixIPv4InterfaceProperties
	{
		public LinuxIPv4InterfaceProperties(LinuxNetworkInterface iface)
			: base(iface)
		{
		}

		public override bool IsForwardingEnabled
		{
			get
			{
				string text = "/proc/sys/net/ipv4/conf/" + this.iface.Name + "/forwarding";
				return File.Exists(text) && LinuxNetworkInterface.ReadLine(text) != "0";
			}
		}

		public override int Mtu
		{
			get
			{
				string text = (this.iface as LinuxNetworkInterface).IfacePath + "mtu";
				int num = 0;
				if (File.Exists(text))
				{
					string text2 = LinuxNetworkInterface.ReadLine(text);
					try
					{
						num = int.Parse(text2);
					}
					catch
					{
					}
				}
				return num;
			}
		}
	}
}
