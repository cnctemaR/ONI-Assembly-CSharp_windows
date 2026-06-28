using System;

namespace FileHelpers.Detection
{
	internal sealed class DelimiterInfo
	{
		public int Max { get; set; }

		public int Min { get; set; }

		public int Average { get; set; }

		public char Delimiter { get; set; }

		public double Deviation { get; set; }

		public DelimiterInfo(char delimiter, double average, int max, int min, double deviation)
		{
			this.Max = max;
			this.Min = min;
			this.Delimiter = delimiter;
			this.Average = (int)Math.Round(average);
			this.Deviation = deviation;
		}
	}
}
