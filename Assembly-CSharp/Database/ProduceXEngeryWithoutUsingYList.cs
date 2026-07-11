using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

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
			float num = 0f;
			foreach (KeyValuePair<Tag, float> keyValuePair in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!this.disallowedBuildings.Contains(keyValuePair.Key))
				{
					num += keyValuePair.Value;
				}
			}
			return num / 1000f > this.amountToProduce;
		}

		public override bool Fail()
		{
			foreach (Tag tag in this.disallowedBuildings)
			{
				if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(tag))
				{
					return true;
				}
			}
			return false;
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

		public float GetProductionAmount(bool complete)
		{
			float num = 0f;
			foreach (KeyValuePair<Tag, float> keyValuePair in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!this.disallowedBuildings.Contains(keyValuePair.Key))
				{
					num += keyValuePair.Value;
				}
			}
			return (!complete) ? num : this.amountToProduce;
		}

		public List<Tag> disallowedBuildings = new List<Tag>();

		public float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;
	}
}
