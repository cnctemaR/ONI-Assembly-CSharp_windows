using System;
using Klei.AI;
using UnityEngine;

public class CritterCondoStates : GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.goingToCondo;
		this.root.Enter(new StateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State.Callback(CritterCondoStates.ReserveCondo)).Exit(new StateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State.Callback(CritterCondoStates.UnreserveCondo));
		this.goingToCondo.MoveTo(new Func<CritterCondoStates.Instance, int>(CritterCondoStates.GetCondoInteractCell), this.interact, null, false).ToggleMainStatusItem((CritterCondoStates.Instance smi) => CritterCondoStates.GetTargetCondo(smi).def.moveToStatusItem, null).OnTargetLost(this.targetCondo, null);
		this.interact.DefaultState(this.interact.pre).OnTargetLost(this.targetCondo, null).Enter(delegate(CritterCondoStates.Instance smi)
		{
			this.SetFacing(smi);
			smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
		})
			.Exit(delegate(CritterCondoStates.Instance smi)
			{
				smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			})
			.ToggleMainStatusItem((CritterCondoStates.Instance smi) => CritterCondoStates.GetTargetCondo(smi).def.interactStatusItem, null);
		this.interact.pre.PlayAnim("cc_working_pre").Enter(delegate(CritterCondoStates.Instance smi)
		{
			CritterCondoStates.PlayCondoBuildingAnim(smi, "cc_working_pre");
		}).OnAnimQueueComplete(this.interact.loop);
		this.interact.loop.PlayAnim("cc_working").Enter(delegate(CritterCondoStates.Instance smi)
		{
			CritterCondoStates.PlayCondoBuildingAnim(smi, smi.def.working_anim);
		}).OnAnimQueueComplete(this.interact.pst);
		this.interact.pst.PlayAnim("cc_working_pst").Enter(delegate(CritterCondoStates.Instance smi)
		{
			CritterCondoStates.PlayCondoBuildingAnim(smi, "cc_working_pst");
		}).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.BehaviourComplete(GameTags.Creatures.Behaviour_InteractWithCritterCondo, false).Exit(new StateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State.Callback(CritterCondoStates.ApplyEffects));
	}

	private void SetFacing(CritterCondoStates.Instance smi)
	{
		bool isRotated = CritterCondoStates.GetTargetCondo(smi).Get<Rotatable>().IsRotated;
		smi.Get<Facing>().SetFacing(isRotated);
	}

	private static CritterCondo.Instance GetTargetCondo(CritterCondoStates.Instance smi)
	{
		GameObject gameObject = smi.sm.targetCondo.Get(smi);
		CritterCondo.Instance instance = ((gameObject != null) ? gameObject.GetSMI<CritterCondo.Instance>() : null);
		if (instance.IsNullOrStopped())
		{
			return null;
		}
		return instance;
	}

	private static void ReserveCondo(CritterCondoStates.Instance smi)
	{
		CritterCondo.Instance instance = smi.GetSMI<CritterCondoInteractMontior.Instance>().targetCondo;
		if (instance == null)
		{
			return;
		}
		smi.sm.targetCondo.Set(instance.gameObject, smi, false);
		instance.SetReserved(true);
	}

	private static void UnreserveCondo(CritterCondoStates.Instance smi)
	{
		CritterCondo.Instance instance = CritterCondoStates.GetTargetCondo(smi);
		if (instance == null)
		{
			return;
		}
		instance.GetComponent<KBatchedAnimController>().Play("on", KAnim.PlayMode.Loop, 1f, 0f);
		smi.sm.targetCondo.Set(null, smi);
		instance.SetReserved(false);
	}

	private static int GetCondoInteractCell(CritterCondoStates.Instance smi)
	{
		CritterCondo.Instance instance = CritterCondoStates.GetTargetCondo(smi);
		if (instance == null)
		{
			return Grid.InvalidCell;
		}
		int num = instance.GetInteractStartCell();
		if (smi.isLargeCritter)
		{
			bool isRotated = instance.Get<Rotatable>().IsRotated;
			Vector2I vector2I = Grid.PosToXY(smi.gameObject.transform.position);
			Vector2I vector2I2 = Grid.CellToXY(num);
			if (vector2I.x > vector2I2.x && !isRotated)
			{
				num = Grid.CellLeft(num);
			}
			else if (vector2I.x < vector2I2.x && isRotated)
			{
				num = Grid.CellRight(num);
			}
		}
		return num;
	}

	private static void ApplyEffects(CritterCondoStates.Instance smi)
	{
		smi.Get<Effects>().Add(CritterCondoStates.GetTargetCondo(smi).def.effectId, true);
	}

	private static void PlayCondoBuildingAnim(CritterCondoStates.Instance smi, string anim_name)
	{
		CritterCondo.Instance smi2 = smi.sm.targetCondo.GetSMI<CritterCondo.Instance>(smi);
		if (smi2 != null)
		{
			smi2.UpdateCritterAnims(anim_name, smi.def.entersBuilding, smi.isLargeCritter);
		}
	}

	public GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State goingToCondo;

	public CritterCondoStates.InteractState interact;

	public GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State behaviourComplete;

	public StateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.TargetParameter targetCondo;

	public class Def : StateMachine.BaseDef
	{
		public bool entersBuilding = true;

		public string working_anim = "cc_working";
	}

	public new class Instance : GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.GameInstance
	{
		public Instance(Chore<CritterCondoStates.Instance> chore, CritterCondoStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviour_InteractWithCritterCondo);
			this.isLargeCritter = base.GetComponent<KPrefabID>().HasTag(GameTags.LargeCreature);
		}

		public bool isLargeCritter;
	}

	public class InteractState : GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State
	{
		public GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State pre;

		public GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State loop;

		public GameStateMachine<CritterCondoStates, CritterCondoStates.Instance, IStateMachineTarget, CritterCondoStates.Def>.State pst;
	}
}
