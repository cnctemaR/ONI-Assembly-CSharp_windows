using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Tutorial : KMonoBehaviour
{
	public static Tutorial Instance { get; private set; }

	private void UpdateNotifierPosition()
	{
		if (this.notifierPosition == Vector3.zero)
		{
			GameObject telepad = GameUtil.GetTelepad();
			if (telepad != null)
			{
				this.notifierPosition = telepad.transform.position;
			}
		}
		this.notifier.transform.position = this.notifierPosition;
	}

	protected override void OnPrefabInit()
	{
		Tutorial.Instance = this;
	}

	protected override void OnSpawn()
	{
		if (this.tutorialMessagesRemaining.Count == 0)
		{
			for (int i = 0; i <= 15; i++)
			{
				this.tutorialMessagesRemaining.Add((Tutorial.TutorialMessages)i);
			}
		}
		List<Tutorial.Item> list = new List<Tutorial.Item>();
		List<Tutorial.Item> list2 = list;
		Tutorial.Item item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NEEDTOILET.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, true, 5f, null, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.ToiletExists);
		list2.Add(item);
		this.itemTree.Add(list);
		List<Tutorial.Item> list3 = new List<Tutorial.Item>();
		List<Tutorial.Item> list4 = list3;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NEEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDFOOD.TOOLTIP.text, null, true, 20f, null, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.FoodSourceExists);
		list4.Add(item);
		List<Tutorial.Item> list5 = list3;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP.text, null, true, 0f, null, null, null);
		list5.Add(item);
		this.itemTree.Add(list3);
		List<Tutorial.Item> list6 = new List<Tutorial.Item>();
		List<Tutorial.Item> list7 = list6;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.HYGENE_NEEDED.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.HYGENE_NEEDED.TOOLTIP, null, true, 20f, null, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.HygeneExists);
		list7.Add(item);
		this.itemTree.Add(list6);
		List<Tutorial.Item> list8 = this.warningItems;
		item = new Tutorial.Item();
		Tutorial.Item item2 = item;
		string text = MISC.NOTIFICATIONS.NEEDOXYGENSOURCE.NAME;
		HashedString invalid = HashedString.Invalid;
		item2.notification = new Notification(text, NotificationType.Tutorial, invalid, new Func<List<Notification>, object, string>(this.OnOxygenTooltip), null, false, 0f, null, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.SufficientOxygen);
		item.minTimeToNotify = 80f;
		item.lastNotifyTime = 0f;
		list8.Add(item);
		this.warningItems.Add(new Tutorial.Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, new Func<List<Notification>, object, string>(this.UnrefrigeratedFoodTooltip), null, false, 0f, null, null, null),
			requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.FoodIsRefrigerated),
			minTimeToNotify = 6f,
			lastNotifyTime = 0f
		});
		this.warningItems.Add(new Tutorial.Item
		{
			notification = new Notification(MISC.NOTIFICATIONS.FOODLOW.NAME, NotificationType.Bad, HashedString.Invalid, new Func<List<Notification>, object, string>(this.OnLowFoodTooltip), null, false, 0f, null, null, null),
			requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.EnoughFood),
			minTimeToNotify = 10f,
			lastNotifyTime = 0f
		});
		List<Tutorial.Item> list9 = this.warningItems;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NO_MEDICAL_COTS.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> n, object o) => MISC.NOTIFICATIONS.NO_MEDICAL_COTS.TOOLTIP, null, false, 0f, null, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.EnoughMedicalCots);
		item.minTimeToNotify = 10f;
		item.lastNotifyTime = 0f;
		list9.Add(item);
	}

	public void TutorialMessage(Tutorial.TutorialMessages tm)
	{
		if (!this.tutorialMessagesRemaining.Contains(tm))
		{
			return;
		}
		Message message = null;
		switch (tm)
		{
		case Tutorial.TutorialMessages.TM_Basics:
			message = new GenericMessage(MISC.NOTIFICATIONS.BASICCONTROLS.NAME, MISC.NOTIFICATIONS.BASICCONTROLS.MESSAGEBODY, MISC.NOTIFICATIONS.BASICCONTROLS.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Welcome:
			message = new GenericMessage(MISC.NOTIFICATIONS.WELCOMEMESSAGE.NAME, MISC.NOTIFICATIONS.WELCOMEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.WELCOMEMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_StressManagement:
			message = new GenericMessage(MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.NAME, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_StorageRegions:
			message = new GenericMessage(MISC.NOTIFICATIONS.STORAGEREGIONSMESSAGE.NAME, MISC.NOTIFICATIONS.STORAGEREGIONSMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.STORAGEREGIONSMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Scheduling:
			message = new GenericMessage(MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Mopping:
			message = new GenericMessage(MISC.NOTIFICATIONS.MOPPINGMESSAGE.NAME, MISC.NOTIFICATIONS.MOPPINGMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.MOPPINGMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Locomotion:
			message = new GenericMessage(MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.NAME, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Priorities:
			message = new GenericMessage(MISC.NOTIFICATIONS.PRIORITIESMESSAGE.NAME, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_FetchingWater:
			message = new GenericMessage(MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.NAME, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_ThermalComfort:
			message = new GenericMessage(MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, MISC.NOTIFICATIONS.THERMALCOMFORT.MESSAGEBODY, MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_OverheatingBuildings:
			message = new GenericMessage(MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.NAME, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.MESSAGEBODY, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_LotsOfGerms:
			message = new GenericMessage(MISC.NOTIFICATIONS.LOTS_OF_GERMS.NAME, MISC.NOTIFICATIONS.LOTS_OF_GERMS.MESSAGEBODY, MISC.NOTIFICATIONS.LOTS_OF_GERMS.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_BeingInfected:
			message = new GenericMessage(MISC.NOTIFICATIONS.BEING_INFECTED.NAME, MISC.NOTIFICATIONS.BEING_INFECTED.MESSAGEBODY, MISC.NOTIFICATIONS.BEING_INFECTED.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_DiseaseCooking:
			message = new GenericMessage(MISC.NOTIFICATIONS.DISEASE_COOKING.NAME, MISC.NOTIFICATIONS.DISEASE_COOKING.MESSAGEBODY, MISC.NOTIFICATIONS.DISEASE_COOKING.TOOLTIP);
			break;
		case Tutorial.TutorialMessages.TM_Suits:
			message = new GenericMessage(MISC.NOTIFICATIONS.SUITS.NAME, MISC.NOTIFICATIONS.SUITS.MESSAGEBODY, MISC.NOTIFICATIONS.SUITS.TOOLTIP);
			break;
		}
		this.tutorialMessagesRemaining.Remove(tm);
		Messenger.Instance.QueueMessage(message);
	}

	private string OnOxygenTooltip(List<Notification> notifications, object data)
	{
		ReportManager.ReportEntry entry = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		string text = MISC.NOTIFICATIONS.NEEDOXYGENSOURCE.TOOLTIP;
		text = text.Replace("{EmittingRate}", GameUtil.GetFormattedMass(entry.Positive, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		return text.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(Mathf.Abs(entry.Negative), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string UnrefrigeratedFoodTooltip(List<Notification> notifications, object data)
	{
		string text = MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.TOOLTIP;
		List<string> list = new List<string>();
		this.GetUnrefrigeratedFood(list);
		for (int i = 0; i < list.Count; i++)
		{
			text = text + "\n" + list[i];
		}
		return text;
	}

	private string OnLowFoodTooltip(List<Notification> notifications, object data)
	{
		float num = RationTracker.Get().CountRations(null, true);
		float num2 = (float)Components.LiveMinionIdentities.Count * -1000000f;
		return string.Format(MISC.NOTIFICATIONS.FOODLOW.TOOLTIP, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(Mathf.Abs(num2), GameUtil.TimeSlice.None, true));
	}

	public void DebugNotification()
	{
		string text = string.Empty;
		NotificationType notificationType;
		if (this.debugMessageCount % 3 == 0)
		{
			notificationType = NotificationType.Tutorial;
			text = "Warning message e.g. \"not enough oxygen\" uses Warning Color";
		}
		else if (this.debugMessageCount % 3 == 1)
		{
			notificationType = NotificationType.BadMinor;
			text = "Normal message e.g. Idle. Uses Normal Color BG";
		}
		else
		{
			notificationType = NotificationType.Bad;
			text = "Urgent important message. Uses Bad Color BG";
		}
		Notification notification = new Notification(string.Format("{0} ({1})", text, this.debugMessageCount++.ToString()), notificationType, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, true, 0f, null, null, null);
		this.notifier.Add(notification, string.Empty);
	}

	public void DebugNotificationMessage()
	{
		Message message = new GenericMessage("This is a message notification. " + this.debugMessageCount++.ToString(), MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP);
		Messenger.Instance.QueueMessage(message);
	}

	private void Update()
	{
		if (App.isLoading)
		{
			return;
		}
		if (Components.LiveMinionIdentities.Count == 0)
		{
			return;
		}
		if (this.itemTree.Count > 0)
		{
			List<Tutorial.Item> list = this.itemTree[0];
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Tutorial.Item item = list[i];
				if (item != null)
				{
					if (item.requirementSatisfied == null || item.requirementSatisfied())
					{
						item.notification.Clear();
						list.RemoveAt(i);
					}
					else
					{
						this.UpdateNotifierPosition();
						this.notifier.Add(item.notification, string.Empty);
					}
				}
			}
			if (list.Count == 0)
			{
				this.itemTree.RemoveAt(0);
			}
		}
		foreach (Tutorial.Item item2 in this.warningItems)
		{
			if (item2.requirementSatisfied())
			{
				item2.notification.Clear();
				item2.lastNotifyTime = Time.time;
			}
			else if (item2.lastNotifyTime == 0f || Time.time - item2.lastNotifyTime > item2.minTimeToNotify)
			{
				this.notifier.Add(item2.notification, string.Empty);
				item2.lastNotifyTime = Time.time;
			}
		}
		if (GameClock.Instance.GetDay() > 0 && !this.tutorialMessagesRemaining.Contains(Tutorial.TutorialMessages.TM_Priorities) && !this.queuedPrioritiesMessage)
		{
			this.queuedPrioritiesMessage = true;
			GameScheduler.Instance.Schedule("PrioritiesTutorial", 2f, delegate(object obj)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Priorities);
			}, null, null);
		}
	}

	private bool SufficientOxygen()
	{
		ReportManager.ReportEntry entry = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		return entry == null || entry.Net > 0.0001f || (GameClock.Instance.GetDay() < 1 && !GameClock.Instance.IsNighttime());
	}

	private bool FoodIsRefrigerated()
	{
		return this.GetUnrefrigeratedFood(null) <= 0;
	}

	private int GetUnrefrigeratedFood(List<string> foods)
	{
		int num = 0;
		if (WorldInventory.Instance != null)
		{
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
			if (pickupables == null)
			{
				return 0;
			}
			for (int i = 0; i < pickupables.Count; i++)
			{
				if (pickupables[i].storage != null && (pickupables[i].storage.GetComponent<RationBox>() != null || pickupables[i].storage.GetComponent<Refrigerator>() != null) && !Rottable.IsRefrigerated(pickupables[i].gameObject) && Rottable.AtmosphereQuality(pickupables[i].gameObject) != Rottable.RotAtmosphereQuality.Sterilizing)
				{
					Rottable.Instance smi = pickupables[i].GetSMI<Rottable.Instance>();
					if (smi != null && smi.RotConstitutionPercentage < 0.8f)
					{
						num++;
						if (foods != null)
						{
							foods.Add(pickupables[i].GetProperName());
						}
					}
				}
			}
		}
		return num;
	}

	private bool EnergySourceExists()
	{
		return Game.Instance.circuitManager.HasGenerators();
	}

	private bool BedExists()
	{
		return Components.Sleepables.Count > 0;
	}

	private bool EnoughFood()
	{
		float num = RationTracker.Get().CountRations(null, true);
		float num2 = (float)Components.LiveMinionIdentities.Count * 1000000f;
		return num / num2 > 1f;
	}

	private bool EnoughMedicalCots()
	{
		int count = Components.Clinics.Count;
		int num = 0;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Diseases diseases = Components.LiveMinionIdentities[i].GetDiseases();
			if (diseases.Count > 0)
			{
				num++;
			}
		}
		return count >= num;
	}

	private bool FoodSourceExists()
	{
		foreach (Fabricator fabricator in Components.Fabricators)
		{
			if (fabricator.GetType() == typeof(MicrobeMusher))
			{
				return true;
			}
		}
		return Components.PlantablePlots.Count > 0;
	}

	private bool HygeneExists()
	{
		return Components.HandSanitizers.Count > 0;
	}

	private bool ToiletExists()
	{
		return Components.Toilets.Count > 0;
	}

	[MyCmpAdd]
	private Notifier notifier;

	[Serialize]
	private SerializedList<Tutorial.TutorialMessages> tutorialMessagesRemaining = new SerializedList<Tutorial.TutorialMessages>();

	private int debugMessageCount;

	private bool queuedPrioritiesMessage;

	private const float LOW_RATION_AMOUNT = 1f;

	private List<List<Tutorial.Item>> itemTree = new List<List<Tutorial.Item>>();

	private List<Tutorial.Item> warningItems = new List<Tutorial.Item>();

	private Vector3 notifierPosition;

	public enum TutorialMessages
	{
		TM_Basics,
		TM_Welcome,
		TM_StressManagement,
		TM_StorageRegions,
		TM_Scheduling,
		TM_Mopping,
		TM_Locomotion,
		TM_Priorities,
		TM_FetchingWater,
		TM_ThermalComfort,
		TM_OverheatingBuildings,
		TM_LotsOfGerms,
		TM_BeingInfected,
		TM_DiseaseCooking,
		TM_Suits,
		TM_COUNT
	}

	private delegate bool RequirementSatisfiedDelegate();

	private class Item
	{
		public Notification notification;

		public Tutorial.RequirementSatisfiedDelegate requirementSatisfied;

		public float minTimeToNotify;

		public float lastNotifyTime;
	}
}
