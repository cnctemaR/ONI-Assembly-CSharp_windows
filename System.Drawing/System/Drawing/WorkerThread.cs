using System;
using System.Threading;

namespace System.Drawing
{
	internal class WorkerThread
	{
		public WorkerThread(EventHandler frmChgHandler, AnimateEventArgs aniEvtArgs, int[] delay)
		{
			this.frameChangeHandler = frmChgHandler;
			this.animateEventArgs = aniEvtArgs;
			this.delay = delay;
		}

		public void LoopHandler()
		{
			try
			{
				int num = 0;
				for (;;)
				{
					Thread.Sleep(this.delay[num++]);
					this.frameChangeHandler(null, this.animateEventArgs);
					if (num == this.delay.Length)
					{
						num = 0;
					}
				}
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
			}
		}

		private EventHandler frameChangeHandler;

		private AnimateEventArgs animateEventArgs;

		private int[] delay;
	}
}
