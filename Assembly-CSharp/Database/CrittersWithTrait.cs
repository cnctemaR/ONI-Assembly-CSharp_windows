using System;
using System.IO;
using KSerialization;

namespace Database
{
	public class CrittersWithTrait : ColonyAchievementRequirement
	{
		public CrittersWithTrait(int numCritters, Tag trait, bool hasTrait = true)
		{
			this.numCritters = numCritters;
			this.trait = trait;
			this.hasTrait = hasTrait;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (Capturable capturable in Components.Capturables.Items)
			{
				if (capturable.HasTag(this.trait) == this.hasTrait)
				{
					num++;
				}
			}
			return num >= this.numCritters;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.numCritters);
			writer.WriteKleiString(this.trait.ToString());
			writer.Write((!this.hasTrait) ? 0 : 1);
		}

		public override void Deserialize(IReader reader)
		{
			this.numCritters = reader.ReadInt32();
			this.trait = new Tag(reader.ReadKleiString());
			this.hasTrait = reader.ReadByte() != 0;
		}

		private int numCritters;

		private Tag trait;

		private bool hasTrait;
	}
}
