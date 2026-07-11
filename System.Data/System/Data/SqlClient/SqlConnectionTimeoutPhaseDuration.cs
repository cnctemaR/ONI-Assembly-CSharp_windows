using System;
using System.Diagnostics;

namespace System.Data.SqlClient
{
	internal class SqlConnectionTimeoutPhaseDuration
	{
		internal void StartCapture()
		{
			this._swDuration.Start();
		}

		internal void StopCapture()
		{
			if (this._swDuration.IsRunning)
			{
				this._swDuration.Stop();
			}
		}

		internal long GetMilliSecondDuration()
		{
			return this._swDuration.ElapsedMilliseconds;
		}

		private Stopwatch _swDuration = new Stopwatch();
	}
}
