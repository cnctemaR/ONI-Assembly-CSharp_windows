using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeLevel
	{
		public AttributeLevel(AttributeInstance attribute)
		{
			Func<List<Notification>, object, string> func = new Func<List<Notification>, object, string>(AttributeLevel.OnLevelUpTooltip);
			this.notification = new Notification(MISC.NOTIFICATIONS.LEVELUP.NAME, NotificationType.Good, null, func, null, true, 0f, null, null, null);
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
			this.modifier = new AttributeModifier(this.attribute.Id, (float)this.GetLevel(), DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME, false);
			attributes.Add("Skill Level", this.modifier);
		}

		public void SetExperience(float experience)
		{
			this.experience = experience;
		}

		public void SetLevel(int level)
		{
			this.level = level;
		}

		private float GetExperienceForNextLevel()
		{
			return 150f * ((float)this.level + 1f);
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
			ReportManager.Instance.ReportValue(ReportManager.ReportType.LevelUp, 1f, null);
			instance.StartSM();
			levels.Trigger(-110704193, null);
		}

		public bool AddExperience(AttributeLevels levels, float experience)
		{
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
	}
}
