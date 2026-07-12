using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RemoteWorkDock")]
public class RemoteWorkerDock : KMonoBehaviour
{
	public RemoteWorkerSM RemoteWorker
	{
		get
		{
			return this.remoteWorker;
		}
		private set
		{
			this.remoteWorker = value;
			this.worker = ((value != null) ? new Ref<KSelectable>(value.GetComponent<KSelectable>()) : null);
		}
	}

	public WorkerBase GetActiveTerminalWorker()
	{
		if (this.terminal == null)
		{
			return null;
		}
		return this.terminal.worker;
	}

	public bool IsOperational
	{
		get
		{
			return this.operational.IsOperational;
		}
	}

	private bool canWork(IRemoteDockWorkTarget provider)
	{
		int num;
		int num2;
		Grid.CellToXY(Grid.PosToCell(this), out num, out num2);
		int num3;
		int num4;
		Grid.CellToXY(provider.Approachable.GetCell(), out num3, out num4);
		return num2 == num4 && Math.Abs(num - num3) <= 12;
	}

	private void considerProvider(IRemoteDockWorkTarget provider)
	{
		if (this.canWork(provider))
		{
			this.providers.Add(provider);
		}
	}

	private void forgetProvider(IRemoteDockWorkTarget provider)
	{
		this.providers.Remove(provider);
	}

	private static string GenerateName()
	{
		string text = "";
		for (int i = 0; i < 3; i++)
		{
			text += "011223345789"[global::UnityEngine.Random.Range(0, "011223345789".Length)].ToString();
		}
		return BUILDINGS.PREFABS.REMOTEWORKERDOCK.NAME_FMT.Replace("{ID}", text);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UserNameable component = base.GetComponent<UserNameable>();
		if (component.savedName == "" || component.savedName == BUILDINGS.PREFABS.REMOTEWORKERDOCK.NAME)
		{
			component.SetName(RemoteWorkerDock.GenerateName());
		}
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		Components.RemoteWorkerDocks.Add(this.GetMyWorldId(), this);
		this.add_provider_binding = new Action<IRemoteDockWorkTarget>(this.considerProvider);
		this.remove_provider_binding = new Action<IRemoteDockWorkTarget>(this.forgetProvider);
		Components.RemoteDockWorkTargets.Register(this.GetMyWorldId(), this.add_provider_binding, this.remove_provider_binding);
		Ref<KSelectable> @ref = this.worker;
		RemoteWorkerSM remoteWorkerSM;
		if (@ref == null)
		{
			remoteWorkerSM = null;
		}
		else
		{
			KSelectable kselectable = @ref.Get();
			remoteWorkerSM = ((kselectable != null) ? kselectable.GetComponent<RemoteWorkerSM>() : null);
		}
		this.remoteWorker = remoteWorkerSM;
		if (this.remoteWorker == null)
		{
			this.RequestNewWorker(null);
			return;
		}
		this.remoteWorkerDestroyedEventId = this.remoteWorker.Subscribe(1969584890, new Action<object>(this.RequestNewWorker));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.RemoteWorkerDocks.Remove(this.GetMyWorldId(), this);
		Components.RemoteDockWorkTargets.Unregister(this.GetMyWorldId(), this.add_provider_binding, this.remove_provider_binding);
		if (this.remoteWorker != null)
		{
			this.remoteWorker.Unsubscribe(this.remoteWorkerDestroyedEventId);
		}
		if (this.newRemoteWorkerHandle.IsValid)
		{
			this.newRemoteWorkerHandle.ClearScheduler();
		}
	}

	public void CollectChores(ChoreConsumerState duplicant_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> incomplete_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		if (this.remoteWorker == null)
		{
			return;
		}
		ChoreConsumerState consumerState = this.remoteWorker.ConsumerState;
		consumerState.resume = duplicant_state.resume;
		foreach (IRemoteDockWorkTarget remoteDockWorkTarget in this.providers)
		{
			Chore remoteDockChore = remoteDockWorkTarget.RemoteDockChore;
			if (remoteDockChore != null)
			{
				remoteDockChore.CollectChores(consumerState, succeeded_contexts, incomplete_contexts, failed_contexts, false);
			}
		}
	}

