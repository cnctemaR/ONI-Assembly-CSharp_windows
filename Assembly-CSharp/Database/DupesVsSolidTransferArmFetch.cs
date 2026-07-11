using System;
using System.Collections.Generic;
using System.IO;

namespace Database
{
	public class DupesVsSolidTransferArmFetch : ColonyAchievementRequirement
	{
		public DupesVsSolidTransferArmFetch(float percentage, int numCycles)
		{
			this.percentage = percentage;
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
			Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
			int num = 0;
			this.currentCycleCount = 0;
			for (int i = GameClock.Instance.GetCycle() - this.numCycles; i < GameClock.Instance.GetCycle(); i++)
			{
				if (fetchAutomatedChoreDeliveries.ContainsKey(i) && (!fetchDupeChoreDeliveries.ContainsKey(i) || (float)fetchDupeChoreDeliveries[i] < (float)fetchAutomatedChoreDeliveries[i] * this.percentage))
				{
					num++;
					if (num >= this.numCycles)
					{
						this.currentCycleCount = this.numCycles;
						return true;
					}
				}
				else
				{
					this.currentCycleCount = Math.Max(this.currentCycleCount, num);
					num = 0;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numCycles);
			writer.Write(this.percentage);
		}

		public override void Deserialize(IReader reader)
		{
			this.numCycles = reader.ReadInt32();
			this.percentage = reader.ReadSingle();
		}

		public float percentage;

		public int numCycles;

		public int currentCycleCount;

		public bool armsOutPerformingDupesThisCycle;
	}
}
