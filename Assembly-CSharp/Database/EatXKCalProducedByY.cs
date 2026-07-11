using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KSerialization;
using STRINGS;

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
			foreach (ComplexRecipe complexRecipe in ComplexRecipeManager.Get().recipes)
			{
				foreach (Tag tag in this.foodProducers)
				{
					using (List<Tag>.Enumerator enumerator3 = complexRecipe.fabricators.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current == tag)
							{
								list.Add(complexRecipe.FirstResult.ToString());
							}
						}
					}
				}
			}
			return RationTracker.Get().GetCaloiresConsumedByFood(list.Distinct<string>().ToList<string>()) / 1000f > (float)this.numCalories;
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

		public override string GetProgress(bool complete)
		{
			string text = "";
			for (int i = 0; i < this.foodProducers.Count; i++)
			{
				if (i != 0)
				{
					text += COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.PREPARED_SEPARATOR;
				}
				BuildingDef buildingDef = Assets.GetBuildingDef(this.foodProducers[i].Name);
				text += buildingDef.Name;
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CONSUME_ITEM, text);
		}

		private int numCalories;

		private List<Tag> foodProducers;
	}
}
