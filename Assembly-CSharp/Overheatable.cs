using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Overheatable : StateMachineComponent<Overheatable.StatesInstance>, IGameObjectEffectDescriptor, IEffectDescriptor
{
	public float temperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	public void ResetTemperature()
	{
		this.primaryElement.Temperature = 293.15f;
	}

	public bool IsOverheated()
	{
		return this.temperature > this.overheatTemp.GetTotalValue();
	}

	public bool IsFatalHot()
	{
		return this.temperature > this.fatalTemp.GetTotalValue();
	}

	public void OverheatDamage()
	{
		base.smi.master.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = 1,
			source = global::STRINGS.BUILDINGS.DAMAGESOURCES.BUILDING_OVERHEATED,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.OVERHEAT
		});
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overheatTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.OverheatTemperature);
		this.fatalTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.FatalTemperature);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AttributeModifier attributeModifier = new AttributeModifier(this.overheatTemp.Id, this.baseOverheatTemp, UI.TOOLTIPS.BASE_VALUE, false, false);
		AttributeModifier attributeModifier2 = new AttributeModifier(this.fatalTemp.Id, this.baseFatalTemp, UI.TOOLTIPS.BASE_VALUE, false, false);
		this.GetAttributes().Add("Base", attributeModifier);
		this.GetAttributes().Add("Base", attributeModifier2);
		base.smi.StartSM();
	}

	public Notification CreateOverheatedNotification()
	{
		return new Notification(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.NAME, NotificationType.BadMinor, HashedString.Invalid, new Func<List<Notification>, object, string>(Overheatable.ToolTipResolver), this.selectable.GetProperName(), false, 0f, null, null, null);
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = string.Empty;
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP, text);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetDescriptors(def.BuildingComplete);
	}

	public static int GetLightDecorBonus(int cell)
	{
		if (Grid.LightCount[cell] > 0)
		{
			return DECOR.LIT_BONUS;
		}
		return 0;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.overheatTemp != null && this.fatalTemp != null)
		{
			string formattedValue = this.overheatTemp.GetFormattedValue(false);
			string formattedValue2 = this.fatalTemp.GetFormattedValue(false);
			string text = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			text = text + "\n\n" + this.overheatTemp.GetAttributeValueTooltip();
			Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedValue, formattedValue2), string.Format(text, formattedValue, formattedValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
		}
		else if (this.baseOverheatTemp != 0f)
		{
			string formattedTemperature = GameUtil.GetFormattedTemperature(this.baseOverheatTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
			string formattedTemperature2 = GameUtil.GetFormattedTemperature(this.baseFatalTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
			string text2 = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			Descriptor descriptor2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedTemperature, formattedTemperature2), string.Format(text2, formattedTemperature, formattedTemperature2), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor2);
		}
		return list;
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private OccupyArea occupyArea;

	private AttributeInstance overheatTemp;

	private AttributeInstance fatalTemp;

	public float baseOverheatTemp;

	public float baseFatalTemp;

	public class StatesInstance : GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.GameInstance
	{
		public StatesInstance(Overheatable smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.safeTemperature;
			this.root.EventTransition(GameHashes.BuildingBroken, this.invulnerable, null);
			this.invulnerable.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(Overheatable.StatesInstance smi)
			{
				smi.master.ResetTemperature();
			}).EventTransition(GameHashes.BuildingPartiallyRepaired, this.safeTemperature, null);
			this.safeTemperature.Transition(this.overheated, (Overheatable.StatesInstance smi) => smi.master.IsOverheated()).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null);
			this.overheated.Enter(delegate(Overheatable.StatesInstance smi)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_OverheatingBuildings);
			}).Transition(this.safeTemperature, (Overheatable.StatesInstance smi) => !smi.master.IsOverheated()).ToggleStatusItem(Db.Get().BuildingStatusItems.Overheated, null)
				.ToggleNotification((Overheatable.StatesInstance smi) => smi.master.CreateOverheatedNotification())
				.TriggerOnEnter(GameHashes.TooHotWarning, null)
				.ToggleSchedulePeriodic("OverheatDamage", 7.5f, delegate(Overheatable.StatesInstance smi)
				{
					smi.master.OverheatDamage();
				});
		}

		public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State invulnerable;

		public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State safeTemperature;

		public GameStateMachine<Overheatable.States, Overheatable.StatesInstance, Overheatable, object>.State overheated;
	}
}
