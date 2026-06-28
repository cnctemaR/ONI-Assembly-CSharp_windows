using System;

namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberExpandable : XmlTypeMapMemberElement
	{
		public int FlatArrayIndex
		{
			get
			{
				return this._flatArrayIndex;
			}
			set
			{
				this._flatArrayIndex = value;
			}
		}

		private int _flatArrayIndex;
	}
}
