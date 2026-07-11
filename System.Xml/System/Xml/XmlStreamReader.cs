using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Xml
{
	internal class XmlStreamReader : NonBlockingStreamReader
	{
		private XmlStreamReader(XmlInputStream input)
			: base(input, (input.ActualEncoding == null) ? XmlInputStream.StrictUTF8 : input.ActualEncoding)
		{
			this.input = input;
		}

		public XmlStreamReader(Stream input)
			: this(new XmlInputStream(input))
		{
		}

		public override void Close()
		{
			this.input.Close();
		}

		public override int Read([In] [Out] char[] dest_buffer, int index, int count)
		{
			int num;
			try
			{
				num = base.Read(dest_buffer, index, count);
			}
			catch (ArgumentException)
			{
				throw XmlStreamReader.invalidDataException;
			}
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.Close();
			}
		}

		private XmlInputStream input;

		private static XmlException invalidDataException = new XmlException("invalid data.");
	}
}
