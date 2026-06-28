using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
	public class Needs : KMonoBehaviour
	{
		protected override void OnSpawn()
		{
			base.Subscribe(-110704193, new Action<object>(this.OnLevelUp));
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			base.Unsubscribe(-110704193, new Action<object>(this.OnLevelUp));
		}

		private void OnLevelUp(object data)
		{
			Attribute attribute = Db.Get().Attributes.Get(data as string);
			if (base.gameObject.GetAttributes().GetProfession().Id == attribute.Id)
			{
				float skillLevel = base.gameObject.GetAttributes().GetProfession().GetSkillLevel();
				if (skillLevel % 5f != 0f || skillLevel < 16f)
				{
				}
			}
		}

		public void GetNewNeed()
		{
			string text = string.Empty;
			int num = 0;
			while (text == string.Empty && num < 100)
			{
				num++;
				int count = DUPLICANTSTATS.NEEDTRAITS.Count;
				int num2 = global::UnityEngine.Random.Range(0, count);
				text = DUPLICANTSTATS.NEEDTRAITS[num2].id;
				if (this.traits.HasTrait(text))
				{
					text = string.Empty;
				}
				else if (DUPLICANTSTATS.NEEDTRAITS[num2].mutuallyExclusiveTraits != null)
				{
					foreach (string text2 in DUPLICANTSTATS.NEEDTRAITS[num2].mutuallyExclusiveTraits)
					{
						if (this.traits.HasTrait(text2))
						{
							text = string.Empty;
							break;
						}
					}
				}
			}
			if (text != string.Empty)
			{
				Trait trait = Db.Get().traits.TryGet(text);
				if (trait == null)
				{
					global::Debug.LogWarning("Trait " + text + " was not found in database", null);
					return;
				}
				this.traits.Add(trait);
				this.notifier.Add(this.CreateNewNeedNotification(trait), string.Empty);
			}
		}

		public Notification CreateNewNeedNotification(Trait t)
		{
			string text = string.Format(MISC.NOTIFICATIONS.NEWTRAIT.NAME, base.gameObject.GetComponent<MinionIdentity>().GetProperName());
			NotificationType notificationType = NotificationType.Neutral;
			HashedString invalid = HashedString.Invalid;
			Func<List<Notification>, object, string> func = new Func<List<Notification>, object, string>(this.ToolTipResolver);
			bool flag = false;
			Notification.ClickCallback clickCallback = new Notification.ClickCallback(this.ClearNotifications);
			Notification notification = new Notification(text, notificationType, invalid, func, t, flag, 0f, clickCallback, null, null);
			this.notifications.Add(notification);
			return notification;
		}

		private string ToolTipResolver(List<Notification> notificationList, object data)
		{
			string text = string.Empty;
			for (int i = 0; i < notificationList.Count; i++)
			{
				Notification notification = notificationList[i];
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					((Trait)notification.tooltipData).Name,
					"\n",
					((Trait)notification.tooltipData).description,
					"\n"
				});
				if (i < notificationList.Count - 1)
				{
					text += "\n";
				}
			}
			return string.Format(MISC.NOTIFICATIONS.NEWTRAIT.TOOLTIP, base.gameObject.GetComponent<MinionIdentity>().GetProperName(), text);
		}

		private void ClearNotifications(object data)
		{
			foreach (Notification notification in this.notifications)
			{
				this.notifier.Remove(notification);
			}
		}

		[MyCmpGet]
		public Traits traits;

		[MyCmpGet]
		public Notifier notifier;

		private List<Notification> notifications = new List<Notification>();
	}
}
