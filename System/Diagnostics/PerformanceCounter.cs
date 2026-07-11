using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics
{
	[InstallerType(typeof(PerformanceCounterInstaller))]
	public sealed class PerformanceCounter : Component, ISupportInitialize
	{
		public PerformanceCounter()
		{
			this.categoryName = (this.counterName = (this.instanceName = ""));
			this.machineName = ".";
		}

		public PerformanceCounter(string categoryName, string counterName)
			: this(categoryName, counterName, false)
		{
		}

		public PerformanceCounter(string categoryName, string counterName, bool readOnly)
			: this(categoryName, counterName, "", readOnly)
		{
		}

		public PerformanceCounter(string categoryName, string counterName, string instanceName)
			: this(categoryName, counterName, instanceName, false)
		{
		}

		public PerformanceCounter(string categoryName, string counterName, string instanceName, bool readOnly)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
			}
			if (instanceName == null)
			{
				throw new ArgumentNullException("instanceName");
			}
			this.CategoryName = categoryName;
			this.CounterName = counterName;
			if (categoryName == "" || counterName == "")
			{
				throw new InvalidOperationException();
			}
			this.InstanceName = instanceName;
			this.instanceName = instanceName;
			this.machineName = ".";
			this.readOnly = readOnly;
			this.changed = true;
		}

		public PerformanceCounter(string categoryName, string counterName, string instanceName, string machineName)
			: this(categoryName, counterName, instanceName, false)
		{
			this.machineName = machineName;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetImpl(string category, string counter, string instance, string machine, out PerformanceCounterType ctype, out bool custom);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetSample(IntPtr impl, bool only_value, out CounterSample sample);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long UpdateValue(IntPtr impl, bool do_incr, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeData(IntPtr impl);

		private void UpdateInfo()
		{
			if (this.impl != IntPtr.Zero)
			{
				this.Close();
			}
			this.impl = PerformanceCounter.GetImpl(this.categoryName, this.counterName, this.instanceName, this.machineName, out this.type, out this.is_custom);
			if (!this.is_custom)
			{
				this.readOnly = true;
			}
			this.changed = false;
		}

		[DefaultValue("")]
		[ReadOnly(true)]
		[SRDescription("The category name for this performance counter.")]
		[SettingsBindable(true)]
		[TypeConverter("System.Diagnostics.Design.CategoryValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("categoryName");
				}
				this.categoryName = value;
				this.changed = true;
			}
		}

		[MonitoringDescription("A description describing the counter.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ReadOnly(true)]
		[MonoTODO]
		public string CounterHelp
		{
			get
			{
				return "";
			}
		}

		[SettingsBindable(true)]
		[ReadOnly(true)]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.CounterNameConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SRDescription("The name of this performance counter.")]
		public string CounterName
		{
			get
			{
				return this.counterName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("counterName");
				}
				this.counterName = value;
				this.changed = true;
			}
		}

		[MonitoringDescription("The type of the counter.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PerformanceCounterType CounterType
		{
			get
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				return this.type;
			}
		}

		[MonoTODO]
		[DefaultValue(PerformanceCounterInstanceLifetime.Global)]
		public PerformanceCounterInstanceLifetime InstanceLifetime
		{
			get
			{
				return this.lifetime;
			}
			set
			{
				this.lifetime = value;
			}
		}

		[SRDescription("The instance name for this performance counter.")]
		[DefaultValue("")]
		[ReadOnly(true)]
		[SettingsBindable(true)]
		[TypeConverter("System.Diagnostics.Design.InstanceNameConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.instanceName = value;
				this.changed = true;
			}
		}

		[SRDescription("The machine where this performance counter resides.")]
		[SettingsBindable(true)]
		[Browsable(false)]
		[DefaultValue(".")]
		[MonoTODO("What's the machine name format?")]
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == "" || value == ".")
				{
					this.machineName = ".";
					this.changed = true;
					return;
				}
				throw new PlatformNotSupportedException();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The raw value of the counter.")]
		public long RawValue
		{
			get
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				CounterSample counterSample;
				PerformanceCounter.GetSample(this.impl, true, out counterSample);
				return counterSample.RawValue;
			}
			set
			{
				if (this.changed)
				{
					this.UpdateInfo();
				}
				if (this.readOnly)
				{
					throw new InvalidOperationException();
				}
				PerformanceCounter.UpdateValue(this.impl, false, value);
			}
		}

		[MonitoringDescription("The accessability level of the counter.")]
		[DefaultValue(true)]
		[Browsable(false)]
		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public void BeginInit()
		{
		}

		public void EndInit()
		{
		}

		public void Close()
		{
			IntPtr intPtr = this.impl;
			this.impl = IntPtr.Zero;
			if (intPtr != IntPtr.Zero)
			{
				PerformanceCounter.FreeData(intPtr);
			}
		}

		public static void CloseSharedResources()
		{
		}

		public long Decrement()
		{
			return this.IncrementBy(-1L);
		}

		protected override void Dispose(bool disposing)
		{
			this.Close();
		}

		public long Increment()
		{
			return this.IncrementBy(1L);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public long IncrementBy(long value)
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			if (this.readOnly)
			{
				return 0L;
			}
			return PerformanceCounter.UpdateValue(this.impl, true, value);
		}

		public CounterSample NextSample()
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			CounterSample counterSample;
			PerformanceCounter.GetSample(this.impl, false, out counterSample);
			this.valid_old = true;
			this.old_sample = counterSample;
			return counterSample;
		}

		public float NextValue()
		{
			if (this.changed)
			{
				this.UpdateInfo();
			}
			CounterSample counterSample;
			PerformanceCounter.GetSample(this.impl, false, out counterSample);
			float num;
			if (this.valid_old)
			{
				num = CounterSampleCalculator.ComputeCounterValue(this.old_sample, counterSample);
			}
			else
			{
				num = CounterSampleCalculator.ComputeCounterValue(counterSample);
			}
			this.valid_old = true;
			this.old_sample = counterSample;
			return num;
		}

		[MonoTODO]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void RemoveInstance()
		{
			throw new NotImplementedException();
		}

		private string categoryName;

		private string counterName;

		private string instanceName;

		private string machineName;

		private IntPtr impl;

		private PerformanceCounterType type;

		private CounterSample old_sample;

		private bool readOnly;

		private bool valid_old;

		private bool changed;

		private bool is_custom;

		private PerformanceCounterInstanceLifetime lifetime;

		[Obsolete]
		public static int DefaultFileMappingSize = 524288;
	}
}
