using System;
using System.Collections.Generic;

namespace Database
{
	public class CritterTypesWithTraits : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public CritterTypesWithTraits(List<Tag> critterTypes)
		{
			foreach (Tag tag in critterTypes)
			{
				if (!this.critterTypesToCheck.ContainsKey(tag))
				{
					this.critterTypesToCheck.Add(tag, false);
				}
			}
			this.hasTrait = false;
			this.trait = GameTags.Creatures.Wild;
		}

		public override bool Success()
		{
			HashSet<Tag> tamedCritterTypes = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().tamedCritterTypes;
			bool flag = true;
			foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
			{
				flag = flag && tamedCritterTypes.Contains(keyValuePair.Key);
			}
			this.UpdateSavedState();
			return flag;
		}

		public void UpdateSavedState()
		{
			this.revisedCritterTypesToCheckState.Clear();
			HashSet<Tag> tamedCritterTypes = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().tamedCritterTypes;
			foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
			{
				this.revisedCritterTypesToCheckState.Add(keyValuePair.Key, tamedCritterTypes.Contains(keyValuePair.Key));
			}
			foreach (KeyValuePair<Tag, bool> keyValuePair2 in this.revisedCritterTypesToCheckState)
			{
				this.critterTypesToCheck[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}

		public void Deserialize(IReader reader)
		{
			this.critterTypesToCheck = new Dictionary<Tag, bool>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadKleiString();
				bool flag = reader.ReadByte() > 0;
				this.critterTypesToCheck.Add(new Tag(text), flag);
			}
			this.hasTrait = reader.ReadByte() > 0;
			this.trait = GameTags.Creatures.Wild;
		}

		public Dictionary<Tag, bool> critterTypesToCheck = new Dictionary<Tag, bool>();

		private Tag trait;

		private bool hasTrait;

		private Dictionary<Tag, bool> revisedCritterTypesToCheckState = new Dictionary<Tag, bool>();
	}
}
