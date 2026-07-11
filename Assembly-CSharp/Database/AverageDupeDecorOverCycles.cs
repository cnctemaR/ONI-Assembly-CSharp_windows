using System;
using System.IO;

namespace Database
{
	public class AverageDupeDecorOverCycles : ColonyAchievementRequirement
	{
		public AverageDupeDecorOverCycles(int averageDecor, int numCycles)
		{
			this.averageDecor = averageDecor;
			this.numCycles = numCycles;
		}

		public override bool Success()
		{
			if (Components.LiveMinionIdentities.Count <= 0)
			{
				return false;
			}
			float num = 0f;
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
			{
				DecorMonitor.Instance smi = minionIdentity.GetSMI<DecorMonitor.Instance>();
				if (smi != null)
				{
					num = smi.GetTodaysAverageDecor();
				}
			}
			float num2 = num / (float)Components.LiveMinionIdentities.Count;
			if (num2 > (float)this.averageDecor)
			{
				if (!this.trackingCycles)
				{
					this.trackingCycles = true;
					this.startCycle = 0;
				}
				this.startCycle++;
			}
			else
			{
				this.trackingCycles = false;
			}
			return num2 > (float)this.averageDecor && this.startCycle >= this.numCycles;
		}

		public override void Deserialize(IReader reader)
		{
			this.averageDecor = reader.ReadInt32();
			this.numCycles = reader.ReadInt32();
			this.trackingCycles = reader.ReadByte() != 0;
			this.startCycle = reader.ReadInt32();
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.averageDecor);
			writer.Write(this.numCycles);
			writer.Write((!this.trackingCycles) ? 0 : 1);
			writer.Write(this.startCycle);
		}

		private int averageDecor;

		private int numCycles;

		private bool trackingCycles;

		private int startCycle;
	}
}
