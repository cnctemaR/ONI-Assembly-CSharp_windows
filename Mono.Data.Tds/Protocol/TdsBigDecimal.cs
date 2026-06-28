using System;

namespace Mono.Data.Tds.Protocol
{
	public class TdsBigDecimal
	{
		public TdsBigDecimal(byte precision, byte scale, bool isNegative, int[] data)
		{
			this.isNegative = isNegative;
			this.precision = precision;
			this.scale = scale;
			this.data = data;
		}

		public int[] Data
		{
			get
			{
				return this.data;
			}
		}

		public byte Precision
		{
			get
			{
				return this.precision;
			}
		}

		public byte Scale
		{
			get
			{
				return this.scale;
			}
		}

		public bool IsNegative
		{
			get
			{
				return this.isNegative;
			}
		}

		private bool isNegative;

		private byte precision;

		private byte scale;

		private int[] data;
	}
}
