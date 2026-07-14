using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class EggProtectionMonitor : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.find_egg;
		this.find_egg.BatchUpdate(new UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.BatchUpdateDelegate(EggProtectionMonitor.Instance.FindEggToGuard), UpdateRate.SIM_200ms).ParamTransition<bool>(this.hasEggToGuard, this.guard.safe, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsTrue);
		this.guard.Enter(delegate(EggProtectionMonitor.Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(smi.def.build), smi.def.animPrefix, "_heat", 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
			smi.threatMonitor.attackOwnFaction = true;
		}).Exit(delegate(EggProtectionMonitor.Instance smi)
		{
			if (!smi.def.animPrefix.IsNullOrWhiteSpace())
			{
				smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(smi.def.build), smi.def.animPrefix, null, 0);
			}
			else
			{
				smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim(smi.def.build).GetData(), 0);
			}
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(smi.def.defaultFaction);
			smi.threatMonitor.attackOwnFaction = false;
		}).Update("CanProtectEgg", delegate(EggProtectionMonitor.Instance smi, float dt)
		{
			smi.CanProtectEgg();
		}, UpdateRate.SIM_1000ms, true)
			.ParamTransition<bool>(this.hasEggToGuard, this.find_egg, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsFalse);
		this.guard.safe.Enter(delegate(EggProtectionMonitor.Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("EggProtectionMonitor.safe", delegate(EggProtectionMonitor.Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_200ms, true).ToggleStatusItem(CREATURES.STATUSITEMS.PROTECTINGENTITY.NAME, CREATURES.STATUSITEMS.PROTECTINGENTITY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, null);
		this.guard.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (EggProtectionMonitor.Instance smi) => smi.threatMonitor.HasThreat(), delegate(EggProtectionMonitor.Instance smi)
		{
			smi.GoTo(this.guard.safe);
		}).Update("Threatened", new Action<EggProtectionMonitor.Instance, float>(EggProtectionMonitor.CritterUpdateThreats), UpdateRate.SIM_200ms, false);
	}

	private static void CritterUpdateThreats(EggProtectionMonitor.Instance smi, float dt)
	{
		if (smi.isMasterNull)
		{
			return;
		}
		if (!smi.threatMonitor.HasThreat())
		{
			smi.GoTo(smi.sm.guard.safe);
		}
	}

	public StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.BoolParameter hasEggToGuard;

	public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State find_egg;

	public EggProtectionMonitor.GuardEggStates guard;

	public class Def : StateMachine.BaseDef
	{
		public Tag[] allyTags;

		public string animPrefix;

		public string build = "pincher_build_kanim";

		public FactionManager.FactionID defaultFaction = FactionManager.FactionID.Pest;

		public List<Tag> eggTags = new List<Tag>
		{
			"CrabEgg".ToTag(),
			"CrabWoodEgg".ToTag(),
			"CrabFreshWaterEgg".ToTag()
		};
	}

	public class GuardEggStates : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State
	{
		public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State safe;

		public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State threatened;
	}

	public new class Instance : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, EggProtectionMonitor.Def def)
			: base(master, def)
		{
			this.navigator = master.GetComponent<Navigator>();
			this.refreshThreatDelegate = new Action<object>(this.RefreshThreat);
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if (this.eggToProtect == null)
			{
				flag = false;
			}
			if (flag)
			{
				int num = 150;
				int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(this.eggToProtect));
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

		public static void FindEggToGuard(List<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry> instances, float time_delta)
		{
			if (instances.Count == 0)
			{
				return;
			}
			ListPool<KPrefabID, EggProtectionMonitor>.PooledList pooledList = ListPool<KPrefabID, EggProtectionMonitor>.Allocate();
			pooledList.Capacity = Mathf.Max(pooledList.Capacity, Components.IncubationMonitors.Count);
			foreach (object obj in Components.IncubationMonitors)
			{
				IncubationMonitor.Instance instance = (IncubationMonitor.Instance)obj;
				pooledList.Add(instance.gameObject.GetComponent<KPrefabID>());
			}
			ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.PooledList pooledList2 = ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.Allocate();
			ListPool<Tag, EggProtectionMonitor>.PooledList pooledList3 = ListPool<Tag, EggProtectionMonitor>.Allocate();
			foreach (UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry entry in instances)
			{
				foreach (Tag tag in entry.data.def.eggTags)
				{
					if (!pooledList3.Contains(tag))
					{
						pooledList3.Add(tag);
					}
				}
			}
			EggProtectionMonitor.Instance.find_eggs_job.Reset(pooledList);
			for (int i = 0; i < pooledList.Count; i += 256)
			{
				EggProtectionMonitor.Instance.find_eggs_job.Add(new EggProtectionMonitor.Instance.FindEggsTask(i, Mathf.Min(i + 256, pooledList.Count), pooledList3));
			}
			GlobalJobManager.Run(EggProtectionMonitor.Instance.find_eggs_job);
			for (int num = 0; num != EggProtectionMonitor.Instance.find_eggs_job.Count; num++)
			{
				EggProtectionMonitor.Instance.find_eggs_job.GetWorkItem(num).Finish(pooledList, pooledList2);
			}
			pooledList.Recycle();
			EggProtectionMonitor.Instance.find_eggs_job.Reset(null);
			ListPool<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry, EggProtectionMonitor>.PooledList pooledList4 = ListPool<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry, EggProtectionMonitor>.Allocate();
			pooledList4.AddRange(instances);
			foreach (UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry entry2 in pooledList4)
			{
				GameObject gameObject = null;
				int num2 = 100;
				List<Tag> eggTags = entry2.data.def.eggTags;
				foreach (EggProtectionMonitor.Instance.Egg egg in pooledList2)
				{
					if (eggTags.Contains(egg.tag))
					{
						int navigationCost = entry2.data.navigator.GetNavigationCost(egg.cell);
						if (navigationCost != -1 && navigationCost < num2)
						{
							gameObject = egg.game_object;
							num2 = navigationCost;
						}
					}
				}
				entry2.data.SetEggToGuard(gameObject);
			}
			pooledList4.Recycle();
			pooledList3.Recycle();
			pooledList2.Recycle();
		}

		public void SetEggToGuard(GameObject egg)
		{
			this.eggToProtect = egg;
			base.sm.hasEggToGuard.Set(egg != null, base.smi, false);
		}

		public void GoToThreatened()
		{
			base.smi.GoTo(base.sm.guard.threatened);
		}

		public void RefreshThreat(object data)
		{
			if (!base.IsRunning() || this.eggToProtect == null)
			{
				return;
			}
			if (base.smi.threatMonitor.HasThreat())
			{
				this.GoToThreatened();
				return;
			}
			if (base.smi.GetCurrentState() != base.sm.guard.safe)
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.guard.safe);
			}
		}

		[MySmiReq]
		public ThreatMonitor.Instance threatMonitor;

		public GameObject eggToProtect;

		private Navigator navigator;

		private Action<object> refreshThreatDelegate;

		private static WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>> find_eggs_job = new WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>>();

		private struct Egg
		{
			public GameObject game_object;

			public int cell;

			public Tag tag;
		}

		private struct FindEggsTask : IWorkItem<List<KPrefabID>>
		{
			public FindEggsTask(int start, int end, List<Tag> eggTags)
			{
				this.start = start;
				this.end = end;
				this.eggTags = eggTags;
				this.eggs = ListPool<int, EggProtectionMonitor>.Allocate();
			}

			public void Run(List<KPrefabID> prefab_ids, int threadIndex)
			{
				for (int num = this.start; num != this.end; num++)
				{
					if (this.eggTags.Contains(prefab_ids[num].PrefabTag))
					{
						this.eggs.Add(num);
					}
				}
			}

			public void Finish(List<KPrefabID> prefab_ids, List<EggProtectionMonitor.Instance.Egg> eggs)
			{
				foreach (int num in this.eggs)
				{
					KPrefabID kprefabID = prefab_ids[num];
					GameObject gameObject = kprefabID.gameObject;
					eggs.Add(new EggProtectionMonitor.Instance.Egg
					{
						game_object = gameObject,
						cell = Grid.PosToCell(gameObject),
						tag = kprefabID.PrefabTag
					});
				}
				this.eggs.Recycle();
			}

			private List<Tag> eggTags;

			private ListPool<int, EggProtectionMonitor>.PooledList eggs;

			private int start;

			private int end;
		}
	}
}
