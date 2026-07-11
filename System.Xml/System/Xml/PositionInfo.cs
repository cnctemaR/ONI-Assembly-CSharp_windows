using System;

namespace System.Xml
{
	internal class PositionInfo : IXmlLineInfo
	{
		public virtual bool HasLineInfo()
		{
			return false;
		}

		public virtual int LineNumber
		{
			get
			{
				return 0;
			}
		}

		public virtual int LinePosition
		{
			get
			{
				return 0;
			}
		}

		public static PositionInfo GetPositionInfo(object o)
		{
			IXmlLineInfo xmlLineInfo = o as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				return new ReaderPositionInfo(xmlLineInfo);
			}
			return new PositionInfo();
		}
	}
}
