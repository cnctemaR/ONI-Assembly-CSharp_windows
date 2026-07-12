using System;
using System.Collections.Generic;

public class NearbyCreatureMonitor : GameStateMachine<NearbyCreatureMonitor, NearbyCreatureMonitor.Instance, IStateMachineTarget>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update("UpdateNearbyCreatures", delegate(NearbyCreatureMonitor.Instance smi, float dt)
		{
			smi.UpdateNearbyCreatures(dt);
		}, UpdateRate.SIM_1000ms, false);
	}

	public new class Instance : GameStateMachine<NearbyCreatureMonitor, NearbyCreatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public event Action<float, List<KPrefabID>, List<KPrefabID>> OnUpdateNearbyCreatures;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void UpdateNearbyCreatures(float dt)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(base.gameObject));
			if (cavityForCell != null)
			{
				this.OnUpdateNearbyCreatures(dt, cavityForCell.creatures, cavityForCell.eggs);
			}
		}
	}
}
