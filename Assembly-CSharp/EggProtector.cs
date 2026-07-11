using System;
using System.Collections;
using UnityEngine;

public class EggProtector : GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		this.idle.ParamTransition<bool>(this.hasEggToGuard, this.guarding.idle, GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.IsTrue).EventHandler(GameHashes.LayEgg, delegate(EggProtector.Instance smi)
		{
			smi.FindEggToGuard();
		}).Update(delegate(EggProtector.Instance smi, float dt)
		{
			smi.FindEggToGuard();
		}, UpdateRate.SIM_4000ms, false);
		this.guarding.Enter(delegate(EggProtector.Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat", 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(EggProtector.Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData(), 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).ParamTransition<bool>(this.hasEggToGuard, this.idle, GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.IsFalse)
			.Update(delegate(EggProtector.Instance smi, float dt)
			{
				smi.CanProtectEgg();
			}, UpdateRate.SIM_1000ms, false);
		this.guarding.idle.ParamTransition<bool>(this.needsToMoveCloser, this.guarding.return_to_egg, GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.IsTrue);
		this.guarding.return_to_egg.MoveTo((EggProtector.Instance smi) => smi.GetEggPos(), null, null, true).ParamTransition<bool>(this.needsToMoveCloser, this.guarding.idle, GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.IsFalse);
	}

	public StateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.BoolParameter needsToMoveCloser;

	public StateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.BoolParameter hasEggToGuard;

	public GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.State idle;

	public EggProtector.GuardingStates guarding;

	public class Def : StateMachine.BaseDef
	{
		public Def(Tag tag, bool shouldProtect)
		{
			this.protectorTag = tag;
			this.shouldProtect = shouldProtect;
		}

		public Tag protectorTag;

		public bool shouldProtect;
	}

	public new class Instance : GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.GameInstance
	{
		public Instance(Chore<EggProtector.Instance> chore, EggProtector.Def def)
			: base(chore, def)
		{
			EntityThreatMonitor.Instance smi = base.gameObject.GetSMI<EntityThreatMonitor.Instance>();
			smi.allyTag = def.protectorTag;
		}

		public void CheckDistanceToEgg()
		{
			Navigator component = base.smi.GetComponent<Navigator>();
			int navigationCost = component.GetNavigationCost(Grid.PosToCell(this.eggToGuard));
			if (navigationCost > 20)
			{
				base.sm.needsToMoveCloser.Set(true, base.smi);
			}
			else if (navigationCost < 0)
			{
				base.sm.needsToMoveCloser.Set(false, base.smi);
			}
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if (this.eggToGuard == null)
			{
				flag = false;
			}
			Navigator component = base.smi.GetComponent<Navigator>();
			if (flag)
			{
				int num = 150;
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(this.eggToGuard));
				if (navigationCost == -1 || navigationCost >= num)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				this.SetEggToGuard(null);
			}
		}

		public void FindEggToGuard()
		{
			if (!base.def.shouldProtect)
			{
				return;
			}
			GameObject gameObject = null;
			int num = 100;
			Navigator component = base.smi.GetComponent<Navigator>();
			IEnumerator enumerator = Components.Pickupables.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Pickupable pickupable = (Pickupable)obj;
					if (pickupable.HasTag("CrabEgg".ToTag()))
					{
						if (Vector2.Distance(base.smi.transform.position, pickupable.transform.position) <= 25f)
						{
							int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable));
							if (navigationCost != -1 && navigationCost < num)
							{
								gameObject = pickupable.gameObject;
								num = navigationCost;
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			this.SetEggToGuard(gameObject);
		}

		public void SetEggToGuard(GameObject egg)
		{
			this.eggToGuard = egg;
			EntityThreatMonitor.Instance smi = base.gameObject.GetSMI<EntityThreatMonitor.Instance>();
			smi.entityToProtect = egg;
			base.sm.hasEggToGuard.Set(egg != null, base.smi);
		}

		public int GetEggPos()
		{
			return Grid.PosToCell(this.eggToGuard);
		}

		public GameObject eggToGuard;
	}

	public class GuardingStates : GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.State
	{
		public GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.State idle;

		public GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>.State return_to_egg;
	}
}
