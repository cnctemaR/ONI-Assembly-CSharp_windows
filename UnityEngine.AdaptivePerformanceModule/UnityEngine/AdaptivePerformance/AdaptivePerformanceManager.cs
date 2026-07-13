using System;
using System.Diagnostics;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace UnityEngine.AdaptivePerformance
{
	internal class AdaptivePerformanceManager : MonoBehaviour, IAdaptivePerformance, IThermalStatus, IPerformanceStatus, IDevicePerformanceControl, IDevelopmentSettings, IPerformanceModeStatus
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event ThermalEventHandler ThermalEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PerformanceModeEventHandler PerformanceModeEvent;

		public ThermalMetrics ThermalMetrics
		{
			get
			{
				return this.m_ThermalMetrics;
			}
		}

		public PerformanceMetrics PerformanceMetrics
		{
			get
			{
				return this.m_PerformanceMetrics;
			}
		}

		public FrameTiming FrameTiming
		{
			get
			{
				return this.m_FrameTiming;
			}
		}

		public PerformanceMode PerformanceMode
		{
			get
			{
				return this.m_PerformanceMode;
			}
		}

		public bool Logging
		{
			get
			{
				return APLog.enabled;
			}
			set
			{
				APLog.enabled = value;
			}
		}

		public int LoggingFrequencyInFrames { get; set; }

		public bool Initialized
		{
			get
			{
				return this.m_Subsystem != null && this.m_Subsystem.Initialized && AdaptivePerformanceGeneralSettings.Instance != null && AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized;
			}
		}

		public bool Active
		{
			get
			{
				return this.m_Subsystem != null && this.m_Subsystem.running && AdaptivePerformanceGeneralSettings.Instance != null && AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized && AdaptivePerformanceGeneralSettings.Instance.IsProviderStarted;
			}
		}

		public int MaxCpuPerformanceLevel
		{
			get
			{
				return (this.m_DevicePerfControl != null) ? this.m_DevicePerfControl.MaxCpuPerformanceLevel : (-1);
			}
		}

		public int MaxGpuPerformanceLevel
		{
			get
			{
				return (this.m_DevicePerfControl != null) ? this.m_DevicePerfControl.MaxGpuPerformanceLevel : (-1);
			}
		}

		public bool AutomaticPerformanceControl
		{
			get
			{
				return this.m_AutomaticPerformanceControl;
			}
			set
			{
				this.m_AutomaticPerformanceControl = value;
				this.m_AutomaticPerformanceControlChanged = true;
			}
		}

		public PerformanceControlMode PerformanceControlMode
		{
			get
			{
				return (this.m_DevicePerfControl != null) ? this.m_DevicePerfControl.PerformanceControlMode : PerformanceControlMode.System;
			}
		}

		public int CpuLevel
		{
			get
			{
				return this.m_RequestedCpuLevel;
			}
			set
			{
				this.m_RequestedCpuLevel = value;
				this.m_NewUserPerformanceLevelRequest = true;
			}
		}

		public int GpuLevel
		{
			get
			{
				return this.m_RequestedGpuLevel;
			}
			set
			{
				this.m_RequestedGpuLevel = value;
				this.m_NewUserPerformanceLevelRequest = true;
			}
		}

		public bool CpuPerformanceBoost
		{
			get
			{
				return this.m_RequestedCpuBoost;
			}
			set
			{
				this.m_RequestedCpuBoost = value;
				this.m_NewUserCpuPerformanceBoostRequest = true;
			}
		}

		public bool GpuPerformanceBoost
		{
			get
			{
				return this.m_RequestedGpuBoost;
			}
			set
			{
				this.m_RequestedGpuBoost = value;
				this.m_NewUserGpuPerformanceBoostRequest = true;
			}
		}

		public IDevelopmentSettings DevelopmentSettings
		{
			get
			{
				return this;
			}
		}

		public IThermalStatus ThermalStatus
		{
			get
			{
				return this;
			}
		}

		public IPerformanceStatus PerformanceStatus
		{
			get
			{
				return this;
			}
		}

		public IDevicePerformanceControl DevicePerformanceControl
		{
			get
			{
				return this;
			}
		}

		public IPerformanceModeStatus PerformanceModeStatus
		{
			get
			{
				return this;
			}
		}

		public AdaptivePerformanceIndexer Indexer { get; private set; }

		public IAdaptivePerformanceSettings Settings
		{
			get
			{
				return this.m_Settings;
			}
			private set
			{
				this.m_Settings = value;
			}
		}

		public AdaptivePerformanceSubsystem Subsystem
		{
			get
			{
				return this.m_Subsystem;
			}
		}

		public bool SupportedFeature(Feature feature)
		{
			return this.m_Subsystem != null && this.m_Subsystem.Capabilities.HasFlag(feature);
		}

		public void Awake()
		{
			APLog.enabled = true;
			bool flag = AdaptivePerformanceGeneralSettings.Instance == null;
			if (!flag)
			{
				bool flag2 = !AdaptivePerformanceGeneralSettings.Instance.InitManagerOnStart;
				if (flag2)
				{
					APLog.Debug("Adaptive Performance is disabled via Settings.", Array.Empty<object>());
				}
				else
				{
					this.InitializeAdaptivePerformance();
				}
			}
		}

		private void LogThermalEvent(ThermalMetrics ev)
		{
			APLog.Debug("[thermal event] temperature level: {0}, warning level: {1}, thermal trend: {2}", new object[] { ev.TemperatureLevel, ev.WarningLevel, ev.TemperatureTrend });
		}

		private void LogBottleneckEvent(PerformanceBottleneckChangeEventArgs ev)
		{
			APLog.Debug("[perf event] bottleneck: {0}", new object[] { ev.PerformanceBottleneck });
		}

		private void LogBoostEvent(PerformanceBoostChangeEventArgs ev)
		{
			APLog.Debug("[perf event] CPU boost: {0}, GPU boost: {1}", new object[] { ev.CpuBoost, ev.GpuBoost });
		}

		private void LogPerformanceModeEvent(PerformanceMode performanceMode)
		{
			APLog.Debug("[performance mode event] performance mode: {0}", new object[] { performanceMode });
		}

		private static string ToStringWithSign(int x)
		{
			return x.ToString("+#;-#;0");
		}

		private void LogPerformanceLevelEvent(PerformanceLevelChangeEventArgs ev)
		{
			APLog.Debug("[perf level change] cpu: {0}({1}) gpu: {2}({3}) control mode: {4} manual override: {5}", new object[]
			{
				ev.CpuLevel,
				AdaptivePerformanceManager.ToStringWithSign(ev.CpuLevelDelta),
				ev.GpuLevel,
				AdaptivePerformanceManager.ToStringWithSign(ev.GpuLevelDelta),
				ev.PerformanceControlMode,
				ev.ManualOverride
			});
		}

		private void AddNonNegativeValue(RunningAverage runningAverage, float value)
		{
			bool flag = value >= 0f && value < 1f;
			if (flag)
			{
				runningAverage.AddValue(value);
			}
		}

		public void LateUpdate()
		{
			bool flag = !this.Active;
			if (!flag)
			{
				bool flag2 = this.m_CpuFrameTimeProvider != null || this.m_GpuFrameTimeProvider != null;
				if (flag2)
				{
					bool flag3 = AdaptivePerformanceManager.WillCurrentFrameRender();
					if (flag3)
					{
						bool flag4 = this.m_CpuFrameTimeProvider != null;
						if (flag4)
						{
							this.m_CpuFrameTimeProvider.Measure();
						}
						bool flag5 = this.m_GpuFrameTimeProvider != null;
						if (flag5)
						{
							this.m_GpuFrameTimeProvider.Measure();
						}
					}
				}
			}
		}

		public void Update()
		{
			bool flag = !this.Active;
			if (!flag)
			{
				this.UpdateSubsystem();
				this.Indexer.Update();
				bool enabled = Profiler.enabled;
				if (enabled)
				{
					this.CollectProfilerStats();
				}
				bool flag2 = APLog.enabled && this.LoggingFrequencyInFrames > 0;
				if (flag2)
				{
					this.m_FrameCount++;
					bool flag3 = this.m_FrameCount % this.LoggingFrequencyInFrames == 0;
					if (flag3)
					{
						APLog.Debug(this.m_Subsystem.Stats, Array.Empty<object>());
						APLog.Debug("Performance level CPU={0}/{1} GPU={2}/{3} thermal warn={4}({5}) thermal level={6} mode={7}", new object[]
						{
							this.m_PerformanceMetrics.CurrentCpuLevel,
							this.MaxCpuPerformanceLevel,
							this.m_PerformanceMetrics.CurrentGpuLevel,
							this.MaxGpuPerformanceLevel,
							this.m_ThermalMetrics.WarningLevel,
							(int)this.m_ThermalMetrics.WarningLevel,
							this.m_ThermalMetrics.TemperatureLevel,
							this.m_DevicePerfControl.PerformanceControlMode
						});
						APLog.Debug("Average GPU frametime = {0} ms (Current = {1} ms)", new object[]
						{
							this.m_FrameTiming.AverageGpuFrameTime * 1000f,
							this.m_FrameTiming.CurrentGpuFrameTime * 1000f
						});
						APLog.Debug("Average CPU frametime = {0} ms (Current = {1} ms)", new object[]
						{
							this.m_FrameTiming.AverageCpuFrameTime * 1000f,
							this.m_FrameTiming.CurrentCpuFrameTime * 1000f
						});
						APLog.Debug("Average frametime = {0} ms (Current = {1} ms)", new object[]
						{
							this.m_FrameTiming.AverageFrameTime * 1000f,
							this.m_FrameTiming.CurrentFrameTime * 1000f
						});
						APLog.Debug("Bottleneck {0}, ThermalTrend {1}", new object[]
						{
							this.m_PerformanceMetrics.PerformanceBottleneck,
							this.m_ThermalMetrics.TemperatureTrend
						});
						APLog.Debug("CPU Boost Mode {0}, GPU Boost Mode {1}", new object[]
						{
							this.m_PerformanceMetrics.CpuPerformanceBoost,
							this.m_PerformanceMetrics.GpuPerformanceBoost
						});
						APLog.Debug("Cluster Info = Big Cores: {0} Medium Cores: {1} Little Cores: {2}", new object[]
						{
							this.m_PerformanceMetrics.ClusterInfo.BigCore,
							this.m_PerformanceMetrics.ClusterInfo.MediumCore,
							this.m_PerformanceMetrics.ClusterInfo.LittleCore
						});
						APLog.Debug("FPS = {0}", new object[] { 1f / this.m_FrameTiming.AverageFrameTime });
						APLog.Debug("Performance Mode = {0}", new object[] { this.m_PerformanceMode });
					}
				}
			}
		}

		private void CollectProfilerStats()
		{
			AdaptivePerformanceProfilerStats.CurrentCPUMarker.Sample(this.m_FrameTiming.CurrentCpuFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.AvgCPUMarker.Sample(this.m_FrameTiming.AverageCpuFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.CurrentGPUMarker.Sample(this.m_FrameTiming.CurrentGpuFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.AvgGPUMarker.Sample(this.m_FrameTiming.AverageGpuFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.CurrentCPULevelMarker.Sample(this.m_PerformanceMetrics.CurrentCpuLevel);
			AdaptivePerformanceProfilerStats.CurrentGPULevelMarker.Sample(this.m_PerformanceMetrics.CurrentGpuLevel);
			AdaptivePerformanceProfilerStats.CurrentFrametimeMarker.Sample(this.m_FrameTiming.CurrentFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.AvgFrametimeMarker.Sample(this.m_FrameTiming.AverageFrameTime * 1E+09f);
			AdaptivePerformanceProfilerStats.WarningLevelMarker.Sample((int)this.m_ThermalMetrics.WarningLevel);
			AdaptivePerformanceProfilerStats.TemperatureLevelMarker.Sample(this.m_ThermalMetrics.TemperatureLevel);
			AdaptivePerformanceProfilerStats.TemperatureTrendMarker.Sample(this.m_ThermalMetrics.TemperatureTrend);
			AdaptivePerformanceProfilerStats.BottleneckMarker.Sample((int)this.m_PerformanceMetrics.PerformanceBottleneck);
			AdaptivePerformanceProfilerStats.PerformanceModeMarker.Sample((int)this.m_PerformanceMode);
		}

		private void AccumulateTimingValue(ref float accu, float newValue)
		{
			bool flag = accu < 0f;
			if (!flag)
			{
				bool flag2 = newValue >= 0f;
				if (flag2)
				{
					accu += newValue;
				}
				else
				{
					accu = -1f;
				}
			}
		}

		private void UpdateSubsystem()
		{
			PerformanceDataRecord performanceDataRecord = this.m_Subsystem.Update();
			this.m_ThermalMetrics.WarningLevel = performanceDataRecord.WarningLevel;
			this.m_ThermalMetrics.TemperatureLevel = performanceDataRecord.TemperatureLevel;
			bool flag = !this.m_JustResumed;
			if (flag)
			{
				bool flag2 = !this.m_UseProviderOverallFrameTime;
				if (flag2)
				{
					this.AccumulateTimingValue(ref this.m_OverallFrameTimeAccu, Time.unscaledDeltaTime);
				}
				bool flag3 = AdaptivePerformanceManager.WillCurrentFrameRender();
				if (flag3)
				{
					this.AddNonNegativeValue(this.m_OverallFrameTime, this.m_UseProviderOverallFrameTime ? performanceDataRecord.OverallFrameTime : this.m_OverallFrameTimeAccu);
					this.AddNonNegativeValue(this.m_GpuFrameTime, (this.m_GpuFrameTimeProvider == null) ? performanceDataRecord.GpuFrameTime : this.m_GpuFrameTimeProvider.GpuFrameTime);
					this.AddNonNegativeValue(this.m_CpuFrameTime, (this.m_CpuFrameTimeProvider == null) ? performanceDataRecord.CpuFrameTime : this.m_CpuFrameTimeProvider.CpuFrameTime);
					this.m_OverallFrameTimeAccu = 0f;
				}
				this.m_TemperatureTrend.Update(performanceDataRecord.TemperatureTrend, performanceDataRecord.TemperatureLevel, performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureLevel), Time.time);
			}
			else
			{
				this.m_TemperatureTrend.Reset();
				this.m_JustResumed = false;
			}
			this.m_ThermalMetrics.TemperatureTrend = this.m_TemperatureTrend.ThermalTrend;
			this.m_FrameTiming.AverageFrameTime = this.m_OverallFrameTime.GetAverageOr(-1f);
			this.m_FrameTiming.CurrentFrameTime = this.m_OverallFrameTime.GetMostRecentValueOr(-1f);
			this.m_FrameTiming.AverageGpuFrameTime = this.m_GpuFrameTime.GetAverageOr(-1f);
			this.m_FrameTiming.CurrentGpuFrameTime = this.m_GpuFrameTime.GetMostRecentValueOr(-1f);
			this.m_FrameTiming.AverageCpuFrameTime = this.m_CpuFrameTime.GetAverageOr(-1f);
			this.m_FrameTiming.CurrentCpuFrameTime = this.m_CpuFrameTime.GetMostRecentValueOr(-1f);
			float num = AdaptivePerformanceManager.EffectiveTargetFrameRate();
			float num2 = -1f;
			bool flag4 = num > 0f;
			if (flag4)
			{
				num2 = 1f / num;
			}
			bool flag5 = false;
			PerformanceBottleneckChangeEventArgs e = default(PerformanceBottleneckChangeEventArgs);
			PerformanceBoostChangeEventArgs e2 = default(PerformanceBoostChangeEventArgs);
			bool flag6 = this.m_OverallFrameTime.GetNumValues() == this.m_OverallFrameTime.GetSampleWindowSize() && this.m_GpuFrameTime.GetNumValues() == this.m_GpuFrameTime.GetSampleWindowSize() && this.m_CpuFrameTime.GetNumValues() == this.m_CpuFrameTime.GetSampleWindowSize();
			if (flag6)
			{
				PerformanceBottleneck performanceBottleneck = BottleneckUtil.DetermineBottleneck(this.m_PerformanceMetrics.PerformanceBottleneck, this.m_FrameTiming.AverageCpuFrameTime, this.m_FrameTiming.AverageGpuFrameTime, this.m_FrameTiming.AverageFrameTime, num2);
				bool flag7 = performanceBottleneck != this.m_PerformanceMetrics.PerformanceBottleneck;
				if (flag7)
				{
					this.m_PerformanceMetrics.PerformanceBottleneck = performanceBottleneck;
					e.PerformanceBottleneck = performanceBottleneck;
					flag5 = this.PerformanceBottleneckChangeEvent != null;
				}
			}
			bool flag8 = this.ThermalEvent != null && (performanceDataRecord.ChangeFlags.HasFlag(Feature.WarningLevel) || performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureLevel) || performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureTrend));
			bool flag9 = this.PerformanceModeEvent != null && performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceMode);
			bool flag10 = performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceLevel);
			if (flag10)
			{
				this.m_DevicePerfControl.CurrentCpuLevel = performanceDataRecord.CpuPerformanceLevel;
			}
			bool flag11 = performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceLevel);
			if (flag11)
			{
				this.m_DevicePerfControl.CurrentGpuLevel = performanceDataRecord.GpuPerformanceLevel;
			}
			bool flag12 = performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceLevelControl) || this.m_AutomaticPerformanceControlChanged;
			if (flag12)
			{
				this.m_AutomaticPerformanceControlChanged = false;
				bool performanceLevelControlAvailable = performanceDataRecord.PerformanceLevelControlAvailable;
				if (performanceLevelControlAvailable)
				{
					bool automaticPerformanceControl = this.AutomaticPerformanceControl;
					if (automaticPerformanceControl)
					{
						this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Automatic;
					}
					else
					{
						this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Manual;
					}
				}
				else
				{
					this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.System;
				}
			}
			this.m_AutoPerformanceLevelController.TargetFrameTime = num2;
			this.m_AutoPerformanceLevelController.Enabled = this.m_DevicePerfControl.PerformanceControlMode == PerformanceControlMode.Automatic;
			PerformanceLevelChangeEventArgs e3 = default(PerformanceLevelChangeEventArgs);
			bool flag13 = this.m_DevicePerfControl.PerformanceControlMode != PerformanceControlMode.System;
			if (flag13)
			{
				bool enabled = this.m_AutoPerformanceLevelController.Enabled;
				if (enabled)
				{
					bool newUserPerformanceLevelRequest = this.m_NewUserPerformanceLevelRequest;
					if (newUserPerformanceLevelRequest)
					{
						this.m_AutoPerformanceLevelController.Override(this.m_RequestedCpuLevel, this.m_RequestedGpuLevel);
						e3.ManualOverride = true;
					}
					this.m_AutoPerformanceLevelController.Update();
				}
				else
				{
					bool newUserPerformanceLevelRequest2 = this.m_NewUserPerformanceLevelRequest;
					if (newUserPerformanceLevelRequest2)
					{
						this.m_DevicePerfControl.CpuLevel = this.m_RequestedCpuLevel;
						this.m_DevicePerfControl.GpuLevel = this.m_RequestedGpuLevel;
					}
				}
			}
			bool flag14 = this.PerformanceBoostChangeEvent != null && (performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceBoost) || performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceBoost));
			bool flag15 = performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceBoost);
			if (flag15)
			{
				bool flag16 = this.m_DevicePerfControl.CpuPerformanceBoost != performanceDataRecord.CpuPerformanceBoost;
				if (flag16)
				{
					this.m_DevicePerfControl.CpuPerformanceBoost = performanceDataRecord.CpuPerformanceBoost;
					this.m_RequestedCpuBoost = performanceDataRecord.CpuPerformanceBoost;
				}
			}
			bool flag17 = performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceBoost);
			if (flag17)
			{
				bool flag18 = this.m_DevicePerfControl.GpuPerformanceBoost != performanceDataRecord.GpuPerformanceBoost;
				if (flag18)
				{
					this.m_DevicePerfControl.GpuPerformanceBoost = performanceDataRecord.GpuPerformanceBoost;
					this.m_RequestedGpuBoost = performanceDataRecord.GpuPerformanceBoost;
				}
			}
			bool flag19 = this.m_NewUserCpuPerformanceBoostRequest && this.PerformanceBoostChangeEvent != null;
			if (flag19)
			{
				this.m_NewUserCpuPerformanceBoostRequest = false;
				this.m_Subsystem.PerformanceLevelControl.EnableCpuBoost();
			}
			bool flag20 = this.m_NewUserGpuPerformanceBoostRequest && this.PerformanceBoostChangeEvent != null;
			if (flag20)
			{
				this.m_NewUserGpuPerformanceBoostRequest = false;
				this.m_Subsystem.PerformanceLevelControl.EnableGpuBoost();
			}
			bool flag21 = this.m_DevicePerfControl.Update(out e3) && this.PerformanceLevelChangeEvent != null;
			if (flag21)
			{
				this.PerformanceLevelChangeEvent(e3);
			}
			this.m_PerformanceMetrics.CurrentCpuLevel = this.m_DevicePerfControl.CurrentCpuLevel;
			this.m_PerformanceMetrics.CurrentGpuLevel = this.m_DevicePerfControl.CurrentGpuLevel;
			this.m_PerformanceMetrics.CpuPerformanceBoost = this.m_DevicePerfControl.CpuPerformanceBoost;
			this.m_PerformanceMetrics.GpuPerformanceBoost = this.m_DevicePerfControl.GpuPerformanceBoost;
			this.m_NewUserPerformanceLevelRequest = false;
			bool flag22 = performanceDataRecord.ChangeFlags.HasFlag(Feature.ClusterInfo);
			if (flag22)
			{
				this.m_PerformanceMetrics.ClusterInfo = performanceDataRecord.ClusterInfo;
			}
			bool flag23 = performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceMode);
			if (flag23)
			{
				this.m_PerformanceMode = performanceDataRecord.PerformanceMode;
			}
			bool flag24 = flag8;
			if (flag24)
			{
				this.ThermalEvent(this.m_ThermalMetrics);
			}
			bool flag25 = flag5;
			if (flag25)
			{
				this.PerformanceBottleneckChangeEvent(e);
			}
			bool flag26 = flag14;
			if (flag26)
			{
				e2.CpuBoost = this.m_DevicePerfControl.CpuPerformanceBoost;
				e2.GpuBoost = this.m_DevicePerfControl.GpuPerformanceBoost;
				this.PerformanceBoostChangeEvent(e2);
			}
			bool flag27 = flag9;
			if (flag27)
			{
				this.PerformanceModeEvent(this.m_PerformanceMode);
			}
		}

		private static bool WillCurrentFrameRender()
		{
			return OnDemandRendering.willCurrentFrameRender;
		}

		public static float EffectiveTargetFrameRate()
		{
			return (float)OnDemandRendering.effectiveRenderFrameRate;
		}

		public void OnDestroy()
		{
			this.DeinitializeAdaptivePerformance();
		}

		public void InitializeAdaptivePerformance()
		{
			bool active = this.Active;
			if (!active)
			{
				bool initialized = this.Initialized;
				if (!initialized)
				{
					APLog.enabled = true;
					bool flag = AdaptivePerformanceGeneralSettings.Instance == null;
					if (!flag)
					{
						bool flag2 = !AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized;
						if (flag2)
						{
							AdaptivePerformanceGeneralSettings.Instance.InitAdaptivePerformance();
						}
						bool flag3 = !AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized;
						if (flag3)
						{
							APLog.Debug("Initialization of Provider was not successful. Are there errors present? Make sure to select your loader in the Adaptive Performance Settings for this platform.", Array.Empty<object>());
						}
						else
						{
							AdaptivePerformanceLoader adaptivePerformanceLoader = AdaptivePerformanceGeneralSettings.Instance.Manager.ActiveLoaderAs<AdaptivePerformanceLoader>();
							bool flag4 = adaptivePerformanceLoader == null;
							if (flag4)
							{
								APLog.Debug("No Active Loader was found. Make sure to select your loader in the Adaptive Performance Settings for this platform.", Array.Empty<object>());
							}
							else
							{
								this.m_Settings = adaptivePerformanceLoader.GetSettings();
								bool flag5 = this.m_Settings == null;
								if (flag5)
								{
									APLog.Debug("No Settings available. Did the Post Process Buildstep fail?", Array.Empty<object>());
								}
								else
								{
									string[] availableScalerProfiles = this.m_Settings.GetAvailableScalerProfiles();
									bool flag6 = availableScalerProfiles.Length == 0;
									if (flag6)
									{
										APLog.Debug("No Scaler Profiles available. Did you remove all profiles manually from the provider Settings?", Array.Empty<object>());
									}
									else
									{
										this.m_Settings.LoadScalerProfile(availableScalerProfiles[this.m_Settings.defaultScalerProfilerIndex]);
										this.AutomaticPerformanceControl = this.m_Settings.automaticPerformanceMode;
										this.LoggingFrequencyInFrames = this.m_Settings.statsLoggingFrequencyInFrames;
										APLog.enabled = this.m_Settings.logging;
										bool flag7 = this.m_Subsystem == null;
										if (flag7)
										{
											AdaptivePerformanceSubsystem adaptivePerformanceSubsystem = (AdaptivePerformanceSubsystem)adaptivePerformanceLoader.GetDefaultSubsystem();
											bool flag8 = adaptivePerformanceSubsystem != null;
											if (flag8)
											{
												bool initialized2 = adaptivePerformanceSubsystem.Initialized;
												if (!initialized2)
												{
													adaptivePerformanceSubsystem.Destroy();
													APLog.Debug("Subsystem not initialized.", Array.Empty<object>());
													return;
												}
												this.m_Subsystem = adaptivePerformanceSubsystem;
												APLog.Debug("Subsystem version={0}", new object[] { this.m_Subsystem.Version });
											}
										}
										bool flag9 = this.m_Subsystem != null;
										if (flag9)
										{
											this.m_UseProviderOverallFrameTime = this.m_Subsystem.Capabilities.HasFlag(Feature.OverallFrameTime);
											this.m_DevicePerfControl = new DevicePerformanceControlImpl(this.m_Subsystem.PerformanceLevelControl);
											this.m_AutoPerformanceLevelController = new AutoPerformanceLevelController(this.m_DevicePerfControl, this.PerformanceStatus, this.ThermalStatus);
											bool automaticGameMode = this.m_Settings.automaticGameMode;
											if (automaticGameMode)
											{
												this.m_AutoPerformanceModeController = new AutoPerformanceModeController(this.PerformanceModeStatus);
											}
											this.m_AppLifecycle = this.m_Subsystem.ApplicationLifecycle;
											bool flag10 = !this.m_Subsystem.Capabilities.HasFlag(Feature.CpuFrameTime);
											if (flag10)
											{
												this.m_CpuFrameTimeProvider = new CpuTimeProvider();
											}
											bool flag11 = !this.m_Subsystem.Capabilities.HasFlag(Feature.GpuFrameTime);
											if (flag11)
											{
												this.m_GpuFrameTimeProvider = new GpuTimeProvider();
											}
											this.m_TemperatureTrend = new TemperatureTrend(this.m_Subsystem.Capabilities.HasFlag(Feature.TemperatureTrend));
											bool flag12 = this.m_RequestedCpuLevel == -1;
											if (flag12)
											{
												this.m_RequestedCpuLevel = this.m_DevicePerfControl.MaxCpuPerformanceLevel;
											}
											bool flag13 = this.m_RequestedGpuLevel == -1;
											if (flag13)
											{
												this.m_RequestedGpuLevel = this.m_DevicePerfControl.MaxGpuPerformanceLevel;
											}
											this.m_NewUserPerformanceLevelRequest = true;
											bool flag14 = this.m_Subsystem.PerformanceLevelControl == null;
											if (flag14)
											{
												this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.System;
											}
											else
											{
												bool automaticPerformanceControl = this.AutomaticPerformanceControl;
												if (automaticPerformanceControl)
												{
													this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Automatic;
												}
												else
												{
													this.m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Manual;
												}
											}
											this.ThermalEvent += this.LogThermalEvent;
											this.PerformanceBottleneckChangeEvent += this.LogBottleneckEvent;
											this.PerformanceLevelChangeEvent += this.LogPerformanceLevelEvent;
											this.PerformanceModeEvent += this.LogPerformanceModeEvent;
											bool flag15 = this.m_Subsystem.Capabilities.HasFlag(Feature.CpuPerformanceBoost);
											if (flag15)
											{
												this.PerformanceBoostChangeEvent += this.LogBoostEvent;
											}
											this.Indexer = new AdaptivePerformanceIndexer(ref this.m_Settings, new PerformanceStateTracker(120));
											this.UpdateSubsystem();
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void StartAdaptivePerformance()
		{
			bool flag = !this.Initialized;
			if (!flag)
			{
				AdaptivePerformanceGeneralSettings.Instance.StartAdaptivePerformance();
			}
		}

		public void StopAdaptivePerformance()
		{
			bool flag = !this.Active;
			if (!flag)
			{
				AdaptivePerformanceGeneralSettings.Instance.StopAdaptivePerformance();
			}
		}

		public void DeinitializeAdaptivePerformance()
		{
			bool flag = !this.Initialized;
			if (!flag)
			{
				AdaptivePerformanceGeneralSettings.Instance.DeInitAdaptivePerformance();
				bool flag2 = this.Indexer != null;
				if (flag2)
				{
					this.Indexer.UnapplyAllScalers();
				}
				this.ThermalEvent -= this.LogThermalEvent;
				this.PerformanceBottleneckChangeEvent -= this.LogBottleneckEvent;
				this.PerformanceLevelChangeEvent -= this.LogPerformanceLevelEvent;
				this.PerformanceBoostChangeEvent -= this.LogBoostEvent;
				this.PerformanceModeEvent -= this.LogPerformanceModeEvent;
				APLog.enabled = false;
				this.m_Settings = null;
				this.m_Subsystem = null;
				this.m_DevicePerfControl = null;
				this.m_AutoPerformanceLevelController = null;
				this.m_AutoPerformanceModeController = null;
				this.m_AppLifecycle = null;
				this.m_CpuFrameTimeProvider = null;
				this.m_GpuFrameTimeProvider = null;
				this.m_TemperatureTrend = null;
				this.Indexer = null;
			}
		}

		public void OnApplicationPause(bool pause)
		{
			bool flag = this.m_Subsystem != null;
			if (flag)
			{
				if (pause)
				{
					bool flag2 = this.m_AppLifecycle != null;
					if (flag2)
					{
						this.m_AppLifecycle.ApplicationPause();
					}
					this.m_OverallFrameTime.Reset();
					this.m_GpuFrameTime.Reset();
					this.m_CpuFrameTime.Reset();
				}
				else
				{
					this.m_ThermalMetrics.WarningLevel = WarningLevel.NoWarning;
					bool flag3 = this.m_AppLifecycle != null;
					if (flag3)
					{
						this.m_AppLifecycle.ApplicationResume();
					}
					this.m_JustResumed = true;
				}
			}
		}

		private bool m_JustResumed = false;

		private int m_RequestedCpuLevel = -1;

		private int m_RequestedGpuLevel = -1;

		private bool m_NewUserPerformanceLevelRequest = false;

		private bool m_RequestedCpuBoost = false;

		private bool m_RequestedGpuBoost = false;

		private bool m_NewUserCpuPerformanceBoostRequest = false;

		private bool m_NewUserGpuPerformanceBoostRequest = false;

		private ThermalMetrics m_ThermalMetrics = new ThermalMetrics
		{
			WarningLevel = WarningLevel.NoWarning,
			TemperatureLevel = -1f,
			TemperatureTrend = 0f
		};

		private PerformanceMetrics m_PerformanceMetrics = new PerformanceMetrics
		{
			CurrentCpuLevel = -1,
			CurrentGpuLevel = -1,
			PerformanceBottleneck = PerformanceBottleneck.Unknown
		};

		private FrameTiming m_FrameTiming = new FrameTiming
		{
			CurrentFrameTime = -1f,
			AverageFrameTime = -1f,
			CurrentGpuFrameTime = -1f,
			AverageGpuFrameTime = -1f,
			CurrentCpuFrameTime = -1f,
			AverageCpuFrameTime = -1f
		};

		private PerformanceMode m_PerformanceMode = PerformanceMode.Unknown;

		private bool m_AutomaticPerformanceControl;

		private bool m_AutomaticPerformanceControlChanged;

		private IAdaptivePerformanceSettings m_Settings;

		private AdaptivePerformanceSubsystem m_Subsystem = null;

		private DevicePerformanceControlImpl m_DevicePerfControl;

		private AutoPerformanceLevelController m_AutoPerformanceLevelController;

		private AutoPerformanceModeController m_AutoPerformanceModeController;

		private CpuTimeProvider m_CpuFrameTimeProvider;

		private GpuTimeProvider m_GpuFrameTimeProvider;

		private IApplicationLifecycle m_AppLifecycle;

		private TemperatureTrend m_TemperatureTrend;

		private bool m_UseProviderOverallFrameTime = false;

		private WaitForEndOfFrame m_WaitForEndOfFrame = new WaitForEndOfFrame();

		private int m_FrameCount = 0;

		private RunningAverage m_OverallFrameTime = new RunningAverage(100);

		private float m_OverallFrameTimeAccu = 0f;

		private RunningAverage m_GpuFrameTime = new RunningAverage(100);

		private RunningAverage m_CpuFrameTime = new RunningAverage(100);
	}
}
