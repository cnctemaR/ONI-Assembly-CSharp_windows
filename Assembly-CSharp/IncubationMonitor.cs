using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class IncubationMonitor : GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.not_incubating;
		this.not_incubating.PlayAnim("idle", KAnim.PlayMode.Loop).ParamTransition<bool>(this.incubatorIsActive, this.incubating, (IncubationMonitor.Instance smi, bool b) => b);
		this.incubating.ParamTransition<bool>(this.incubatorIsActive, this.not_incubating, (IncubationMonitor.Instance smi, bool b) => !b).ToggleAttributeModifier("Incubating", (IncubationMonitor.Instance smi) => smi.incubationRate, null).Transition(this.ready_to_hatch, (IncubationMonitor.Instance smi) => smi.incubation.value >= smi.incubation.GetMax(), UpdateRate.SIM_200ms);
		this.ready_to_hatch.ToggleTag(GameTags.FullyIncubated).TriggerOnEnter(GameHashes.ReadyToHatch, null).EventTransition(GameHashes.Hatch, this.hatching, null);
		this.hatching.PlayAnim("hatching").OnAnimQueueComplete(this.hatch);
		this.hatch.Enter("Hatch", delegate(IncubationMonitor.Instance smi)
		{
			smi.Hatch();
		});
	}

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter incubatorIsActive;

	public StateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.BoolParameter inIncubator;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State not_incubating;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State incubating;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatching;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State ready_to_hatch;

	public GameStateMachine<IncubationMonitor, IncubationMonitor.Instance, IStateMachineTarget, IncubationMonitor.Def>.State hatch;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
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
			this.incubationRate = new AttributeModifier(Db.Get().Amounts.Incubation.deltaAttribute.Id, 0.016666668f, CREATURES.MODIFIERS.INCUBATOR_INCUBATION_RATE.NAME, false, false, true);
			master.Subscribe(856640610, new Action<object>(this.OnStore));
			master.Subscribe(1309017699, new Action<object>(this.OnStore));
			master.Subscribe(1628751838, new Action<object>(this.OnOperationalChanged));
			master.Subscribe(960378201, new Action<object>(this.OnOperationalChanged));
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

		public void Hatch()
		{
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.smi.def.spawnedCreature), Folder.Creatures, position);
			gameObject.SetActive(true);
			base.gameObject.DeleteObject();
		}

		public AmountInstance incubation;

		public AttributeModifier incubationRate;

		private EggIncubator incubator;
	}
}
