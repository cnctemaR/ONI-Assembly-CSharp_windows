using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal class MimeWriter
	{
		internal MimeWriter(Stream stream, string boundary)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			if (boundary == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("boundary");
			}
			this.stream = stream;
			this.boundaryBytes = MimeWriter.GetBoundaryBytes(boundary);
			this.state = MimeWriterState.Start;
			this.bufferedWrite = new BufferedWrite();
		}

		internal static int GetHeaderSize(string name, string value, int maxSizeInBytes)
		{
			if (name == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			int num = XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, 0, MimeGlobals.COLONSPACE.Length + MimeGlobals.CRLF.Length);
			num += XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, name.Length);
			return num + XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, value.Length);
		}

		internal static byte[] GetBoundaryBytes(string boundary)
		{
			byte[] array = new byte[boundary.Length + MimeGlobals.BoundaryPrefix.Length];
			for (int i = 0; i < MimeGlobals.BoundaryPrefix.Length; i++)
			{
				array[i] = MimeGlobals.BoundaryPrefix[i];
			}
			Encoding.ASCII.GetBytes(boundary, 0, boundary.Length, array, MimeGlobals.BoundaryPrefix.Length);
			return array;
		}

		internal MimeWriterState WriteState
		{
			get
			{
				return this.state;
			}
		}

		internal int GetBoundarySize()
		{
			return this.boundaryBytes.Length;
		}

		internal void StartPreface()
		{
			if (this.state != MimeWriterState.Start)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("MIME writer is at invalid state for starting preface.", new object[] { this.state.ToString() })));
			}
			this.state = MimeWriterState.StartPreface;
		}

		internal void StartPart()
		{
			MimeWriterState mimeWriterState = this.state;
			if (mimeWriterState == MimeWriterState.StartPart || mimeWriterState == MimeWriterState.Closed)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("MIME writer is at invalid state for starting a part.", new object[] { this.state.ToString() })));
			}
			this.state = MimeWriterState.StartPart;
			if (this.contentStream != null)
			{
				this.contentStream.Flush();
				this.contentStream = null;
			}
			this.bufferedWrite.Write(this.boundaryBytes);
			this.bufferedWrite.Write(MimeGlobals.CRLF);
		}

		internal void Close()
		{
			if (this.state == MimeWriterState.Closed)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("MIME writer is at invalid state for closing.", new object[] { this.state.ToString() })));
			}
			this.state = MimeWriterState.Closed;
			if (this.contentStream != null)
			{
				this.contentStream.Flush();
				this.contentStream = null;
			}
			this.bufferedWrite.Write(this.boundaryBytes);
			this.bufferedWrite.Write(MimeGlobals.DASHDASH);
			this.bufferedWrite.Write(MimeGlobals.CRLF);
			this.Flush();
		}

		private void Flush()
		{
			if (this.bufferedWrite.Length > 0)
			{
				this.stream.Write(this.bufferedWrite.GetBuffer(), 0, this.bufferedWrite.Length);
				this.bufferedWrite.Reset();
			}
		}

		internal void WriteHeader(string name, string value)
		{
			if (name == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			MimeWriterState mimeWriterState = this.state;
			if (mimeWriterState == MimeWriterState.Start || mimeWriterState - MimeWriterState.Content <= 1)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("MIME writer is at invalid state for header.", new object[] { this.state.ToString() })));
			}
			this.state = MimeWriterState.Header;
			this.bufferedWrite.Write(name);
			this.bufferedWrite.Write(MimeGlobals.COLONSPACE);
			this.bufferedWrite.Write(value);
			this.bufferedWrite.Write(MimeGlobals.CRLF);
		}

		internal Stream GetContentStream()
		{
			MimeWriterState mimeWriterState = this.state;
			if (mimeWriterState == MimeWriterState.Start || mimeWriterState - MimeWriterState.Content <= 1)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("MIME writer is at invalid state for content.", new object[] { this.state.ToString() })));
			}
			this.state = MimeWriterState.Content;
			this.bufferedWrite.Write(MimeGlobals.CRLF);
			this.Flush();
			this.contentStream = this.stream;
			return this.contentStream;
		}

		private Stream stream;

		private byte[] boundaryBytes;

		private MimeWriterState state;

		private BufferedWrite bufferedWrite;

		private Stream contentStream;
	}
}
