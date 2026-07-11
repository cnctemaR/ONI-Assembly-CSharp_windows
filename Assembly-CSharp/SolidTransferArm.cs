using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using FMODUnity;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidTransferArm : StateMachineComponent<SolidTransferArm.SMInstance>, ISim1000ms, ISim33ms, IRenderEveryTick
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
		this.simRenderLoadBalance = true;
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
		this.rotateSound = GlobalAssets.GetSound(this.rotateSound, false);
		KPrefabID kprefabID = this.arm_go.AddComponent<KPrefabID>();
		kprefabID.PrefabTag = new Tag(text);
		this.arm_anim_ctrl = this.arm_go.AddComponent<KBatchedAnimController>();
		this.arm_anim_ctrl.AnimFiles = new KAnimFile[] { component.AnimFiles[0] };
		this.arm_anim_ctrl.initialAnim = "arm";
		this.arm_anim_ctrl.isMovable = true;
		this.arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
		component.SetSymbolVisiblity("arm_target", false);
		bool flag;
		Vector4 column = component.GetSymbolTransform(new HashedString("arm_target"), out flag).GetColumn(3);
		Vector3 vector = column;
		vector.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
		this.arm_go.transform.SetPosition(vector);
		this.arm_go.SetActive(true);
		this.link = new KAnimLink(component, this.arm_anim_ctrl);
		this.pickupableExtents = new Extents(this.NaturalBuildingCell(), this.pickupRange);
		this.pickupablesChangedEntry = GameScenePartitioner.Instance.Add("SolidTransferArm.PickupablesChanged", base.gameObject, this.pickupableExtents, GameScenePartitioner.Instance.pickupablesChangedLayer, new Action<object>(this.OnPickupablesChanged));
		this.pickupablesDirty = true;
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
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref this.pickupablesChangedEntry);
	}

	public void Sim1000ms(float dt)
	{
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.RefreshReachableCells();
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		if (this.choreConsumer.FindNextChore(ref context))
		{
			FetchAreaChore fetchAreaChore = context.chore as FetchAreaChore;
			if (fetchAreaChore != null)
			{
				this.choreDriver.SetChore(context);
				this.arm_anim_ctrl.enabled = false;
				this.arm_anim_ctrl.enabled = true;
			}
		}
		this.operational.SetActive(this.choreDriver.HasChore(), false);
	}

	private void UpdateArmAnim()
	{
		FetchAreaChore fetchAreaChore = this.choreDriver.GetCurrentChore() as FetchAreaChore;
		if (this.worker.workable && fetchAreaChore != null && this.rotation_complete)
		{
			this.StopRotateSound();
			if (fetchAreaChore.IsDelivering)
			{
				this.SetArmAnim(SolidTransferArm.ArmAnim.Drop);
			}
			else
			{
				this.SetArmAnim(SolidTransferArm.ArmAnim.Pickup);
			}
		}
		else
		{
			this.SetArmAnim(SolidTransferArm.ArmAnim.Idle);
		}
	}

	private void RefreshReachableCells()
	{
		this.reachableCells.Clear();
		int num;
		int num2;
		Grid.CellToXY(Grid.PosToCell(this), out num, out num2);
		for (int i = num2 - this.pickupRange; i < num2 + this.pickupRange + 1; i++)
		{
			for (int j = num - this.pickupRange; j < num + this.pickupRange + 1; j++)
			{
				int num3 = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num3) && Grid.IsPhysicallyAccessible(num, num2, j, i, true))
				{
					this.reachableCells.Add(num3);
				}
			}
		}
	}

	private void MarkReachableCells()
	{
		foreach (int num in this.reachableCells)
		{
			MinionGroupProber.Get().SetProberCell(num);
		}
	}

	public bool IsCellReachable(int cell)
	{
		return this.reachableCells.Contains(cell);
	}

	private void RefreshPickupables()
	{
		if (!this.pickupablesDirty)
		{
			return;
		}
		this.pickupables.Clear();
		int num = Grid.PosToCell(this);
		foreach (KeyValuePair<Tag, FetchManager.FetchablesByPrefabId> keyValuePair in Game.Instance.fetchManager.prefabIdToFetchables)
		{
			foreach (FetchManager.Fetchable fetchable in keyValuePair.Value.fetchables.GetDataList())
			{
				Pickupable pickupable = fetchable.pickupable;
				int pickupableCell = this.GetPickupableCell(pickupable);
				int cellRange = Grid.GetCellRange(num, pickupableCell);
				if (cellRange <= this.pickupRange)
				{
					if (this.IsPickupableRelevantToMyInterests(pickupable))
					{
						if (pickupable.CouldBePickedUpByTransferArm(base.gameObject))
						{
							this.pickupables.Add(pickupable);
						}
					}
				}
			}
		}
		this.pickupablesDirty = false;
	}

	private void OnPickupablesChanged(object data)
	{
		Pickupable pickupable = data as Pickupable;
		if (pickupable && this.IsPickupableRelevantToMyInterests(pickupable))
		{
			this.pickupablesDirty = true;
		}
	}

	private bool IsPickupableRelevantToMyInterests(Pickupable pickupable)
	{
		KPrefabID kprefabID = pickupable.KPrefabID;
		if (!kprefabID.HasAnyTags(ref SolidTransferArm.tagBits))
		{
			return false;
		}
		int pickupableCell = this.GetPickupableCell(pickupable);
		return this.IsCellReachable(pickupableCell);
	}

	public void FindFetchTarget(Storage destination, TagBits tag_bits, TagBits required_tags, TagBits forbid_tags, float required_amount, ref Pickupable target)
	{
		target = null;
		this.pickupablesDirty = true;
		this.RefreshPickupables();
		target = FetchManager.FindFetchTarget(this.pickupables, destination, ref tag_bits, ref required_tags, ref forbid_tags, required_amount);
	}

	public void Sim33ms(float dt)
	{
		if (this.operational.IsOperational)
		{
			this.MarkReachableCells();
		}
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

	private int GetPickupableCell(Pickupable pickupable)
	{
		if (pickupable.storage)
		{
			return Grid.PosToCell(pickupable.storage);
		}
		return pickupable.cachedCell;
	}

	private void SetArmAnim(SolidTransferArm.ArmAnim new_anim)
	{
		if (new_anim != this.arm_anim)
		{
			this.arm_anim = new_anim;
			SolidTransferArm.ArmAnim armAnim = this.arm_anim;
			if (armAnim != SolidTransferArm.ArmAnim.Idle)
			{
				if (armAnim != SolidTransferArm.ArmAnim.Pickup)
				{
					if (armAnim == SolidTransferArm.ArmAnim.Drop)
					{
						this.arm_anim_ctrl.Play("arm_drop", KAnim.PlayMode.Loop, 1f, 0f);
					}
				}
				else
				{
					this.arm_anim_ctrl.Play("arm_pickup", KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
			else
			{
				this.arm_anim_ctrl.Play("arm", KAnim.PlayMode.Loop, 1f, 0f);
			}
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

	private void OnEndChore(object data)
	{
		this.DropLeftovers();
	}

	private void DropLeftovers()
	{
		if (!this.storage.IsEmpty() && !this.choreDriver.HasChore())
		{
			this.storage.DropAll(false);
		}
	}

	private void SetArmRotation(float rot)
	{
		this.arm_rot = rot;
		this.arm_go.transform.rotation = Quaternion.Euler(0f, 0f, this.arm_rot);
	}

	private void RotateArm(Vector3 target_dir, bool warp, float dt)
	{
		float num = MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward);
		float num2 = num - this.arm_rot;
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (!warp)
		{
			num2 = Mathf.Clamp(num2, -this.turn_rate * dt, this.turn_rate * dt);
		}
		this.arm_rot += num2;
		this.SetArmRotation(this.arm_rot);
		this.rotation_complete = Mathf.Approximately(num2, 0f);
		if (!warp && !this.rotation_complete)
		{
			if (!this.rotateSoundPlaying)
			{
				this.StartRotateSound();
			}
			this.SetRotateSoundParameter(this.arm_rot);
		}
		else
		{
			this.StopRotateSound();
		}
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

	[MyCmpGet]
	private KSelectable selectable;

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

	private HandleVector<int>.Handle pickupablesChangedEntry;

	public static TagBits tagBits = new TagBits(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat<Tag>(STORAGEFILTERS.FOOD).ToArray<Tag>());

	private bool pickupablesDirty;

	private Extents pickupableExtents;

	private KBatchedAnimController arm_anim_ctrl;

	private GameObject arm_go;

	private LoopingSounds looping_sounds;

	private bool rotateSoundPlaying;

	[EventRef]
	private string rotateSound = "TransferArm_rotate";

	private KAnimLink link;

	private float arm_rot = 45f;

	private float turn_rate = 360f;

	private bool rotation_complete;

	private SolidTransferArm.ArmAnim arm_anim;

	private List<int> reachableCells = new List<int>(100);

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SolidTransferArm> OnEndChoreDelegate = new EventSystem.IntraObjectHandler<SolidTransferArm>(delegate(SolidTransferArm component, object data)
	{
		component.OnEndChore(data);
	});

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
}
