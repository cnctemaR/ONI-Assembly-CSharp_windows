using System;

namespace System.Xml.Xsl
{
	internal struct StringPair
	{
		public StringPair(string left, string right)
		{
			this.left = left;
			this.right = right;
		}

		public string Left
		{
			get
			{
				return this.left;
			}
		}

		public string Right
		{
			get
			{
				return this.right;
			}
		}

		private string left;

		private string right;
	}
}
