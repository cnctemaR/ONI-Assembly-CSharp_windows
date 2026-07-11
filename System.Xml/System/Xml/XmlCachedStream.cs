using System;
using System.IO;

namespace System.Xml
{
	internal class XmlCachedStream : MemoryStream
	{
		internal XmlCachedStream(Uri uri, Stream stream)
		{
			this.uri = uri;
			try
			{
				byte[] array = new byte[4096];
				int num;
				while ((num = stream.Read(array, 0, 4096)) > 0)
				{
					this.Write(array, 0, num);
				}
				base.Position = 0L;
			}
			finally
			{
				stream.Close();
			}
		}

		private const int MoveBufferSize = 4096;

		private Uri uri;
	}
}
