using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IncubationMonitor : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.incubating;
		this.root.Enter(delegate(IncubationMonitor.Instance smi)
		{
			smi.OnOperationalChanged(null);
		});
		this.incubating.PlayAnim("idle", KAnim.PlayMode.Loop).Transition(this.hatching_pre, new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.Transition.ConditionCallback(IncubationMonitor.IsReadyToHatch), UpdateRate.SIM_1000ms).TagTransition(GameTags.Entombed, this.entombed, false)
			.ParamTransition<bool>(this.isSuppressed, this.suppressed, GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.IsTrue)
			.ToggleEffect((IncubationMonitor.Instance smi) => smi.incubatingEffect);
		this.entombed.TagTransition(GameTags.Entombed, this.incubating, true);
		this.suppressed.ToggleEffect((IncubationMonitor.Instance smi) => this.suppressedEffect).ParamTransition<bool>(this.isSuppressed, this.incubating, GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.IsFalse).TagTransition(GameTags.Entombed, this.entombed, false)
			.Transition(this.not_viable, new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.Transition.ConditionCallback(IncubationMonitor.NoLongerViable), UpdateRate.SIM_1000ms);
		this.hatching_pre.Enter(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.DropSelfFromStorage)).PlayAnim("hatching_pre").OnAnimQueueComplete(this.hatching_pst);
		this.hatching_pst.Enter(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.SpawnBaby)).PlayAnim("hatching_pst").OnAnimQueueComplete(null)
			.Exit(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.DeleteSelf));
		this.not_viable.Enter(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.SpawnGenericEgg)).GoTo(null).Exit(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.DeleteSelf));
		this.suppressedEffect = new Effect("IncubationSuppressed", CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.TOOLTIP, 0f, true, false, true, null, 0f, null);
		this.suppressedEffect.Add(new AttributeModifier(Db.Get().Amounts.Viability.deltaAttribute.Id, -0.016666668f, CREATURES.MODIFIERS.INCUBATING_SUPPRESSED.NAME, false, false, true));
	}

	private static bool IsReadyToHatch(IncubationMonitor.Instance smi)
	{
		return !smi.gameObject.HasTag(GameTags.Entombed) && smi.incubation.value >= smi.incubation.GetMax();
	}

	private static void SpawnBaby(IncubationMonitor.Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.def.spawnedCreature), position);
		gameObject.SetActive(true);
		gameObject.GetSMI<AnimInterruptMonitor.Instance>().Play("hatching_pst", KAnim.PlayMode.Once);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
		Db.Get().Amounts.Wildness.Copy(gameObject, smi.gameObject);
		if (smi.incubator != null)
		{
			smi.incubator.StoreBaby(gameObject);
		}
		IncubationMonitor.SpawnShell(smi);
		SaveLoader.Instance.saveManager.Unregister(smi.GetComponent<SaveLoadRoot>());
	}

	private static bool NoLongerViable(IncubationMonitor.Instance smi)
	{
		return !smi.gameObject.HasTag(GameTags.Entombed) && smi.viability.value <= smi.viability.GetMin();
	}

	private static GameObject SpawnShell(IncubationMonitor.Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("EggShell"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(true);
		return gameObject;
	}

	private static GameObject SpawnEggInnards(IncubationMonitor.Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("RawEgg"), position);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		PrimaryElement component2 = smi.GetComponent<PrimaryElement>();
		component.Mass = component2.Mass * 0.5f;
		gameObject.SetActive(true);
		return gameObject;
	}

	private static void SpawnGenericEgg(IncubationMonitor.Instance smi)
	{
		IncubationMonitor.SpawnShell(smi);
		GameObject gameObject = IncubationMonitor.SpawnEggInnards(smi);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
	}

	private static void DeleteSelf(IncubationMonitor.Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	private static void DropSelfFromStorage(IncubationMonitor.Instance smi)
	{
		if (!smi.sm.inIncubator.Get(smi))
		{
			Storage storage = smi.GetStorage();
			if (storage)
			{
				storage.Drop(smi.gameObject);
			}
			smi.gameObject.AddTag(GameTags.StoredPrivate);
		}
	}

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter incubatorIsActive;

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter inIncubator;

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter isSuppressed;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State incubating;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State entombed;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State suppressed;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatching_pre;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatching_pst;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State not_viable;

	private Effect suppressedEffect;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			List<string> initialAmounts = prefab.GetComponent<Modifiers>().initialAmounts;
			initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
			initialAmounts.Add(Db.Get().Amounts.Incubation.Id);
			initialAmounts.Add(Db.Get().Amounts.Viability.Id);
		}

		public float baseIncubationRate;

		public Tag spawnedCreature;
	}

	public new class Instance : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, IncubationMonitor.Def def)
			: base(master, def)
		{
			this.incubation = Db.Get().Amounts.Incubation.Lookup(base.gameObject);
			Action<object> action = new Action<object>(this.OnStore);
			master.Subscribe(856640610, action);
			master.Subscribe(1309017699, action);
			Action<object> action2 = new Action<object>(this.OnOperationalChanged);
			master.Subscribe(1628751838, action2);
			master.Subscribe(960378201, action2);
			this.wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			this.wildness.value = this.wildness.GetMax();
			this.viability = Db.Get().Amounts.Viability.Lookup(base.gameObject);
			this.viability.value = this.viability.GetMax();
			float num = def.baseIncubationRate;
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				num = 33.333332f;
			}
			AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, num, CREATURES.MODIFIERS.BASE_INCUBATION_RATE.NAME, false, false, true);
			this.incubatingEffect = new Effect("Incubating", CREATURES.MODIFIERS.INCUBATING.NAME, CREATURES.MODIFIERS.INCUBATING.TOOLTIP, 0f, true, false, false, null, 0f, null);
			this.incubatingEffect.Add(attributeModifier);
		}

		public Storage GetStorage()
		{
			return (!(base.transform.parent != null)) ? null : base.transform.parent.GetComponent<Storage>();
		}

		public void OnStore(object data)
		{
			Storage storage = data as Storage;
			bool flag = storage || (data != null && (bool)data);
			EggIncubator eggIncubator = ((!storage) ? null : storage.GetComponent<EggIncubator>());
			this.UpdateIncubationState(flag, eggIncubator);
		}

		public void OnOperationalChanged(object data = null)
		{
			bool flag = base.gameObject.HasTag(GameTags.Stored);
			Storage storage = this.GetStorage();
			EggIncubator eggIncubator = ((!storage) ? null : storage.GetComponent<EggIncubator>());
			this.UpdateIncubationState(flag, eggIncubator);
		}

		private void UpdateIncubationState(bool stored, EggIncubator incubator)
		{
			this.incubator = incubator;
			base.smi.sm.inIncubator.Set(incubator != null, base.smi);
			bool flag = stored && !incubator;
			base.smi.sm.isSuppressed.Set(flag, base.smi);
			Operational operational = ((!incubator) ? null : incubator.GetComponent<Operational>());
			bool flag2 = incubator && (operational == null || operational.IsOperational);
			base.smi.sm.incubatorIsActive.Set(flag2, base.smi);
		}

		public void ApplySongBuff()
		{
			base.GetComponent<Effects>().Add("EggSong", true);
		}

		public bool HasSongBuff()
		{
			return base.GetComponent<Effects>().HasEffect("EggSong");
		}

		public AmountInstance incubation;

		public AmountInstance wildness;

		public AmountInstance viability;

		public EggIncubator incubator;

		public Effect incubatingEffect;
	}
}
