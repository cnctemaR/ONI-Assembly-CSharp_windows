using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

namespace Database
{
	public class EatXCaloriesFromY : ColonyAchievementRequirement
	{
		public EatXCaloriesFromY(int numCalories, List<string> fromFoodType)
		{
			this.numCalories = numCalories;
			this.fromFoodType = fromFoodType;
		}

		public override bool Success()
		{
			return RationTracker.Get().GetCaloiresConsumedByFood(this.fromFoodType) / 1000f > (float)this.numCalories;
		}

		public override void Deserialize(IReader reader)
		{
			this.numCalories = reader.ReadInt32();
			int num = reader.ReadInt32();
			this.fromFoodType = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.fromFoodType.Add(text);
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numCalories);
			writer.Write(this.fromFoodType.Count);
			for (int i = 0; i < this.fromFoodType.Count; i++)
			{
				writer.WriteKleiString(this.fromFoodType[i]);
			}
		}

		private int numCalories;

		private List<string> fromFoodType = new List<string>();
	}
}
