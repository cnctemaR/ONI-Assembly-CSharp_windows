using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Diagnostics.Tracing
{
	[EventData]
	internal class EventCounterPayload : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		public string Name { get; set; }

		public float Mean { get; set; }

		public float StandardDeviation { get; set; }

		public int Count { get; set; }

		public float Min { get; set; }

		public float Max { get; set; }

		public float IntervalSec { get; internal set; }

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this.ForEnumeration.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.ForEnumeration.GetEnumerator();
		}

		private IEnumerable<KeyValuePair<string, object>> ForEnumeration
		{
			get
			{
				yield return new KeyValuePair<string, object>("Name", this.Name);
				yield return new KeyValuePair<string, object>("Mean", this.Mean);
				yield return new KeyValuePair<string, object>("StandardDeviation", this.StandardDeviation);
				yield return new KeyValuePair<string, object>("Count", this.Count);
				yield return new KeyValuePair<string, object>("Min", this.Min);
				yield return new KeyValuePair<string, object>("Max", this.Max);
				yield break;
			}
		}
	}
}
