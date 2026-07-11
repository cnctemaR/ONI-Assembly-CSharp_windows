using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct StringConcat
	{
		public void Clear()
		{
			this.idxStr = 0;
			this.delimiter = null;
		}

		public string Delimiter
		{
			get
			{
				return this.delimiter;
			}
			set
			{
				this.delimiter = value;
			}
		}

		internal int Count
		{
			get
			{
				return this.idxStr;
			}
		}

		public void Concat(string value)
		{
			if (this.delimiter != null && this.idxStr != 0)
			{
				this.ConcatNoDelimiter(this.delimiter);
			}
			this.ConcatNoDelimiter(value);
		}

		public string GetResult()
		{
			switch (this.idxStr)
			{
			case 0:
				return string.Empty;
			case 1:
				return this.s1;
			case 2:
				return this.s1 + this.s2;
			case 3:
				return this.s1 + this.s2 + this.s3;
			case 4:
				return this.s1 + this.s2 + this.s3 + this.s4;
			default:
				return string.Concat(this.strList.ToArray());
			}
		}

		internal void ConcatNoDelimiter(string s)
		{
			switch (this.idxStr)
			{
			case 0:
				this.s1 = s;
				goto IL_00A8;
			case 1:
				this.s2 = s;
				goto IL_00A8;
			case 2:
				this.s3 = s;
				goto IL_00A8;
			case 3:
				this.s4 = s;
				goto IL_00A8;
			case 4:
			{
				int num = ((this.strList == null) ? 8 : this.strList.Count);
				List<string> list = (this.strList = new List<string>(num));
				list.Add(this.s1);
				list.Add(this.s2);
				list.Add(this.s3);
				list.Add(this.s4);
				break;
			}
			}
			this.strList.Add(s);
			IL_00A8:
			this.idxStr++;
		}

		private string s1;

		private string s2;

		private string s3;

		private string s4;

		private string delimiter;

		private List<string> strList;

		private int idxStr;
	}
}
