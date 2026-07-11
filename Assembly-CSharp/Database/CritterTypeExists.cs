using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;

namespace Database
{
	public class CritterTypeExists : ColonyAchievementRequirement
	{
		public CritterTypeExists(List<Tag> critterTypes)
		{
			this.critterTypes = critterTypes;
		}

		public override bool Success()
		{
			foreach (Capturable capturable in Components.Capturables.Items)
			{
				if (this.critterTypes.Contains(capturable.PrefabID()))
				{
					return true;
				}
			}
			return false;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(this.critterTypes.Count);
			foreach (Tag tag in this.critterTypes)
			{
				writer.WriteKleiString(tag.ToString());
			}
		}

		public override void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			this.critterTypes = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				this.critterTypes.Add(new Tag(text));
			}
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.HATCH_A_MORPH;
		}

		private List<Tag> critterTypes = new List<Tag>();
	}
}
