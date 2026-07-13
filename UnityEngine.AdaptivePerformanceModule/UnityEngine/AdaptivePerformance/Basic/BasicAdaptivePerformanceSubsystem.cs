using System;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance.Basic
{
	internal class BasicAdaptivePerformanceSubsystem : AdaptivePerformanceSubsystem
	{
		internal class BasicProvider : AdaptivePerformanceSubsystem.APProvider, IApplicationLifecycle, IDevicePerformanceLevelControl
		{
			public BasicProvider()
			{
				this.Capabilities = Feature.None;
				this.m_UpdatedPerfRecord.PerformanceLevelControlAvailable = false;
				this.m_UpdatedPerfRecord.CpuPerformanceBoost = false;
				this.m_UpdatedPerfRecord.GpuPerformanceBoost = false;
				this.m_UpdatedPerfRecord.TemperatureLevel = -1f;
				this.m_UpdatedPerfRecord.TemperatureTrend = -1f;
			}

			public override IApplicationLifecycle ApplicationLifecycle
			{
				get
				{
					return this;
				}
			}

			public override IDevicePerformanceLevelControl PerformanceLevelControl
			{
				get
				{
					return this;
				}
			}

			public override string Stats
			{
				get
				{
					return "Basic provider";
				}
			}

			public override bool Initialized { get; set; }

			public override Feature Capabilities { get; set; }

			protected internal override bool TryInitialize()
			{
				this.Initialized = true;
				return this.Initialized;
			}

			public override void Start()
			{
				this.m_Running = true;
			}

			public override void Stop()
			{
				this.m_Running = false;
			}

			public override void Destroy()
			{
				this.Initialized = false;
			}

			public override PerformanceDataRecord Update()
			{
				this.m_UpdatedPerfRecord.ChangeFlags = this.m_UpdatedPerfRecord.ChangeFlags & this.Capabilities;
				PerformanceDataRecord updatedPerfRecord = this.m_UpdatedPerfRecord;
				this.m_UpdatedPerfRecord.ChangeFlags = Feature.None;
				return updatedPerfRecord;
			}

			public void ApplicationPause()
			{
			}

			public void ApplicationResume()
			{
			}

			public bool SetPerformanceLevel(ref int cpuLevel, ref int gpuLevel)
			{
				bool flag = !this.m_UpdatedPerfRecord.PerformanceLevelControlAvailable;
				bool flag2;
				if (flag)
				{
					this.m_UpdatedPerfRecord.CpuPerformanceLevel = -1;
					this.m_UpdatedPerfRecord.ChangeFlags = this.m_UpdatedPerfRecord.ChangeFlags | Feature.CpuPerformanceLevel;
					this.m_UpdatedPerfRecord.GpuPerformanceLevel = -1;
					this.m_UpdatedPerfRecord.ChangeFlags = this.m_UpdatedPerfRecord.ChangeFlags | Feature.GpuPerformanceLevel;
					flag2 = false;
				}
				else
				{
					flag2 = cpuLevel >= 0 && gpuLevel >= 0 && cpuLevel <= this.MaxCpuPerformanceLevel && gpuLevel <= this.MaxGpuPerformanceLevel;
				}
				return flag2;
			}

			public bool EnableCpuBoost()
			{
				return false;
			}

			public bool EnableGpuBoost()
			{
				return false;
			}

			public override Version Version
			{
				get
				{
					return new Version(6, 0, 0);
				}
			}

			public int MaxCpuPerformanceLevel
			{
				get
				{
					return -1;
				}
			}

			public int MaxGpuPerformanceLevel
			{
				get
				{
					return -1;
				}
			}

			private PerformanceDataRecord m_UpdatedPerfRecord;
		}
	}
}
