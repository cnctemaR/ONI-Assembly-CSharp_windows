using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics
{
	[global::System.ComponentModel.InstallerType(typeof(PerformanceCounterInstaller))]
	public sealed class PerformanceCounter : global::System.ComponentModel.Component, global::System.ComponentModel.ISupportInitialize
	{
		public PerformanceCounter()
		{
			this.categoryName = (this.counterName = (this.instanceName = string.Empty));
			this.machineName = ".";
		}

		public PerformanceCounter(string categoryName, string counterName)
			: this(categoryName, counterName, false)
		{
		}

		public PerformanceCounter(string categoryName, string counterName, bool readOnly)
			: this(categoryName, counterName, string.Empty, readOnly)
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
			if (categoryName == string.Empty || counterName == string.Empty)
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

		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.CategoryValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.SRDescription("The category name for this performance counter.")]
		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.ReadOnly(true)]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
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

		[global::System.ComponentModel.ReadOnly(true)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("A description describing the counter.")]
		[global::System.MonoTODO]
		public string CounterHelp
		{
			get
			{
				return string.Empty;
			}
		}

		[global::System.ComponentModel.ReadOnly(true)]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.CounterNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.SRDescription("The name of this performance counter.")]
		[global::System.ComponentModel.DefaultValue("")]
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

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The type of the counter.")]
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

		[global::System.ComponentModel.DefaultValue(PerformanceCounterInstanceLifetime.Global)]
		[global::System.MonoTODO]
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

		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.InstanceNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.SRDescription("The instance name for this performance counter.")]
		[global::System.ComponentModel.ReadOnly(true)]
		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
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

		[global::System.MonoTODO("What's the machine name format?")]
		[global::System.ComponentModel.DefaultValue(".")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.SRDescription("The machine where this performance counter resides.")]
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
				if (value == string.Empty || value == ".")
				{
					this.machineName = ".";
					this.changed = true;
					return;
				}
				throw new PlatformNotSupportedException();
			}
		}

		[MonitoringDescription("The raw value of the counter.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
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
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DefaultValue(true)]
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

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[global::System.MonoTODO]
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
