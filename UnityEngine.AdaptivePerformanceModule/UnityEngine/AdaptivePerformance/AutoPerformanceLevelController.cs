using System;

namespace UnityEngine.AdaptivePerformance
{
	internal class AutoPerformanceLevelController
	{
		public float TargetFrameTime { get; set; }

		public float AllowedCpuActiveTimeRatio { get; set; }

		public float AllowedGpuActiveTimeRatio { get; set; }

		public float GpuLevelBounceAvoidanceThreshold { get; set; }

		public float CpuLevelBounceAvoidanceThreshold { get; set; }

		public float UpdateInterval { get; set; }

		public float MinTargetFrameRateHitTime { get; set; }

		public float MaxTemperatureLevel { get; set; }

		public AutoPerformanceLevelController(IDevicePerformanceControl perfControl, IPerformanceStatus perfStat, IThermalStatus thermalStat)
		{
			this.UpdateInterval = 5f;
			this.TargetFrameTime = -1f;
			this.AllowedCpuActiveTimeRatio = 0.8f;
			this.AllowedGpuActiveTimeRatio = 0.9f;
			this.GpuLevelBounceAvoidanceThreshold = 10f;
			this.CpuLevelBounceAvoidanceThreshold = 10f;
			this.MinTargetFrameRateHitTime = 10f;
			this.MaxTemperatureLevel = 0.9f;
			this.m_PerfStats = perfStat;
			this.m_PerfControl = perfControl;
			this.m_ThermalStats = thermalStat;
			perfStat.PerformanceBottleneckChangeEvent += delegate(PerformanceBottleneckChangeEventArgs ev)
			{
				this.OnBottleneckChange(ev);
			};
		}

		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				bool flag = this.m_Enabled == value;
				if (!flag)
				{
					this.m_Enabled = value;
				}
			}
		}

		public void Update()
		{
			bool flag = !this.m_Enabled;
			if (!flag)
			{
				this.UpdateImpl(Time.time);
			}
		}

		public void Override(int requestedCpuLevel, int requestedGpuLevel)
		{
			this.m_LastChangeTimeStamp = Time.time;
			bool flag = requestedCpuLevel > this.m_PerfControl.CpuLevel;
			if (flag)
			{
				this.m_LastCpuLevelRaiseTimeStamp = this.m_LastChangeTimeStamp;
			}
			bool flag2 = requestedGpuLevel > this.m_PerfControl.GpuLevel;
			if (flag2)
			{
				this.m_LastCpuLevelRaiseTimeStamp = this.m_LastChangeTimeStamp;
			}
			this.m_PerfControl.CpuLevel = requestedCpuLevel;
			this.m_PerfControl.GpuLevel = requestedGpuLevel;
		}

		private void UpdateImpl(float timestamp)
		{
			bool flag = timestamp - this.m_LastChangeTimeStamp < this.UpdateInterval;
			if (!flag)
			{
				switch (this.m_PerfStats.PerformanceMetrics.PerformanceBottleneck)
				{
				case PerformanceBottleneck.Unknown:
				{
					bool flag2 = !this.m_TriedToResolveUnknownBottleneck && timestamp - this.m_BottleneckUnknownTimestamp > 10f;
					if (flag2)
					{
						bool flag3 = this.AllowRaiseCpuLevel();
						if (flag3)
						{
							this.RaiseCpuLevel(timestamp);
							this.m_TriedToResolveUnknownBottleneck = true;
						}
						else
						{
							bool flag4 = this.AllowRaiseGpuLevel();
							if (flag4)
							{
								this.RaiseGpuLevel(timestamp);
								this.m_TriedToResolveUnknownBottleneck = true;
							}
						}
					}
					break;
				}
				case PerformanceBottleneck.CPU:
				{
					bool flag5 = this.AllowRaiseCpuLevel();
					if (flag5)
					{
						this.RaiseCpuLevel(timestamp);
					}
					break;
				}
				case PerformanceBottleneck.GPU:
				{
					bool flag6 = this.AllowRaiseGpuLevel();
					if (flag6)
					{
						this.RaiseGpuLevel(timestamp);
					}
					break;
				}
				case PerformanceBottleneck.TargetFrameRate:
				{
					bool flag7 = timestamp - this.m_TargetFrameRateHitTimestamp > this.MinTargetFrameRateHitTime;
					if (flag7)
					{
						bool flag8 = this.AllowLowerCpuLevel(timestamp);
						if (flag8)
						{
							this.LowerCpuLevel(timestamp);
						}
						bool flag9 = this.AllowLowerGpuLevel(timestamp);
						if (flag9)
						{
							this.LowerGpuLevel(timestamp);
						}
					}
					break;
				}
				}
			}
		}

		private void OnBottleneckChange(PerformanceBottleneckChangeEventArgs ev)
		{
			bool flag = ev.PerformanceBottleneck == PerformanceBottleneck.TargetFrameRate;
			if (flag)
			{
				this.m_TargetFrameRateHitTimestamp = Time.time;
			}
			bool flag2 = ev.PerformanceBottleneck == PerformanceBottleneck.Unknown;
			if (flag2)
			{
				this.m_BottleneckUnknownTimestamp = Time.time;
			}
			else
			{
				this.m_TriedToResolveUnknownBottleneck = false;
			}
		}

		private void RaiseGpuLevel(float timestamp)
		{
			IDevicePerformanceControl perfControl = this.m_PerfControl;
			int num = perfControl.GpuLevel + 1;
			perfControl.GpuLevel = num;
			this.m_LastChangeTimeStamp = timestamp;
			this.m_LastGpuLevelRaiseTimeStamp = timestamp;
			APLog.Debug("Auto Perf Level: raise GPU level to {0}", new object[] { this.m_PerfControl.GpuLevel });
		}

		private void RaiseCpuLevel(float timestamp)
		{
			IDevicePerformanceControl perfControl = this.m_PerfControl;
			int num = perfControl.CpuLevel + 1;
			perfControl.CpuLevel = num;
			this.m_LastChangeTimeStamp = timestamp;
			this.m_LastCpuLevelRaiseTimeStamp = timestamp;
			APLog.Debug("Auto Perf Level: raise CPU level to {0}", new object[] { this.m_PerfControl.CpuLevel });
		}

		private void LowerCpuLevel(float timestamp)
		{
			IDevicePerformanceControl perfControl = this.m_PerfControl;
			int num = perfControl.CpuLevel - 1;
			perfControl.CpuLevel = num;
			this.m_LastChangeTimeStamp = timestamp;
			APLog.Debug("Auto Perf Level: lower CPU level to {0}", new object[] { this.m_PerfControl.CpuLevel });
		}

		private void LowerGpuLevel(float timestamp)
		{
			IDevicePerformanceControl perfControl = this.m_PerfControl;
			int num = perfControl.GpuLevel - 1;
			perfControl.GpuLevel = num;
			this.m_LastChangeTimeStamp = timestamp;
			APLog.Debug("Auto Perf Level: lower GPU level to {0}", new object[] { this.m_PerfControl.GpuLevel });
		}

		private bool AllowLowerCpuLevel(float timestamp)
		{
			bool flag = this.m_PerfControl.CpuLevel > 0 && timestamp - this.m_LastCpuLevelRaiseTimeStamp > this.CpuLevelBounceAvoidanceThreshold;
			if (flag)
			{
				bool flag2 = this.TargetFrameTime <= 0f;
				if (flag2)
				{
					return true;
				}
				FrameTiming frameTiming = this.m_PerfStats.FrameTiming;
				bool flag3 = frameTiming.AverageCpuFrameTime <= 0f;
				if (flag3)
				{
					return true;
				}
				bool flag4 = frameTiming.AverageCpuFrameTime < this.AllowedCpuActiveTimeRatio * this.TargetFrameTime;
				if (flag4)
				{
					return true;
				}
			}
			return false;
		}

		private bool AllowLowerGpuLevel(float timestamp)
		{
			bool flag = this.m_PerfControl.GpuLevel > 0 && timestamp - this.m_LastGpuLevelRaiseTimeStamp > this.GpuLevelBounceAvoidanceThreshold;
			if (flag)
			{
				bool flag2 = this.TargetFrameTime <= 0f;
				if (flag2)
				{
					return true;
				}
				FrameTiming frameTiming = this.m_PerfStats.FrameTiming;
				bool flag3 = frameTiming.AverageGpuFrameTime <= 0f;
				if (flag3)
				{
					return true;
				}
				bool flag4 = frameTiming.AverageGpuFrameTime < this.AllowedGpuActiveTimeRatio * this.TargetFrameTime;
				if (flag4)
				{
					return true;
				}
			}
			return false;
		}

		private bool AllowRaiseLevels()
		{
			float temperatureLevel = this.m_ThermalStats.ThermalMetrics.TemperatureLevel;
			bool flag = temperatureLevel < 0f;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = temperatureLevel < this.MaxTemperatureLevel;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					APLog.Debug("Auto Perf Level: cannot raise performance level, current temperature level ({0}) exceeds {1}", new object[] { temperatureLevel, this.MaxTemperatureLevel });
					flag2 = false;
				}
			}
			return flag2;
		}

		private bool AllowRaiseCpuLevel()
		{
			bool flag = this.m_PerfControl.CpuLevel >= this.m_PerfControl.MaxCpuPerformanceLevel;
			return !flag && this.AllowRaiseLevels();
		}

		private bool AllowRaiseGpuLevel()
		{
			bool flag = this.m_PerfControl.GpuLevel >= this.m_PerfControl.MaxGpuPerformanceLevel;
			return !flag && this.AllowRaiseLevels();
		}

		private IDevicePerformanceControl m_PerfControl;

		private IPerformanceStatus m_PerfStats;

		private IThermalStatus m_ThermalStats;

		private float m_LastChangeTimeStamp = 0f;

		private float m_LastGpuLevelRaiseTimeStamp = 0f;

		private float m_LastCpuLevelRaiseTimeStamp = 0f;

		private float m_TargetFrameRateHitTimestamp = 0f;

		private float m_BottleneckUnknownTimestamp = 0f;

		private bool m_TriedToResolveUnknownBottleneck = false;

		private bool m_Enabled = false;

		private string m_FeatureName = "Auto Performance Control";
	}
}
