using System;
using System.Globalization;
using System.IO;

namespace System.Net.NetworkInformation
{
	internal sealed class LinuxNetworkInterface : UnixNetworkInterface
	{
		internal string IfacePath
		{
			get
			{
				return this.iface_path;
			}
		}

		internal LinuxNetworkInterface(string name)
			: base(name)
		{
			this.iface_path = "/sys/class/net/" + name + "/";
			this.iface_operstate_path = this.iface_path + "operstate";
			this.iface_flags_path = this.iface_path + "flags";
		}

		public override IPInterfaceProperties GetIPProperties()
		{
			if (this.ipproperties == null)
			{
				this.ipproperties = new LinuxIPInterfaceProperties(this, this.addresses);
			}
			return this.ipproperties;
		}

		public override IPv4InterfaceStatistics GetIPv4Statistics()
		{
			if (this.ipv4stats == null)
			{
				this.ipv4stats = new LinuxIPv4InterfaceStatistics(this);
			}
			return this.ipv4stats;
		}

		public override OperationalStatus OperationalStatus
		{
			get
			{
				if (!Directory.Exists(this.iface_path))
				{
					return OperationalStatus.Unknown;
				}
				try
				{
					string text = LinuxNetworkInterface.ReadLine(this.iface_operstate_path);
					uint num = global::<PrivateImplementationDetails>.ComputeStringHash(text);
					if (num <= 2313571237U)
					{
						if (num != 1035581717U)
						{
							if (num != 1128467232U)
							{
								if (num == 2313571237U)
								{
									if (text == "notpresent")
									{
										return OperationalStatus.NotPresent;
									}
								}
							}
							else if (text == "up")
							{
								return OperationalStatus.Up;
							}
						}
						else if (text == "down")
						{
							return OperationalStatus.Down;
						}
					}
					else if (num <= 2966218339U)
					{
						if (num != 2608177081U)
						{
							if (num == 2966218339U)
							{
								if (text == "lowerlayerdown")
								{
									return OperationalStatus.LowerLayerDown;
								}
							}
						}
						else if (text == "unknown")
						{
							return OperationalStatus.Unknown;
						}
					}
					else if (num != 3340047486U)
					{
						if (num == 3948890523U)
						{
							if (text == "testing")
							{
								return OperationalStatus.Testing;
							}
						}
					}
					else if (text == "dormant")
					{
						return OperationalStatus.Dormant;
					}
				}
				catch
				{
				}
				return OperationalStatus.Unknown;
			}
		}

		public override bool SupportsMulticast
		{
			get
			{
				if (!Directory.Exists(this.iface_path))
				{
					return false;
				}
				bool flag;
				try
				{
					string text = LinuxNetworkInterface.ReadLine(this.iface_flags_path);
					if (text.Length > 2 && text[0] == '0' && text[1] == 'x')
					{
						text = text.Substring(2);
					}
					flag = (ulong.Parse(text, NumberStyles.HexNumber) & 4096UL) == 4096UL;
				}
				catch
				{
					flag = false;
				}
				return flag;
			}
		}

		internal static string ReadLine(string path)
		{
			string text;
			using (FileStream fileStream = File.OpenRead(path))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					text = streamReader.ReadLine();
				}
			}
			return text;
		}

		private string iface_path;

		private string iface_operstate_path;

		private string iface_flags_path;
	}
}
