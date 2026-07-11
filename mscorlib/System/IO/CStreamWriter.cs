using System;
using System.Text;

namespace System.IO
{
	internal class CStreamWriter : StreamWriter
	{
		public CStreamWriter(Stream stream, Encoding encoding, bool leaveOpen)
			: base(stream, encoding, 1024, leaveOpen)
		{
			this.driver = (TermInfoDriver)ConsoleDriver.driver;
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (count <= 0)
			{
				return;
			}
			if (!this.driver.Initialized)
			{
				try
				{
					base.Write(buffer, index, count);
				}
				catch (IOException)
				{
				}
				return;
			}
			lock (this)
			{
				int num = index + count;
				int num2 = index;
				int num3 = 0;
				do
				{
					char c = buffer[num2++];
					if (this.driver.IsSpecialKey(c))
					{
						if (num3 > 0)
						{
							try
							{
								base.Write(buffer, index, num3);
							}
							catch (IOException)
							{
							}
							num3 = 0;
						}
						this.driver.WriteSpecialKey(c);
						index = num2;
					}
					else
					{
						num3++;
					}
				}
				while (num2 < num);
				if (num3 > 0)
				{
					try
					{
						base.Write(buffer, index, num3);
					}
					catch (IOException)
					{
					}
				}
			}
		}

		public override void Write(char val)
		{
			lock (this)
			{
				try
				{
					if (this.driver.IsSpecialKey(val))
					{
						this.driver.WriteSpecialKey(val);
					}
					else
					{
						this.InternalWriteChar(val);
					}
				}
				catch (IOException)
				{
				}
			}
		}

		public void InternalWriteString(string val)
		{
			try
			{
				base.Write(val);
			}
			catch (IOException)
			{
			}
		}

		public void InternalWriteChar(char val)
		{
			try
			{
				base.Write(val);
			}
			catch (IOException)
			{
			}
		}

		public void InternalWriteChars(char[] buffer, int n)
		{
			try
			{
				base.Write(buffer, 0, n);
			}
			catch (IOException)
			{
			}
		}

		public override void Write(char[] val)
		{
			this.Write(val, 0, val.Length);
		}

		public override void Write(string val)
		{
			if (val == null)
			{
				return;
			}
			if (this.driver.Initialized)
			{
				this.Write(val.ToCharArray());
				return;
			}
			try
			{
				base.Write(val);
			}
			catch (IOException)
			{
			}
		}

		private TermInfoDriver driver;
	}
}
