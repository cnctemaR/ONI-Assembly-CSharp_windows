using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class AttributeLevels : KMonoBehaviour, ISaveLoadable
	{
		public IEnumerator<AttributeLevel> GetEnumerator()
		{
			return this.levels.GetEnumerator();
		}

		public AttributeLevels.LevelSaveLoad[] SaveLoadLevels
		{
			get
			{
				return this.saveLoadLevels;
			}
			set
			{
				this.saveLoadLevels = value;
			}
		}

		protected override void OnPrefabInit()
		{
			foreach (AttributeInstance attributeInstance in this.GetAttributes())
			{
				if (attributeInstance.Attribute.IsTrainable)
				{
					AttributeLevel attributeLevel = new AttributeLevel(attributeInstance);
					this.levels.Add(attributeLevel);
					attributeLevel.Apply(this);
				}
			}
		}

		[OnSerializing]
		public void OnSerializing()
		{
			this.saveLoadLevels = new AttributeLevels.LevelSaveLoad[this.levels.Count];
			for (int i = 0; i < this.levels.Count; i++)
			{
				this.saveLoadLevels[i].attributeId = this.levels[i].attribute.Attribute.Id;
				this.saveLoadLevels[i].experience = this.levels[i].experience;
				this.saveLoadLevels[i].level = this.levels[i].level;
			}
		}

		[OnDeserialized]
		public void OnDeserialized()
		{
			foreach (AttributeLevels.LevelSaveLoad levelSaveLoad in this.saveLoadLevels)
			{
				this.SetExperience(levelSaveLoad.attributeId, levelSaveLoad.experience);
				this.SetLevel(levelSaveLoad.attributeId, levelSaveLoad.level);
			}
		}

		public int GetLevel(Attribute attribute)
		{
			foreach (AttributeLevel attributeLevel in this.levels)
			{
				if (attribute == attributeLevel.attribute.Attribute)
				{
					return attributeLevel.GetLevel();
				}
			}
			return 1;
		}

		public AttributeLevel GetAttributeLevel(string attribute_id)
		{
			foreach (AttributeLevel attributeLevel in this.levels)
			{
				if (attributeLevel.attribute.Attribute.Id == attribute_id)
				{
					return attributeLevel;
				}
			}
			return null;
		}

		public bool AddExperience(string attribute_id, float time_spent, float multiplier)
		{
			AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
			if (attributeLevel == null)
			{
				Debug.LogWarning(attribute_id + " has no level.", null);
				return false;
			}
			time_spent *= multiplier;
			AttributeConverterInstance attributeConverterInstance = Db.Get().AttributeConverters.TrainingSpeed.Lookup(this);
			if (attributeConverterInstance != null)
			{
				float num = attributeConverterInstance.Evaluate();
				time_spent += time_spent * num;
			}
			bool flag = attributeLevel.AddExperience(this, time_spent);
			attributeLevel.Apply(this);
			return flag;
		}

		public void SetLevel(string attribute_id, int level)
		{
			AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
			if (attributeLevel != null)
			{
				attributeLevel.SetLevel(level);
				attributeLevel.Apply(this);
			}
		}

		public void SetExperience(string attribute_id, float experience)
		{
			AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
			if (attributeLevel != null)
			{
				attributeLevel.SetExperience(experience);
				attributeLevel.Apply(this);
			}
		}

		public float GetPercentComplete(string attribute_id)
		{
			AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
			return attributeLevel.GetPercentComplete();
		}

		public int GetMaxLevel()
		{
			int num = 0;
			foreach (AttributeLevel attributeLevel in this)
			{
				if (attributeLevel.GetLevel() > num)
				{
					num = attributeLevel.GetLevel();
				}
			}
			return num;
		}

		private List<AttributeLevel> levels = new List<AttributeLevel>();

		[Serialize]
		private AttributeLevels.LevelSaveLoad[] saveLoadLevels = new AttributeLevels.LevelSaveLoad[0];

		[Serializable]
		public struct LevelSaveLoad
		{
			public string attributeId;

			public float experience;

			public int level;
		}
	}
}
