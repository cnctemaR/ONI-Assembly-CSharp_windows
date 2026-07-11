using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.CounterCreationDataConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Serializable]
	public class CounterCreationData
	{
		public CounterCreationData()
		{
		}

		public CounterCreationData(string counterName, string counterHelp, PerformanceCounterType counterType)
		{
			this.CounterName = counterName;
			this.CounterHelp = counterHelp;
			this.CounterType = counterType;
		}

		[MonitoringDescription("Description of this counter.")]
		[global::System.ComponentModel.DefaultValue("")]
		public string CounterHelp
		{
			get
			{
				return this.help;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.help = value;
			}
		}

		[MonitoringDescription("Name of this counter.")]
		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string CounterName
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == string.Empty)
				{
					throw new ArgumentException("value");
				}
				this.name = value;
			}
		}

		[MonitoringDescription("Type of this counter.")]
		[global::System.ComponentModel.DefaultValue(typeof(PerformanceCounterType), "NumberOfItems32")]
		public PerformanceCounterType CounterType
		{
			get
			{
				return this.type;
			}
			set
			{
				if (!Enum.IsDefined(typeof(PerformanceCounterType), value))
				{
					throw new global::System.ComponentModel.InvalidEnumArgumentException();
				}
				this.type = value;
			}
		}

		private string help = string.Empty;

		private string name;

		private PerformanceCounterType type;
	}
}
