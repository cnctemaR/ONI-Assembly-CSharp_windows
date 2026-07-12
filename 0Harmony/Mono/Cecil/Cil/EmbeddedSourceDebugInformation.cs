using System;
using Mono.Cecil.Metadata;

namespace Mono.Cecil.Cil
{
	internal sealed class EmbeddedSourceDebugInformation : CustomDebugInformation
	{
		public byte[] Content
		{
			get
			{
				if (!this.resolved)
				{
					this.Resolve();
				}
				return this.content;
			}
			set
			{
				this.content = value;
				this.resolved = true;
			}
		}

		public bool Compress
		{
			get
			{
				if (!this.resolved)
				{
					this.Resolve();
				}
				return this.compress;
			}
			set
			{
				this.compress = value;
				this.resolved = true;
			}
		}

		public override CustomDebugInformationKind Kind
		{
			get
			{
				return CustomDebugInformationKind.EmbeddedSource;
			}
		}

		internal EmbeddedSourceDebugInformation(uint index, MetadataReader debug_reader)
			: base(EmbeddedSourceDebugInformation.KindIdentifier)
		{
			this.index = index;
			this.debug_reader = debug_reader;
		}

		public EmbeddedSourceDebugInformation(byte[] content, bool compress)
			: base(EmbeddedSourceDebugInformation.KindIdentifier)
		{
			this.resolved = true;
			this.content = content;
			this.compress = compress;
		}

		internal byte[] ReadRawEmbeddedSourceDebugInformation()
		{
			if (this.debug_reader == null)
			{
				throw new InvalidOperationException();
			}
			return this.debug_reader.ReadRawEmbeddedSourceDebugInformation(this.index);
		}

		private void Resolve()
		{
			if (this.resolved)
			{
				return;
			}
			if (this.debug_reader == null)
			{
				throw new InvalidOperationException();
			}
			Row<byte[], bool> row = this.debug_reader.ReadEmbeddedSourceDebugInformation(this.index);
			this.content = row.Col1;
			this.compress = row.Col2;
			this.resolved = true;
		}

		internal uint index;

		internal MetadataReader debug_reader;

		internal bool resolved;

		internal byte[] content;

		internal bool compress;

		public static Guid KindIdentifier = new Guid("{0E8A571B-6926-466E-B4AD-8AB04611F5FE}");
	}
}
