using System;
using System.IO;
using KSerialization;
using UnityEngine;

namespace Database
{
	public class ProduceXEngeryWithoutUsingY : ColonyAchievementRequirement
	{
		public ProduceXEngeryWithoutUsingY(float amountToProduce, Tag disallowedBuilding)
		{
			this.disallowedBuilding = disallowedBuilding;
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
			writer.WriteKleiString(this.disallowedBuilding.ToString());
			writer.Write((double)this.amountProduced);
			writer.Write((double)this.amountToProduce);
			writer.Write((!this.usedDisallowedBuilding) ? 0 : 1);
		}

		public override void Deserialize(IReader reader)
		{
			string text = reader.ReadKleiString();
			this.disallowedBuilding = new Tag(text);
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
					if (component.HasTag(this.disallowedBuilding))
					{
						this.usedDisallowedBuilding = true;
					}
					this.amountProduced = Mathf.Max(generator.JoulesAvailable, generator.JoulesAvailable + this.amountProduced);
				}
			}
		}

		private Tag disallowedBuilding;

		private float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;
	}
}
