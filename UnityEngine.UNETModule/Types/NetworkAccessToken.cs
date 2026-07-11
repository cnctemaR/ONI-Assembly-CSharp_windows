using System;

namespace UnityEngine.Networking.Types
{
	public class NetworkAccessToken
	{
		public NetworkAccessToken()
		{
			this.array = new byte[64];
		}

		public NetworkAccessToken(byte[] array)
		{
			this.array = array;
		}

		public NetworkAccessToken(string strArray)
		{
			try
			{
				this.array = Convert.FromBase64String(strArray);
			}
			catch (Exception)
			{
				this.array = new byte[64];
			}
		}

		public string GetByteString()
		{
			return Convert.ToBase64String(this.array);
		}

		public bool IsValid()
		{
			bool flag = this.array == null || this.array.Length != 64;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = false;
				foreach (byte b in this.array)
				{
					bool flag4 = b > 0;
					if (flag4)
					{
						flag3 = true;
						break;
					}
				}
				flag2 = flag3;
			}
			return flag2;
		}

		private const int NETWORK_ACCESS_TOKEN_SIZE = 64;

		public byte[] array;
	}
}
