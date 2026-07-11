using System;

namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberFlatList : XmlTypeMapMemberExpandable
	{
		public ListMap ListMap
		{
			get
			{
				return this._listMap;
			}
			set
			{
				this._listMap = value;
			}
		}

		private ListMap _listMap;
	}
}