	public bool AvailableForWorkBy(RemoteWorkTerminal terminal)
	{
		return this.terminal == null || this.terminal == terminal;
	}

	public bool HasWorker()
	{
		return this.remoteWorker != null;
	}

	public void SetNextChore(RemoteWorkTerminal terminal, Chore.Precondition.Context chore_context)
	{
		global::Debug.Assert(this.worker != null);
		global::Debug.Assert(this.terminal == null || this.terminal == terminal);
		this.terminal = terminal;
		if (this.remoteWorker != null)
		{
			this.remoteWorker.SetNextChore(chore_context);
		}
	}

	public bool StartWorking(RemoteWorkTerminal terminal)
	{
		if (this.terminal == null)
		{
			this.terminal = terminal;
		}
		if (this.terminal == terminal && this.remoteWorker != null)
		{
			this.remoteWorker.ActivelyControlled = true;
			return true;
		}
		return false;
	}

	public void StopWorking(RemoteWorkTerminal terminal)
	{
		if (terminal == this.terminal)
		{
			this.terminal = null;
			if (this.remoteWorker != null)
			{
				this.remoteWorker.ActivelyControlled = false;
			}
		}
	}

	public bool OnRemoteWorkTick(float dt)
	{
		return this.remoteWorker == null || (!this.remoteWorker.ActivelyWorking && !this.remoteWorker.HasChoreQueued());
	}

	private void OnStorageChanged(object _)
	{
		if (this.remoteWorker == null || this.worker.Get() == null)
		{
			this.RequestNewWorker(null);
		}
	}

