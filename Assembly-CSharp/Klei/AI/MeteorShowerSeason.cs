using System;
using System.Diagnostics;
using Klei.CustomSettings;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public class MeteorShowerSeason : GameplaySeason
	{
		public MeteorShowerSeason(string id, GameplaySeason.Type type, string dlcId, float period, bool synchronizedToPeriod, float randomizedEventStartTime = -1f, bool startActive = false, int finishAfterNumEvents = -1, float minCycle = 0f, float maxCycle = float.PositiveInfinity, int numEventsToStartEachPeriod = 1, bool affectedByDifficultySettings = true, float clusterTravelDuration = -1f)
			: base(id, type, dlcId, period, synchronizedToPeriod, randomizedEventStartTime, startActive, finishAfterNumEvents, minCycle, maxCycle, numEventsToStartEachPeriod)
		{
			this.affectedByDifficultySettings = affectedByDifficultySettings;
			this.clusterTravelDuration = clusterTravelDuration;
		}

		public override void AdditionalEventInstanceSetup(StateMachine.Instance generic_smi)
		{
			(generic_smi as MeteorShowerEvent.StatesInstance).clusterTravelDuration = this.clusterTravelDuration;
		}

		public override float GetSeasonPeriod()
		{
			SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.MeteorShowers);
			float num = base.GetSeasonPeriod();
			if (this.affectedByDifficultySettings && currentQualitySetting != null)
			{
				string id = currentQualitySetting.id;
				if (id != null)
				{
					if (!(id == "Infrequent"))
					{
						if (!(id == "Intense"))
						{
							if (id == "Doomed")
							{
								num *= 1f;
							}
						}
						else
						{
							num *= 1f;
						}
					}
					else
					{
						num *= 2f;
					}
				}
			}
			return num;
		}

		public bool affectedByDifficultySettings = true;

		public float clusterTravelDuration = -1f;
	}
}
