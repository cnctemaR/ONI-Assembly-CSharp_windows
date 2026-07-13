using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/BlobObject/BlobObject.h")]
	internal struct FixedBlobObjectReference
	{
		public void RemoveFromList()
		{
			this.blobData = 0UL;
			this.blobSize = 0U;
			bool flag = this.prevReference > 0UL;
			if (flag)
			{
				((UIntPtr)this.prevReference).nextReference = this.nextReference;
			}
			bool flag2 = this.nextReference > 0UL;
			if (flag2)
			{
				((UIntPtr)this.nextReference).prevReference = this.prevReference;
			}
			this.prevReference = (this.nextReference = 0UL);
		}

		public ulong blobTypeHash;

		public ulong blobData;

		public uint blobSize;

		public ulong prevReference;

		public ulong nextReference;
	}
}
