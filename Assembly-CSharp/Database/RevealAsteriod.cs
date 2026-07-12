using System;
using STRINGS;

namespace Database
{
	public class RevealAsteriod : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public RevealAsteriod(float percentToReveal)
		{
			this.percentToReveal = percentToReveal;
		}

		public override bool Success()
		{
			this.amountRevealed = 0f;
			float num = 0f;
			for (int i = 0; i < Grid.Visible.Length; i++)
			{
				if (Grid.Visible[i] > 0)
				{
					num += 1f;
				}
			}
			this.amountRevealed = num / (float)Grid.Visible.Length;
			return num / (float)Grid.Visible.Length > this.percentToReveal;
		}

		public void Deserialize(IReader reader)
		{
			this.percentToReveal = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REVEALED, this.amountRevealed * 100f, this.percentToReveal * 100f);
		}

		private float percentToReveal;

		private float amountRevealed;
	}
}
