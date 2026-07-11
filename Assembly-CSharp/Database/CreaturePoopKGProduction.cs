using System;
using System.IO;
using KSerialization;
using STRINGS;

namespace Database
{
	public class CreaturePoopKGProduction : ColonyAchievementRequirement
	{
		public CreaturePoopKGProduction(Tag poopElement, float amountToPoop)
		{
			this.poopElement = poopElement;
			this.amountToPoop = amountToPoop;
		}

		public override bool Success()
		{
			return Game.Instance.savedInfo.creaturePoopAmount.ContainsKey(this.poopElement) && Game.Instance.savedInfo.creaturePoopAmount[this.poopElement] >= this.amountToPoop;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.amountToPoop);
			writer.WriteKleiString(this.poopElement.ToString());
		}

		public override void Deserialize(IReader reader)
		{
			this.amountToPoop = reader.ReadSingle();
			string text = reader.ReadKleiString();
			this.poopElement = new Tag(text);
		}

		public override string GetProgress(bool complete)
		{
			float num = 0f;
			Game.Instance.savedInfo.creaturePoopAmount.TryGetValue(this.poopElement, out num);
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.POOP_PRODUCTION, GameUtil.GetFormattedMass(complete ? this.amountToPoop : num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.amountToPoop, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		}

		private Tag poopElement;

		private float amountToPoop;
	}
}
