using System;
using STRINGS;
using UnityEngine;

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
			WorldContainer startWorld = ClusterManager.Instance.GetStartWorld();
			Vector2 minimumBounds = startWorld.minimumBounds;
			Vector2 maximumBounds = startWorld.maximumBounds;
			int num2 = (int)minimumBounds.x;
			while ((float)num2 <= maximumBounds.x)
			{
				int num3 = (int)minimumBounds.y;
				while ((float)num3 <= maximumBounds.y)
				{
					if (Grid.Visible[Grid.PosToCell(new Vector2((float)num2, (float)num3))] > 0)
					{
						num += 1f;
					}
					num3++;
				}
				num2++;
			}
			this.amountRevealed = num / (float)(startWorld.Width * startWorld.Height);
			return this.amountRevealed > this.percentToReveal;
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
