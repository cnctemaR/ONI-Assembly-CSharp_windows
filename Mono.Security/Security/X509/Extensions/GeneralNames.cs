using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	internal class GeneralNames
	{
		public GeneralNames()
		{
		}

		public GeneralNames(string[] rfc822s, string[] dnsNames, string[] ipAddresses, string[] uris)
		{
			this.asn = new ASN1(48);
			if (rfc822s != null)
			{
				this.rfc822Name = new ArrayList();
				foreach (string text in rfc822s)
				{
					this.asn.Add(new ASN1(129, Encoding.ASCII.GetBytes(text)));
					this.rfc822Name.Add(rfc822s);
				}
			}
			if (dnsNames != null)
			{
				this.dnsName = new ArrayList();
				foreach (string text2 in dnsNames)
				{
					this.asn.Add(new ASN1(130, Encoding.ASCII.GetBytes(text2)));
					this.dnsName.Add(text2);
				}
			}
			if (ipAddresses != null)
			{
				this.ipAddr = new ArrayList();
				foreach (string text3 in ipAddresses)
				{
					string[] array = text3.Split(new char[] { '.', ':' });
					byte[] array2 = new byte[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = byte.Parse(array[j]);
					}
					this.asn.Add(new ASN1(135, array2));
					this.ipAddr.Add(text3);
				}
			}
			if (uris != null)
			{
				this.uris = new ArrayList();
				foreach (string text4 in uris)
				{
					this.asn.Add(new ASN1(134, Encoding.ASCII.GetBytes(text4)));
					this.uris.Add(text4);
				}
			}
		}

		public GeneralNames(ASN1 sequence)
		{
			int i = 0;
			while (i < sequence.Count)
			{
				byte tag = sequence[i].Tag;
				switch (tag)
				{
				case 129:
					if (this.rfc822Name == null)
					{
						this.rfc822Name = new ArrayList();
					}
					this.rfc822Name.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 130:
					if (this.dnsName == null)
					{
						this.dnsName = new ArrayList();
					}
					this.dnsName.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 131:
				case 133:
					break;
				case 132:
					goto IL_00C6;
				case 134:
					if (this.uris == null)
					{
						this.uris = new ArrayList();
					}
					this.uris.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 135:
				{
					if (this.ipAddr == null)
					{
						this.ipAddr = new ArrayList();
					}
					byte[] value = sequence[i].Value;
					string text = ((value.Length == 4) ? "." : ":");
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < value.Length; j++)
					{
						stringBuilder.Append(value[j].ToString());
						if (j < value.Length - 1)
						{
							stringBuilder.Append(text);
						}
					}
					this.ipAddr.Add(stringBuilder.ToString());
					if (this.ipAddr == null)
					{
						this.ipAddr = new ArrayList();
					}
					break;
				}
				default:
					if (tag == 164)
					{
						goto IL_00C6;
					}
					break;
				}
				IL_01CB:
				i++;
				continue;
				IL_00C6:
				if (this.directoryNames == null)
				{
					this.directoryNames = new ArrayList();
				}
				this.directoryNames.Add(X501.ToString(sequence[i][0]));
				goto IL_01CB;
			}
		}

		public string[] RFC822
		{
			get
			{
				if (this.rfc822Name == null)
				{
					return new string[0];
				}
				return (string[])this.rfc822Name.ToArray(typeof(string));
			}
		}

		public string[] DirectoryNames
		{
			get
			{
				if (this.directoryNames == null)
				{
					return new string[0];
				}
				return (string[])this.directoryNames.ToArray(typeof(string));
			}
		}

		public string[] DNSNames
		{
			get
			{
				if (this.dnsName == null)
				{
					return new string[0];
				}
				return (string[])this.dnsName.ToArray(typeof(string));
			}
		}

		public string[] UniformResourceIdentifiers
		{
			get
			{
				if (this.uris == null)
				{
					return new string[0];
				}
				return (string[])this.uris.ToArray(typeof(string));
			}
		}

		public string[] IPAddresses
		{
			get
			{
				if (this.ipAddr == null)
				{
					return new string[0];
				}
				return (string[])this.ipAddr.ToArray(typeof(string));
			}
		}

		public byte[] GetBytes()
		{
			return this.asn.GetBytes();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.rfc822Name != null)
			{
				foreach (object obj in this.rfc822Name)
				{
					string text = (string)obj;
					stringBuilder.Append("RFC822 Name=");
					stringBuilder.Append(text);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (this.dnsName != null)
			{
				foreach (object obj2 in this.dnsName)
				{
					string text2 = (string)obj2;
					stringBuilder.Append("DNS Name=");
					stringBuilder.Append(text2);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (this.directoryNames != null)
			{
				foreach (object obj3 in this.directoryNames)
				{
					string text3 = (string)obj3;
					stringBuilder.Append("Directory Address: ");
					stringBuilder.Append(text3);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (this.uris != null)
			{
				foreach (object obj4 in this.uris)
				{
					string text4 = (string)obj4;
					stringBuilder.Append("URL=");
					stringBuilder.Append(text4);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (this.ipAddr != null)
			{
				foreach (object obj5 in this.ipAddr)
				{
					string text5 = (string)obj5;
					stringBuilder.Append("IP Address=");
					stringBuilder.Append(text5);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		private ArrayList rfc822Name;

		private ArrayList dnsName;

		private ArrayList directoryNames;

		private ArrayList uris;

		private ArrayList ipAddr;

		private ASN1 asn;
	}
}
