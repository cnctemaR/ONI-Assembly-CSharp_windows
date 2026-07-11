using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class StrongNamePublicKeyBlob
	{
		public StrongNamePublicKeyBlob(byte[] publicKey)
		{
			if (publicKey == null)
			{
				throw new ArgumentNullException("publicKey");
			}
			this.pubkey = publicKey;
		}

		internal static StrongNamePublicKeyBlob FromString(string s)
		{
			if (s == null || s.Length == 0)
			{
				return null;
			}
			byte[] array = new byte[s.Length / 2];
			int i = 0;
			int num = 0;
			while (i < s.Length)
			{
				byte b = StrongNamePublicKeyBlob.CharToByte(s[i]);
				byte b2 = StrongNamePublicKeyBlob.CharToByte(s[i + 1]);
				array[num] = Convert.ToByte((int)(b * 16 + b2));
				i += 2;
				num++;
			}
			return new StrongNamePublicKeyBlob(array);
		}

		private static byte CharToByte(char c)
		{
			char c2 = char.ToLowerInvariant(c);
			if (char.IsDigit(c2))
			{
				return (byte)(c2 - '0');
			}
			return (byte)(c2 - 'a' + '\n');
		}

		public override bool Equals(object obj)
		{
			StrongNamePublicKeyBlob strongNamePublicKeyBlob = obj as StrongNamePublicKeyBlob;
			if (strongNamePublicKeyBlob == null)
			{
				return false;
			}
			bool flag = this.pubkey.Length == strongNamePublicKeyBlob.pubkey.Length;
			if (flag)
			{
				for (int i = 0; i < this.pubkey.Length; i++)
				{
					if (this.pubkey[i] != strongNamePublicKeyBlob.pubkey[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 0;
			int i = 0;
			int num2 = Math.Min(this.pubkey.Length, 4);
			while (i < num2)
			{
				num = (num << 8) + (int)this.pubkey[i++];
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.pubkey.Length; i++)
			{
				stringBuilder.Append(this.pubkey[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		internal byte[] pubkey;
	}
}
