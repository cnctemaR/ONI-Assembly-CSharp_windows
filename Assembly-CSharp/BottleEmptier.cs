using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BottleEmptier : StateMachineComponent<BottleEmptier.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		this.DefineManualPumpingAffectedBuildings();
		base.Subscribe<BottleEmptier>(493375141, BottleEmptier.OnRefreshUserMenuDelegate);
		base.Subscribe<BottleEmptier>(-905833192, BottleEmptier.OnCopySettingsDelegate);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return null;
	}

	private void DefineManualPumpingAffectedBuildings()
	{
		if (BottleEmptier.manualPumpingAffectedBuildings.ContainsKey(this.isGasEmptier))
		{
			return;
		}
		List<string> list = new List<string>();
		Tag tag = (this.isGasEmptier ? GameTags.GasSource : GameTags.LiquidSource);
		foreach (BuildingDef buildingDef in Assets.BuildingDefs)
		{
			if (buildingDef.BuildingComplete.HasTag(tag))
			{
				list.Add(buildingDef.Name);
			}
		}
		BottleEmptier.manualPumpingAffectedBuildings.Add(this.isGasEmptier, list.ToArray());
	}

	private void OnChangeAllowManualPumpingStationFetching()
	{
		this.allowManualPumpingStationFetching = !this.allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		string text = (this.isGasEmptier ? UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.TOOLTIP : UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP);
		string text2 = (this.isGasEmptier ? UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.TOOLTIP : UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP);
		if (BottleEmptier.manualPumpingAffectedBuildings.ContainsKey(this.isGasEmptier))
		{
			foreach (string text3 in BottleEmptier.manualPumpingAffectedBuildings[this.isGasEmptier])
			{
				string text4 = string.Format(UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.ITEM, text3);
				text += text4;
				text2 += text4;
			}
		}
		if (this.isGasEmptier)
		{
			KIconButtonMenu.ButtonInfo buttonInfo = (this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.NAME, new global::System.Action(this.OnChangeAllowManualPumpingStationFetching), global::Action.NumActions, null, null, null, text2, true) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.NAME, new global::System.Action(this.OnChangeAllowManualPumpingStationFetching), global::Action.NumActions, null, null, null, text, true));
			Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 0.4f);
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = (this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new global::System.Action(this.OnChangeAllowManualPumpingStationFetching), global::Action.NumActions, null, null, null, text2, true) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new global::System.Action(this.OnChangeAllowManualPumpingStationFetching), global::Action.NumActions, null, null, null, text, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 0.4f);
	}

	private void OnCopySettings(object data)
	{
		BottleEmptier component = ((GameObject)data).GetComponent<BottleEmptier>();
		this.allowManualPumpingStationFetching = component.allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}

	public float emptyRate = 10f;

	[Serialize]
	public bool allowManualPumpingStationFetching;

	[Serialize]
	public bool emit = true;

	public bool isGasEmptier;

	private static Dictionary<bool, string[]> manualPumpingAffectedBuildings = new Dictionary<bool, string[]>();

	private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>(delegate(BottleEmptier component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>(delegate(BottleEmptier component, object data)
	{
		component.OnCopySettings(data);
	});

	public class StatesInstance : GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.GameInstance
	{
		public MeterController meter { get; private set; }

		public StatesInstance(BottleEmptier smi)
			: base(smi)
		{
			TreeFilterable component = base.master.GetComponent<TreeFilterable>();
			component.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(component.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_arrow", "meter_scale" });
			this.meter.meterController.GetComponent<KBatchedAnimTracker>().synchronizeEnabledState = false;
			this.meter.meterController.enabled = false;
			base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
			base.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		}

		public void CreateChore()
		{
			HashSet<Tag> tags = base.GetComponent<TreeFilterable>().GetTags();
			Tag[] array;
			if (!base.master.allowManualPumpingStationFetching)
			{
				array = new Tag[]
				{
					GameTags.LiquidSource,
					GameTags.GasSource
				};
			}
			else
			{
				array = new Tag[0];
			}
			Storage component = base.GetComponent<Storage>();
			this.chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component, component.Capacity(), tags, FetchChore.MatchCriteria.MatchID, Tag.Invalid, array, null, true, null, null, null, Operational.State.Operational, 0);
		}

		public void CancelChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Storage Changed");
				this.chore = null;
			}
		}

		public void RefreshChore()
		{
			this.GoTo(base.sm.unoperational);
		}

		private void OnFilterChanged(HashSet<Tag> tags)
		{
			this.RefreshChore();
		}

		private void OnStorageChange(object data)
		{
			this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.RemainingCapacity() / this.storage.capacityKg));
			this.meter.meterController.enabled = this.storage.MassStored() > 0f;
		}

		private void OnOnlyFetchMarkedItemsSettingChanged(object data)
		{
			this.RefreshChore();
		}

		public void StartMeter()
		{
			PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
			if (firstPrimaryElement == null)
			{
				return;
			}
			base.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
			this.meter.meterController.SwapAnims(firstPrimaryElement.Element.substance.anims);
			this.meter.meterController.Play("empty", KAnim.PlayMode.Paused, 1f, 0f);
			Color32 colour = firstPrimaryElement.Element.substance.colour;
			colour.a = byte.MaxValue;
			this.meter.SetSymbolTint(new KAnimHashedString("meter_fill"), colour);
			this.meter.SetSymbolTint(new KAnimHashedString("water1"), colour);
			this.meter.SetSymbolTint(new KAnimHashedString("substance_tinter"), colour);
			this.meter.SetSymbolTint(new KAnimHashedString("substance_tinter_cap"), colour);
			this.OnStorageChange(null);
		}

		private PrimaryElement GetFirstPrimaryElement()
		{
			for (int i = 0; i < this.storage.Count; i++)
			{
				GameObject gameObject = this.storage[i];
				if (!(gameObject == null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (!(component == null))
					{
						return component;
					}
				}
			}
			return null;
		}

		public void Emit(float dt)
		{
			if (!base.smi.master.emit)
			{
				return;
			}
			PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
			if (firstPrimaryElement == null)
			{
				return;
			}
			float num = Mathf.Min(firstPrimaryElement.Mass, base.master.emptyRate * dt);
			if (num <= 0f)
			{
				return;
			}
			Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
			float num2;
			SimUtil.DiseaseInfo diseaseInfo;
			float num3;
			this.storage.ConsumeAndGetDisease(prefabTag, num, out num2, out diseaseInfo, out num3);
			Vector3 position = base.transform.GetPosition();
			position.y += 1.8f;
			bool flag = base.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
			position.x += (flag ? (-0.2f) : 0.2f);
			int num4 = Grid.PosToCell(position) + (flag ? (-1) : 1);
			if (Grid.Solid[num4])
			{
				num4 += (flag ? 1 : (-1));
			}
			Element element = firstPrimaryElement.Element;
			ushort idx = element.idx;
			if (element.IsLiquid)
			{
				FallingWater.instance.AddParticle(num4, idx, num2, num3, diseaseInfo.idx, diseaseInfo.count, true, false, false, false);
				return;
			}
			SimMessages.ModifyCell(num4, idx, num3, num2, diseaseInfo.idx, diseaseInfo.count, SimMessages.ReplaceType.None, false, -1);
		}

		[MyCmpGet]
		public Storage storage;

		private FetchChore chore;
	}

	public class States : GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.waitingfordelivery;
			this.statusItem = new StatusItem("BottleEmptier", "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022, true, null);
			this.statusItem.resolveStringCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier = (BottleEmptier)data;
				if (bottleEmptier == null)
				{
					return str;
				}
				if (bottleEmptier.allowManualPumpingStationFetching)
				{
					return bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME;
				}
				return bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
			};
			this.statusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier2 = (BottleEmptier)data;
				if (bottleEmptier2 == null)
				{
					return str;
				}
				string text;
				if (bottleEmptier2.allowManualPumpingStationFetching)
				{
					if (bottleEmptier2.isGasEmptier)
					{
						text = BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.TOOLTIP;
					}
					else
					{
						text = BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP;
					}
				}
				else if (bottleEmptier2.isGasEmptier)
				{
					text = BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.TOOLTIP;
				}
				else
				{
					text = BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
				}
				return text;
			};
			this.root.ToggleStatusItem(this.statusItem, (BottleEmptier.StatesInstance smi) => smi.master);
			this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery, false).PlayAnim("off");
			this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.emptying, (BottleEmptier.StatesInstance smi) => smi.GetComponent<Storage>().MassStored() > 0f).Enter("CreateChore", delegate(BottleEmptier.StatesInstance smi)
			{
				smi.CreateChore();
			})
				.Exit("CancelChore", delegate(BottleEmptier.StatesInstance smi)
				{
					smi.CancelChore();
				})
				.PlayAnim("on");
			this.emptying.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (BottleEmptier.StatesInstance smi) => smi.GetComponent<Storage>().MassStored() == 0f).Enter("StartMeter", delegate(BottleEmptier.StatesInstance smi)
			{
				smi.StartMeter();
			})
				.Update("Emit", delegate(BottleEmptier.StatesInstance smi, float dt)
				{
					smi.Emit(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}

		private StatusItem statusItem;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State unoperational;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State waitingfordelivery;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State emptying;
	}
}
