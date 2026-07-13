using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace UnityEngine.AdaptivePerformance
{
	public class AdaptivePerformanceIndexer
	{
		public float TimeUntilNextAction { get; private set; }

		public StateAction ThermalAction { get; private set; }

		public StateAction PerformanceAction { get; private set; }

		public void GetAppliedScalers(ref List<AdaptivePerformanceScaler> scalers)
		{
			scalers.Clear();
			scalers.AddRange(this.m_AppliedScalers);
		}

		public void GetUnappliedScalers(ref List<AdaptivePerformanceScaler> scalers)
		{
			scalers.Clear();
			scalers.AddRange(this.m_UnappliedScalers);
		}

		public void GetDisabledScalers(ref List<AdaptivePerformanceScaler> scalers)
		{
			scalers.Clear();
			scalers.AddRange(this.m_DisabledScalers);
		}

		public void GetAllRegisteredScalers(ref List<AdaptivePerformanceScaler> scalers)
		{
			scalers.Clear();
			scalers.AddRange(this.m_DisabledScalers);
			scalers.AddRange(this.m_UnappliedScalers);
			scalers.AddRange(this.m_AppliedScalers);
		}

		public void UnapplyAllScalers()
		{
			this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay;
			while (this.m_AppliedScalers.Count != 0)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler = this.m_AppliedScalers[0];
				this.UnapplyScaler(adaptivePerformanceScaler);
			}
		}

		internal void UpdateOverrideLevel(AdaptivePerformanceScaler scaler)
		{
			bool flag = scaler.OverrideLevel == -1;
			if (!flag)
			{
				while (scaler.OverrideLevel > scaler.CurrentLevel)
				{
					this.ApplyScaler(scaler);
				}
				while (scaler.OverrideLevel < scaler.CurrentLevel)
				{
					this.UnapplyScaler(scaler);
				}
			}
		}

		internal void AddScaler(AdaptivePerformanceScaler scaler)
		{
			bool flag = this.m_UnappliedScalers.Contains(scaler) || this.m_AppliedScalers.Contains(scaler);
			if (!flag)
			{
				this.m_UnappliedScalers.Add(scaler);
			}
		}

		internal void RemoveScaler(AdaptivePerformanceScaler scaler)
		{
			bool flag = this.m_UnappliedScalers.Contains(scaler);
			if (flag)
			{
				this.m_UnappliedScalers.Remove(scaler);
			}
			else
			{
				bool flag2 = this.m_AppliedScalers.Contains(scaler);
				if (flag2)
				{
					while (!scaler.NotLeveled)
					{
						scaler.DecreaseLevel();
					}
					this.m_AppliedScalers.Remove(scaler);
				}
			}
		}

		internal AdaptivePerformanceIndexer(ref IAdaptivePerformanceSettings settings, PerformanceStateTracker tracker)
		{
			this.m_Settings = settings;
			this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay;
			this.m_ThermalStateTracker = new ThermalStateTracker();
			this.m_PerformanceStateTracker = tracker;
			this.m_UnappliedScalers = new List<AdaptivePerformanceScaler>();
			this.m_AppliedScalers = new List<AdaptivePerformanceScaler>();
			this.m_DisabledScalers = new List<AdaptivePerformanceScaler>();
			this.m_ScalerEfficiencyTracker = new AdaptivePerformanceScalerEfficiencyTracker();
		}

		internal void Update()
		{
			bool flag = Holder.Instance == null || !this.m_Settings.indexerSettings.active;
			if (!flag)
			{
				this.DeactivateDisabledScalers();
				this.ActivateEnabledScalers();
				StateAction stateAction = this.m_ThermalStateTracker.Update();
				StateAction stateAction2 = this.m_PerformanceStateTracker.Update();
				this.ThermalAction = stateAction;
				this.PerformanceAction = stateAction2;
				bool enabled = Profiler.enabled;
				if (enabled)
				{
					this.CollectProfilerStats();
				}
				this.TimeUntilNextAction = Mathf.Max(this.TimeUntilNextAction - this.DeltaTime(), 0f);
				bool flag2 = this.TimeUntilNextAction != 0f;
				if (!flag2)
				{
					bool isRunning = this.m_ScalerEfficiencyTracker.IsRunning;
					if (isRunning)
					{
						this.m_ScalerEfficiencyTracker.Stop();
					}
					bool flag3 = stateAction == StateAction.Increase && stateAction2 == StateAction.Stale;
					if (flag3)
					{
						this.UnapplyHighestCostScaler();
						this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay;
					}
					else
					{
						bool flag4 = stateAction == StateAction.Stale && stateAction2 == StateAction.Stale;
						if (flag4)
						{
							this.UnapplyHighestCostScaler();
							this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay;
						}
						else
						{
							bool flag5 = stateAction == StateAction.Decrease;
							if (flag5)
							{
								this.ApplyLowestCostScaler();
								this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay;
							}
							else
							{
								bool flag6 = stateAction2 == StateAction.Decrease;
								if (flag6)
								{
									this.ApplyLowestCostScaler();
									this.TimeUntilNextAction = this.m_Settings.indexerSettings.performanceActionDelay;
								}
								else
								{
									bool flag7 = stateAction == StateAction.FastDecrease;
									if (flag7)
									{
										this.ApplyLowestCostScaler();
										this.TimeUntilNextAction = this.m_Settings.indexerSettings.thermalActionDelay / 2f;
									}
									else
									{
										bool flag8 = stateAction2 == StateAction.FastDecrease;
										if (flag8)
										{
											this.ApplyLowestCostScaler();
											this.TimeUntilNextAction = this.m_Settings.indexerSettings.performanceActionDelay / 2f;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected virtual float DeltaTime()
		{
			return Time.deltaTime;
		}

		private void CollectProfilerStats()
		{
			for (int i = this.m_UnappliedScalers.Count - 1; i >= 0; i--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler = this.m_UnappliedScalers[i];
			}
			for (int j = this.m_AppliedScalers.Count - 1; j >= 0; j--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler2 = this.m_AppliedScalers[j];
			}
			for (int k = this.m_DisabledScalers.Count - 1; k >= 0; k--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler3 = this.m_DisabledScalers[k];
			}
			AdaptivePerformanceProfilerStats.FlushScalerDataToProfilerStream();
		}

		private void DeactivateDisabledScalers()
		{
			for (int i = this.m_UnappliedScalers.Count - 1; i >= 0; i--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler = this.m_UnappliedScalers[i];
				bool flag = !adaptivePerformanceScaler.Enabled && !this.m_DisabledScalers.Contains(adaptivePerformanceScaler);
				if (flag)
				{
					APLog.Debug("[Indexer] Deactivated " + adaptivePerformanceScaler.Name + " scaler.", Array.Empty<object>());
					adaptivePerformanceScaler.Deactivate();
					this.m_DisabledScalers.Add(adaptivePerformanceScaler);
					this.m_UnappliedScalers.RemoveAt(i);
				}
			}
			for (int j = this.m_AppliedScalers.Count - 1; j >= 0; j--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler2 = this.m_AppliedScalers[j];
				bool flag2 = !adaptivePerformanceScaler2.Enabled && !this.m_DisabledScalers.Contains(adaptivePerformanceScaler2);
				if (flag2)
				{
					APLog.Debug("[Indexer] Deactivated " + adaptivePerformanceScaler2.Name + " scaler.", Array.Empty<object>());
					adaptivePerformanceScaler2.Deactivate();
					this.m_DisabledScalers.Add(adaptivePerformanceScaler2);
					this.m_AppliedScalers.RemoveAt(j);
				}
			}
		}

		private void ActivateEnabledScalers()
		{
			for (int i = this.m_DisabledScalers.Count - 1; i >= 0; i--)
			{
				AdaptivePerformanceScaler adaptivePerformanceScaler = this.m_DisabledScalers[i];
				bool enabled = adaptivePerformanceScaler.Enabled;
				if (enabled)
				{
					adaptivePerformanceScaler.Activate();
					this.AddScaler(adaptivePerformanceScaler);
					this.m_DisabledScalers.RemoveAt(i);
					APLog.Debug("[Indexer] Activated " + adaptivePerformanceScaler.Name + " scaler.", Array.Empty<object>());
				}
			}
		}

		private bool ApplyLowestCostScaler()
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = null;
			float num = float.PositiveInfinity;
			foreach (AdaptivePerformanceScaler adaptivePerformanceScaler2 in this.m_UnappliedScalers)
			{
				bool flag = !adaptivePerformanceScaler2.Enabled;
				if (!flag)
				{
					bool flag2 = adaptivePerformanceScaler2.OverrideLevel != -1;
					if (!flag2)
					{
						int num2 = adaptivePerformanceScaler2.CalculateCost();
						bool flag3 = num > (float)num2;
						if (flag3)
						{
							adaptivePerformanceScaler = adaptivePerformanceScaler2;
							num = (float)num2;
						}
					}
				}
			}
			foreach (AdaptivePerformanceScaler adaptivePerformanceScaler3 in this.m_AppliedScalers)
			{
				bool flag4 = !adaptivePerformanceScaler3.Enabled;
				if (!flag4)
				{
					bool flag5 = adaptivePerformanceScaler3.OverrideLevel != -1;
					if (!flag5)
					{
						bool isMaxLevel = adaptivePerformanceScaler3.IsMaxLevel;
						if (!isMaxLevel)
						{
							int num3 = adaptivePerformanceScaler3.CalculateCost();
							bool flag6 = num > (float)num3;
							if (flag6)
							{
								adaptivePerformanceScaler = adaptivePerformanceScaler3;
								num = (float)num3;
							}
						}
					}
				}
			}
			bool flag7 = adaptivePerformanceScaler != null;
			bool flag8;
			if (flag7)
			{
				this.m_ScalerEfficiencyTracker.Start(adaptivePerformanceScaler, true);
				this.ApplyScaler(adaptivePerformanceScaler);
				flag8 = true;
			}
			else
			{
				flag8 = false;
			}
			return flag8;
		}

		private void ApplyScaler(AdaptivePerformanceScaler scaler)
		{
			APLog.Debug(string.Format("[Indexer] Applying {0} scaler at level {1} and try to increase level to {2}", scaler.Name, scaler.CurrentLevel, scaler.CurrentLevel + 1), Array.Empty<object>());
			bool notLeveled = scaler.NotLeveled;
			if (notLeveled)
			{
				this.m_UnappliedScalers.Remove(scaler);
				this.m_AppliedScalers.Add(scaler);
			}
			scaler.IncreaseLevel();
		}

		private bool UnapplyHighestCostScaler()
		{
			AdaptivePerformanceScaler adaptivePerformanceScaler = null;
			float num = float.NegativeInfinity;
			foreach (AdaptivePerformanceScaler adaptivePerformanceScaler2 in this.m_AppliedScalers)
			{
				bool flag = adaptivePerformanceScaler2.OverrideLevel != -1;
				if (!flag)
				{
					int num2 = adaptivePerformanceScaler2.CalculateCost();
					bool flag2 = num < (float)num2;
					if (flag2)
					{
						adaptivePerformanceScaler = adaptivePerformanceScaler2;
						num = (float)num2;
					}
				}
			}
			bool flag3 = adaptivePerformanceScaler != null;
			bool flag4;
			if (flag3)
			{
				this.m_ScalerEfficiencyTracker.Start(adaptivePerformanceScaler, false);
				this.UnapplyScaler(adaptivePerformanceScaler);
				flag4 = true;
			}
			else
			{
				flag4 = false;
			}
			return flag4;
		}

		private void UnapplyScaler(AdaptivePerformanceScaler scaler)
		{
			APLog.Debug(string.Format("[Indexer] Unapplying {0} scaler at level {1} and try to decrease level to {2}", scaler.Name, scaler.CurrentLevel, scaler.CurrentLevel - 1), Array.Empty<object>());
			scaler.DecreaseLevel();
			bool notLeveled = scaler.NotLeveled;
			if (notLeveled)
			{
				this.m_AppliedScalers.Remove(scaler);
				this.m_UnappliedScalers.Add(scaler);
			}
		}

		private List<AdaptivePerformanceScaler> m_UnappliedScalers;

		private List<AdaptivePerformanceScaler> m_AppliedScalers;

		private List<AdaptivePerformanceScaler> m_DisabledScalers;

		private ThermalStateTracker m_ThermalStateTracker;

		private PerformanceStateTracker m_PerformanceStateTracker;

		private AdaptivePerformanceScalerEfficiencyTracker m_ScalerEfficiencyTracker;

		private IAdaptivePerformanceSettings m_Settings;

		private const string m_FeatureName = "Indexer";
	}
}
