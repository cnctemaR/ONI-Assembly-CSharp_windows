using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Net
{
	internal class MonoChunkStream
	{
		public MonoChunkStream(byte[] buffer, int offset, int size, WebHeaderCollection headers)
			: this(headers)
		{
			this.Write(buffer, offset, size);
		}

		public MonoChunkStream(WebHeaderCollection headers)
		{
			this.headers = headers;
			this.saved = new StringBuilder();
			this.chunks = new ArrayList();
			this.chunkSize = -1;
			this.totalWritten = 0;
		}

		public void ResetBuffer()
		{
			this.chunkSize = -1;
			this.chunkRead = 0;
			this.totalWritten = 0;
			this.chunks.Clear();
		}

		public void WriteAndReadBack(byte[] buffer, int offset, int size, ref int read)
		{
			if (offset + read > 0)
			{
				this.Write(buffer, offset, offset + read);
			}
			read = this.Read(buffer, offset, size);
		}

		public int Read(byte[] buffer, int offset, int size)
		{
			return this.ReadFromChunks(buffer, offset, size);
		}

		private int ReadFromChunks(byte[] buffer, int offset, int size)
		{
			int count = this.chunks.Count;
			int num = 0;
			List<MonoChunkStream.Chunk> list = new List<MonoChunkStream.Chunk>(count);
			for (int i = 0; i < count; i++)
			{
				MonoChunkStream.Chunk chunk = (MonoChunkStream.Chunk)this.chunks[i];
				if (chunk.Offset == chunk.Bytes.Length)
				{
					list.Add(chunk);
				}
				else
				{
					num += chunk.Read(buffer, offset + num, size - num);
					if (num == size)
					{
						break;
					}
				}
			}
			foreach (MonoChunkStream.Chunk chunk2 in list)
			{
				this.chunks.Remove(chunk2);
			}
			return num;
		}

		public void Write(byte[] buffer, int offset, int size)
		{
			if (offset < size)
			{
				this.InternalWrite(buffer, ref offset, size);
			}
		}

		private void InternalWrite(byte[] buffer, ref int offset, int size)
		{
			if (this.state == MonoChunkStream.State.None || this.state == MonoChunkStream.State.PartialSize)
			{
				this.state = this.GetChunkSize(buffer, ref offset, size);
				if (this.state == MonoChunkStream.State.PartialSize)
				{
					return;
				}
				this.saved.Length = 0;
				this.sawCR = false;
				this.gotit = false;
			}
			if (this.state == MonoChunkStream.State.Body && offset < size)
			{
				this.state = this.ReadBody(buffer, ref offset, size);
				if (this.state == MonoChunkStream.State.Body)
				{
					return;
				}
			}
			if (this.state == MonoChunkStream.State.BodyFinished && offset < size)
			{
				this.state = this.ReadCRLF(buffer, ref offset, size);
				if (this.state == MonoChunkStream.State.BodyFinished)
				{
					return;
				}
				this.sawCR = false;
			}
			if (this.state == MonoChunkStream.State.Trailer && offset < size)
			{
				this.state = this.ReadTrailer(buffer, ref offset, size);
				if (this.state == MonoChunkStream.State.Trailer)
				{
					return;
				}
				this.saved.Length = 0;
				this.sawCR = false;
				this.gotit = false;
			}
			if (offset < size)
			{
				this.InternalWrite(buffer, ref offset, size);
			}
		}

		public bool WantMore
		{
			get
			{
				return this.chunkRead != this.chunkSize || this.chunkSize != 0 || this.state > MonoChunkStream.State.None;
			}
		}

		public bool DataAvailable
		{
			get
			{
				int count = this.chunks.Count;
				for (int i = 0; i < count; i++)
				{
					MonoChunkStream.Chunk chunk = (MonoChunkStream.Chunk)this.chunks[i];
					if (chunk != null && chunk.Bytes != null && chunk.Bytes.Length != 0 && chunk.Offset < chunk.Bytes.Length)
					{
						return this.state != MonoChunkStream.State.Body;
					}
				}
				return false;
			}
		}

		public int TotalDataSize
		{
			get
			{
				return this.totalWritten;
			}
		}

		public int ChunkLeft
		{
			get
			{
				return this.chunkSize - this.chunkRead;
			}
		}

		private MonoChunkStream.State ReadBody(byte[] buffer, ref int offset, int size)
		{
			if (this.chunkSize == 0)
			{
				return MonoChunkStream.State.BodyFinished;
			}
			int num = size - offset;
			if (num + this.chunkRead > this.chunkSize)
			{
				num = this.chunkSize - this.chunkRead;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(buffer, offset, array, 0, num);
			this.chunks.Add(new MonoChunkStream.Chunk(array));
			offset += num;
			this.chunkRead += num;
			this.totalWritten += num;
			if (this.chunkRead != this.chunkSize)
			{
				return MonoChunkStream.State.Body;
			}
			return MonoChunkStream.State.BodyFinished;
		}

		private MonoChunkStream.State GetChunkSize(byte[] buffer, ref int offset, int size)
		{
			this.chunkRead = 0;
			this.chunkSize = 0;
			char c = '\0';
			while (offset < size)
			{
				int num = offset;
				offset = num + 1;
				c = (char)buffer[num];
				if (c == '\r')
				{
					if (this.sawCR)
					{
						MonoChunkStream.ThrowProtocolViolation("2 CR found");
					}
					this.sawCR = true;
				}
				else
				{
					if (this.sawCR && c == '\n')
					{
						break;
					}
					if (c == ' ')
					{
						this.gotit = true;
					}
					if (!this.gotit)
					{
						this.saved.Append(c);
					}
					if (this.saved.Length > 20)
					{
						MonoChunkStream.ThrowProtocolViolation("chunk size too long.");
					}
				}
			}
			if (!this.sawCR || c != '\n')
			{
				if (offset < size)
				{
					MonoChunkStream.ThrowProtocolViolation("Missing \\n");
				}
				try
				{
					if (this.saved.Length > 0)
					{
						this.chunkSize = int.Parse(MonoChunkStream.RemoveChunkExtension(this.saved.ToString()), NumberStyles.HexNumber);
					}
				}
				catch (Exception)
				{
					MonoChunkStream.ThrowProtocolViolation("Cannot parse chunk size.");
				}
				return MonoChunkStream.State.PartialSize;
			}
			this.chunkRead = 0;
			try
			{
				this.chunkSize = int.Parse(MonoChunkStream.RemoveChunkExtension(this.saved.ToString()), NumberStyles.HexNumber);
			}
			catch (Exception)
			{
				MonoChunkStream.ThrowProtocolViolation("Cannot parse chunk size.");
			}
			if (this.chunkSize == 0)
			{
				this.trailerState = 2;
				return MonoChunkStream.State.Trailer;
			}
			return MonoChunkStream.State.Body;
		}

		private static string RemoveChunkExtension(string input)
		{
			int num = input.IndexOf(';');
			if (num == -1)
			{
				return input;
			}
			return input.Substring(0, num);
		}

		private MonoChunkStream.State ReadCRLF(byte[] buffer, ref int offset, int size)
		{
			if (!this.sawCR)
			{
				int num = offset;
				offset = num + 1;
				if (buffer[num] != 13)
				{
					MonoChunkStream.ThrowProtocolViolation("Expecting \\r");
				}
				this.sawCR = true;
				if (offset == size)
				{
					return MonoChunkStream.State.BodyFinished;
				}
			}
			if (this.sawCR)
			{
				int num = offset;
				offset = num + 1;
				if (buffer[num] != 10)
				{
					MonoChunkStream.ThrowProtocolViolation("Expecting \\n");
				}
			}
			return MonoChunkStream.State.None;
		}

		private MonoChunkStream.State ReadTrailer(byte[] buffer, ref int offset, int size)
		{
			if (this.trailerState == 2 && buffer[offset] == 13 && this.saved.Length == 0)
			{
				offset++;
				if (offset < size && buffer[offset] == 10)
				{
					offset++;
					return MonoChunkStream.State.None;
				}
				offset--;
			}
			int num = this.trailerState;
			string text = "\r\n\r";
			while (offset < size && num < 4)
			{
				int num2 = offset;
				offset = num2 + 1;
				char c = (char)buffer[num2];
				if ((num == 0 || num == 2) && c == '\r')
				{
					num++;
				}
				else if ((num == 1 || num == 3) && c == '\n')
				{
					num++;
				}
				else if (num > 0)
				{
					this.saved.Append(text.Substring(0, (this.saved.Length == 0) ? (num - 2) : num));
					num = 0;
					if (this.saved.Length > 4196)
					{
						MonoChunkStream.ThrowProtocolViolation("Error reading trailer (too long).");
					}
				}
			}
			if (num < 4)
			{
				this.trailerState = num;
				if (offset < size)
				{
					MonoChunkStream.ThrowProtocolViolation("Error reading trailer.");
				}
				return MonoChunkStream.State.Trailer;
			}
			StringReader stringReader = new StringReader(this.saved.ToString());
			string text2;
			while ((text2 = stringReader.ReadLine()) != null && text2 != "")
			{
				this.headers.Add(text2);
			}
			return MonoChunkStream.State.None;
		}

		private static void ThrowProtocolViolation(string message)
		{
			throw new WebException(message, null, WebExceptionStatus.ServerProtocolViolation, null);
		}

		internal WebHeaderCollection headers;

		private int chunkSize;

		private int chunkRead;

		private int totalWritten;

		private MonoChunkStream.State state;

		private StringBuilder saved;

		private bool sawCR;

		private bool gotit;

		private int trailerState;

		private ArrayList chunks;

		private enum State
		{
			None,
			PartialSize,
			Body,
			BodyFinished,
			Trailer
		}

		private class Chunk
		{
			public Chunk(byte[] chunk)
			{
				this.Bytes = chunk;
			}

			public int Read(byte[] buffer, int offset, int size)
			{
				int num = ((size > this.Bytes.Length - this.Offset) ? (this.Bytes.Length - this.Offset) : size);
				Buffer.BlockCopy(this.Bytes, this.Offset, buffer, offset, num);
				this.Offset += num;
				return num;
			}

			public byte[] Bytes;

			public int Offset;
		}
	}
}
