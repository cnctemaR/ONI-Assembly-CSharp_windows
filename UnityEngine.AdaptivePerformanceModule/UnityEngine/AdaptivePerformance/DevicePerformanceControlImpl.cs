using System;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance
{
	internal class DevicePerformanceControlImpl : IDevicePerformanceControl
	{
		public DevicePerformanceControlImpl(IDevicePerformanceLevelControl performanceLevelControl)
		{
			this.m_PerformanceLevelControl = performanceLevelControl;
			this.PerformanceControlMode = PerformanceControlMode.Automatic;
			this.CurrentCpuLevel = -1;
			this.CurrentGpuLevel = -1;
			this.CpuLevel = -1;
			this.GpuLevel = -1;
		}

		public bool Update(out PerformanceLevelChangeEventArgs changeArgs)
		{
			changeArgs = default(PerformanceLevelChangeEventArgs);
			changeArgs.PerformanceControlMode = this.PerformanceControlMode;
			bool flag = this.PerformanceControlMode == PerformanceControlMode.System;
			bool flag4;
			if (flag)
			{
				bool flag2 = this.CurrentCpuLevel != -1 || this.CurrentGpuLevel != -1;
				this.CurrentCpuLevel = -1;
				this.CurrentGpuLevel = -1;
				bool flag3 = flag2;
				if (flag3)
				{
					changeArgs.CpuLevel = this.CurrentCpuLevel;
					changeArgs.GpuLevel = this.CurrentGpuLevel;
					changeArgs.CpuLevelDelta = 0;
					changeArgs.GpuLevelDelta = 0;
				}
				flag4 = flag2;
			}
			else
			{
				bool flag5 = this.CpuLevel != -1 || this.GpuLevel != -1;
				if (flag5)
				{
					bool flag6 = this.CpuLevel != this.CurrentCpuLevel || this.GpuLevel != this.CurrentGpuLevel;
					if (flag6)
					{
						int cpuLevel = this.CpuLevel;
						int gpuLevel = this.GpuLevel;
						bool flag7 = this.m_PerformanceLevelControl.SetPerformanceLevel(ref cpuLevel, ref gpuLevel);
						if (flag7)
						{
							changeArgs.CpuLevelDelta = this.ComputeDelta(this.CurrentCpuLevel, cpuLevel);
							changeArgs.GpuLevelDelta = this.ComputeDelta(this.CurrentGpuLevel, gpuLevel);
							bool flag8 = cpuLevel != this.CpuLevel || gpuLevel != this.GpuLevel;
							if (flag8)
							{
								Debug.Log(string.Format("Requested CPU level {0} and GPU level {1} was overriden by System with CPU level {2} and GPU level {3}", new object[] { this.CpuLevel, this.GpuLevel, cpuLevel, gpuLevel }));
							}
							this.CurrentCpuLevel = this.CpuLevel;
							this.CurrentGpuLevel = this.GpuLevel;
							changeArgs.CpuLevel = this.CurrentCpuLevel;
							changeArgs.GpuLevel = this.CurrentGpuLevel;
							return true;
						}
						changeArgs.CpuLevelDelta = 0;
						changeArgs.GpuLevelDelta = 0;
						this.CurrentCpuLevel = -1;
						this.CurrentGpuLevel = -1;
						this.CpuLevel = -1;
						this.GpuLevel = -1;
						return false;
					}
				}
				flag4 = false;
			}
			return flag4;
		}

		private int ComputeDelta(int oldLevel, int newLevel)
		{
			bool flag = oldLevel < 0 || newLevel < 0;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				num = newLevel - oldLevel;
			}
			return num;
		}

		public bool AutomaticPerformanceControl
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public PerformanceControlMode PerformanceControlMode { get; set; }

		public int MaxCpuPerformanceLevel
		{
			get
			{
				return (this.m_PerformanceLevelControl != null) ? this.m_PerformanceLevelControl.MaxCpuPerformanceLevel : (-1);
			}
		}

		public int MaxGpuPerformanceLevel
		{
			get
			{
				return (this.m_PerformanceLevelControl != null) ? this.m_PerformanceLevelControl.MaxGpuPerformanceLevel : (-1);
			}
		}

		public int CpuLevel { get; set; }

		public int GpuLevel { get; set; }

		public int CurrentCpuLevel { get; set; }

		public int CurrentGpuLevel { get; set; }

		public bool CpuPerformanceBoost { get; set; }

		public bool GpuPerformanceBoost { get; set; }

		private IDevicePerformanceLevelControl m_PerformanceLevelControl;
	}
}
