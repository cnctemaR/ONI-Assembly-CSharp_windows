using System;

namespace Iced.Intel.BlockEncoderInternal
{
	internal sealed class BlockData
	{
		public ulong Address
		{
			get
			{
				if (!this.IsValid)
				{
					ThrowHelper.ThrowInvalidOperationException();
				}
				if (!this.__dont_use_address_initd)
				{
					ThrowHelper.ThrowInvalidOperationException();
				}
				return this.__dont_use_address;
			}
		}

		internal ulong __dont_use_address;

		internal bool __dont_use_address_initd;

		public bool IsValid;

		public ulong Data;
	}
}
