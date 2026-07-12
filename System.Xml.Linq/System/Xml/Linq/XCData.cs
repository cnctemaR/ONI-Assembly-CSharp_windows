using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	public class XCData : XText
	{
		public XCData(string value)
			: base(value)
		{
		}

		public XCData(XCData other)
			: base(other)
		{
		}

		internal XCData(XmlReader r)
			: base(r)
		{
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.CDATA;
			}
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteCData(this.text);
		}

		public override Task WriteToAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			return writer.WriteCDataAsync(this.text);
		}

		internal override XNode CloneNode()
		{
			return new XCData(this);
		}
	}
}
