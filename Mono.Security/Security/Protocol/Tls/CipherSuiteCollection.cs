using System;
using System.Collections.Generic;

namespace Mono.Security.Protocol.Tls
{
	internal sealed class CipherSuiteCollection : List<CipherSuite>
	{
		public CipherSuite this[string name]
		{
			get
			{
				int num = this.IndexOf(name);
				if (num != -1)
				{
					return base[num];
				}
				return null;
			}
		}

		public CipherSuite this[short code]
		{
			get
			{
				int num = this.IndexOf(code);
				if (num != -1)
				{
					return base[num];
				}
				return null;
			}
		}

		public CipherSuiteCollection(SecurityProtocolType protocol)
		{
			if (protocol <= SecurityProtocolType.Ssl2)
			{
				if (protocol != SecurityProtocolType.Default)
				{
					if (protocol != SecurityProtocolType.Ssl2)
					{
						goto IL_002F;
					}
					goto IL_002F;
				}
			}
			else if (protocol != SecurityProtocolType.Ssl3 && protocol != SecurityProtocolType.Tls)
			{
				goto IL_002F;
			}
			this.protocol = protocol;
			return;
			IL_002F:
			throw new NotSupportedException("Unsupported security protocol type.");
		}

		public int IndexOf(string name)
		{
			int num = 0;
			foreach (CipherSuite cipherSuite in this)
			{
				if (string.CompareOrdinal(name, cipherSuite.Name) == 0)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public int IndexOf(short code)
		{
			int num = 0;
			using (List<CipherSuite>.Enumerator enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Code == code)
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		public void Add(short code, string name, CipherAlgorithmType cipherType, HashAlgorithmType hashType, ExchangeAlgorithmType exchangeType, bool exportable, bool blockMode, byte keyMaterialSize, byte expandedKeyMaterialSize, short effectiveKeyBytes, byte ivSize, byte blockSize)
		{
			SecurityProtocolType securityProtocolType = this.protocol;
			if (securityProtocolType != SecurityProtocolType.Default)
			{
				if (securityProtocolType != SecurityProtocolType.Ssl3)
				{
					if (securityProtocolType == SecurityProtocolType.Tls)
					{
						goto IL_001C;
					}
				}
				else
				{
					base.Add(new SslCipherSuite(code, name, cipherType, hashType, exchangeType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize));
				}
				return;
			}
			IL_001C:
			base.Add(new TlsCipherSuite(code, name, cipherType, hashType, exchangeType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize));
		}

		public IList<string> GetNames()
		{
			List<string> list = new List<string>(base.Count);
			foreach (CipherSuite cipherSuite in this)
			{
				list.Add(cipherSuite.Name);
			}
			return list;
		}

		private SecurityProtocolType protocol;
	}
}
