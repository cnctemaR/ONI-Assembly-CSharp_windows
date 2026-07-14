using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FertilityShearable : GameStateMachine<FertilityShearable, FertilityShearable.Instance, IStateMachineTarget, FertilityShearable.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.noMilk;
		this.noMilk.Transition(this.hasMilk, (FertilityShearable.Instance smi) => FertilityShearable.IsFertileEnoughToHarvest(smi), UpdateRate.SIM_200ms).Enter(delegate(FertilityShearable.Instance smi)
		{
			FertilityShearable.ShowBellySymbol(smi, false);
		});
		this.hasMilk.Transition(this.noMilk, (FertilityShearable.Instance smi) => !FertilityShearable.IsFertileEnoughToHarvest(smi), UpdateRate.SIM_200ms).Enter(delegate(FertilityShearable.Instance smi)
		{
			FertilityShearable.ShowBellySymbol(smi, true);
		}).Update(delegate(FertilityShearable.Instance smi, float dt)
		{
			smi.milkReadyStatusGuid = smi.selectable.ToggleStatusItem(Db.Get().CreatureStatusItems.FishFullMilk, smi.milkReadyStatusGuid, smi.IsReadyToBeMilked(), smi);
		}, UpdateRate.SIM_1000ms, false)
			.Exit(delegate(FertilityShearable.Instance smi)
			{
				smi.milkReadyStatusGuid = smi.selectable.RemoveStatusItem(smi.milkReadyStatusGuid, false);
			});
	}

	private static bool IsFertileEnoughToHarvest(FertilityShearable.Instance smi)
	{
		return smi.fertility.value >= smi.def.minimumFertility;
	}

	private static void ShowBellySymbol(FertilityShearable.Instance smi, bool show)
	{
		if (show)
		{
			FertilityShearable.AddBellyOverride(smi);
			return;
		}
		FertilityShearable.RemoveBellyOverride(smi);
	}

	private static void AddBellyOverride(FertilityShearable.Instance smi)
	{
		SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
		KAnim.Build.Symbol symbol = smi.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol("belly_full");
		if (symbol != null)
		{
			component.AddSymbolOverride("belly", symbol, 1);
		}
	}

	private static void RemoveBellyOverride(FertilityShearable.Instance smi)
	{
		smi.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride("belly", 1);
	}

	public GameStateMachine<FertilityShearable, FertilityShearable.Instance, IStateMachineTarget, FertilityShearable.Def>.State hasMilk;

	public GameStateMachine<FertilityShearable, FertilityShearable.Instance, IStateMachineTarget, FertilityShearable.Def>.State noMilk;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public override void Configure(GameObject prefab)
		{
		}

		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			string text = ElementLoader.FindElementByHash(this.milkElement).tag.ProperName();
			string formattedMass = GameUtil.GetFormattedMass(this.dropMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			string text2 = GlobalStringBuilderPool.ReturnAndFree(GlobalStringBuilderPool.Alloc().Append(UI.BUILDINGEFFECTS.SCALE_GROWTH_FERTILITY).Replace("{Item}", text)
				.Replace("{Amount}", formattedMass)
				.Replace("{Percent}", GameUtil.GetFormattedPercent(this.minimumFertility, GameUtil.TimeSlice.None)));
			string text3 = GlobalStringBuilderPool.ReturnAndFree(GlobalStringBuilderPool.Alloc().Append(UI.BUILDINGEFFECTS.TOOLTIPS.SCALE_GROWTH_FERTILE).Replace("{Item}", text)
				.Replace("{Amount}", formattedMass));
			return new List<Descriptor>
			{
				new Descriptor(text2, text3, Descriptor.DescriptorType.Effect, false)
			};
		}

		private const float DEFAULT_MINIMUM_FERTILITY = 50f;

		private const float DEFAULT_PERCENT_FERTILITY_CONSUMED_PER_MILKING = 1f;

		public SimHashes milkElement;

		public float dropMass;

		public float minimumFertility = 50f;

		public float percentFertilityConsumedPerMilking = 1f;

		public bool requiresHappy;

		public bool suppressedByElderly;
	}

	public new class Instance : GameStateMachine<FertilityShearable, FertilityShearable.Instance, IStateMachineTarget, FertilityShearable.Def>.GameInstance, IMilkable
	{
		public Instance(IStateMachineTarget master, FertilityShearable.Def def)
			: base(master, def)
		{
			this.fertility = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
		}

		public override void StartSM()
		{
			base.StartSM();
			this.wildnessMonitor = base.gameObject.GetSMI<WildnessMonitor.Instance>();
			this.ageMonitor = base.gameObject.GetSMI<AgeMonitor.Instance>();
		}

		public bool IsReadyToBeMilked()
		{
			return this.fertility.value >= base.def.minimumFertility && (this.wildnessMonitor == null || !this.wildnessMonitor.IsWild()) && (!base.def.requiresHappy || this.prefabId.HasTag(GameTags.Creatures.Happy)) && (!base.def.suppressedByElderly || this.ageMonitor == null || !this.ageMonitor.IsElderly);
		}

		public SimHashes GetMilkElement()
		{
			return base.def.milkElement;
		}

		public void MilkingComplete(Storage storage)
		{
			storage.GetComponent<Storage>().AddLiquid(base.def.milkElement, base.def.dropMass, 310.15f, byte.MaxValue, 0, false, true);
			float num = this.fertility.value - this.fertility.GetMax() * base.def.percentFertilityConsumedPerMilking;
			this.fertility.value = Mathf.Max(0f, num);
		}

		[MyCmpGet]
		private Effects effects;

		[MyCmpGet]
		public KBatchedAnimController animController;

		[MyCmpReq]
		private KPrefabID prefabId;

		[MyCmpReq]
		public KSelectable selectable;

		public AmountInstance fertility;

		public Guid milkReadyStatusGuid;

		private WildnessMonitor.Instance wildnessMonitor;

		private AgeMonitor.Instance ageMonitor;
	}
}
