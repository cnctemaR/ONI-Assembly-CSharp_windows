using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

namespace Database
{
	public class ProduceXEngeryWithoutUsingYList : ColonyAchievementRequirement
	{
		public ProduceXEngeryWithoutUsingYList(float amountToProduce, List<Tag> disallowedBuildings)
		{
			this.disallowedBuildings = disallowedBuildings;
			this.amountToProduce = amountToProduce;
			this.usedDisallowedBuilding = false;
		}

		public override bool Success()
		{
			return !this.usedDisallowedBuilding && this.amountProduced / 1000f > this.amountToProduce;
		}

		public override bool Fail()
		{
			return this.usedDisallowedBuilding;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.disallowedBuildings.Count);
			foreach (Tag tag in this.disallowedBuildings)
			{
				writer.WriteKleiString(tag.ToString());
			}
			writer.Write((double)this.amountProduced);
			writer.Write((double)this.amountToProduce);
			writer.Write((!this.usedDisallowedBuilding) ? 0 : 1);
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			this.disallowedBuildings = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.disallowedBuildings.Add(new Tag(text));
			}
			this.amountProduced = (float)reader.ReadDouble();
			this.amountToProduce = (float)reader.ReadDouble();
			this.usedDisallowedBuilding = reader.ReadByte() != 0;
		}

		public override void Update()
		{
			foreach (Generator generator in Game.Instance.energySim.Generators)
			{
				if (generator.JoulesAvailable > 0f)
				{
					KPrefabID component = generator.GetComponent<KPrefabID>();
					if (component.HasAnyTags(this.disallowedBuildings))
					{
						this.usedDisallowedBuilding = true;
					}
					this.amountProduced = Mathf.Max(generator.JoulesAvailable, generator.JoulesAvailable + this.amountProduced);
				}
			}
		}

		private List<Tag> disallowedBuildings = new List<Tag>();

		private float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;
	}
}
