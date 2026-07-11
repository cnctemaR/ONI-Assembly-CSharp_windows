using System;
using System.Globalization;

namespace System.Data.Common
{
	internal static class ActivityCorrelator
	{
		internal static ActivityCorrelator.ActivityId Current
		{
			get
			{
				if (ActivityCorrelator.t_tlsActivity == null)
				{
					ActivityCorrelator.t_tlsActivity = new ActivityCorrelator.ActivityId();
				}
				return new ActivityCorrelator.ActivityId(ActivityCorrelator.t_tlsActivity);
			}
		}

		internal static ActivityCorrelator.ActivityId Next()
		{
			if (ActivityCorrelator.t_tlsActivity == null)
			{
				ActivityCorrelator.t_tlsActivity = new ActivityCorrelator.ActivityId();
			}
			ActivityCorrelator.t_tlsActivity.Increment();
			return new ActivityCorrelator.ActivityId(ActivityCorrelator.t_tlsActivity);
		}

		[ThreadStatic]
		private static ActivityCorrelator.ActivityId t_tlsActivity;

		internal class ActivityId
		{
			internal Guid Id { get; private set; }

			internal uint Sequence { get; private set; }

			internal ActivityId()
			{
				this.Id = Guid.NewGuid();
				this.Sequence = 0U;
			}

			internal ActivityId(ActivityCorrelator.ActivityId activity)
			{
				this.Id = activity.Id;
				this.Sequence = activity.Sequence;
			}

			internal void Increment()
			{
				uint num = this.Sequence + 1U;
				this.Sequence = num;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Id, this.Sequence);
			}
		}
	}
}
