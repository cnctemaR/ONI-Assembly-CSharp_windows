using System;
using System.IO;

namespace Hjg.Pngcs
{
	internal abstract class ProgressiveOutputStream : MemoryStream
	{
		public ProgressiveOutputStream(int size_0)
		{
			this.size = size_0;
			if (this.size < 8)
			{
				throw new PngjException("bad size for ProgressiveOutputStream: " + this.size);
			}
		}

		public override void Close()
		{
			this.Flush();
			base.Close();
		}

		public override void Flush()
		{
			base.Flush();
			this.CheckFlushBuffer(true);
		}

		public override void Write(byte[] b, int off, int len)
		{
			base.Write(b, off, len);
			this.CheckFlushBuffer(false);
		}

		public void Write(byte[] b)
		{
			this.Write(b, 0, b.Length);
			this.CheckFlushBuffer(false);
		}

		private void CheckFlushBuffer(bool forced)
		{
			int num = (int)this.Position;
			byte[] buffer = this.GetBuffer();
			while (forced || num >= this.size)
			{
				int num2 = this.size;
				if (num2 > num)
				{
					num2 = num;
				}
				if (num2 == 0)
				{
					return;
				}
				this.FlushBuffer(buffer, num2);
				this.countFlushed += (long)num2;
				int num3 = num - num2;
				num = num3;
				this.Position = (long)num;
				if (num3 > 0)
				{
					Array.Copy(buffer, num2, buffer, 0, num3);
				}
			}
		}

		protected abstract void FlushBuffer(byte[] b, int n);

		public long GetCountFlushed()
		{
			return this.countFlushed;
		}

		private readonly int size;

		private long countFlushed;
	}
}
