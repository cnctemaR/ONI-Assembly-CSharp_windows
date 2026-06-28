using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using Ionic.Zlib;

namespace TiledSharp
{
	public class TmxBase64Data
	{
		public Stream Data { get; private set; }

		public TmxBase64Data(XElement xData)
		{
			if ((string)xData.Attribute("encoding") != "base64")
			{
				throw new Exception("TmxBase64Data: Only Base64-encoded data is supported.");
			}
			byte[] array = Convert.FromBase64String(xData.Value);
			this.Data = new MemoryStream(array, false);
			string text = (string)xData.Attribute("compression");
			if (text == "gzip")
			{
				this.Data = new global::System.IO.Compression.GZipStream(this.Data, global::System.IO.Compression.CompressionMode.Decompress, false);
			}
			else if (text == "zlib")
			{
				this.Data = new ZlibStream(this.Data, Ionic.Zlib.CompressionMode.Decompress, false);
			}
			else if (text != null)
			{
				throw new Exception("TmxBase64Data: Unknown compression.");
			}
		}
	}
}
