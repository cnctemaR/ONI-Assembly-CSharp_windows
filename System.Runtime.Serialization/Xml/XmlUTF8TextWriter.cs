using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal class XmlUTF8TextWriter : XmlBaseWriter, IXmlTextWriterInitializer
	{
		internal override bool FastAsync
		{
			get
			{
				return true;
			}
		}

		public void SetOutput(Stream stream, Encoding encoding, bool ownsStream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			if (encoding.WebName != Encoding.UTF8.WebName)
			{
				stream = new EncodingStreamWrapper(stream, encoding, true);
			}
			if (this.writer == null)
			{
				this.writer = new XmlUTF8NodeWriter();
			}
			this.writer.SetOutput(stream, ownsStream, encoding);
			base.SetOutput(this.writer);
		}

		public override bool CanFragment
		{
			get
			{
				return this.writer.Encoding == null;
			}
		}

		protected override XmlSigningNodeWriter CreateSigningNodeWriter()
		{
			return new XmlSigningNodeWriter(true);
		}

		private XmlUTF8NodeWriter writer;
	}
}
