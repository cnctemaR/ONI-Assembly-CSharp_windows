using System;
using System.IO;
using System.Reflection;
using Mono.Cecil.Cil;

namespace Mono.Cecil
{
	internal sealed class WriterParameters
	{
		public uint? Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				this.timestamp = value;
			}
		}

		public Stream SymbolStream
		{
			get
			{
				return this.symbol_stream;
			}
			set
			{
				this.symbol_stream = value;
			}
		}

		public ISymbolWriterProvider SymbolWriterProvider
		{
			get
			{
				return this.symbol_writer_provider;
			}
			set
			{
				this.symbol_writer_provider = value;
			}
		}

		public bool WriteSymbols
		{
			get
			{
				return this.write_symbols;
			}
			set
			{
				this.write_symbols = value;
			}
		}

		public bool HasStrongNameKey
		{
			get
			{
				return this.key_pair != null || this.key_blob != null || this.key_container != null;
			}
		}

		public byte[] StrongNameKeyBlob
		{
			get
			{
				return this.key_blob;
			}
			set
			{
				this.key_blob = value;
			}
		}

		public string StrongNameKeyContainer
		{
			get
			{
				return this.key_container;
			}
			set
			{
				this.key_container = value;
			}
		}

		public StrongNameKeyPair StrongNameKeyPair
		{
			get
			{
				return this.key_pair;
			}
			set
			{
				this.key_pair = value;
			}
		}

		public bool DeterministicMvid { get; set; }

		private uint? timestamp;

		private Stream symbol_stream;

		private ISymbolWriterProvider symbol_writer_provider;

		private bool write_symbols;

		private byte[] key_blob;

		private string key_container;

		private StrongNameKeyPair key_pair;
	}
}
