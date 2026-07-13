using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using FMODUnity;
using Klei.AI;
using KSerialization;
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
		this.gameCell = Grid.PosToCell(this.arm_go);
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
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		MinionGroupProber.Get().Vacate(this.reachableCells.ToList<int>());
		base.OnCleanUp();
	}

	public static void BatchUpdate(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> solid_transfer_arms, float time_delta)
	{
		SolidTransferArm.SolidTransferArmBatchUpdater.Instance.Reset(solid_transfer_arms);
		GlobalJobManager.Run(SolidTransferArm.SolidTransferArmBatchUpdater.Instance);
		SolidTransferArm.SolidTransferArmBatchUpdater.Instance.Finish();
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
		if (this.worker.GetWorkable() && fetchAreaChore != null && this.rotation_complete)
		{
			this.StopRotateSound();
			this.SetArmAnim(fetchAreaChore.IsDelivering ? SolidTransferArm.ArmAnim.Drop : SolidTransferArm.ArmAnim.Pickup);
			return;
		}
		this.SetArmAnim(SolidTransferArm.ArmAnim.Idle);
	}

	private bool AsyncUpdate()
	{
		int num;
		int num2;
		Grid.CellToXY(this.gameCell, out num, out num2);
		bool flag = false;
		for (int i = num2 - this.pickupRange; i < num2 + this.pickupRange + 1; i++)
		{
			for (int j = num - this.pickupRange; j < num + this.pickupRange + 1; j++)
			{
				int num3 = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num3) && Grid.IsPhysicallyAccessible(num, num2, j, i, true) != this.reachableCells.Contains(num3))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			ListPool<int, SolidTransferArm>.PooledList pooledList = ListPool<int, SolidTransferArm>.Allocate();
			ListPool<int, SolidTransferArm>.PooledList pooledList2 = ListPool<int, SolidTransferArm>.Allocate();
			pooledList.AddRange(this.reachableCells);
			this.reachableCells.Clear();
			for (int k = num2 - this.pickupRange; k < num2 + this.pickupRange + 1; k++)
			{
				for (int l = num - this.pickupRange; l < num + this.pickupRange + 1; l++)
				{
					int num4 = Grid.XYToCell(l, k);
					if (Grid.IsValidCell(num4) && Grid.IsPhysicallyAccessible(num, num2, l, k, true))
					{
						this.reachableCells.Add(num4);
						pooledList2.Add(num4);
					}
				}
			}
			MinionGroupProber.Get().Occupy(pooledList2);
			MinionGroupProber.Get().Vacate(pooledList);
			pooledList2.Recycle();
			pooledList.Recycle();
		}
		this.pickupables.Clear();
		GameScenePartitioner.Instance.ReadonlyVisitEntries<SolidTransferArm>(num - this.pickupRange, num2 - this.pickupRange, 2 * this.pickupRange + 1, 2 * this.pickupRange + 1, GameScenePartitioner.Instance.pickupablesLayer, SolidTransferArm.AsyncUpdateVisitor, this);
		GameScenePartitioner.Instance.ReadonlyVisitEntries<SolidTransferArm>(num - this.pickupRange, num2 - this.pickupRange, 2 * this.pickupRange + 1, 2 * this.pickupRange + 1, GameScenePartitioner.Instance.storedPickupablesLayer, SolidTransferArm.AsyncUpdateVisitor, this);
		return flag;
	}

	public bool IsCellReachable(int cell)
	{
		return this.reachableCells.Contains(cell);
	}

	private bool IsPickupableRelevantToMyInterests(KPrefabID prefabID, int storage_cell)
	{
		return Assets.IsTagSolidTransferArmConveyable(prefabID.PrefabTag) && this.IsCellReachable(storage_cell);
	}

	public Pickupable FindFetchTarget(Storage destination, FetchChore chore)
	{
		return FetchManager.FindFetchTarget(this.pickupables, destination, chore);
	}

	public void RenderEveryTick(float dt)
	{
		if (this.worker.GetWorkable())
		{
			Vector3 targetPoint = this.worker.GetWorkable().GetTargetPoint();
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
		if (!((Boxed<bool>)data).value)
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

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private KPrefabID kPrefabID;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpAdd]
	private StandardWorker worker;

	[MyCmpAdd]
	private ChoreConsumer choreConsumer;

	[MyCmpAdd]
	private ChoreDriver choreDriver;

	public int pickupRange = 4;

	private float max_carry_weight = 1000f;

	private List<Pickupable> pickupables = new List<Pickupable>();

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

	private int gameCell;

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

	private static Func<object, SolidTransferArm, Util.IterationInstruction> AsyncUpdateVisitor = delegate(object obj, SolidTransferArm arm)
	{
		Pickupable pickupable = obj as Pickupable;
		if (Grid.GetCellRange(arm.gameCell, pickupable.cachedCell) <= arm.pickupRange && arm.IsPickupableRelevantToMyInterests(pickupable.KPrefabID, pickupable.cachedCell) && pickupable.CouldBePickedUpByTransferArm(arm.kPrefabID.InstanceID))
		{
			arm.pickupables.Add(pickupable);
		}
		return Util.IterationInstruction.Continue;
	};

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

	private class SolidTransferArmBatchUpdater : WorkItemCollection<List<UpdateBucketWithUpdater<ISim1000ms>.Entry>>
	{
		public static SolidTransferArm.SolidTransferArmBatchUpdater Instance
		{
			get
			{
				if (SolidTransferArm.SolidTransferArmBatchUpdater.instance == null)
				{
					SolidTransferArm.SolidTransferArmBatchUpdater.instance = new SolidTransferArm.SolidTransferArmBatchUpdater();
				}
				return SolidTransferArm.SolidTransferArmBatchUpdater.instance;
			}
		}

		public void Reset(List<UpdateBucketWithUpdater<ISim1000ms>.Entry> entries)
		{
			this.sharedData = entries;
			this.count = (entries.Count + 8 - 1) / 8;
		}

		public override void RunItem(int item, ref List<UpdateBucketWithUpdater<ISim1000ms>.Entry> shared_data, int threadIndex)
		{
			int num = item * 8;
			int num2 = Math.Min(shared_data.Count, num + 8);
			for (int i = num; i < num2; i++)
			{
				SolidTransferArm solidTransferArm = (SolidTransferArm)shared_data[i].data;
				if (solidTransferArm.operational.IsOperational)
				{
					solidTransferArm.AsyncUpdate();
				}
			}
		}

		public void Finish()
		{
			foreach (UpdateBucketWithUpdater<ISim1000ms>.Entry entry in this.sharedData)
			{
				SolidTransferArm solidTransferArm = (SolidTransferArm)entry.data;
				if (solidTransferArm.operational.IsOperational)
				{
					solidTransferArm.Sim();
				}
			}
			this.Reset(SolidTransferArm.SolidTransferArmBatchUpdater.EmptyList);
		}

		private static readonly List<UpdateBucketWithUpdater<ISim1000ms>.Entry> EmptyList = new List<UpdateBucketWithUpdater<ISim1000ms>.Entry>();

		private const int kBatchSize = 8;

		private static SolidTransferArm.SolidTransferArmBatchUpdater instance;
	}

	public struct CachedPickupable
	{
		public Pickupable pickupable;

		public int storage_cell;
	}
}
