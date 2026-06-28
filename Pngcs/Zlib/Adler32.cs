using System;

namespace Hjg.Pngcs.Zlib
{
	public class Adler32
	{
		public void Update(byte data)
		{
			if (this.pend >= 5550)
			{
				this.updateModulus();
			}
			this.a += (uint)data;
			this.b += this.a;
			this.pend++;
		}

		public void Update(byte[] data)
		{
			this.Update(data, 0, data.Length);
		}

		public void Update(byte[] data, int offset, int length)
		{
			int num = 5550 - this.pend;
			for (int i = 0; i < length; i++)
			{
				if (i == num)
				{
					this.updateModulus();
					num = i + 5550;
				}
				this.a += (uint)data[i + offset];
				this.b += this.a;
				this.pend++;
			}
		}

		public void Reset()
		{
			this.a = 1U;
			this.b = 0U;
			this.pend = 0;
		}

		private void updateModulus()
		{
			this.a %= 65521U;
			this.b %= 65521U;
			this.pend = 0;
		}

		public uint GetValue()
		{
			if (this.pend > 0)
			{
				this.updateModulus();
			}
			return (this.b << 16) | this.a;
		}

		private const int _base = 65521;

		private const int _nmax = 5550;

		private uint a = 1U;

		private uint b;

		private int pend;
	}
}
