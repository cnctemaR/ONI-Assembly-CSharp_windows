using System;
using System.IO;
using System.Text;

namespace System
{
	internal class TermInfoReader
	{
		public TermInfoReader(string term, string filename)
		{
			using (FileStream fileStream = File.OpenRead(filename))
			{
				long length = fileStream.Length;
				if (length > 4096L)
				{
					throw new Exception("File must be smaller than 4K");
				}
				this.buffer = new byte[(int)length];
				if (fileStream.Read(this.buffer, 0, this.buffer.Length) != this.buffer.Length)
				{
					throw new Exception("Short read");
				}
				this.ReadHeader(this.buffer, ref this.booleansOffset);
				this.ReadNames(this.buffer, ref this.booleansOffset);
			}
		}

		public TermInfoReader(string term, byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.buffer = buffer;
			this.ReadHeader(buffer, ref this.booleansOffset);
			this.ReadNames(buffer, ref this.booleansOffset);
		}

		private void ReadHeader(byte[] buffer, ref int position)
		{
			short @int = this.GetInt16(buffer, position);
			position += 2;
			if (@int != 282)
			{
				throw new Exception(string.Format("Magic number is wrong: {0}", @int));
			}
			this.GetInt16(buffer, position);
			position += 2;
			this.boolSize = this.GetInt16(buffer, position);
			position += 2;
			this.numSize = this.GetInt16(buffer, position);
			position += 2;
			this.strOffsets = this.GetInt16(buffer, position);
			position += 2;
			this.GetInt16(buffer, position);
			position += 2;
		}

		private void ReadNames(byte[] buffer, ref int position)
		{
			string @string = this.GetString(buffer, position);
			position += @string.Length + 1;
		}

		public bool Get(TermInfoBooleans boolean)
		{
			if (boolean < TermInfoBooleans.AutoLeftMargin || boolean >= TermInfoBooleans.Last || boolean >= (TermInfoBooleans)this.boolSize)
			{
				return false;
			}
			int num = this.booleansOffset;
			num = (int)(num + boolean);
			return this.buffer[num] != 0;
		}

		public int Get(TermInfoNumbers number)
		{
			if (number < TermInfoNumbers.Columns || number >= TermInfoNumbers.Last || number > (TermInfoNumbers)this.numSize)
			{
				return -1;
			}
			int num = this.booleansOffset + (int)this.boolSize;
			if (num % 2 == 1)
			{
				num++;
			}
			num = (int)(num + number * TermInfoNumbers.Lines);
			return (int)this.GetInt16(this.buffer, num);
		}

		public string Get(TermInfoStrings tstr)
		{
			if (tstr < TermInfoStrings.BackTab || tstr >= TermInfoStrings.Last || tstr > (TermInfoStrings)this.strOffsets)
			{
				return null;
			}
			int num = this.booleansOffset + (int)this.boolSize;
			if (num % 2 == 1)
			{
				num++;
			}
			num += (int)(this.numSize * 2);
			int @int = (int)this.GetInt16(this.buffer, (int)(num + tstr * TermInfoStrings.CarriageReturn));
			if (@int == -1)
			{
				return null;
			}
			return this.GetString(this.buffer, num + (int)(this.strOffsets * 2) + @int);
		}

		public byte[] GetStringBytes(TermInfoStrings tstr)
		{
			if (tstr < TermInfoStrings.BackTab || tstr >= TermInfoStrings.Last || tstr > (TermInfoStrings)this.strOffsets)
			{
				return null;
			}
			int num = this.booleansOffset + (int)this.boolSize;
			if (num % 2 == 1)
			{
				num++;
			}
			num += (int)(this.numSize * 2);
			int @int = (int)this.GetInt16(this.buffer, (int)(num + tstr * TermInfoStrings.CarriageReturn));
			if (@int == -1)
			{
				return null;
			}
			return this.GetStringBytes(this.buffer, num + (int)(this.strOffsets * 2) + @int);
		}

		private short GetInt16(byte[] buffer, int offset)
		{
			int num = (int)buffer[offset];
			int num2 = (int)buffer[offset + 1];
			if (num == 255 && num2 == 255)
			{
				return -1;
			}
			return (short)(num + num2 * 256);
		}

		private string GetString(byte[] buffer, int offset)
		{
			int num = 0;
			int num2 = offset;
			while (buffer[num2++] != 0)
			{
				num++;
			}
			return Encoding.ASCII.GetString(buffer, offset, num);
		}

		private byte[] GetStringBytes(byte[] buffer, int offset)
		{
			int num = 0;
			int num2 = offset;
			while (buffer[num2++] != 0)
			{
				num++;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopyInternal(buffer, offset, array, 0, num);
			return array;
		}

		internal static string Escape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in s)
			{
				if (char.IsControl(c))
				{
					stringBuilder.AppendFormat("\\x{0:X2}", (int)c);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private short boolSize;

		private short numSize;

		private short strOffsets;

		private byte[] buffer;

		private int booleansOffset;
	}
}
