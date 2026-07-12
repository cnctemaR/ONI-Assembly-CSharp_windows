using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Database;
using FMODUnity;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidTransferArm : StateMachineComponent<SolidTransferArm.SMInstance>, ISim1000ms, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreConsumer.AddProvider(GlobalChoreProvider.Instance);
		this.choreConsumer.SetReach(this.pickupRange);
		Klei.AI.Attributes attributes = this.GetAttributes();
		if (attributes.Get(Db.Get().Attributes.CarryAmount) == null)
		{
			attributes.Add(Db.Get().Attributes.CarryAmount);
		}
		AttributeModifier attributeModifier = new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, this.max_carry_weight, base.gameObject.GetProperName(), false, false, true);
		this.GetAttributes().Add(attributeModifier);
		this.worker.usesMultiTool = false;
		this.storage.fxPrefix = Storage.FXPrefix.PickedUp;
		this.simRenderLoadBalance = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		string text = component.name + ".arm";
		this.arm_go = new GameObject(text);
		this.arm_go.SetActive(false);
		this.arm_go.transform.parent = component.transform;
		this.looping_sounds = this.arm_go.AddComponent<LoopingSounds>();
		string sound = GlobalAssets.GetSound(this.rotateSoundName, false);
		this.rotateSound = RuntimeManager.PathToEventReference(sound);
		this.arm_go.AddComponent<KPrefabID>().PrefabTag = new Tag(text);
		this.arm_anim_ctrl = this.arm_go.AddComponent<KBatchedAnimController>();
		this.arm_anim_ctrl.AnimFiles = new KAnimFile[] { component.AnimFiles[0] };
		this.arm_anim_ctrl.initialAnim = "arm";
		this.arm_anim_ctrl.isMovable = true;
		this.arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
		component.SetSymbolVisiblity("arm_target", false);
		bool flag;
		Vector3 vector = component.GetSymbolTransform(new HashedString("arm_target"), out flag).GetColumn(3);
		vector.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
		this.arm_go.transform.SetPosition(vector);
		this.arm_go.SetActive(true);
		this.link = new KAnimLink(component, this.arm_anim_ctrl);
		ChoreGroups choreGroups = Db.Get().ChoreGroups;
		for (int i = 0; i < choreGroups.Count; i++)
		{
			this.choreConsumer.SetPermittedByUser(choreGroups[i], true);
		}
		base.Subscribe<SolidTransferArm>(-592767678, SolidTransferArm.OnOperationalChangedDelegate);
		base.Subscribe<SolidTransferArm>(1745615042, SolidTransferArm.OnEndChoreDelegate);
		this.RotateArm(this.rotatable.GetRotatedOffset(Vector3.up), true, 0f);
		this.DropLeftovers();
		component.enabled = false;
		component.enabled = true;
		MinionGroupProber.Get().SetValidSerialNos(this, this.serial_no, this.serial_no);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		MinionGroupProber.Get().ReleaseProber(this);
		base.OnCleanUp();
	}

	public static void BatchUpdate(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> solid_transfer_arms, float time_delta)
	{
		SolidTransferArm.BatchUpdateContext batchUpdateContext = new SolidTransferArm.BatchUpdateContext(solid_transfer_arms);
		if (batchUpdateContext.solid_transfer_arms.Count == 0)
		{
			batchUpdateContext.Finish();
			return;
		}
		SolidTransferArm.cached_pickupables.Clear();
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> keyValuePair in Game.Instance.fetchManager.prefabIdToFetchables)
		{
			List<FetchManager.Fetchable> dataList = keyValuePair.Value.fetchables.GetDataList();
			SolidTransferArm.cached_pickupables.Capacity = Math.Max(SolidTransferArm.cached_pickupables.Capacity, SolidTransferArm.cached_pickupables.Count + dataList.Count);
			foreach (FetchManager.Fetchable fetchable in dataList)
			{
				SolidTransferArm.cached_pickupables.Add(new SolidTransferArm.CachedPickupable
				{
					pickupable = fetchable.pickupable,
					storage_cell = fetchable.pickupable.cachedCell
				});
			}
		}
		SolidTransferArm.batch_update_job.Reset(batchUpdateContext);
		int num = Math.Max(1, batchUpdateContext.solid_transfer_arms.Count / CPUBudget.coreCount);
		int num2 = Math.Min(batchUpdateContext.solid_transfer_arms.Count, CPUBudget.coreCount);
		for (int num3 = 0; num3 != num2; num3++)
		{
			int num4 = num3 * num;
			int num5 = ((num3 == num2 - 1) ? batchUpdateContext.solid_transfer_arms.Count : (num4 + num));
			SolidTransferArm.batch_update_job.Add(new SolidTransferArm.BatchUpdateTask(num4, num5));
		}
		GlobalJobManager.Run(SolidTransferArm.batch_update_job);
		for (int num6 = 0; num6 != SolidTransferArm.batch_update_job.Count; num6++)
		{
			SolidTransferArm.batch_update_job.GetWorkItem(num6).Finish();
		}
		batchUpdateContext.Finish();
		SolidTransferArm.batch_update_job.Reset(null);
		SolidTransferArm.cached_pickupables.Clear();
	}

	private void Sim()
	{
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		if (this.choreConsumer.FindNextChore(ref context))
		{
			if (context.chore is FetchChore)
			{
				this.choreDriver.SetChore(context);
				FetchChore fetchChore = context.chore as FetchChore;
				this.storage.DropUnlessMatching(fetchChore);
				this.arm_anim_ctrl.enabled = false;
				this.arm_anim_ctrl.enabled = true;
			}
			else
			{
				bool flag = false;
				string text = "I am but a lowly transfer arm. I should only acquire FetchChores: ";
				Chore chore = context.chore;
				global::Debug.Assert(flag, text + ((chore != null) ? chore.ToString() : null));
			}
		}
		this.operational.SetActive(this.choreDriver.HasChore(), false);
	}

	public void Sim1000ms(float dt)
	{
	}

	private void UpdateArmAnim()
	{
		FetchAreaChore fetchAreaChore = this.choreDriver.GetCurrentChore() as FetchAreaChore;
		if (this.worker.workable && fetchAreaChore != null && this.rotation_complete)
		{
			this.StopRotateSound();
			this.SetArmAnim(fetchAreaChore.IsDelivering ? SolidTransferArm.ArmAnim.Drop : SolidTransferArm.ArmAnim.Pickup);
			return;
		}
		this.SetArmAnim(SolidTransferArm.ArmAnim.Idle);
	}

	private bool AsyncUpdate(int cell, HashSet<int> workspace, GameObject game_object)
	{
		workspace.Clear();
		int num;
		int num2;
		Grid.CellToXY(cell, out num, out num2);
		for (int i = num2 - this.pickupRange; i < num2 + this.pickupRange + 1; i++)
		{
			for (int j = num - this.pickupRange; j < num + this.pickupRange + 1; j++)
			{
				int num3 = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num3) && Grid.IsPhysicallyAccessible(num, num2, j, i, true))
				{
					workspace.Add(num3);
				}
			}
		}
		bool flag = !this.reachableCells.SetEquals(workspace);
		if (flag)
		{
			this.reachableCells.Clear();
			this.reachableCells.UnionWith(workspace);
		}
		this.pickupables.Clear();
		foreach (SolidTransferArm.CachedPickupable cachedPickupable in SolidTransferArm.cached_pickupables)
		{
			if (Grid.GetCellRange(cell, cachedPickupable.storage_cell) <= this.pickupRange && this.IsPickupableRelevantToMyInterests(cachedPickupable.pickupable.KPrefabID, cachedPickupable.storage_cell) && cachedPickupable.pickupable.CouldBePickedUpByTransferArm(game_object))
			{
				this.pickupables.Add(cachedPickupable.pickupable);
			}
		}
		return flag;
	}

	private void IncrementSerialNo()
	{
		this.serial_no += 1;
		MinionGroupProber.Get().SetValidSerialNos(this, this.serial_no, this.serial_no);
		MinionGroupProber.Get().Occupy(this, this.serial_no, this.reachableCells);
	}

	public bool IsCellReachable(int cell)
	{
		return this.reachableCells.Contains(cell);
	}

	private bool IsPickupableRelevantToMyInterests(KPrefabID prefabID, int storage_cell)
	{
		return prefabID.HasAnyTags(ref SolidTransferArm.tagBits) && this.IsCellReachable(storage_cell);
	}

	public Pickupable FindFetchTarget(Storage destination, FetchChore chore)
	{
		return FetchManager.FindFetchTarget(this.pickupables, destination, chore);
	}

	public void RenderEveryTick(float dt)
	{
		if (this.worker.workable)
		{
			Vector3 targetPoint = this.worker.workable.GetTargetPoint();
			targetPoint.z = 0f;
			Vector3 position = base.transform.GetPosition();
			position.z = 0f;
			Vector3 vector = Vector3.Normalize(targetPoint - position);
			this.RotateArm(vector, false, dt);
		}
		this.UpdateArmAnim();
	}

	private void OnEndChore(object data)
	{
		this.DropLeftovers();
	}

	private void DropLeftovers()
	{
		if (!this.storage.IsEmpty() && !this.choreDriver.HasChore())
		{
			this.storage.DropAll(false, false, default(Vector3), true, null);
		}
	}

	private void SetArmAnim(SolidTransferArm.ArmAnim new_anim)
	{
		if (new_anim == this.arm_anim)
		{
			return;
		}
		this.arm_anim = new_anim;
		switch (this.arm_anim)
		{
		case SolidTransferArm.ArmAnim.Idle:
			this.arm_anim_ctrl.Play("arm", KAnim.PlayMode.Loop, 1f, 0f);
			return;
		case SolidTransferArm.ArmAnim.Pickup:
			this.arm_anim_ctrl.Play("arm_pickup", KAnim.PlayMode.Loop, 1f, 0f);
			return;
		case SolidTransferArm.ArmAnim.Drop:
			this.arm_anim_ctrl.Play("arm_drop", KAnim.PlayMode.Loop, 1f, 0f);
			return;
		default:
			return;
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			if (this.choreDriver.HasChore())
			{
				this.choreDriver.StopChore();
			}
			this.UpdateArmAnim();
		}
	}

	private void SetArmRotation(float rot)
	{
		this.arm_rot = rot;
		this.arm_go.transform.rotation = Quaternion.Euler(0f, 0f, this.arm_rot);
	}

	private void RotateArm(Vector3 target_dir, bool warp, float dt)
	{
		float num = MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward) - this.arm_rot;
		if (num < -180f)
		{
			num += 360f;
		}
		if (num > 180f)
		{
			num -= 360f;
		}
		if (!warp)
		{
			num = Mathf.Clamp(num, -this.turn_rate * dt, this.turn_rate * dt);
		}
		this.arm_rot += num;
		this.SetArmRotation(this.arm_rot);
		this.rotation_complete = Mathf.Approximately(num, 0f);
		if (!warp && !this.rotation_complete)
		{
			if (!this.rotateSoundPlaying)
			{
				this.StartRotateSound();
			}
			this.SetRotateSoundParameter(this.arm_rot);
			return;
		}
		this.StopRotateSound();
	}

	private void StartRotateSound()
	{
		if (!this.rotateSoundPlaying)
		{
			this.looping_sounds.StartSound(this.rotateSound);
			this.rotateSoundPlaying = true;
		}
	}

	private void SetRotateSoundParameter(float arm_rot)
	{
		if (this.rotateSoundPlaying)
		{
			this.looping_sounds.SetParameter(this.rotateSound, SolidTransferArm.HASH_ROTATION, arm_rot);
		}
	}

	private void StopRotateSound()
	{
		if (this.rotateSoundPlaying)
		{
			this.looping_sounds.StopSound(this.rotateSound);
			this.rotateSoundPlaying = false;
		}
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void BeginDetailedSample(string region_name, int count)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_FETCH_PROFILING")]
	private static void EndDetailedSample(string region_name, int count)
	{
	}

	[MyCmpReq]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private Worker worker;

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;

	[MyCmpAdd]
	private ChoreDriver choreDriver;

	public int pickupRange = 4;

	private float max_carry_weight = 1000f;

	private List<Pickupable> pickupables = new List<Pickupable>();

	public static TagBits tagBits = new TagBits(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat<Tag>(STORAGEFILTERS.FOOD).Concat<Tag>(STORAGEFILTERS.PAYLOADS).ToArray<Tag>());

	private KBatchedAnimController arm_anim_ctrl;

	private GameObject arm_go;

	private LoopingSounds looping_sounds;

	private bool rotateSoundPlaying;

	private string rotateSoundName = "TransferArm_rotate";

	private EventReference rotateSound;

	private KAnimLink link;

	private float arm_rot = 45f;

	private float turn_rate = 360f;

	private bool rotation_complete;

	private SolidTransferArm.ArmAnim arm_anim;

	private HashSet<int> reachableCells = new HashSet<int>();

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnEndChoreDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnEndChore(data);
	});

	private static WorkItemCollection<SolidTransferArm.BatchUpdateTask, SolidTransferArm.BatchUpdateContext> batch_update_job = new WorkItemCollection<SolidTransferArm.BatchUpdateTask, SolidTransferArm.BatchUpdateContext>();

	private static List<SolidTransferArm.CachedPickupable> cached_pickupables = new List<SolidTransferArm.CachedPickupable>();

	private short serial_no;

	private static HashedString HASH_ROTATION = "rotation";

	private enum ArmAnim
	{
		Idle,
		Pickup,
		Drop
	}

	public class SMInstance : GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.GameInstance
	{
		public SMInstance(SolidTransferArm master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.root.DoNothing();
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (SolidTransferArm.SMInstance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(SolidTransferArm.SMInstance smi)
			{
				smi.master.StopRotateSound();
			});
			this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (SolidTransferArm.SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, this.on.working, (SolidTransferArm.SMInstance smi) => smi.GetComponent<Operational>().IsActive);
			this.on.working.PlayAnim("working").EventTransition(GameHashes.ActiveChanged, this.on.idle, (SolidTransferArm.SMInstance smi) => !smi.GetComponent<Operational>().IsActive);
		}

		public StateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.BoolParameter transferring;

		public GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.State off;

		public SolidTransferArm.States.ReadyStates on;

		public class ReadyStates : GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.State
		{
			public GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.State idle;

			public GameStateMachine<SolidTransferArm.States, SolidTransferArm.SMInstance, SolidTransferArm, object>.State working;
		}
	}

	private class BatchUpdateContext
	{
		public BatchUpdateContext(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> solid_transfer_arms)
		{
			this.solid_transfer_arms = ListPool<SolidTransferArm, SolidTransferArm.BatchUpdateContext>.Allocate();
			this.solid_transfer_arms.Capacity = solid_transfer_arms.Count;
			this.refreshed_reachable_cells = ListPool<bool, SolidTransferArm.BatchUpdateContext>.Allocate();
			this.refreshed_reachable_cells.Capacity = solid_transfer_arms.Count;
			this.cells = ListPool<int, SolidTransferArm.BatchUpdateContext>.Allocate();
			this.cells.Capacity = solid_transfer_arms.Count;
			this.game_objects = ListPool<GameObject, SolidTransferArm.BatchUpdateContext>.Allocate();
			this.game_objects.Capacity = solid_transfer_arms.Count;
			for (int num = 0; num != solid_transfer_arms.Count; num++)
			{
				UpdateBucketWithUpdater<ISim1000ms>.Entry entry = solid_transfer_arms[num];
				entry.lastUpdateTime = 0f;
				solid_transfer_arms[num] = entry;
				SolidTransferArm solidTransferArm = (SolidTransferArm)entry.data;
				if (solidTransferArm.operational.IsOperational)
				{
					this.solid_transfer_arms.Add(solidTransferArm);
					this.refreshed_reachable_cells.Add(false);
					this.cells.Add(Grid.PosToCell(solidTransferArm));
					this.game_objects.Add(solidTransferArm.gameObject);
				}
			}
		}

		public void Finish()
		{
			for (int num = 0; num != this.solid_transfer_arms.Count; num++)
			{
				if (this.refreshed_reachable_cells[num])
				{
					this.solid_transfer_arms[num].IncrementSerialNo();
				}
				this.solid_transfer_arms[num].Sim();
			}
			this.refreshed_reachable_cells.Recycle();
			this.cells.Recycle();
			this.game_objects.Recycle();
			this.solid_transfer_arms.Recycle();
		}

		public ListPool<SolidTransferArm, SolidTransferArm.BatchUpdateContext>.PooledList solid_transfer_arms;

		public ListPool<bool, SolidTransferArm.BatchUpdateContext>.PooledList refreshed_reachable_cells;

		public ListPool<int, SolidTransferArm.BatchUpdateContext>.PooledList cells;

		public ListPool<GameObject, SolidTransferArm.BatchUpdateContext>.PooledList game_objects;
	}

	private struct BatchUpdateTask : IWorkItem<SolidTransferArm.BatchUpdateContext>
	{
		public BatchUpdateTask(int start, int end)
		{
			this.start = start;
			this.end = end;
			this.reachable_cells_workspace = HashSetPool<int, SolidTransferArm>.Allocate();
		}

		public void Run(SolidTransferArm.BatchUpdateContext context)
		{
			for (int num = this.start; num != this.end; num++)
			{
				context.refreshed_reachable_cells[num] = context.solid_transfer_arms[num].AsyncUpdate(context.cells[num], this.reachable_cells_workspace, context.game_objects[num]);
			}
		}

		public void Finish()
		{
			this.reachable_cells_workspace.Recycle();
		}

		private int start;

		private int end;

		private HashSetPool<int, SolidTransferArm>.PooledHashSet reachable_cells_workspace;
	}

	public struct CachedPickupable
	{
		public Pickupable pickupable;

		public int storage_cell;
	}
}
