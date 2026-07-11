using System;
using System.Collections.Generic;
using UnityEngine;

public class CropSleepingMonitor : GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.awake;
		base.serializable = false;
		this.root.Update("CropSleepingMonitor.root", delegate(CropSleepingMonitor.Instance smi, float dt)
		{
			int num = Grid.PosToCell(smi.master.gameObject);
			GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State state = (smi.IsCellSafe(num) ? this.awake : this.sleeping);
			smi.GoTo(state);
		}, UpdateRate.SIM_1000ms, false);
		this.sleeping.TriggerOnEnter(GameHashes.CropSleep, null).ToggleStatusItem(Db.Get().CreatureStatusItems.CropSleeping, (CropSleepingMonitor.Instance smi) => smi);
		this.awake.TriggerOnEnter(GameHashes.CropWakeUp, null);
	}

	public GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State sleeping;

	public GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State awake;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject obj)
		{
			return null;
		}

		public float lightIntensityThreshold;

		public bool prefersDarkness;
	}

	public new class Instance : GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CropSleepingMonitor.Def def)
			: base(master, def)
		{
		}

		public bool IsSleeping()
		{
			return this.GetCurrentState() == base.smi.sm.sleeping;
		}

		public bool IsCellSafe(int cell)
		{
			float num = (float)Grid.LightIntensity[cell];
			if (!base.def.prefersDarkness)
			{
				return num >= base.def.lightIntensityThreshold;
			}
			return num <= base.def.lightIntensityThreshold;
		}
	}
}
