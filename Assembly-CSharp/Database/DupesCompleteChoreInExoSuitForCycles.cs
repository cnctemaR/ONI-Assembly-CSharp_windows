using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Database
{
	public class DupesCompleteChoreInExoSuitForCycles : ColonyAchievementRequirement
	{
		public DupesCompleteChoreInExoSuitForCycles(int numCycles)
		{
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			Dictionary<int, List<int>> dupesCompleteChoresInSuits = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().dupesCompleteChoresInSuits;
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			foreach (MinionIdentity minionIdentity in Components.MinionIdentities.Items)
			{
				dictionary.Add(minionIdentity.GetComponent<KPrefabID>().InstanceID, minionIdentity.arrivalTime);
			}
			int num = 0;
			int num2 = Math.Min(dupesCompleteChoresInSuits.Count, this.numCycles);
			for (int i = GameClock.Instance.GetCycle() - num2; i <= GameClock.Instance.GetCycle(); i++)
			{
				if (dupesCompleteChoresInSuits.ContainsKey(i))
				{
					List<int> list = dictionary.Keys.Except<int>(dupesCompleteChoresInSuits[i]).ToList<int>();
					bool flag = true;
					foreach (int num3 in list)
					{
						if (dictionary[num3] < (float)i)
						{
							flag = false;
							break;
						}
					}
					num = (flag ? (num + 1) : 0);
					this.currentCycleStreak = num;
					if (num >= this.numCycles)
					{
						this.currentCycleStreak = this.numCycles;
						return true;
					}
				}
				else
				{
					this.currentCycleStreak = Math.Max(this.currentCycleStreak, num);
					num = 0;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numCycles);
		}

		public override void Deserialize(IReader reader)
		{
			this.numCycles = reader.ReadInt32();
		}

		public int GetNumberOfDupesForCycle(int cycle)
		{
			int num = 0;
			Dictionary<int, List<int>> dupesCompleteChoresInSuits = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().dupesCompleteChoresInSuits;
			if (dupesCompleteChoresInSuits.ContainsKey(GameClock.Instance.GetCycle()))
			{
				num = dupesCompleteChoresInSuits[GameClock.Instance.GetCycle()].Count;
			}
			return num;
		}

		public int currentCycleStreak;

		public int numCycles;
	}
}