	private void RequestNewWorker(object _ = null)
	{
		if (this.newRemoteWorkerHandle.IsValid)
		{
			return;
		}
		Tag build_MATERIAL_TAG = RemoteWorkerConfig.BUILD_MATERIAL_TAG;
		if (this.storage.FindFirstWithMass(build_MATERIAL_TAG, 200f) == null)
		{
			if (!this.activeFetch)
			{
				this.activeFetch = true;
				FetchList2 fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.Fetch);
				fetchList.Add(build_MATERIAL_TAG, null, 200f, Operational.State.None);
				fetchList.Submit(delegate
				{
					this.activeFetch = false;
					this.RequestNewWorker(null);
				}, true);
				return;
			}
		}
		else
		{
			this.MakeNewWorker(null);
		}
	}

	private void MakeNewWorker(object _ = null)
	{
		if (this.newRemoteWorkerHandle.IsValid)
		{
			return;
		}
		if (this.storage.GetAmountAvailable(RemoteWorkerConfig.BUILD_MATERIAL_TAG) < 200f)
		{
			return;
		}
		PrimaryElement elem = this.storage.FindFirstWithMass(RemoteWorkerConfig.BUILD_MATERIAL_TAG, 200f);
		if (elem == null)
		{
			return;
		}
		float temperature;
		SimUtil.DiseaseInfo disease;
		float num;
		this.storage.ConsumeAndGetDisease(elem.ElementID.CreateTag(), 200f, out num, out disease, out temperature);
		this.status_item_handle = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.RemoteWorkDockMakingWorker, null);
		this.newRemoteWorkerHandle = GameScheduler.Instance.Schedule("MakeRemoteWorker", 2f, delegate(object _)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(RemoteWorkerConfig.ID), this.transform.position, Grid.SceneLayer.Creatures, null, 0);
			if (this.remoteWorkerDestroyedEventId != -1 && this.remoteWorker != null)
			{
				this.remoteWorker.Unsubscribe(this.remoteWorkerDestroyedEventId);
			}
			this.RemoteWorker = gameObject.GetComponent<RemoteWorkerSM>();
			this.remoteWorker.HomeDepot = this;
			this.remoteWorker.playNewWorker = true;
			gameObject.SetActive(true);
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			component.ElementID = elem.ElementID;
			component.Temperature = temperature;
			if (disease.idx != 255)
			{
				component.AddDisease(disease.idx, disease.count, "Inherited from construction material");
			}
			this.remoteWorkerDestroyedEventId = gameObject.Subscribe(1969584890, new Action<object>(this.RequestNewWorker));
			this.newRemoteWorkerHandle.ClearScheduler();
			this.GetComponent<KSelectable>().RemoveStatusItem(this.status_item_handle, false);
		}, null, null);
	}

	[Serialize]
	protected Ref<KSelectable> worker;

	protected RemoteWorkerSM remoteWorker;

	private int remoteWorkerDestroyedEventId = -1;

	protected RemoteWorkTerminal terminal;

	[MyCmpGet]
	private Storage storage;

	[MyCmpGet]
	private Operational operational;

	[MyCmpAdd]
	private UserNameable nameable;

	[MyCmpAdd]
	private RemoteWorkerDock.NewWorker new_worker_;

	[MyCmpAdd]
	private RemoteWorkerDock.EnterableDock enter_;

	[MyCmpAdd]
	private RemoteWorkerDock.ExitableDock exit_;

	[MyCmpAdd]
	private RemoteWorkerDock.WorkerRecharger recharger_;

	[MyCmpAdd]
	private RemoteWorkerDock.WorkerGunkRemover gunk_remover_;

	[MyCmpAdd]
	private RemoteWorkerDock.WorkerOilRefiller oil_refiller_;

	private Guid status_item_handle;

	private SchedulerHandle newRemoteWorkerHandle;

	private List<IRemoteDockWorkTarget> providers = new List<IRemoteDockWorkTarget>();

	private Action<IRemoteDockWorkTarget> add_provider_binding;

	private Action<IRemoteDockWorkTarget> remove_provider_binding;

	private bool activeFetch;

	public class NewWorker : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workAnims = RemoteWorkerDock.NewWorker.WORK_ANIMS;
			this.workingPstComplete = null;
			this.workingPstFailed = null;
			this.workAnimPlayMode = KAnim.PlayMode.Once;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.resetProgressOnStop = true;
			KAnim.Anim anim = Assets.GetAnim(RemoteWorkerConfig.DOCK_ANIM_OVERRIDES).GetData().GetAnim("new_worker");
			base.SetWorkTime((float)anim.numFrames / anim.frameRate);
		}

		protected override void OnStartWork(WorkerBase worker)
		{
			base.OnStartWork(worker);
		}

		protected override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
			worker.GetComponent<RemoteWorkerSM>().Docked = true;
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "new_worker" };
	}

	public class EnterableDock : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workerStatusItem = Db.Get().DuplicantStatusItems.EnteringDock;
			this.workAnims = RemoteWorkerDock.EnterableDock.WORK_ANIMS;
			this.workingPstComplete = null;
			this.workingPstFailed = null;
			this.workAnimPlayMode = KAnim.PlayMode.Once;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.resetProgressOnStop = true;
			KAnim.Anim anim = Assets.GetAnim(RemoteWorkerConfig.DOCK_ANIM_OVERRIDES).GetData().GetAnim("enter_dock");
			base.SetWorkTime((float)anim.numFrames / anim.frameRate);
		}

		protected override void OnCompleteWork(WorkerBase worker)
		{
			worker.GetComponent<RemoteWorkerSM>().Docked = true;
			base.OnCompleteWork(worker);
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "enter_dock" };
	}

	public class ExitableDock : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workAnims = RemoteWorkerDock.ExitableDock.WORK_ANIMS;
			this.workingPstComplete = null;
			this.workingPstFailed = null;
			this.workAnimPlayMode = KAnim.PlayMode.Once;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.resetProgressOnStop = true;
			KAnim.Anim anim = Assets.GetAnim(RemoteWorkerConfig.DOCK_ANIM_OVERRIDES).GetData().GetAnim("exit_dock");
			base.SetWorkTime((float)anim.numFrames / anim.frameRate);
		}

		protected override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
			worker.GetComponent<RemoteWorkerSM>().Docked = false;
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "exit_dock" };
	}

	public class WorkerRecharger : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workAnims = RemoteWorkerDock.WorkerRecharger.WORK_ANIMS;
			this.workingPstComplete = RemoteWorkerDock.WorkerRecharger.WORK_PST_ANIM;
			this.workingPstFailed = RemoteWorkerDock.WorkerRecharger.WORK_PST_ANIM;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerRecharging;
			base.SetWorkTime(float.PositiveInfinity);
		}

		protected override void OnStartWork(WorkerBase worker)
		{
			base.OnStartWork(worker);
			RemoteWorkerCapacitor component = worker.GetComponent<RemoteWorkerCapacitor>();
			this.progress = ((component != null) ? component.ChargeRatio : 0f);
			if (this.progressBar != null)
			{
				this.progressBar.SetUpdateFunc(() => this.progress);
			}
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			RemoteWorkerCapacitor component = worker.GetComponent<RemoteWorkerCapacitor>();
			if (component != null)
			{
				this.progress = component.ChargeRatio;
				return component.ApplyDeltaEnergy(7.5f * dt) == 0f;
			}
			return true;
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "recharge_pre", "recharge_loop" };

		private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "recharge_pst" };

		private float progress;
	}

	public class WorkerGunkRemover : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_remote_work_dock_kanim") };
			this.workAnims = RemoteWorkerDock.WorkerGunkRemover.WORK_ANIMS;
			this.workingPstComplete = RemoteWorkerDock.WorkerGunkRemover.WORK_PST_ANIM;
			this.workingPstFailed = RemoteWorkerDock.WorkerGunkRemover.WORK_PST_ANIM;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerDraining;
			base.SetWorkTime(float.PositiveInfinity);
		}

		protected override void OnStartWork(WorkerBase worker)
		{
			base.OnStartWork(worker);
			Storage component = worker.GetComponent<Storage>();
			if (component != null)
			{
				this.progress = 1f - component.GetMassAvailable(SimHashes.LiquidGunk) / 20.000002f;
			}
			if (this.progressBar != null)
			{
				this.progressBar.SetUpdateFunc(() => this.progress);
			}
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			Storage component = worker.GetComponent<Storage>();
			if (component != null)
			{
				float massAvailable = component.GetMassAvailable(SimHashes.LiquidGunk);
				float num = Math.Min(massAvailable, 3.3333337f * dt);
				this.progress = 1f - massAvailable / 20.000002f;
				if (num > 0f)
				{
					component.TransferMass(this.storage, SimHashes.LiquidGunk.CreateTag(), num, false, false, true);
					return false;
				}
			}
			return true;
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "drain_gunk_pre", "drain_gunk_loop" };

		private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "drain_gunk_pst" };

		[MyCmpGet]
		private Storage storage;

		private float progress;
	}

	public class WorkerOilRefiller : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_remote_work_dock_kanim") };
			this.workAnims = RemoteWorkerDock.WorkerOilRefiller.WORK_ANIMS;
			this.workingPstComplete = RemoteWorkerDock.WorkerOilRefiller.WORK_PST_ANIM;
			this.workingPstFailed = RemoteWorkerDock.WorkerOilRefiller.WORK_PST_ANIM;
			this.synchronizeAnims = true;
			this.triggerWorkReactions = false;
			this.workLayer = Grid.SceneLayer.BuildingUse;
			this.workerStatusItem = Db.Get().DuplicantStatusItems.RemoteWorkerOiling;
			base.SetWorkTime(float.PositiveInfinity);
		}

		protected override void OnStartWork(WorkerBase worker)
		{
			base.OnStartWork(worker);
			Storage component = worker.GetComponent<Storage>();
			if (component != null)
			{
				float massAvailable = component.GetMassAvailable(GameTags.LubricatingOil);
				this.progress = massAvailable / 20.000002f;
			}
			if (this.progressBar != null)
			{
				this.progressBar.SetUpdateFunc(() => this.progress);
			}
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			Storage component = worker.GetComponent<Storage>();
			if (component != null)
			{
				float massAvailable = component.GetMassAvailable(GameTags.LubricatingOil);
				float num = Math.Min(20.000002f - massAvailable, 2.5000002f * dt);
				this.progress = massAvailable / 20.000002f;
				if (num > 0f)
				{
					this.storage.TransferMass(component, GameTags.LubricatingOil, num, false, false, true);
					return false;
				}
			}
			return true;
		}

		private static readonly HashedString[] WORK_ANIMS = new HashedString[] { "oil_pre", "oil_loop" };

		private static readonly HashedString[] WORK_PST_ANIM = new HashedString[] { "oil_pst" };

		[MyCmpGet]
		private Storage storage;

		private float progress;
	}
}
