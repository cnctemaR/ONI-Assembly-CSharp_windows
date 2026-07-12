using System;

namespace Microsoft.Cci.Pdb
{
	internal class PdbSynchronizationPoint
	{
		internal PdbSynchronizationPoint(BitAccess bits)
		{
			bits.ReadUInt32(out this.synchronizeOffset);
			bits.ReadUInt32(out this.continuationMethodToken);
			bits.ReadUInt32(out this.continuationOffset);
		}

		public uint SynchronizeOffset
		{
			get
			{
				return this.synchronizeOffset;
			}
		}

		public uint ContinuationOffset
		{
			get
			{
				return this.continuationOffset;
			}
		}

		internal uint synchronizeOffset;

		internal uint continuationMethodToken;

		internal uint continuationOffset;
	}
}
