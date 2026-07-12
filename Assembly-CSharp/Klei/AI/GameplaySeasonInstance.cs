using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class GameplaySeasonInstance : ISaveLoadable
	{
		public float NextEventTime
		{
			get
			{
				return this.nextPeriodTime + this.randomizedNextTime;
			}
		}

		public GameplaySeason Season
		{
			get
			{
				if (this._season == null)
				{
					this._season = Db.Get().GameplaySeasons.TryGet(this.seasonId);
				}
				return this._season;
			}
		}

		public GameplaySeasonInstance(GameplaySeason season, int worldId)
		{
			this.seasonId = season.Id;
			this.worldId = worldId;
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			if (season.synchronizedToPeriod)
			{
				float seasonPeriod = this.GetSeasonPeriod();
				this.nextPeriodTime = (Mathf.Floor(currentTimeInCycles / seasonPeriod) + 1f) * seasonPeriod;
			}
			else
			{
				this.nextPeriodTime = currentTimeInCycles;
			}
			this.CalculateNextEventTime();
		}

		private void CalculateNextEventTime()
		{
			float seasonPeriod = this.GetSeasonPeriod();
			this.randomizedNextTime = global::UnityEngine.Random.Range(this.Season.randomizedEventStartTime.min, this.Season.randomizedEventStartTime.max);
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			while (this.nextPeriodTime < currentTimeInCycles || this.NextEventTime < this.Season.minCycle)
			{
				this.nextPeriodTime += seasonPeriod;
			}
		}

		public bool StartEvent(bool ignorePreconditions = false)
		{
			bool flag = false;
			this.CalculateNextEventTime();
			this.numStartEvents++;
			List<GameplayEvent> list;
			if (!ignorePreconditions)
			{
				list = this.Season.events.Where<GameplayEvent>((GameplayEvent x) => x.IsAllowed()).ToList<GameplayEvent>();
			}
			else
			{
				list = this.Season.events;
			}
			List<GameplayEvent> list2 = list;
			if (list2.Count > 0)
			{
				list2.ForEach(delegate(GameplayEvent x)
				{
					x.CalculatePriority();
				});
				list2.Sort();
				int num = Mathf.Min(list2.Count, 5);
				GameplayEvent gameplayEvent = list2[global::UnityEngine.Random.Range(0, num)];
				GameplayEventManager.Instance.StartNewEvent(gameplayEvent, this.worldId);
				flag = true;
			}
			this.allEventWillNotRunAgain = true;
			using (List<GameplayEvent>.Enumerator enumerator = this.Season.events.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.WillNeverRunAgain())
					{
						this.allEventWillNotRunAgain = false;
						break;
					}
				}
			}
			return flag;
		}

		private float GetSeasonPeriod()
		{
			return this.Season.period;
		}

		public bool ShouldGenerateEvents()
		{
			return this.Season.minCycle > GameUtil.GetCurrentTimeInCycles() || ((this.Season.finishAfterNumEvents != -1 && this.numStartEvents >= this.Season.finishAfterNumEvents) || this.allEventWillNotRunAgain) || (this.Season.maxCycle != float.PositiveInfinity && GameUtil.GetCurrentTimeInCycles() > this.Season.maxCycle);
		}

		public const int LIMIT_SELECTION = 5;

		[Serialize]
		public int numStartEvents;

		[Serialize]
		public int worldId;

		[Serialize]
		private readonly string seasonId;

		[Serialize]
		private float nextPeriodTime;

		[Serialize]
		private float randomizedNextTime;

		private bool allEventWillNotRunAgain;

		private GameplaySeason _season;
	}
}
