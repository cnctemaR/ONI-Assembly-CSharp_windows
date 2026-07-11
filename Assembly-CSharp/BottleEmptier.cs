using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class BottleEmptier : StateMachineComponent<BottleEmptier.StatesInstance>, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	private void OnChangeAllowManualPumpingStationFetching()
	{
		this.allowManualPumpingStationFetching = !this.allowManualPumpingStationFetching;
		base.smi.RefreshChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.allowManualPumpingStationFetching)
		{
			string text = "action_bottler_delivery";
			string text2 = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME;
			global::System.Action action = new global::System.Action(this.OnChangeAllowManualPumpingStationFetching);
			string text3 = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_bottler_delivery";
			string text2 = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME;
			global::System.Action action = new global::System.Action(this.OnChangeAllowManualPumpingStationFetching);
			string text = UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	[Serialize]
	public bool allowManualPumpingStationFetching;

	[SerializeField]
	public Color noFilterTint = FilteredStorage.NO_FILTER_TINT;

	[SerializeField]
	public Color filterTint = FilteredStorage.FILTER_TINT;

	public class StatesInstance : GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.GameInstance
	{
		public StatesInstance(BottleEmptier smi)
			: base(smi)
		{
			TreeFilterable component = base.master.GetComponent<TreeFilterable>();
			component.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(component.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_target", "meter_arrow", "meter_scale" });
			base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		}

		public MeterController meter { get; private set; }

		public void CreateChore()
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			Tag[] tags = base.GetComponent<TreeFilterable>().GetTags();
			if (tags == null || tags.Length == 0)
			{
				component.TintColour = base.master.noFilterTint;
				return;
			}
			component.TintColour = base.master.filterTint;
			Tag[] array;
			if (!base.master.allowManualPumpingStationFetching)
			{
				array = new Tag[] { GameTags.LiquidSource };
			}
			else
			{
				array = new Tag[0];
			}
			Storage component2 = base.GetComponent<Storage>();
			ChoreType fetch = Db.Get().ChoreTypes.Fetch;
			Storage storage = component2;
			float num = component2.Capacity();
			Tag[] tags2 = base.GetComponent<TreeFilterable>().GetTags();
			Tag[] array2 = array;
			this.chore = new FetchChore(fetch, storage, num, tags2, null, array2, null, true, null, null, null, FetchOrder2.OperationalRequirement.Operational, 0, null);
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

		private void OnFilterChanged(Tag[] tags)
		{
			this.RefreshChore();
		}

		private void OnStorageChange(object data)
		{
			Storage component = base.GetComponent<Storage>();
			this.meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
		}

		public void StartMeter()
		{
			PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
			if (firstPrimaryElement == null)
			{
				return;
			}
			this.meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
			this.meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
			base.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
		}

		private PrimaryElement GetFirstPrimaryElement()
		{
			Storage component = base.GetComponent<Storage>();
			for (int i = 0; i < component.Count; i++)
			{
				GameObject gameObject = component[i];
				if (!(gameObject == null))
				{
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					if (!(component2 == null))
					{
						return component2;
					}
				}
			}
			return null;
		}

		public void DripLiquid(float dt)
		{
			PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
			if (firstPrimaryElement == null)
			{
				return;
			}
			Storage component = base.GetComponent<Storage>();
			float mass = firstPrimaryElement.Mass;
			float num = 10f;
			float num2 = Mathf.Min(mass, num * dt);
			if (num2 <= 0f)
			{
				return;
			}
			Tag prefabTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
			SimUtil.DiseaseInfo diseaseInfo;
			float num3;
			component.ConsumeAndGetDisease(prefabTag, num2, out diseaseInfo, out num3);
			Vector3 position = base.transform.GetPosition();
			position.y += 1.8f;
			bool flag = base.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
			position.x += ((!flag) ? 0.2f : (-0.2f));
			int num4 = Grid.PosToCell(position) + ((!flag) ? 1 : (-1));
			if (Grid.Solid[num4])
			{
				num4 += ((!flag) ? (-1) : 1);
			}
			FallingWater.instance.AddParticle(num4, (byte)ElementLoader.GetElementIndex(firstPrimaryElement.ElementID), num2, num3, diseaseInfo.idx, diseaseInfo.count, true, false, false, false);
		}

		private FetchChore chore;
	}

	public class States : GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.waitingfordelivery;
			this.statusItem = new StatusItem("BottleEmptier", string.Empty, string.Empty, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 63486);
			this.statusItem.resolveStringCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier = (BottleEmptier)data;
				if (bottleEmptier == null)
				{
					return str;
				}
				if (bottleEmptier.allowManualPumpingStationFetching)
				{
					return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME;
				}
				return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
			};
			this.statusItem.resolveTooltipCallback = delegate(string str, object data)
			{
				BottleEmptier bottleEmptier2 = (BottleEmptier)data;
				if (bottleEmptier2 == null)
				{
					return str;
				}
				if (bottleEmptier2.allowManualPumpingStationFetching)
				{
					return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP;
				}
				return BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
			};
			this.root.ToggleStatusItem(this.statusItem, (BottleEmptier.StatesInstance smi) => smi.master);
			this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery, false).PlayAnim("off");
			this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.emptying, (BottleEmptier.StatesInstance smi) => !smi.GetComponent<Storage>().IsEmpty()).Enter("CreateChore", delegate(BottleEmptier.StatesInstance smi)
			{
				smi.CreateChore();
			})
				.Exit("CancelChore", delegate(BottleEmptier.StatesInstance smi)
				{
					smi.CancelChore();
				})
				.PlayAnim("on");
			this.emptying.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (BottleEmptier.StatesInstance smi) => smi.GetComponent<Storage>().IsEmpty()).Enter("StartMeter", delegate(BottleEmptier.StatesInstance smi)
			{
				smi.StartMeter();
			})
				.Update("DripLiquid", delegate(BottleEmptier.StatesInstance smi, float dt)
				{
					smi.DripLiquid(dt);
				}, UpdateRate.SIM_200ms, false)
				.PlayAnim("working_loop", KAnim.PlayMode.Loop);
		}

		private StatusItem statusItem;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State unoperational;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State waitingfordelivery;

		public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State emptying;
	}
}
