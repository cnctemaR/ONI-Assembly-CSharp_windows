using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

namespace Database
{
	public class CritterTypesWithTraits : ColonyAchievementRequirement
	{
		public CritterTypesWithTraits(List<Tag> critterTypes, bool hasTrait = true)
		{
			foreach (Tag tag in critterTypes)
			{
				if (!this.critterTypesToCheck.ContainsKey(tag))
				{
					this.critterTypesToCheck.Add(tag, false);
				}
			}
			this.hasTrait = hasTrait;
			this.trait = GameTags.Creatures.Wild;
		}

		public override void Update()
		{
			foreach (Capturable capturable in Components.Capturables.Items)
			{
				if (capturable.HasTag(this.trait) == this.hasTrait && this.critterTypesToCheck.ContainsKey(capturable.PrefabID()))
				{
					this.critterTypesToCheck[capturable.PrefabID()] = true;
				}
			}
		}

		public override bool Success()
		{
			foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
			{
				if (!keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.critterTypesToCheck.Count);
			foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
			{
				writer.WriteKleiString(keyValuePair.Key.ToString());
				writer.Write((!keyValuePair.Value) ? 0 : 1);
			}
			writer.Write((!this.hasTrait) ? 0 : 1);
		}

		public override void Deserialize(IReader reader)
		{
			this.critterTypesToCheck = new Dictionary<Tag, bool>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				bool flag = reader.ReadByte() != 0;
				this.critterTypesToCheck.Add(new Tag(text), flag);
			}
			this.hasTrait = reader.ReadByte() != 0;
			this.trait = GameTags.Creatures.Wild;
		}

		public Dictionary<Tag, bool> critterTypesToCheck = new Dictionary<Tag, bool>();

		private Tag trait;

		private bool hasTrait;
	}
}
