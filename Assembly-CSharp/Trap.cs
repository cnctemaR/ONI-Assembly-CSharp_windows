using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Trap : StateMachineComponent<Trap.StatesInstance>
{
	private void SetStoredPosition(GameObject go)
	{
		go.transform.position = Grid.CellToPosCBC(Grid.PosToCell(base.transform.position), Grid.SceneLayer.BuildingBack);
	}

	private static void CreateStatusItems()
	{
		if (Trap.statusSprung == null)
		{
			Trap.statusReady = new StatusItem("Ready", BUILDING.STATUSITEMS.CREATURE_TRAP.READY.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.READY.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			Trap.statusSprung = new StatusItem("Sprung", BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			Trap.statusSprung.resolveTooltipCallback = delegate(string str, object obj)
			{
				Trap.StatesInstance statesInstance = (Trap.StatesInstance)obj;
				return string.Format(str, statesInstance.master.contents.Get().GetProperName());
			};
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.contents = new Ref<KPrefabID>();
		Trap.CreateStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage component = base.GetComponent<Storage>();
		List<GameObject> items = component.items;
		foreach (GameObject gameObject in items)
		{
			this.SetStoredPosition(gameObject);
		}
		base.smi.StartSM();
		if (!component.IsEmpty())
		{
			KPrefabID component2 = component.items[0].GetComponent<KPrefabID>();
			if (component2 != null)
			{
				this.contents.Set(component2);
				base.smi.GoTo(base.smi.sm.occupied);
			}
			else
			{
				component.DropAll();
			}
		}
	}

	public KPrefabID GetContents()
	{
		return this.contents.Get();
	}

	[MyCmpReq]
	private UserMenu userMenu;

	[Serialize]
	private Ref<KPrefabID> contents;

	public TagSet captureTags = new TagSet();

	private static StatusItem statusReady;

	private static StatusItem statusSprung;

	public class StatesInstance : GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.GameInstance
	{
		public StatesInstance(Trap master)
			: base(master)
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Trap", base.gameObject, Grid.PosToCell(base.gameObject), GameScenePartitioner.Instance.trapsLayer, new Action<object>(this.OnCreatureOnTrap));
		}

		public void OnCreatureOnTrap(object data)
		{
			Storage component = base.master.GetComponent<Storage>();
			if (!component.IsEmpty())
			{
				return;
			}
			Trappable trappable = (Trappable)data;
			KPrefabID component2 = trappable.GetComponent<KPrefabID>();
			Tag baggedCreatureTag = EntityTemplates.GetBaggedCreatureTag(component2.PrefabTag);
			GameObject prefab = Assets.GetPrefab(baggedCreatureTag);
			GameObject gameObject = Util.KInstantiate(prefab, Folder.Entities);
			KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
			base.master.contents.Set(component3);
			gameObject.SetActive(true);
			component.Store(gameObject, true, false, true);
			base.master.SetStoredPosition(gameObject);
			Util.KDestroyGameObject(trappable.gameObject);
			base.smi.sm.trapTriggered.Trigger(base.smi);
		}

		public override void StopSM(string reason)
		{
			this.DisableEvents();
			base.StopSM(reason);
		}

		public void DisableEvents()
		{
			if (this.partitionerEntry != null)
			{
				this.partitionerEntry.Release();
				this.partitionerEntry = null;
			}
		}

		private GameScenePartitionerEntry partitionerEntry;
	}

	public class States : GameStateMachine<Trap.States, Trap.StatesInstance, Trap>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.ready;
			base.serializable = false;
			Trap.CreateStatusItems();
			this.ready.OnSignal(this.trapTriggered, this.trapping).ToggleStatusItem(Trap.statusReady, null).Exit(delegate(Trap.StatesInstance smi)
			{
				smi.DisableEvents();
			});
			this.trapping.PlayAnim("working_pre").Enter(delegate(Trap.StatesInstance smi)
			{
				KPrefabID kprefabID = smi.master.contents.Get();
				if (kprefabID != null)
				{
					kprefabID.GetComponent<KBatchedAnimController>().Play("working_idle", KAnim.PlayMode.Once, 1f, 0f);
				}
			}).OnAnimQueueComplete(this.occupied);
			this.occupied.ToggleTag(GameTags.Trapped).ToggleStatusItem(Trap.statusSprung, (Trap.StatesInstance smi) => smi).DefaultState(this.occupied.idle)
				.EventTransition(GameHashes.OnStorageChange, this.finishedUsing, (Trap.StatesInstance smi) => smi.master.GetComponent<Storage>().IsEmpty());
			this.occupied.idle.PlayAnim("working_idle_loop").Enter(delegate(Trap.StatesInstance smi)
			{
				smi.master.contents.Get().GetComponent<KBatchedAnimController>().Play("working_idle_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}).ScheduleGoTo((Trap.StatesInstance smi) => 7f + global::UnityEngine.Random.value * 5f, this.occupied.reacting);
			this.occupied.reacting.PlayAnim("working_loop").Enter(delegate(Trap.StatesInstance smi)
			{
				KPrefabID kprefabID2 = smi.master.contents.Get();
				if (kprefabID2 != null)
				{
					kprefabID2.GetComponent<KBatchedAnimController>().Play("working_loop", KAnim.PlayMode.Once, 1f, 0f);
				}
			}).OnAnimQueueComplete(this.occupied.idle);
			this.finishedUsing.PlayAnim("working_pst").OnAnimQueueComplete(this.destroySelf);
			this.destroySelf.Enter(delegate(Trap.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}

		public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State ready;

		public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State trapping;

		public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State finishedUsing;

		public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State destroySelf;

		public StateMachine<Trap.States, Trap.StatesInstance, Trap, object>.Signal trapTriggered;

		public Trap.States.OccupiedStates occupied;

		public class OccupiedStates : GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State
		{
			public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State idle;

			public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State reacting;
		}
	}
}
