using System;
using UnityEngine;

public class CargoDropperStorage : GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventHandler(GameHashes.JettisonCargo, delegate(CargoDropperStorage.StatesInstance smi, object data)
		{
			smi.JettisonCargo(data);
		});
	}

	public class Def : StateMachine.BaseDef
	{
		public Vector3 dropOffset;
	}

	public class StatesInstance : GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, CargoDropperStorage.Def def)
			: base(master, def)
		{
		}

		public void JettisonCargo(object data)
		{
			Vector3 vector = base.master.transform.GetPosition() + base.def.dropOffset;
			Storage component = base.GetComponent<Storage>();
			if (component != null)
			{
				GameObject gameObject = component.FindFirst("ScoutRover");
				if (gameObject != null)
				{
					component.Drop(gameObject, true);
					Vector3 position = base.master.transform.GetPosition();
					position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
					gameObject.transform.SetPosition(position);
					ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
					if (component2 != null)
					{
						KBatchedAnimController component3 = gameObject.GetComponent<KBatchedAnimController>();
						if (component3 != null)
						{
							component3.Play("enter", KAnim.PlayMode.Once, 1f, 0f);
						}
						new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, null, new HashedString[] { "enter" }, KAnim.PlayMode.Once, false);
					}
				}
				component.DropAll(vector, false, false, default(Vector3), true, null);
			}
		}
	}
}
