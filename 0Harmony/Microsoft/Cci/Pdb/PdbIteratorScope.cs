using System;

namespace Microsoft.Cci.Pdb
{
	internal sealed class PdbIteratorScope : ILocalScope
	{
		internal PdbIteratorScope(uint offset, uint length)
		{
			this.offset = offset;
			this.length = length;
		}

		public uint Offset
		{
			get
			{
				return this.offset;
			}
		}

		public uint Length
		{
			get
			{
				return this.length;
			}
		}

		private uint offset;

		private uint length;
	}
}
