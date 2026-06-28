using System;

namespace System.Diagnostics
{
	public class InstanceData
	{
		public InstanceData(string instanceName, CounterSample sample)
		{
			this.instanceName = instanceName;
			this.sample = sample;
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
		}

		public long RawValue
		{
			get
			{
				return this.sample.RawValue;
			}
		}

		public CounterSample Sample
		{
			get
			{
				return this.sample;
			}
		}

		private string instanceName;

		private CounterSample sample;
	}
}
