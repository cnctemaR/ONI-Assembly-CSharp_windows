using System;
using Unity;

namespace System.Drawing.Drawing2D
{
	public sealed class RegionData
	{
		internal RegionData(byte[] data)
		{
			this.Data = data;
		}

		public byte[] Data { get; set; }

		internal RegionData()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
