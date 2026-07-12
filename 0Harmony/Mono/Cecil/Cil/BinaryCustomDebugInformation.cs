using System;

namespace Mono.Cecil.Cil
{
	internal sealed class BinaryCustomDebugInformation : CustomDebugInformation
	{
		public byte[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public override CustomDebugInformationKind Kind
		{
			get
			{
				return CustomDebugInformationKind.Binary;
			}
		}

		public BinaryCustomDebugInformation(Guid identifier, byte[] data)
			: base(identifier)
		{
			this.data = data;
		}

		private byte[] data;
	}
}
