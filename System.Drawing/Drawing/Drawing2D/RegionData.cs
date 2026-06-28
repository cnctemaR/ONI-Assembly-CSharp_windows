using System;

namespace System.Drawing.Drawing2D
{
	public sealed class RegionData
	{
		internal RegionData()
		{
		}

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

		private byte[] data;
	}
}
