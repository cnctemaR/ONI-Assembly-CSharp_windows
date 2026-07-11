using System;

namespace System.Text
{
	[Serializable]
	public sealed class EncodingInfo
	{
		internal EncodingInfo(int cp)
		{
			this.codepage = cp;
		}

		public int CodePage
		{
			get
			{
				return this.codepage;
			}
		}

		[MonoTODO]
		public string DisplayName
		{
			get
			{
				return this.Name;
			}
		}

		public string Name
		{
			get
			{
				if (this.encoding == null)
				{
					this.encoding = this.GetEncoding();
				}
				return this.encoding.WebName;
			}
		}

		public override bool Equals(object value)
		{
			EncodingInfo encodingInfo = value as EncodingInfo;
			return encodingInfo != null && encodingInfo.codepage == this.codepage;
		}

		public override int GetHashCode()
		{
			return this.codepage;
		}

		public Encoding GetEncoding()
		{
			return Encoding.GetEncoding(this.codepage);
		}

		private readonly int codepage;

		private Encoding encoding;
	}
}
