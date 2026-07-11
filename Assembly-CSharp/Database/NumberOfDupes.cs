using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class NumberOfDupes : VictoryColonyAchievementRequirement
	{
		public NumberOfDupes(int num)
		{
			this.numDupes = num;
		}

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS, this.numDupes);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS_DESCRIPTION, this.numDupes);
		}

		public override bool Success()
		{
			return Components.LiveMinionIdentities.Items.Count >= this.numDupes;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numDupes);
		}

		public override void Deserialize(IReader reader)
		{
			this.numDupes = reader.ReadInt32();
		}

		private int numDupes;
	}
}
