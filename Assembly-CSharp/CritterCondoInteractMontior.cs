using System;
using System.Collections.Generic;
using UnityEngine;

public class CritterCondoInteractMontior : GameStateMachine<CritterCondoInteractMontior, CritterCondoInteractMontior.Instance, IStateMachineTarget, CritterCondoInteractMontior.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.lookingForCondo;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.ParamTransition<float>(this.remainingSecondsForEffect, this.satisfied, (CritterCondoInteractMontior.Instance smi, float val) => val > 0f);
		this.lookingForCondo.PreBrainUpdate(new Action<CritterCondoInteractMontior.Instance>(CritterCondoInteractMontior.FindCondoTarget)).ToggleBehaviour(GameTags.Creatures.Behaviour_InteractWithCritterCondo, (CritterCondoInteractMontior.Instance smi) => !smi.targetCondo.IsNullOrStopped() && !smi.targetCondo.IsReserved(), delegate(CritterCondoInteractMontior.Instance smi)
		{
			smi.GoTo(this.satisfied);
		});
		this.satisfied.Enter(delegate(CritterCondoInteractMontior.Instance smi)
		{
			this.remainingSecondsForEffect.Set(600f, smi, false);
		}).ScheduleGoTo((CritterCondoInteractMontior.Instance smi) => this.remainingSecondsForEffect.Get(smi), this.lookingForCondo);
	}

	private static void FindCondoTarget(CritterCondoInteractMontior.Instance smi)
	{
		using (ListPool<CritterCondo.Instance, CritterCondoInteractMontior>.PooledList pooledList = PoolsFor<CritterCondoInteractMontior>.AllocateList<CritterCondo.Instance>())
		{
			if (!smi.def.requireCavity)
			{
				Vector3 position = smi.gameObject.transform.GetPosition();
				using (List<CritterCondo.Instance>.Enumerator enumerator = Components.CritterCondos.GetItems(smi.GetMyWorldId()).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CritterCondo.Instance instance = enumerator.Current;
						if (!instance.IsNullOrDestroyed() && !(instance.def.condoTag != smi.def.condoPrefabTag) && (instance.transform.GetPosition() - position).sqrMagnitude <= 256f && instance.CanBeReserved())
						{
							pooledList.Add(instance);
						}
					}
					goto IL_0152;
				}
			}
			int num = Grid.PosToCell(smi.gameObject);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell != null && cavityForCell.room != null)
			{
				foreach (KPrefabID kprefabID in cavityForCell.buildings)
				{
					if (!kprefabID.IsNullOrDestroyed())
					{
						CritterCondo.Instance smi2 = kprefabID.GetSMI<CritterCondo.Instance>();
						if (smi2 != null && kprefabID.HasTag(smi.def.condoPrefabTag) && smi2.CanBeReserved())
						{
							pooledList.Add(smi2);
						}
					}
				}
			}
			IL_0152:
			Navigator component = smi.GetComponent<Navigator>();
			int num2 = -1;
			foreach (CritterCondo.Instance instance2 in pooledList)
			{
				int interactStartCell = instance2.GetInteractStartCell();
				int navigationCost = component.GetNavigationCost(interactStartCell);
				if (navigationCost != -1 && (navigationCost < num2 || num2 == -1))
				{
					num2 = navigationCost;
					smi.targetCondo = instance2;
				}
			}
		}
	}

	public GameStateMachine<CritterCondoInteractMontior, CritterCondoInteractMontior.Instance, IStateMachineTarget, CritterCondoInteractMontior.Def>.State lookingForCondo;

	public GameStateMachine<CritterCondoInteractMontior, CritterCondoInteractMontior.Instance, IStateMachineTarget, CritterCondoInteractMontior.Def>.State satisfied;

	private StateMachine<CritterCondoInteractMontior, CritterCondoInteractMontior.Instance, IStateMachineTarget, CritterCondoInteractMontior.Def>.FloatParameter remainingSecondsForEffect;

	public class Def : StateMachine.BaseDef
	{
		public bool requireCavity = true;

		public Tag condoPrefabTag = "CritterCondo";
	}

	public new class Instance : GameStateMachine<CritterCondoInteractMontior, CritterCondoInteractMontior.Instance, IStateMachineTarget, CritterCondoInteractMontior.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CritterCondoInteractMontior.Def def)
			: base(master, def)
		{
		}

		public CritterCondo.Instance targetCondo;
	}
}
