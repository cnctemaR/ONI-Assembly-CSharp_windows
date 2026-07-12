using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterCooler : StateMachineComponent<WaterCooler.StatesInstance>, IApproachable, IGameObjectEffectDescriptor, FewOptionSideScreen.IFewOptionSideScreen
{
	public Tag ChosenBeverage
	{
		get
		{
			return this.chosenBeverage;
		}
		set
		{
			if (this.chosenBeverage != value)
			{
				this.chosenBeverage = value;
				base.GetComponent<ManualDeliveryKG>().RequestedItemTag = this.chosenBeverage;
				this.storage.DropAll(false, false, default(Vector3), true, null);
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<ManualDeliveryKG>().RequestedItemTag = this.chosenBeverage;
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, true);
		}, null, null);
		this.workables = new SocialGatheringPointWorkable[this.socializeOffsets.Length];
		for (int i = 0; i < this.workables.Length; i++)
		{
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), this.socializeOffsets[i]), Grid.SceneLayer.Move);
			SocialGatheringPointWorkable socialGatheringPointWorkable = ChoreHelpers.CreateLocator("WaterCoolerWorkable", vector).AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.specificEffect = "Socialized";
			socialGatheringPointWorkable.SetWorkTime(this.workTime);
			this.workables[i] = socialGatheringPointWorkable;
		}
		this.chores = new Chore[this.socializeOffsets.Length];
		Extents extents = new Extents(Grid.PosToCell(this), this.socializeOffsets);
		this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WaterCooler", this, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
		base.Subscribe<WaterCooler>(-1697596308, WaterCooler.OnStorageChangeDelegate);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.validNavCellChangedPartitionerEntry);
		this.CancelDrinkChores();
		for (int i = 0; i < this.workables.Length; i++)
		{
			if (this.workables[i])
			{
				Util.KDestroyGameObject(this.workables[i]);
				this.workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	public void UpdateDrinkChores(bool force = true)
	{
		if (!force && !this.choresDirty)
		{
			return;
		}
		float num = this.storage.GetMassAvailable(this.ChosenBeverage);
		int num2 = 0;
		for (int i = 0; i < this.socializeOffsets.Length; i++)
		{
			CellOffset cellOffset = this.socializeOffsets[i];
			Chore chore = this.chores[i];
			if (num2 < this.choreCount && this.IsOffsetValid(cellOffset) && num >= 1f)
			{
				num2++;
				num -= 1f;
				if (chore == null || chore.isComplete)
				{
					this.chores[i] = new WaterCoolerChore(this, this.workables[i], null, null, new Action<Chore>(this.OnChoreEnd));
				}
			}
			else if (chore != null)
			{
				chore.Cancel("invalid");
				this.chores[i] = null;
			}
		}
		this.choresDirty = false;
	}

	public void CancelDrinkChores()
	{
		for (int i = 0; i < this.socializeOffsets.Length; i++)
		{
			Chore chore = this.chores[i];
			if (chore != null)
			{
				chore.Cancel("cancelled");
				this.chores[i] = null;
			}
		}
	}

	private bool IsOffsetValid(CellOffset offset)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(this), offset);
		int num2 = Grid.CellBelow(num);
		return GameNavGrids.FloorValidator.IsWalkableCell(num, num2, false);
	}

	private void OnChoreEnd(Chore chore)
	{
		this.choresDirty = true;
	}

	private void OnCellChanged(object data)
	{
		this.choresDirty = true;
	}

	private void OnStorageChange(object data)
	{
		this.choresDirty = true;
	}

	public CellOffset[] GetOffsets()
	{
		return this.drinkOffsets;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
	{
		string text = tag.ProperName();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, text, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		descs.Add(descriptor);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		Effect.AddModifierDescriptions(base.gameObject, list, "Socialized", true);
		foreach (global::Tuple<Tag, string> tuple in WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS)
		{
			this.AddRequirementDesc(list, tuple.first, 1f);
		}
		return list;
	}

	public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
	{
		Effect.CreateTooltip(Db.Get().effects.Get("DuplicantGotMilk"), true, "\n    • ", true);
		FewOptionSideScreen.IFewOptionSideScreen.Option[] array = new FewOptionSideScreen.IFewOptionSideScreen.Option[WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = Strings.Get("STRINGS.BUILDINGS.PREFABS.WATERCOOLER.OPTION_TOOLTIPS." + WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].first.ToString().ToUpper());
			if (!WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].second.IsNullOrWhiteSpace())
			{
				text = text + "\n\n" + Effect.CreateTooltip(Db.Get().effects.Get(WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].second), false, "\n    • ", true);
			}
			array[i] = new FewOptionSideScreen.IFewOptionSideScreen.Option(WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].first, ElementLoader.GetElement(WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].first).name, Def.GetUISprite(WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS[i].first, "ui", false), text);
		}
		return array;
	}

	public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
	{
		this.ChosenBeverage = option.tag;
	}

	public Tag GetSelectedOption()
	{
		return this.ChosenBeverage;
	}

	public const float DRINK_MASS = 1f;

	public const string SPECIFIC_EFFECT = "Socialized";

	public CellOffset[] socializeOffsets = new CellOffset[]
	{
		new CellOffset(-1, 0),
		new CellOffset(2, 0),
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public float workTime = 5f;

	private CellOffset[] drinkOffsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public static Action<GameObject, GameObject> OnDuplicantDrank;

	private Chore[] chores;

	private HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private SocialGatheringPointWorkable[] workables;

	[MyCmpGet]
	private Storage storage;

	public bool choresDirty;

	[Serialize]
	private Tag chosenBeverage = GameTags.Water;

	private static readonly EventSystem.IntraObjectHandler<WaterCooler> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<WaterCooler>(delegate(WaterCooler component, object data)
	{
		component.OnStorageChange(data);
	});

	public class States : GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery, false).PlayAnim("off");
			this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).Transition(this.dispensing, (WaterCooler.StatesInstance smi) => smi.HasMinimumMass(), UpdateRate.SIM_200ms).EventTransition(GameHashes.OnStorageChange, this.dispensing, (WaterCooler.StatesInstance smi) => smi.HasMinimumMass())
				.PlayAnim("off");
			this.dispensing.Enter("StartMeter", delegate(WaterCooler.StatesInstance smi)
			{
				smi.StartMeter();
			}).Enter("Set Active", delegate(WaterCooler.StatesInstance smi)
			{
				smi.SetOperationalActiveState(true);
			}).Enter("UpdateDrinkChores.force", delegate(WaterCooler.StatesInstance smi)
			{
				smi.master.UpdateDrinkChores(true);
			})
				.Update("UpdateDrinkChores", delegate(WaterCooler.StatesInstance smi, float dt)
				{
					smi.master.UpdateDrinkChores(true);
				}, UpdateRate.SIM_200ms, false)
				.Exit("CancelDrinkChores", delegate(WaterCooler.StatesInstance smi)
				{
					smi.master.CancelDrinkChores();
				})
				.Exit("Set Inactive", delegate(WaterCooler.StatesInstance smi)
				{
					smi.SetOperationalActiveState(false);
				})
				.TagTransition(GameTags.Operational, this.unoperational, true)
				.EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (WaterCooler.StatesInstance smi) => !smi.HasMinimumMass())
				.PlayAnim("working");
		}

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State unoperational;

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State waitingfordelivery;

		public GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.State dispensing;
	}

	public class StatesInstance : GameStateMachine<WaterCooler.States, WaterCooler.StatesInstance, WaterCooler, object>.GameInstance
	{
		public StatesInstance(WaterCooler smi)
			: base(smi)
		{
			this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_bottle", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[] { "meter_bottle" });
			this.storage = base.master.GetComponent<Storage>();
			base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		}

		public void Drink(GameObject druplicant, bool triggerOnDrinkCallback = true)
		{
			if (!this.HasMinimumMass())
			{
				return;
			}
			Tag tag = this.storage.items[0].PrefabID();
			float num;
			SimUtil.DiseaseInfo diseaseInfo;
			float num2;
			this.storage.ConsumeAndGetDisease(tag, 1f, out num, out diseaseInfo, out num2);
			GermExposureMonitor.Instance smi = druplicant.GetSMI<GermExposureMonitor.Instance>();
			if (smi != null)
			{
				smi.TryInjectDisease(diseaseInfo.idx, diseaseInfo.count, tag, Sickness.InfectionVector.Digestion);
			}
			Effects component = druplicant.GetComponent<Effects>();
			if (tag == SimHashes.Milk.CreateTag())
			{
				component.Add("DuplicantGotMilk", true);
			}
			if (triggerOnDrinkCallback)
			{
				Action<GameObject, GameObject> onDuplicantDrank = WaterCooler.OnDuplicantDrank;
				if (onDuplicantDrank == null)
				{
					return;
				}
				onDuplicantDrank(druplicant, base.gameObject);
			}
		}

		private void OnStorageChange(object data)
		{
			float num = Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg);
			this.meter.SetPositionPercent(num);
		}

		public void SetOperationalActiveState(bool isActive)
		{
			this.operational.SetActive(isActive, false);
		}

		public void StartMeter()
		{
			PrimaryElement primaryElement = this.storage.FindFirstWithMass(base.smi.master.ChosenBeverage, 0f);
			if (primaryElement == null)
			{
				return;
			}
			this.meter.SetSymbolTint(new KAnimHashedString("meter_water"), primaryElement.Element.substance.colour);
			this.OnStorageChange(null);
		}

		public bool HasMinimumMass()
		{
			return this.storage.GetMassAvailable(ElementLoader.GetElement(base.smi.master.ChosenBeverage).id) >= 1f;
		}

		[MyCmpGet]
		private Operational operational;

		private Storage storage;

		private MeterController meter;
	}
}
