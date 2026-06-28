using System;

namespace System.Xml
{
	public sealed class XmlDictionaryReaderQuotas
	{
		public XmlDictionaryReaderQuotas()
			: this(false)
		{
		}

		private XmlDictionaryReaderQuotas(bool max)
		{
			this.is_readonly = max;
			this.array_len = ((!max) ? 16384 : int.MaxValue);
			this.bytes = ((!max) ? 4096 : int.MaxValue);
			this.depth = ((!max) ? 32 : int.MaxValue);
			this.nt_chars = ((!max) ? 16384 : int.MaxValue);
			this.text_len = ((!max) ? 8192 : int.MaxValue);
		}

		public static XmlDictionaryReaderQuotas Max
		{
			get
			{
				return XmlDictionaryReaderQuotas.max;
			}
		}

		public int MaxArrayLength
		{
			get
			{
				return this.array_len;
			}
			set
			{
				this.array_len = this.Check(value);
			}
		}

		public int MaxBytesPerRead
		{
			get
			{
				return this.bytes;
			}
			set
			{
				this.bytes = this.Check(value);
			}
		}

		public int MaxDepth
		{
			get
			{
				return this.depth;
			}
			set
			{
				this.depth = this.Check(value);
			}
		}

		public int MaxNameTableCharCount
		{
			get
			{
				return this.nt_chars;
			}
			set
			{
				this.nt_chars = this.Check(value);
			}
		}

		public int MaxStringContentLength
		{
			get
			{
				return this.text_len;
			}
			set
			{
				this.text_len = this.Check(value);
			}
		}

		private int Check(int value)
		{
			if (this.is_readonly)
			{
				throw new InvalidOperationException("This quota is read-only.");
			}
			if (value <= 0)
			{
				throw new ArgumentException("Value must be positive integer.");
			}
			return value;
		}

		public void CopyTo(XmlDictionaryReaderQuotas quota)
		{
			quota.array_len = this.array_len;
			quota.bytes = this.bytes;
			quota.depth = this.depth;
			quota.nt_chars = this.nt_chars;
			quota.text_len = this.text_len;
		}

		private static XmlDictionaryReaderQuotas max = new XmlDictionaryReaderQuotas(true);

		private readonly bool is_readonly;

		private int array_len;

		private int bytes;

		private int depth;

		private int nt_chars;

		private int text_len;
	}
}
