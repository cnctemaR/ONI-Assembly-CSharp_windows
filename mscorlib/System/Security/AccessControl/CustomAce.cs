using System;

namespace System.Security.AccessControl
{
	public sealed class CustomAce : GenericAce
	{
		public CustomAce(AceType type, AceFlags flags, byte[] opaque)
			: base(type)
		{
			base.AceFlags = flags;
			this.SetOpaque(opaque);
		}

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int OpaqueLength
		{
			get
			{
				return this.opaque.Length;
			}
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public byte[] GetOpaque()
		{
			return (byte[])this.opaque.Clone();
		}

		public void SetOpaque(byte[] opaque)
		{
			if (opaque == null)
			{
				throw new ArgumentNullException("opaque");
			}
			this.opaque = (byte[])opaque.Clone();
		}

		private byte[] opaque;

		[MonoTODO]
		public static readonly int MaxOpaqueLength;
	}
}
