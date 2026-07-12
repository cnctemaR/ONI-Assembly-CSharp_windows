using System;

namespace Mono.Cecil.Cil
{
	internal sealed class EmbeddedSourceDebugInformation : CustomDebugInformation
	{
		public byte[] Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		public bool Compress
		{
			get
			{
				return this.compress;
			}
			set
			{
				this.compress = value;
			}
		}

		public override CustomDebugInformationKind Kind
		{
			get
			{
				return CustomDebugInformationKind.EmbeddedSource;
			}
		}

		public EmbeddedSourceDebugInformation(byte[] content, bool compress)
			: base(EmbeddedSourceDebugInformation.KindIdentifier)
		{
			this.content = content;
			this.compress = compress;
		}

		internal byte[] content;

		internal bool compress;

		public static Guid KindIdentifier = new Guid("{0E8A571B-6926-466E-B4AD-8AB04611F5FE}");
	}
}
