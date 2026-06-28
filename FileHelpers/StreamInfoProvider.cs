using System;
using System.IO;

namespace FileHelpers
{
	internal sealed class StreamInfoProvider
	{
		public StreamInfoProvider(TextReader reader)
		{
			if (reader is StreamReader)
			{
				Stream stream2 = ((StreamReader)reader).BaseStream;
				if (stream2.CanSeek)
				{
					this.mLength = stream2.Length;
				}
				this.mPositionCalculator = () => stream2.Position;
				return;
			}
			if (reader is InternalStreamReader)
			{
				InternalStreamReader reader2 = (InternalStreamReader)reader;
				Stream baseStream = reader2.BaseStream;
				if (baseStream.CanSeek)
				{
					this.mLength = baseStream.Length;
				}
				this.mPositionCalculator = () => reader2.Position;
				return;
			}
			if (reader is InternalStringReader)
			{
				InternalStringReader stream = (InternalStringReader)reader;
				this.mLength = (long)stream.Length;
				this.mPositionCalculator = () => (long)stream.Position;
			}
		}

		public StreamInfoProvider(TextWriter writer)
		{
			StreamWriter streamWriter = writer as StreamWriter;
			if (streamWriter == null)
			{
				return;
			}
			Stream stream = streamWriter.BaseStream;
			if (stream.CanSeek)
			{
				this.mLength = stream.Length;
			}
			this.mPositionCalculator = () => stream.Position;
		}

		public long Position
		{
			get
			{
				return this.mPositionCalculator();
			}
		}

		public long TotalBytes
		{
			get
			{
				return this.mLength;
			}
		}

		private readonly StreamInfoProvider.GetValue mPositionCalculator = () => -1L;

		private readonly long mLength = -1L;

		private delegate long GetValue();
	}
}
