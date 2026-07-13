using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public class GameplaySeason : Resource, IHasDlcRestrictions
	{
		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public GameplaySeason(string id, GameplaySeason.Type type, float period, bool synchronizedToPeriod, float randomizedEventStartTime = -1f, bool startActive = false, int finishAfterNumEvents = -1, float minCycle = 0f, float maxCycle = float.PositiveInfinity, int numEventsToStartEachPeriod = 1, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: base(id, null, null)
		{
			this.type = type;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
			this.period = period;
			this.synchronizedToPeriod = synchronizedToPeriod;
			global::Debug.Assert(period > 0f, "Season " + id + "'s Period cannot be 0 or negative");
			if (randomizedEventStartTime == -1f)
			{
				this.randomizedEventStartTime = new MathUtil.MinMax(-0f * period, 0f * period);
			}
			else
			{
				this.randomizedEventStartTime = new MathUtil.MinMax(-randomizedEventStartTime, randomizedEventStartTime);
				DebugUtil.DevAssert((this.randomizedEventStartTime.max - this.randomizedEventStartTime.min) * 0.4f < period, string.Format("Season {0} randomizedEventStartTime is greater than {1}% of its period.", id, 0.4f), null);
			}
			this.startActive = startActive;
			this.finishAfterNumEvents = finishAfterNumEvents;
			this.minCycle = minCycle;
			this.maxCycle = maxCycle;
			this.events = new List<GameplayEvent>();
			this.numEventsToStartEachPeriod = numEventsToStartEachPeriod;
		}

		[Obsolete]
		public GameplaySeason(string id, GameplaySeason.Type type, string dlcId, float period, bool synchronizedToPeriod, float randomizedEventStartTime = -1f, bool startActive = false, int finishAfterNumEvents = -1, float minCycle = 0f, float maxCycle = float.PositiveInfinity, int numEventsToStartEachPeriod = 1)
			: this(id, type, period, synchronizedToPeriod, randomizedEventStartTime, startActive, finishAfterNumEvents, minCycle, maxCycle, numEventsToStartEachPeriod, new string[] { dlcId }, null)
		{
		}

		public virtual void AdditionalEventInstanceSetup(StateMachine.Instance generic_smi)
		{
		}

		public virtual float GetSeasonPeriod()
		{
			return this.period;
		}

		public GameplaySeason AddEvent(GameplayEvent evt)
		{
			this.events.Add(evt);
			return this;
		}

		public virtual GameplaySeasonInstance Instantiate(int worldId)
		{
			return new GameplaySeasonInstance(this, worldId);
		}

		public const float DEFAULT_PERCENTAGE_RANDOMIZED_EVENT_START = 0f;

		public const float PERCENTAGE_WARNING = 0.4f;

		public const float USE_DEFAULT = -1f;

		public const int INFINITE = -1;

		public float period;

		public bool synchronizedToPeriod;

		public MathUtil.MinMax randomizedEventStartTime;

		public int finishAfterNumEvents = -1;

		public bool startActive;

		public int numEventsToStartEachPeriod;

		public float minCycle;

		public float maxCycle;

		public List<GameplayEvent> events;

		private string[] requiredDlcIds;

		private string[] forbiddenDlcIds;

		public GameplaySeason.Type type;

		[Obsolete]
		public string dlcId;

		public enum Type
		{
			World,
			Cluster
		}
	}
}
