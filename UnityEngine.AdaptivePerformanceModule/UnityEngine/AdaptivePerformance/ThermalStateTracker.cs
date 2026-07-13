using System;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance
{
	internal class ThermalStateTracker
	{
		public StateAction Update()
		{
			bool flag = !Holder.Instance.SupportedFeature(Feature.TemperatureLevel);
			StateAction stateAction;
			if (flag)
			{
				stateAction = StateAction.Stale;
			}
			else
			{
				float temperatureTrend = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureTrend;
				float temperatureLevel = Holder.Instance.ThermalStatus.ThermalMetrics.TemperatureLevel;
				WarningLevel warningLevel = Holder.Instance.ThermalStatus.ThermalMetrics.WarningLevel;
				bool flag2 = warningLevel == WarningLevel.ThrottlingImminent && this.warningTemp == 1f;
				if (flag2)
				{
					this.warningTemp = temperatureLevel;
				}
				bool flag3 = warningLevel == WarningLevel.Throttling && this.throttlingTemp == 1f;
				if (flag3)
				{
					this.throttlingTemp = temperatureLevel;
				}
				bool flag4 = warningLevel == WarningLevel.Throttling || temperatureLevel >= this.throttlingTemp;
				if (flag4)
				{
					stateAction = StateAction.FastDecrease;
				}
				else
				{
					bool flag5 = warningLevel == WarningLevel.ThrottlingImminent || temperatureLevel >= this.warningTemp;
					if (flag5)
					{
						bool flag6 = temperatureLevel > (this.warningTemp + this.throttlingTemp) / 2f;
						if (flag6)
						{
							stateAction = StateAction.Decrease;
						}
						else
						{
							bool flag7 = temperatureTrend <= 0f;
							if (flag7)
							{
								stateAction = StateAction.Stale;
							}
							else
							{
								bool flag8 = (double)temperatureTrend > 0.5;
								if (flag8)
								{
									stateAction = StateAction.FastDecrease;
								}
								else
								{
									stateAction = StateAction.Decrease;
								}
							}
						}
					}
					else
					{
						bool flag9 = warningLevel == WarningLevel.NoWarning && temperatureLevel < this.warningTemp;
						if (flag9)
						{
							bool flag10 = temperatureTrend <= 0f;
							if (flag10)
							{
								return StateAction.Increase;
							}
							bool flag11 = (double)temperatureTrend > 0.5;
							if (flag11)
							{
								return StateAction.FastDecrease;
							}
							bool flag12 = (double)temperatureTrend > 0.1;
							if (flag12)
							{
								return StateAction.Decrease;
							}
						}
						stateAction = StateAction.Stale;
					}
				}
			}
			return stateAction;
		}

		private float warningTemp = 1f;

		private float throttlingTemp = 1f;
	}
}
