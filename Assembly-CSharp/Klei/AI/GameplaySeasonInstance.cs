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
				float seasonPeriod = this.Season.GetSeasonPeriod();
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
			float seasonPeriod = this.Season.GetSeasonPeriod();
			this.randomizedNextTime = global::UnityEngine.Random.Range(this.Season.randomizedEventStartTime.min, this.Season.randomizedEventStartTime.max);
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			float num = this.nextPeriodTime + this.randomizedNextTime;
			while (num < currentTimeInCycles || num < this.Season.minCycle)
			{
				this.nextPeriodTime += seasonPeriod;
				num = this.nextPeriodTime + this.randomizedNextTime;
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
				GameplayEventManager.Instance.StartNewEvent(gameplayEvent, this.worldId, new Action<StateMachine.Instance>(this.Season.AdditionalEventInstanceSetup));
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

		public bool ShouldGenerateEvents()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(this.worldId);
			if (!world.IsDupeVisited && !world.IsRoverVisted)
			{
				return false;
			}
			if ((this.Season.finishAfterNumEvents != -1 && this.numStartEvents >= this.Season.finishAfterNumEvents) || this.allEventWillNotRunAgain)
			{
				return false;
			}
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			return currentTimeInCycles > this.Season.minCycle && currentTimeInCycles < this.Season.maxCycle;
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
