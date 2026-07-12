using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeLevel
	{
		public AttributeLevel(AttributeInstance attribute)
		{
			this.notification = new Notification(MISC.NOTIFICATIONS.LEVELUP.NAME, NotificationType.Good, new Func<List<Notification>, object, string>(AttributeLevel.OnLevelUpTooltip), null, true, 0f, null, null, null, true, false, false);
			this.attribute = attribute;
		}

		public int GetLevel()
		{
			return this.level;
		}

		public void Apply(AttributeLevels levels)
		{
			Attributes attributes = levels.GetAttributes();
			if (this.modifier != null)
			{
				attributes.Remove(this.modifier);
				this.modifier = null;
			}
			this.modifier = new AttributeModifier(this.attribute.Id, (float)this.GetLevel(), DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME, false, false, true);
			attributes.Add(this.modifier);
		}

		public void SetExperience(float experience)
		{
			this.experience = experience;
		}

		public void SetLevel(int level)
		{
			this.level = level;
		}

		public float GetExperienceForNextLevel()
		{
			float num = Mathf.Pow((float)this.level / (float)this.maxGainedLevel, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER) * (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600f;
			return Mathf.Pow(((float)this.level + 1f) / (float)this.maxGainedLevel, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER) * (float)DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600f - num;
		}

		public float GetPercentComplete()
		{
			return this.experience / this.GetExperienceForNextLevel();
		}

		public void LevelUp(AttributeLevels levels)
		{
			this.level++;
			this.experience = 0f;
			this.Apply(levels);
			this.experience = 0f;
			if (PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, this.attribute.modifier.Name, levels.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
			}
			levels.GetComponent<Notifier>().Add(this.notification, string.Format(MISC.NOTIFICATIONS.LEVELUP.SUFFIX, this.attribute.modifier.Name, this.level));
			StateMachine.Instance instance = new UpgradeFX.Instance(levels.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
			ReportManager.Instance.ReportValue(ReportManager.ReportType.LevelUp, 1f, levels.GetProperName(), null);
			instance.StartSM();
			levels.Trigger(-110704193, this.attribute.Id);
		}

		public bool AddExperience(AttributeLevels levels, float experience)
		{
			if (this.level >= this.maxGainedLevel)
			{
				return false;
			}
			this.experience += experience;
			this.experience = Mathf.Max(0f, this.experience);
			if (this.experience >= this.GetExperienceForNextLevel())
			{
				this.LevelUp(levels);
				return true;
			}
			return false;
		}

		private static string OnLevelUpTooltip(List<Notification> notifications, object data)
		{
			return MISC.NOTIFICATIONS.LEVELUP.TOOLTIP + notifications.ReduceMessages(false);
		}

		public float experience;

		public int level;

		public AttributeInstance attribute;

		public AttributeModifier modifier;

		public Notification notification;

		public int maxGainedLevel = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL;
	}
}
