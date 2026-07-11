using System;

namespace System.Runtime
{
	internal abstract class ScheduleActionItemAsyncResult : AsyncResult
	{
		protected ScheduleActionItemAsyncResult(AsyncCallback callback, object state)
			: base(callback, state)
		{
		}

		protected void Schedule()
		{
			ActionItem.Schedule(ScheduleActionItemAsyncResult.doWork, this);
		}

		private static void DoWork(object state)
		{
			ScheduleActionItemAsyncResult scheduleActionItemAsyncResult = (ScheduleActionItemAsyncResult)state;
			Exception ex = null;
			try
			{
				scheduleActionItemAsyncResult.OnDoWork();
			}
			catch (Exception ex2)
			{
				if (Fx.IsFatal(ex2))
				{
					throw;
				}
				ex = ex2;
			}
			scheduleActionItemAsyncResult.Complete(false, ex);
		}

		protected abstract void OnDoWork();

		public static void End(IAsyncResult result)
		{
			AsyncResult.End<ScheduleActionItemAsyncResult>(result);
		}

		private static Action<object> doWork = new Action<object>(ScheduleActionItemAsyncResult.DoWork);
	}
}
