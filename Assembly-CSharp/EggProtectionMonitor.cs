using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggProtectionMonitor : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.find_egg;
		this.root.EventHandler(GameHashes.ObjectDestroyed, delegate(EggProtectionMonitor.Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		this.find_egg.BatchUpdate(new UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.BatchUpdateDelegate(EggProtectionMonitor.Instance.FindEggToGuard), UpdateRate.SIM_200ms).ParamTransition<bool>(this.hasEggToGuard, this.guard.safe, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsTrue);
		this.guard.Enter(delegate(EggProtectionMonitor.Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat", 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(EggProtectionMonitor.Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData(), 0);
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).Update("evaulate_egg", delegate(EggProtectionMonitor.Instance smi, float dt)
		{
			smi.CanProtectEgg();
		}, UpdateRate.SIM_1000ms, true)
			.ParamTransition<bool>(this.hasEggToGuard, this.find_egg, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsFalse);
		this.guard.safe.Enter(delegate(EggProtectionMonitor.Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("safe", delegate(EggProtectionMonitor.Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_200ms, true);
		this.guard.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (EggProtectionMonitor.Instance smi) => smi.MainThreat != null, delegate(EggProtectionMonitor.Instance smi)
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
		if (!smi.CheckForThreats())
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
			this.alignment = master.GetComponent<FactionAlignment>();
			this.navigator = master.GetComponent<Navigator>();
			this.refreshThreatDelegate = new Action<object>(this.RefreshThreat);
		}

		public GameObject MainThreat
		{
			get
			{
				return this.mainThreat;
			}
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
			ListPool<KPrefabID, EggProtectionMonitor>.PooledList pooledList = ListPool<KPrefabID, EggProtectionMonitor>.Allocate();
			pooledList.Capacity = Mathf.Max(pooledList.Capacity, Components.Pickupables.Count);
			IEnumerator enumerator = Components.IncubationMonitors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					IncubationMonitor.Instance instance = (IncubationMonitor.Instance)obj;
					pooledList.Add(instance.gameObject.GetComponent<KPrefabID>());
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
			ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.PooledList pooledList2 = ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.Allocate();
			EggProtectionMonitor.Instance.find_eggs_job.Reset(pooledList);
			for (int i = 0; i < pooledList.Count; i += 256)
			{
				EggProtectionMonitor.Instance.find_eggs_job.Add(new EggProtectionMonitor.Instance.FindEggsTask(i, Mathf.Min(i + 256, pooledList.Count)));
			}
			GlobalJobManager.Run(EggProtectionMonitor.Instance.find_eggs_job);
			for (int num = 0; num != EggProtectionMonitor.Instance.find_eggs_job.Count; num++)
			{
				EggProtectionMonitor.Instance.find_eggs_job.GetWorkItem(num).Finish(pooledList, pooledList2);
			}
			pooledList.Recycle();
			foreach (UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry entry in instances)
			{
				GameObject gameObject = null;
				int num2 = 100;
				foreach (EggProtectionMonitor.Instance.Egg egg in pooledList2)
				{
					int navigationCost = entry.data.navigator.GetNavigationCost(egg.cell);
					if (navigationCost != -1 && navigationCost < num2)
					{
						gameObject = egg.game_object;
						num2 = navigationCost;
					}
				}
				entry.data.SetEggToGuard(gameObject);
			}
			pooledList2.Recycle();
		}

		public void SetEggToGuard(GameObject egg)
		{
			this.eggToProtect = egg;
			base.sm.hasEggToGuard.Set(egg != null, base.smi);
		}

		public void SetMainThreat(GameObject threat)
		{
			if (threat == this.mainThreat)
			{
				return;
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
				if (threat == null)
				{
					base.Trigger(2144432245, null);
				}
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
			}
			this.mainThreat = threat;
			if (this.mainThreat != null)
			{
				this.mainThreat.Subscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Subscribe(1969584890, this.refreshThreatDelegate);
			}
		}

		public void Cleanup(object data)
		{
			if (this.mainThreat)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
			}
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
			bool flag = base.smi.CheckForThreats();
			if (flag)
			{
				this.GoToThreatened();
			}
			else if (base.smi.GetCurrentState() != base.sm.guard.safe)
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.guard.safe);
			}
		}

		public bool CheckForThreats()
		{
			if (this.eggToProtect == null)
			{
				return false;
			}
			GameObject gameObject = this.FindThreat();
			this.SetMainThreat(gameObject);
			return gameObject != null;
		}

		public GameObject FindThreat()
		{
			this.threats.Clear();
			ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
			Extents extents = new Extents(Grid.PosToCell(this.eggToProtect), this.maxThreatDistance);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
				FactionAlignment factionAlignment = scenePartitionerEntry.obj as FactionAlignment;
				if (!(factionAlignment.transform == null))
				{
					if (!(factionAlignment == this.alignment))
					{
						if (factionAlignment.IsAlignmentActive())
						{
							if (this.navigator.CanReach(factionAlignment.attackable))
							{
								bool flag = false;
								foreach (Tag tag in base.def.allyTags)
								{
									if (factionAlignment.HasTag(tag))
									{
										flag = true;
									}
								}
								if (!flag)
								{
									this.threats.Add(factionAlignment);
								}
							}
						}
					}
				}
			}
			pooledList.Recycle();
			return this.PickBestTarget(this.threats);
		}

		public GameObject PickBestTarget(List<FactionAlignment> threats)
		{
			float num = 1f;
			Vector2 vector = base.gameObject.transform.GetPosition();
			GameObject gameObject = null;
			float num2 = float.PositiveInfinity;
			for (int i = threats.Count - 1; i >= 0; i--)
			{
				FactionAlignment factionAlignment = threats[i];
				float num3 = Vector2.Distance(vector, factionAlignment.transform.GetPosition()) / num;
				if (num3 < num2)
				{
					num2 = num3;
					gameObject = factionAlignment.gameObject;
				}
			}
			return gameObject;
		}

		public GameObject eggToProtect;

		public FactionAlignment alignment;

		private Navigator navigator;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 12;

		private Action<object> refreshThreatDelegate;

		private static WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>> find_eggs_job = new WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>>();

		private struct Egg
		{
			public GameObject game_object;

			public int cell;
		}

		private struct FindEggsTask : IWorkItem<List<KPrefabID>>
		{
			public FindEggsTask(int start, int end)
			{
				this.start = start;
				this.end = end;
				this.eggs = ListPool<int, EggProtectionMonitor>.Allocate();
			}

			public void Run(List<KPrefabID> prefab_ids)
			{
				for (int num = this.start; num != this.end; num++)
				{
					if (prefab_ids[num].HasTag(EggProtectionMonitor.Instance.FindEggsTask.EGG_TAG))
					{
						this.eggs.Add(num);
					}
				}
			}

			public void Finish(List<KPrefabID> prefab_ids, List<EggProtectionMonitor.Instance.Egg> eggs)
			{
				foreach (int num in this.eggs)
				{
					GameObject gameObject = prefab_ids[num].gameObject;
					eggs.Add(new EggProtectionMonitor.Instance.Egg
					{
						game_object = gameObject,
						cell = Grid.PosToCell(gameObject)
					});
				}
				this.eggs.Recycle();
			}

			private static readonly Tag EGG_TAG = "CrabEgg".ToTag();

			private ListPool<int, EggProtectionMonitor>.PooledList eggs;

			private int start;

			private int end;
		}
	}
}
