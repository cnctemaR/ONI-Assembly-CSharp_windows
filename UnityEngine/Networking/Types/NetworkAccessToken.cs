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
			bool flag;
			if (this.array == null || this.array.Length != 64)
			{
				flag = false;
			}
			else
			{
				bool flag2 = false;
				foreach (byte b in this.array)
				{
					if (b != 0)
					{
						flag2 = true;
						break;
					}
				}
				flag = flag2;
			}
			return flag;
		}

		private const int NETWORK_ACCESS_TOKEN_SIZE = 64;

		public byte[] array;
	}
}
