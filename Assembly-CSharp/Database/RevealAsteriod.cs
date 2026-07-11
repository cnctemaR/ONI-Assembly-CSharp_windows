using System;
using System.IO;

namespace Database
{
	public class RevealAsteriod : ColonyAchievementRequirement
	{
		public RevealAsteriod(float percentToReveal)
		{
			this.percentToReveal = percentToReveal;
		}

		public override bool Success()
		{
			float num = 0f;
			for (int i = 0; i < Grid.Visible.Length; i++)
			{
				if (Grid.Visible[i] > 0)
				{
					num += 1f;
				}
			}
			return num / (float)Grid.Visible.Length > this.percentToReveal;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.percentToReveal);
		}

		public override void Deserialize(IReader reader)
		{
			this.percentToReveal = reader.ReadSingle();
		}

		private float percentToReveal;
	}
}
