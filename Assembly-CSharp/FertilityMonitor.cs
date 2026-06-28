using System;
using Klei.AI;
using UnityEngine;

public class FertilityMonitor : GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleBehaviour(GameTags.Creatures.Fertile, (FertilityMonitor.Instance smi) => smi.IsReadyToLayEgg(), null).EventHandler(GameHashes.LayEgg, delegate(FertilityMonitor.Instance smi)
		{
			smi.OnLayEgg();
		});
	}

	public GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State idle;

	public GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State fertile;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Fertility.Id);
		}

		public Tag eggPrefab;
	}

	public new class Instance : GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FertilityMonitor.Def def)
			: base(master, def)
		{
			this.fertility = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
		}

		public void OnLayEgg()
		{
			this.fertility.value = 0f;
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			Util.KInstantiate(Assets.GetPrefab(base.def.eggPrefab), Folder.Creatures, position).gameObject.SetActive(true);
		}

		public bool IsReadyToLayEgg()
		{
			return base.smi.fertility.value >= base.smi.fertility.GetMax();
		}

		public AmountInstance fertility;
	}
}
