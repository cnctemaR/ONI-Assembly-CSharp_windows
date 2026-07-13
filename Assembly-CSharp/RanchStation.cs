using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RanchStation : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Operational;
		this.Unoperational.TagTransition(GameTags.Operational, this.Operational, false);
		this.Operational.TagTransition(GameTags.Operational, this.Unoperational, true).ToggleChore((RanchStation.Instance smi) => smi.CreateChore(), new Action<RanchStation.Instance, Chore>(RanchStation.SetRemoteChore), this.Unoperational, this.Unoperational).Update("FindRanachable", delegate(RanchStation.Instance smi, float dt)
		{
			smi.FindRanchable(null);
		}, UpdateRate.SIM_200ms, false);
	}

	private static void SetRemoteChore(RanchStation.Instance smi, Chore chore)
	{
		smi.remoteChore.SetChore(chore);
	}

	public StateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.BoolParameter RancherIsReady;

	public GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State Unoperational;

	public RanchStation.OperationalState Operational;

	public class Def : StateMachine.BaseDef
	{
		public Func<GameObject, RanchStation.Instance, bool> IsCritterEligibleToBeRanchedCb;

		public Action<GameObject, WorkerBase> OnRanchCompleteCb;

		public Action<RanchedStates.Instance, Workable> OnRanchWorkBegins;

		public Action<GameObject, float, Workable> OnRanchWorkTick;

		public HashedString RanchedPreAnim = "idle_loop";

		public HashedString RanchedLoopAnim = "idle_loop";

		public HashedString RanchedPstAnim = "idle_loop";

		public HashedString RanchedAbortAnim = "idle_loop";

		public HashedString RancherInteractAnim = "anim_interacts_rancherstation_kanim";

		public bool RancherWipesBrowAnim;

		public StatusItem RanchingStatusItem = Db.Get().DuplicantStatusItems.Ranching;

		public StatusItem CreatureRanchingStatusItem = Db.Get().CreatureStatusItems.GettingRanched;

		public float WorkTime = 12f;

		public Func<RanchStation.Instance, int> GetTargetRanchCell = (RanchStation.Instance smi) => Grid.PosToCell(smi);
	}

	public class OperationalState : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State
	{
	}

	public new class Instance : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.GameInstance
	{
		public RanchedStates.Instance ActiveRanchable
		{
			get
			{
				return this.activeRanchable;
			}
		}

		private bool isCritterAvailableForRanching
		{
			get
			{
				return this.targetRanchables.Count > 0;
			}
		}

		public bool IsCritterAvailableForRanching
		{
			get
			{
				this.ValidateTargetRanchables();
				return this.isCritterAvailableForRanching;
			}
		}

		public bool HasRancher
		{
			get
			{
				return this.rancher != null;
			}
		}

		public bool IsRancherReady
		{
			get
			{
				return base.sm.RancherIsReady.Get(this);
			}
		}

		public Extents StationExtents
		{
			get
			{
				return this.station.GetExtents();
			}
		}

		public int GetRanchNavTarget()
		{
			return base.def.GetTargetRanchCell(this);
		}

		public Instance(IStateMachineTarget master, RanchStation.Def def)
			: base(master, def)
		{
			base.gameObject.AddOrGet<RancherChore.RancherWorkable>();
			this.station = base.GetComponent<BuildingComplete>();
		}

		public Chore CreateChore()
		{
			RancherChore rancherChore = new RancherChore(base.GetComponent<KPrefabID>());
			StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.TargetParameter targetParameter = rancherChore.smi.sm.rancher;
			StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.Parameter<GameObject>.Context context = targetParameter.GetContext(rancherChore.smi);
			context.onDirty = (Action<RancherChore.RancherChoreStates.Instance>)Delegate.Combine(context.onDirty, new Action<RancherChore.RancherChoreStates.Instance>(this.OnRancherChanged));
			this.rancher = targetParameter.Get<WorkerBase>(rancherChore.smi);
			return rancherChore;
		}

		public int GetTargetRanchCell()
		{
			return base.def.GetTargetRanchCell(this);
		}

		public override void StartSM()
		{
			base.StartSM();
			this.onRoomUpdatedHandle = base.Subscribe(144050788, new Action<object>(this.OnRoomUpdated));
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.GetTargetRanchCell());
			if (cavityForCell != null && cavityForCell.room != null)
			{
				this.OnRoomUpdated(cavityForCell.room);
			}
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			base.Unsubscribe(ref this.onRoomUpdatedHandle);
		}

		private void OnRoomUpdated(object data)
		{
			if (data == null)
			{
				return;
			}
			this.ranch = data as Room;
			if (this.ranch.roomType != Db.Get().RoomTypes.CreaturePen)
			{
				this.TriggerRanchStationNoLongerAvailable();
				this.ranch = null;
			}
		}

		private void OnRancherChanged(RancherChore.RancherChoreStates.Instance choreInstance)
		{
			this.rancher = choreInstance.sm.rancher.Get<WorkerBase>(choreInstance);
			this.TriggerRanchStationNoLongerAvailable();
		}

		public bool TryGetRanched(RanchedStates.Instance ranchable)
		{
			return this.activeRanchable == null || this.activeRanchable == ranchable;
		}

		public void MessageCreatureArrived(RanchedStates.Instance critter)
		{
			this.activeRanchable = critter;
			base.sm.RancherIsReady.Set(false, this, false);
			base.Trigger(-1357116271, null);
		}

		public void MessageRancherReady()
		{
			base.sm.RancherIsReady.Set(true, base.smi, false);
			this.MessageRanchables(GameHashes.RancherReadyAtRanchStation);
		}

		private bool CanRanchableBeRanchedAtRanchStation(RanchableMonitor.Instance ranchable)
		{
			bool flag = !ranchable.IsNullOrStopped();
			if (flag && ranchable.TargetRanchStation != null && ranchable.TargetRanchStation != this)
			{
				flag = !ranchable.TargetRanchStation.IsRunning() || !ranchable.TargetRanchStation.HasRancher;
			}
			flag = flag && base.def.IsCritterEligibleToBeRanchedCb(ranchable.gameObject, this);
			flag = flag && ranchable.ChoreConsumer.IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>();
			if (flag)
			{
				int num = Grid.PosToCell(ranchable.transform.GetPosition());
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
				if (cavityForCell == null || this.ranch == null || cavityForCell != this.ranch.cavity)
				{
					flag = false;
				}
				else
				{
					int num2 = this.GetRanchNavTarget();
					if (ranchable.HasTag(GameTags.Creatures.Flyer))
					{
						num2 = Grid.CellAbove(num2);
					}
					flag = ranchable.NavComponent.GetNavigationCost(num2) != -1;
				}
			}
			return flag;
		}

		public void ValidateTargetRanchables()
		{
			if (!this.HasRancher)
			{
				return;
			}
			List<RanchableMonitor.Instance> list = CollectionPool<List<RanchableMonitor.Instance>, RanchableMonitor.Instance>.Get();
			list.AddRange(this.targetRanchables);
			foreach (RanchableMonitor.Instance instance in list)
			{
				if (instance.States == null || !this.CanRanchableBeRanchedAtRanchStation(instance))
				{
					this.Abandon(instance);
				}
			}
			CollectionPool<List<RanchableMonitor.Instance>, RanchableMonitor.Instance>.Release(list);
		}

		public void FindRanchable(object _ = null)
		{
			if (this.ranch == null)
			{
				return;
			}
			this.ValidateTargetRanchables();
			if (this.targetRanchables.Count == 2)
			{
				return;
			}
			List<KPrefabID> creatures = this.ranch.cavity.creatures;
			if (this.HasRancher && !this.isCritterAvailableForRanching && creatures.Count == 0)
			{
				this.TryNotifyEmptyRanch();
			}
			for (int i = 0; i < creatures.Count; i++)
			{
				KPrefabID kprefabID = creatures[i];
				if (!(kprefabID == null))
				{
					RanchableMonitor.Instance smi = kprefabID.GetSMI<RanchableMonitor.Instance>();
					if (!this.targetRanchables.Contains(smi) && this.CanRanchableBeRanchedAtRanchStation(smi) && smi != null)
					{
						smi.States.SetRanchStation(this);
						this.targetRanchables.Add(smi);
						return;
					}
				}
			}
		}

		public Option<CavityInfo> GetCavityInfo()
		{
			if (this.ranch.IsNullOrDestroyed())
			{
				return Option.None;
			}
			return this.ranch.cavity;
		}

		public void RanchCreature()
		{
			if (this.activeRanchable.IsNullOrStopped())
			{
				return;
			}
			global::Debug.Assert(this.activeRanchable != null, "targetRanchable was null");
			global::Debug.Assert(this.activeRanchable.GetMaster() != null, "GetMaster was null");
			global::Debug.Assert(base.def != null, "def was null");
			global::Debug.Assert(base.def.OnRanchCompleteCb != null, "onRanchCompleteCb cb was null");
			base.def.OnRanchCompleteCb(this.activeRanchable.gameObject, this.rancher);
			this.targetRanchables.Remove(this.activeRanchable.Monitor);
			this.activeRanchable.Trigger(1827504087, null);
			this.activeRanchable = null;
			this.FindRanchable(null);
		}

		public void TriggerRanchStationNoLongerAvailable()
		{
			for (int i = this.targetRanchables.Count - 1; i >= 0; i--)
			{
				RanchableMonitor.Instance instance = this.targetRanchables[i];
				if (instance.IsNullOrStopped() || instance.States.IsNullOrStopped())
				{
					this.targetRanchables.RemoveAt(i);
				}
				else
				{
					this.targetRanchables.Remove(instance);
					instance.Trigger(1689625967, null);
				}
			}
			global::Debug.Assert(this.targetRanchables.Count == 0, "targetRanchables is not empty");
			this.activeRanchable = null;
			base.sm.RancherIsReady.Set(false, this, false);
		}

		public void MessageRanchables(GameHashes hash)
		{
			for (int i = 0; i < this.targetRanchables.Count; i++)
			{
				RanchableMonitor.Instance instance = this.targetRanchables[i];
				if (!instance.IsNullOrStopped())
				{
					Game.BrainScheduler.PrioritizeBrain(instance.GetComponent<CreatureBrain>());
					if (!instance.States.IsNullOrStopped())
					{
						instance.Trigger((int)hash, null);
					}
				}
			}
		}

		public void Abandon(RanchableMonitor.Instance critter)
		{
			if (critter == null)
			{
				global::Debug.LogWarning("Null critter trying to abandon ranch station");
				this.targetRanchables.Remove(critter);
				return;
			}
			critter.TargetRanchStation = null;
			if (this.targetRanchables.Remove(critter))
			{
				if (critter.States == null)
				{
					return;
				}
				bool flag = !this.isCritterAvailableForRanching;
				if (critter.States == this.activeRanchable)
				{
					flag = true;
					this.activeRanchable = null;
				}
				if (flag)
				{
					this.TryNotifyEmptyRanch();
				}
			}
		}

		private void TryNotifyEmptyRanch()
		{
			if (!this.HasRancher)
			{
				return;
			}
			this.rancher.Trigger(-364750427, null);
		}

		public bool IsCritterInQueue(RanchableMonitor.Instance critter)
		{
			return this.targetRanchables.Contains(critter);
		}

		public List<RanchableMonitor.Instance> DEBUG_GetTargetRanchables()
		{
			return this.targetRanchables;
		}

		[MyCmpAdd]
		public ManuallySetRemoteWorkTargetComponent remoteChore;

		private const int QUEUE_SIZE = 2;

		private List<RanchableMonitor.Instance> targetRanchables = new List<RanchableMonitor.Instance>();

		private RanchedStates.Instance activeRanchable;

		private Room ranch;

		private WorkerBase rancher;

		private BuildingComplete station;

		private int onRoomUpdatedHandle = -1;
	}
}
