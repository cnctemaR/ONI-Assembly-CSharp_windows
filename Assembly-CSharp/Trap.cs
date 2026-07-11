using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Trap : StateMachineComponent<Trap.StatesInstance>
{
	private void SetStoredPosition(GameObject go)
	{
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(base.transform.GetPosition()), Grid.SceneLayer.BuildingBack);
		vector.x += this.trappedOffset.x;
		vector.y += this.trappedOffset.y;
		go.transform.SetPosition(vector);
		go.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingBack);
	}

	private static void CreateStatusItems()
	{
		if (Trap.statusSprung == null)
		{
			Trap.statusReady = new StatusItem("Ready", BUILDING.STATUSITEMS.CREATURE_TRAP.READY.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.READY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
			Trap.statusSprung = new StatusItem("Sprung", BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, 129022);
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
		foreach (GameObject gameObject in component.items)
		{
			this.SetStoredPosition(gameObject);
			KBoxCollider2D component2 = gameObject.GetComponent<KBoxCollider2D>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
		}
		base.smi.StartSM();
		if (!component.IsEmpty())
		{
			KPrefabID component3 = component.items[0].GetComponent<KPrefabID>();
			if (component3 != null)
			{
				this.contents.Set(component3);
				base.smi.GoTo(base.smi.sm.occupied);
				return;
			}
			component.DropAll(false, false, default(Vector3), true);
		}
	}

	public KPrefabID GetContents()
	{
		return this.contents.Get();
	}

	public Tag[] trappableCreatures;

	public Vector2 trappedOffset = Vector2.zero;

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
			if (trappable.HasTag(GameTags.Stored))
			{
				return;
			}
			if (trappable.HasTag(GameTags.Trapped))
			{
				return;
			}
			if (trappable.HasTag(GameTags.Creatures.Bagged))
			{
				return;
			}
			bool flag = false;
			foreach (Tag tag in base.master.trappableCreatures)
			{
				if (trappable.HasTag(tag))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			KPrefabID component2 = trappable.GetComponent<KPrefabID>();
			base.master.contents.Set(component2);
			component.Store(trappable.gameObject, true, false, true, false);
			base.master.SetStoredPosition(trappable.gameObject);
			base.smi.sm.trapTriggered.Trigger(base.smi);
		}

		public override void StopSM(string reason)
		{
			this.DisableEvents();
			base.StopSM(reason);
		}

		public void DisableEvents()
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		}

		private HandleVector<int>.Handle partitionerEntry;
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
			this.trapping.PlayAnim("working_pre").OnAnimQueueComplete(this.occupied);
			this.occupied.ToggleTag(GameTags.Trapped).ToggleStatusItem(Trap.statusSprung, (Trap.StatesInstance smi) => smi).DefaultState(this.occupied.idle)
				.EventTransition(GameHashes.OnStorageChange, this.finishedUsing, (Trap.StatesInstance smi) => smi.master.GetComponent<Storage>().IsEmpty());
			this.occupied.idle.PlayAnim("working_loop", KAnim.PlayMode.Loop);
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
		}
	}
}
