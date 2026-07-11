using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KSerialization;

namespace Database
{
	public class EatXKCalProducedByY : ColonyAchievementRequirement
	{
		public EatXKCalProducedByY(int numCalories, List<Tag> foodProducers)
		{
			this.numCalories = numCalories;
			this.foodProducers = foodProducers;
		}

		public override bool Success()
		{
			List<string> list = new List<string>();
			List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
			List<ComplexRecipe> list2 = new List<ComplexRecipe>();
			foreach (ComplexRecipe complexRecipe in recipes)
			{
				foreach (Tag tag in this.foodProducers)
				{
					foreach (Tag tag2 in complexRecipe.fabricators)
					{
						if (tag2 == tag)
						{
							list.Add(complexRecipe.FirstResult.ToString());
						}
					}
				}
			}
			float caloiresConsumedByFood = RationTracker.Get().GetCaloiresConsumedByFood(list.Distinct<string>().ToList<string>());
			return caloiresConsumedByFood / 1000f > (float)this.numCalories;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.foodProducers.Count);
			foreach (Tag tag in this.foodProducers)
			{
				writer.WriteKleiString(tag.ToString());
			}
			writer.Write(this.numCalories);
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			this.foodProducers = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.foodProducers.Add(new Tag(text));
			}
			this.numCalories = reader.ReadInt32();
		}

		private int numCalories;

		private List<Tag> foodProducers;
	}
}
