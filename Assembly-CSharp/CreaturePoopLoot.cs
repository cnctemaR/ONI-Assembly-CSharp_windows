using System;
using UnityEngine;

public class CreaturePoopLoot : GameStateMachine<CreaturePoopLoot, CreaturePoopLoot.Instance, IStateMachineTarget, CreaturePoopLoot.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.EventTransition(GameHashes.Poop, this.roll, null);
		this.roll.Enter(new StateMachine<CreaturePoopLoot, CreaturePoopLoot.Instance, IStateMachineTarget, CreaturePoopLoot.Def>.State.Callback(CreaturePoopLoot.RollForLoot)).GoTo(this.idle);
	}

	public static void RollForLoot(CreaturePoopLoot.Instance smi)
	{
		for (int i = 0; i < smi.def.Loot.Length; i++)
		{
			float value = global::UnityEngine.Random.value;
			CreaturePoopLoot.LootData lootData = smi.def.Loot[i];
			if (lootData.probability > 0f && value <= lootData.probability)
			{
				Tag tag = lootData.tag;
				Vector3 position = smi.transform.position;
				position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
				Util.KInstantiate(Assets.GetPrefab(tag), position).SetActive(true);
			}
		}
	}

	public GameStateMachine<CreaturePoopLoot, CreaturePoopLoot.Instance, IStateMachineTarget, CreaturePoopLoot.Def>.State idle;

	public GameStateMachine<CreaturePoopLoot, CreaturePoopLoot.Instance, IStateMachineTarget, CreaturePoopLoot.Def>.State roll;

	public struct LootData
	{
		public Tag tag;

		public float probability;
	}

	public class Def : StateMachine.BaseDef
	{
		public CreaturePoopLoot.LootData[] Loot;
	}

	public new class Instance : GameStateMachine<CreaturePoopLoot, CreaturePoopLoot.Instance, IStateMachineTarget, CreaturePoopLoot.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreaturePoopLoot.Def def)
			: base(master, def)
		{
		}
	}
}
