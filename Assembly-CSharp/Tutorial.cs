using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Tutorial : KMonoBehaviour, IRender1000ms
{
	public static void ResetHiddenTutorialMessages()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(Tutorial.TutorialMessages)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Tutorial.TutorialMessages tutorialMessages = (Tutorial.TutorialMessages)obj;
				string text = "HideTutorial_" + tutorialMessages.ToString();
				KPlayerPrefs.SetInt(text, 0);
				if (Tutorial.Instance != null)
				{
					Tutorial.Instance.hiddenTutorialMessages[tutorialMessages] = false;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		KPlayerPrefs.SetInt("HideTutorial_CheckState", 0);
	}

	private void LoadHiddenTutorialMessages()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(Tutorial.TutorialMessages)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Tutorial.TutorialMessages tutorialMessages = (Tutorial.TutorialMessages)obj;
				string text = "HideTutorial_" + tutorialMessages.ToString();
				bool flag = KPlayerPrefs.GetInt(text, 0) != 0;
				this.hiddenTutorialMessages[tutorialMessages] = flag;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void HideTutorialMessage(Tutorial.TutorialMessages message)
	{
		this.hiddenTutorialMessages[message] = true;
		string text = "HideTutorial_" + message.ToString();
		KPlayerPrefs.SetInt(text, 1);
	}

	public static Tutorial Instance { get; private set; }

	public static void DestroyInstance()
	{
		Tutorial.Instance = null;
	}

	private void UpdateNotifierPosition()
	{
		if (this.notifierPosition == Vector3.zero)
		{
			GameObject telepad = GameUtil.GetTelepad();
			if (telepad != null)
			{
				this.notifierPosition = telepad.transform.GetPosition();
			}
		}
		this.notifier.transform.SetPosition(this.notifierPosition);
	}

	protected override void OnPrefabInit()
	{
		Tutorial.Instance = this;
		this.LoadHiddenTutorialMessages();
	}

	protected override void OnSpawn()
	{
		if (this.tutorialMessagesRemaining.Count == 0)
		{
			for (int i = 0; i <= 20; i++)
			{
				this.tutorialMessagesRemaining.Add((Tutorial.TutorialMessages)i);
			}
		}
		List<Tutorial.Item> list = new List<Tutorial.Item>();
		List<Tutorial.Item> list2 = list;
		Tutorial.Item item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NEEDTOILET.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDTOILET.TOOLTIP.text, null, true, 5f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Plumbing");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.ToiletExists);
		list2.Add(item);
		this.itemTree.Add(list);
		List<Tutorial.Item> list3 = new List<Tutorial.Item>();
		List<Tutorial.Item> list4 = list3;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NEEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NEEDFOOD.TOOLTIP.text, null, true, 20f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Food");
		}, null, null);
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
		item.notification = new Notification(MISC.NOTIFICATIONS.HYGENE_NEEDED.NAME, NotificationType.Tutorial, HashedString.Invalid, (List<Notification> n, object d) => MISC.NOTIFICATIONS.HYGENE_NEEDED.TOOLTIP, null, true, 20f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Medicine");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.HygeneExists);
		list7.Add(item);
		this.itemTree.Add(list6);
		List<Tutorial.Item> list8 = this.warningItems;
		item = new Tutorial.Item();
		Tutorial.Item item2 = item;
		string text = MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.NAME;
		HashedString hashedString = HashedString.Invalid;
		item2.notification = new Notification(text, NotificationType.Tutorial, hashedString, (List<Notification> n, object d) => MISC.NOTIFICATIONS.NO_OXYGEN_GENERATOR.TOOLTIP, null, false, 0f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Oxygen");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.OxygenGeneratorBuilt);
		item.minTimeToNotify = 80f;
		item.lastNotifyTime = 0f;
		list8.Add(item);
		List<Tutorial.Item> list9 = this.warningItems;
		item = new Tutorial.Item();
		Tutorial.Item item3 = item;
		text = MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.NAME;
		hashedString = HashedString.Invalid;
		item3.notification = new Notification(text, NotificationType.Tutorial, hashedString, new Func<List<Notification>, object, string>(this.OnOxygenTooltip), null, false, 0f, delegate(object d)
		{
			this.ZoomToNextOxygenGenerator();
		}, null, null);
		item.hideCondition = new Tutorial.HideConditionDelegate(this.OxygenGeneratorNotBuilt);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.SufficientOxygenLastCycleAndThisCycle);
		item.minTimeToNotify = 80f;
		item.lastNotifyTime = 0f;
		list9.Add(item);
		List<Tutorial.Item> list10 = this.warningItems;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME, NotificationType.Tutorial, HashedString.Invalid, new Func<List<Notification>, object, string>(this.UnrefrigeratedFoodTooltip), null, false, 0f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Food");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.FoodIsRefrigerated);
		item.minTimeToNotify = 6f;
		item.lastNotifyTime = 0f;
		list10.Add(item);
		List<Tutorial.Item> list11 = this.warningItems;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.FOODLOW.NAME, NotificationType.Bad, HashedString.Invalid, new Func<List<Notification>, object, string>(this.OnLowFoodTooltip), null, false, 0f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Food");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.EnoughFood);
		item.minTimeToNotify = 10f;
		item.lastNotifyTime = 0f;
		list11.Add(item);
		List<Tutorial.Item> list12 = this.warningItems;
		item = new Tutorial.Item();
		item.notification = new Notification(MISC.NOTIFICATIONS.NO_MEDICAL_COTS.NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> n, object o) => MISC.NOTIFICATIONS.NO_MEDICAL_COTS.TOOLTIP, null, false, 0f, delegate(object d)
		{
			PlanScreen.Instance.OpenCategoryByName("Medicine");
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.CanTreatSickDuplicant);
		item.minTimeToNotify = 10f;
		item.lastNotifyTime = 0f;
		list12.Add(item);
		List<Tutorial.Item> list13 = this.warningItems;
		item = new Tutorial.Item();
		item.notification = new Notification(string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_TITLE, new object[0]), NotificationType.BadMinor, HashedString.Invalid, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.TRAVELTIMEWARNING.WARNING_MESSAGE, GameUtil.GetFormattedPercent(40f, GameUtil.TimeSlice.None)), null, true, 0f, delegate(object d)
		{
			ManagementMenu.Instance.OpenReports(GameClock.Instance.GetCycle());
		}, null, null);
		item.requirementSatisfied = new Tutorial.RequirementSatisfiedDelegate(this.LongTravelTimes);
		item.minTimeToNotify = 1f;
		item.lastNotifyTime = 0f;
		list13.Add(item);
	}

	public Message TutorialMessage(Tutorial.TutorialMessages tm, bool queueMessage = true)
	{
		Message message = null;
		switch (tm)
		{
		case Tutorial.TutorialMessages.TM_Basics:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Basics, MISC.NOTIFICATIONS.BASICCONTROLS.NAME, MISC.NOTIFICATIONS.BASICCONTROLS.MESSAGEBODY, MISC.NOTIFICATIONS.BASICCONTROLS.TOOLTIP, null, null, null, string.Empty);
			break;
		case Tutorial.TutorialMessages.TM_Welcome:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Welcome, MISC.NOTIFICATIONS.WELCOMEMESSAGE.NAME, MISC.NOTIFICATIONS.WELCOMEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.WELCOMEMESSAGE.TOOLTIP, null, null, null, string.Empty);
			break;
		case Tutorial.TutorialMessages.TM_StressManagement:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_StressManagement, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.NAME, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.STRESSMANAGEMENTMESSAGE.TOOLTIP, null, null, null, "hud_stress");
			break;
		case Tutorial.TutorialMessages.TM_Scheduling:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Scheduling, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP, null, null, null, "OverviewUI_schedule2_icon");
			break;
		case Tutorial.TutorialMessages.TM_Mopping:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Mopping, MISC.NOTIFICATIONS.MOPPINGMESSAGE.NAME, MISC.NOTIFICATIONS.MOPPINGMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.MOPPINGMESSAGE.TOOLTIP, null, null, null, "icon_action_mop");
			break;
		case Tutorial.TutorialMessages.TM_Locomotion:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.NAME, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.LOCOMOTIONMESSAGE.TOOLTIP, "tutorials\\Locomotion", "Tute_Locomotion", VIDEOS.LOCOMOTION, "action_navigable_regions");
			break;
		case Tutorial.TutorialMessages.TM_Priorities:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Priorities, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.NAME, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.PRIORITIESMESSAGE.TOOLTIP, null, null, null, "icon_action_prioritize");
			break;
		case Tutorial.TutorialMessages.TM_FetchingWater:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.NAME, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.FETCHINGWATERMESSAGE.TOOLTIP, null, null, null, "element_liquid");
			break;
		case Tutorial.TutorialMessages.TM_ThermalComfort:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort, MISC.NOTIFICATIONS.THERMALCOMFORT.NAME, MISC.NOTIFICATIONS.THERMALCOMFORT.MESSAGEBODY, MISC.NOTIFICATIONS.THERMALCOMFORT.TOOLTIP, null, null, null, "temperature");
			break;
		case Tutorial.TutorialMessages.TM_OverheatingBuildings:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_OverheatingBuildings, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.NAME, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.MESSAGEBODY, MISC.NOTIFICATIONS.TUTORIAL_OVERHEATING.TOOLTIP, null, null, null, "temperature");
			break;
		case Tutorial.TutorialMessages.TM_LotsOfGerms:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms, MISC.NOTIFICATIONS.LOTS_OF_GERMS.NAME, MISC.NOTIFICATIONS.LOTS_OF_GERMS.MESSAGEBODY, MISC.NOTIFICATIONS.LOTS_OF_GERMS.TOOLTIP, null, null, null, "overlay_disease");
			break;
		case Tutorial.TutorialMessages.TM_BeingInfected:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_BeingInfected, MISC.NOTIFICATIONS.BEING_INFECTED.NAME, MISC.NOTIFICATIONS.BEING_INFECTED.MESSAGEBODY, MISC.NOTIFICATIONS.BEING_INFECTED.TOOLTIP, null, null, null, "overlay_disease");
			break;
		case Tutorial.TutorialMessages.TM_DiseaseCooking:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking, MISC.NOTIFICATIONS.DISEASE_COOKING.NAME, MISC.NOTIFICATIONS.DISEASE_COOKING.MESSAGEBODY, MISC.NOTIFICATIONS.DISEASE_COOKING.TOOLTIP, null, null, null, "icon_category_food");
			break;
		case Tutorial.TutorialMessages.TM_Suits:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Suits, MISC.NOTIFICATIONS.SUITS.NAME, MISC.NOTIFICATIONS.SUITS.MESSAGEBODY, MISC.NOTIFICATIONS.SUITS.TOOLTIP, null, null, null, "overlay_suit");
			break;
		case Tutorial.TutorialMessages.TM_Morale:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Morale, MISC.NOTIFICATIONS.MORALE.NAME, MISC.NOTIFICATIONS.MORALE.MESSAGEBODY, MISC.NOTIFICATIONS.MORALE.TOOLTIP, "tutorials\\Morale", "Tute_Morale", VIDEOS.MORALE, "icon_category_morale");
			break;
		case Tutorial.TutorialMessages.TM_Schedule:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.NAME, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.MESSAGEBODY, MISC.NOTIFICATIONS.SCHEDULEMESSAGE.TOOLTIP, null, null, null, "OverviewUI_schedule2_icon");
			break;
		case Tutorial.TutorialMessages.TM_Digging:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Digging, MISC.NOTIFICATIONS.DIGGING.NAME, MISC.NOTIFICATIONS.DIGGING.MESSAGEBODY, MISC.NOTIFICATIONS.DIGGING.TOOLTIP, "tutorials\\Digging", "Tute_Digging", VIDEOS.DIGGING, "icon_action_dig");
			break;
		case Tutorial.TutorialMessages.TM_Power:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Power, MISC.NOTIFICATIONS.POWER.NAME, MISC.NOTIFICATIONS.POWER.MESSAGEBODY, MISC.NOTIFICATIONS.POWER.TOOLTIP, "tutorials\\Power", "Tute_Power", VIDEOS.POWER, "overlay_power");
			break;
		case Tutorial.TutorialMessages.TM_Insulation:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Insulation, MISC.NOTIFICATIONS.INSULATION.NAME, MISC.NOTIFICATIONS.INSULATION.MESSAGEBODY, MISC.NOTIFICATIONS.INSULATION.TOOLTIP, null, null, null, string.Empty);
			break;
		case Tutorial.TutorialMessages.TM_Plumbing:
			message = new TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing, MISC.NOTIFICATIONS.PLUMBING.NAME, MISC.NOTIFICATIONS.PLUMBING.MESSAGEBODY, MISC.NOTIFICATIONS.PLUMBING.TOOLTIP, "tutorials\\Piping", "Tute_Plumbing", VIDEOS.PLUMBING, "icon_category_plumbing");
			break;
		}
		global::Debug.Assert(message != null, string.Format("No Tutorial message: {0}", tm.ToString()));
		if (queueMessage)
		{
			if (!this.tutorialMessagesRemaining.Contains(tm))
			{
				return null;
			}
			if (this.hiddenTutorialMessages.ContainsKey(tm) && this.hiddenTutorialMessages[tm])
			{
				return null;
			}
			this.tutorialMessagesRemaining.Remove(tm);
			Messenger.Instance.QueueMessage(message);
		}
		return message;
	}

	private string OnOxygenTooltip(List<Notification> notifications, object data)
	{
		ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		string text = MISC.NOTIFICATIONS.INSUFFICIENTOXYGENLASTCYCLE.TOOLTIP;
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

	public void Render1000ms(float dt)
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
					else if (item.hideCondition != null && item.hideCondition())
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
			else if (item2.hideCondition != null && item2.hideCondition())
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
		if (GameClock.Instance.GetCycle() > 0 && !this.tutorialMessagesRemaining.Contains(Tutorial.TutorialMessages.TM_Priorities) && !this.queuedPrioritiesMessage)
		{
			this.queuedPrioritiesMessage = true;
			GameScheduler.Instance.Schedule("PrioritiesTutorial", 2f, delegate(object obj)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Priorities, true);
			}, null, null);
		}
	}

	private bool OxygenGeneratorBuilt()
	{
		return this.oxygenGenerators.Count > 0;
	}

	private bool OxygenGeneratorNotBuilt()
	{
		return this.oxygenGenerators.Count == 0;
	}

	private bool SufficientOxygenLastCycleAndThisCycle()
	{
		if (ReportManager.Instance.YesterdaysReport == null)
		{
			return true;
		}
		ReportManager.ReportEntry entry = ReportManager.Instance.YesterdaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		ReportManager.ReportEntry entry2 = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.OxygenCreated);
		return entry2.Net > 0.0001f || entry.Net > 0.0001f || (GameClock.Instance.GetCycle() < 1 && !GameClock.Instance.IsNighttime());
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

	private bool CanTreatSickDuplicant()
	{
		bool flag = Components.Clinics.Count >= 1;
		bool flag2 = false;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Sicknesses sicknesses = Components.LiveMinionIdentities[i].GetSicknesses();
			foreach (SicknessInstance sicknessInstance in sicknesses)
			{
				if (sicknessInstance.Sickness.severity >= Sickness.Severity.Major)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				break;
			}
		}
		return !flag2 || flag;
	}

	private bool LongTravelTimes()
	{
		int num = 3;
		if (ReportManager.Instance.reports.Count < num)
		{
			return true;
		}
		float num2 = 0f;
		float num3 = 0f;
		for (int i = ReportManager.Instance.reports.Count - 1; i >= ReportManager.Instance.reports.Count - num; i--)
		{
			ReportManager.ReportEntry entry = ReportManager.Instance.reports[i].GetEntry(ReportManager.ReportType.TravelTime);
			num2 += entry.Net;
			num3 += 600f * (float)entry.contextEntries.Count;
		}
		float num4 = num2 / num3;
		return num4 <= 0.4f;
	}

	private bool FoodSourceExists()
	{
		foreach (ComplexFabricator complexFabricator in Components.ComplexFabricators.Items)
		{
			if (complexFabricator.GetType() == typeof(MicrobeMusher))
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

	private void ZoomToNextOxygenGenerator()
	{
		if (this.oxygenGenerators.Count == 0)
		{
			return;
		}
		this.focusedOxygenGenerator %= this.oxygenGenerators.Count;
		GameObject gameObject = this.oxygenGenerators[this.focusedOxygenGenerator];
		if (gameObject != null)
		{
			Vector3 position = gameObject.transform.position;
			CameraController.Instance.SetTargetPos(position, 8f, true);
		}
		else
		{
			DebugUtil.DevLogErrorFormat("ZoomToNextOxygenGenerator generator was null: {0}", new object[] { gameObject });
		}
		this.focusedOxygenGenerator++;
	}

	[MyCmpAdd]
	private Notifier notifier;

	[Serialize]
	private SerializedList<Tutorial.TutorialMessages> tutorialMessagesRemaining = new SerializedList<Tutorial.TutorialMessages>();

	private const string HIDDEN_TUTORIAL_PREF_KEY_PREFIX = "HideTutorial_";

	public const string HIDDEN_TUTORIAL_PREF_BUTTON_KEY = "HideTutorial_CheckState";

	private Dictionary<Tutorial.TutorialMessages, bool> hiddenTutorialMessages = new Dictionary<Tutorial.TutorialMessages, bool>();

	private int debugMessageCount;

	private bool queuedPrioritiesMessage;

	private const float LOW_RATION_AMOUNT = 1f;

	private List<List<Tutorial.Item>> itemTree = new List<List<Tutorial.Item>>();

	private List<Tutorial.Item> warningItems = new List<Tutorial.Item>();

	private Vector3 notifierPosition;

	public List<GameObject> oxygenGenerators = new List<GameObject>();

	private int focusedOxygenGenerator;

	public enum TutorialMessages
	{
		TM_Basics,
		TM_Welcome,
		TM_StressManagement,
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
		TM_Morale,
		TM_Schedule,
		TM_Digging,
		TM_Power,
		TM_Insulation,
		TM_Plumbing,
		TM_COUNT
	}

	private delegate bool HideConditionDelegate();

	private delegate bool RequirementSatisfiedDelegate();

	private class Item
	{
		public Notification notification;

		public Tutorial.HideConditionDelegate hideCondition;

		public Tutorial.RequirementSatisfiedDelegate requirementSatisfied;

		public float minTimeToNotify;

		public float lastNotifyTime;
	}
}
