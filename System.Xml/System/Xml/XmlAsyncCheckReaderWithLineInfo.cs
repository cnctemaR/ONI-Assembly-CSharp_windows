using System;

namespace System.Xml
{
	internal class XmlAsyncCheckReaderWithLineInfo : XmlAsyncCheckReader, IXmlLineInfo
	{
		public XmlAsyncCheckReaderWithLineInfo(XmlReader reader)
			: base(reader)
		{
			this.readerAsIXmlLineInfo = (IXmlLineInfo)reader;
		}

		public virtual bool HasLineInfo()
		{
			return this.readerAsIXmlLineInfo.HasLineInfo();
		}

		public virtual int LineNumber
		{
			get
			{
				return this.readerAsIXmlLineInfo.LineNumber;
			}
		}

		public virtual int LinePosition
		{
			get
			{
				return this.readerAsIXmlLineInfo.LinePosition;
			}
		}

		private readonly IXmlLineInfo readerAsIXmlLineInfo;
	}
}
