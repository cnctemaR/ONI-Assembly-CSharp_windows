using System;

namespace FileHelpers.Events
{
	public class ProgressEventArgs : EventArgs
	{
		public double Percent { get; private set; }

		public int CurrentRecord { get; private set; }

		public int TotalRecords { get; private set; }

		public long CurrentBytes { get; private set; }

		public long TotalBytes { get; private set; }

		public ProgressEventArgs(int currentRecord, int totalRecords)
			: this(currentRecord, totalRecords, -1L, -1L)
		{
		}

		public ProgressEventArgs(int currentRecord, int totalRecords, long currentBytes, long totalBytes)
		{
			this.CurrentRecord = currentRecord;
			this.TotalRecords = totalRecords;
			this.CurrentBytes = currentBytes;
			this.TotalBytes = totalBytes;
			if (totalRecords > 0)
			{
				this.Percent = (double)currentRecord / (double)totalRecords * 100.0;
				return;
			}
			if (totalBytes > 0L)
			{
				this.Percent = (double)currentBytes / (double)totalBytes * 100.0;
				return;
			}
			this.Percent = -1.0;
		}
	}
}
