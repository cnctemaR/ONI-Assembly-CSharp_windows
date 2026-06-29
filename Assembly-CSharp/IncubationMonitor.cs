using System;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IncubationMonitor : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.root.ToggleAttributeModifier(string.Empty, (IncubationMonitor.Instance smi) => this.baseIncubation, null);
		this.idle.PlayAnim("idle", KAnim.PlayMode.Loop).ParamTransition<bool>(this.incubatorIsActive, this.active_incubator, (IncubationMonitor.Instance smi, bool b) => b).Transition(this.hatching_pre, new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.Transition.ConditionCallback(IncubationMonitor.IsReadyToHatch), UpdateRate.SIM_1000ms);
		this.active_incubator.ParamTransition<bool>(this.incubatorIsActive, this.idle, (IncubationMonitor.Instance smi, bool b) => !b).ToggleEffect((IncubationMonitor.Instance smi) => this.incubatingEffect).Transition(this.hatching_pre, new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.Transition.ConditionCallback(IncubationMonitor.IsReadyToHatch), UpdateRate.SIM_1000ms);
		this.hatching_pre.QueueAnim("hatching_pre", false, null).OnAnimQueueComplete(this.hatching_pst);
		this.hatching_pst.Enter(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.SpawnBaby)).QueueAnim("hatching_pst", false, null).OnAnimQueueComplete(null)
			.Exit(new StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State.Callback(IncubationMonitor.DeleteSelf));
		float num = 0.008333334f;
		if (GenericGameSettings.instance.acceleratedLifecycle)
		{
			num = 33.333332f;
		}
		this.baseIncubation = new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, num, CREATURES.MODIFIERS.BASE_INCUBATION_RATE.NAME, false, false, true);
		this.incubatingEffect = new Effect("Incubator", CREATURES.MODIFIERS.INCUBATOR.NAME, CREATURES.MODIFIERS.INCUBATOR.TOOLTIP, 0f, true, false, false);
		this.incubatingEffect.Add(new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 4f, CREATURES.MODIFIERS.INCUBATOR.NAME, true, false, true));
	}

	private static bool IsReadyToHatch(IncubationMonitor.Instance smi)
	{
		return smi.incubation.value >= smi.incubation.GetMax();
	}

	private static void SpawnBaby(IncubationMonitor.Instance smi)
	{
		Vector3 position = smi.transform.GetPosition();
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(smi.def.spawnedCreature), Folder.Creatures, position);
		gameObject.SetActive(true);
		gameObject.GetSMI<AnimInterruptMonitor.Instance>().Play("hatching_pst", KAnim.PlayMode.Once);
		KSelectable component = smi.gameObject.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>(), false);
		}
		Db.Get().Amounts.Wildness.Copy(gameObject, smi.gameObject);
	}

	private static void DeleteSelf(IncubationMonitor.Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter incubatorIsActive;

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter inIncubator;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State idle;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State active_incubator;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatching_pre;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatching_pst;

	private Effect incubatingEffect;

	private Effect wildEffect;

	private AttributeModifier baseIncubation;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Wildness.Id);
			prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Incubation.Id);
		}

		public float baseIncubation;

		public Tag spawnedCreature;
	}

	public new class Instance : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, IncubationMonitor.Def def)
			: base(master, def)
		{
			this.incubation = Db.Get().Amounts.Incubation.Lookup(base.gameObject);
			master.Subscribe(856640610, new Action<object>(this.OnStore));
			master.Subscribe(1309017699, new Action<object>(this.OnStore));
			master.Subscribe(1628751838, new Action<object>(this.OnOperationalChanged));
			master.Subscribe(960378201, new Action<object>(this.OnOperationalChanged));
			this.wildness = Db.Get().Amounts.Wildness.Lookup(base.gameObject);
			this.wildness.value = this.wildness.GetMax();
		}

		public void OnStore(object data)
		{
			Storage storage = data as Storage;
			this.incubator = ((!storage) ? null : storage.GetComponent<EggIncubator>());
			this.OnOperationalChanged(null);
		}

		public void OnOperationalChanged(object data = null)
		{
			base.smi.sm.inIncubator.Set(this.incubator != null, base.smi);
			Operational operational = ((!this.incubator) ? null : this.incubator.GetComponent<Operational>());
			bool flag = this.incubator && (operational == null || operational.IsOperational);
			base.smi.sm.incubatorIsActive.Set(flag, base.smi);
		}

		public AmountInstance incubation;

		public AmountInstance wildness;

		private EggIncubator incubator;
	}
}
