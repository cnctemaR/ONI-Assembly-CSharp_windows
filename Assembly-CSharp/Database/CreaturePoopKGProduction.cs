using System;
using System.IO;
using KSerialization;

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

		private Tag poopElement;

		private float amountToPoop;
	}
}
