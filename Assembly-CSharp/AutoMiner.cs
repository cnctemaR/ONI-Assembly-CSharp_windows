using System;
using FMODUnity;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AutoMiner : StateMachineComponent<AutoMiner.Instance>, ISim1000ms
{
	private bool HasDigCell
	{
		get
		{
			return this.dig_cell != Grid.InvalidCell;
		}
	}

	private bool RotationComplete
	{
		get
		{
			return this.HasDigCell && this.rotation_complete;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.simRenderLoadBalance = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.hitEffectPrefab = Assets.GetPrefab("fx_dig_splash");
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		string text = component.name + ".gun";
		this.arm_go = new GameObject(text);
		this.arm_go.SetActive(false);
		this.arm_go.transform.parent = component.transform;
		this.looping_sounds = this.arm_go.AddComponent<LoopingSounds>();
		this.rotateSound = GlobalAssets.GetSound(this.rotateSound, false);
		this.arm_go.AddComponent<KPrefabID>().PrefabTag = new Tag(text);
		this.arm_anim_ctrl = this.arm_go.AddComponent<KBatchedAnimController>();
		this.arm_anim_ctrl.AnimFiles = new KAnimFile[] { component.AnimFiles[0] };
		this.arm_anim_ctrl.initialAnim = "gun";
		this.arm_anim_ctrl.isMovable = true;
		this.arm_anim_ctrl.sceneLayer = Grid.SceneLayer.TransferArm;
		component.SetSymbolVisiblity("gun_target", false);
		bool flag;
		Vector3 vector = component.GetSymbolTransform(new HashedString("gun_target"), out flag).GetColumn(3);
		vector.z = Grid.GetLayerZ(Grid.SceneLayer.TransferArm);
		this.arm_go.transform.SetPosition(vector);
		this.arm_go.SetActive(true);
		this.link = new KAnimLink(component, this.arm_anim_ctrl);
		base.Subscribe<AutoMiner>(-592767678, AutoMiner.OnOperationalChangedDelegate);
		this.RotateArm(this.rotatable.GetRotatedOffset(Quaternion.Euler(0f, 0f, -45f) * Vector3.up), true, 0f);
		this.StopDig();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void Sim1000ms(float dt)
	{
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.RefreshDiggableCell();
		this.operational.SetActive(this.HasDigCell, false);
	}

	private void OnOperationalChanged(object data)
	{
		if (!(bool)data)
		{
			this.dig_cell = Grid.InvalidCell;
			this.rotation_complete = false;
		}
	}

	public void UpdateRotation(float dt)
	{
		if (this.HasDigCell)
		{
			Vector3 vector = Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.TileMain);
			vector.z = 0f;
			Vector3 position = this.arm_go.transform.GetPosition();
			position.z = 0f;
			Vector3 vector2 = Vector3.Normalize(vector - position);
			this.RotateArm(vector2, false, dt);
		}
	}

	private Element GetTargetElement()
	{
		if (this.HasDigCell)
		{
			return Grid.Element[this.dig_cell];
		}
		return null;
	}

	public void StartDig()
	{
		Element targetElement = this.GetTargetElement();
		base.Trigger(-1762453998, targetElement);
		this.CreateHitEffect();
		this.arm_anim_ctrl.Play("gun_digging", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void StopDig()
	{
		base.Trigger(939543986, null);
		this.DestroyHitEffect();
		this.arm_anim_ctrl.Play("gun", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void UpdateDig(float dt)
	{
		if (!this.HasDigCell)
		{
			return;
		}
		if (!this.rotation_complete)
		{
			return;
		}
		Diggable.DoDigTick(this.dig_cell, dt);
		float num = Grid.Damage[this.dig_cell];
		this.mining_sounds.SetPercentComplete(num);
		Vector3 vector = Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.FXFront2);
		vector.z = 0f;
		Vector3 position = this.arm_go.transform.GetPosition();
		position.z = 0f;
		float sqrMagnitude = (vector - position).sqrMagnitude;
		this.arm_anim_ctrl.GetBatchInstanceData().SetClipRadius(position.x, position.y, sqrMagnitude, true);
		if (!AutoMiner.ValidDigCell(this.dig_cell))
		{
			this.dig_cell = Grid.InvalidCell;
			this.rotation_complete = false;
		}
	}

	private void CreateHitEffect()
	{
		if (this.hitEffectPrefab == null)
		{
			return;
		}
		if (this.hitEffect != null)
		{
			this.DestroyHitEffect();
		}
		Vector3 vector = Grid.CellToPosCCC(this.dig_cell, Grid.SceneLayer.FXFront2);
		this.hitEffect = GameUtil.KInstantiate(this.hitEffectPrefab, vector, Grid.SceneLayer.FXFront2, null, 0);
		this.hitEffect.SetActive(true);
		KBatchedAnimController component = this.hitEffect.GetComponent<KBatchedAnimController>();
		component.sceneLayer = Grid.SceneLayer.FXFront2;
		component.initialMode = KAnim.PlayMode.Loop;
		component.enabled = false;
		component.enabled = true;
	}

	private void DestroyHitEffect()
	{
		if (this.hitEffectPrefab == null)
		{
			return;
		}
		if (this.hitEffect != null)
		{
			this.hitEffect.DeleteObject();
			this.hitEffect = null;
		}
	}

	private void RefreshDiggableCell()
	{
		CellOffset rotatedCellOffset = this.vision_offset;
		if (this.rotatable)
		{
			rotatedCellOffset = this.rotatable.GetRotatedCellOffset(this.vision_offset);
		}
		int num = Grid.PosToCell(base.transform.gameObject);
		int num2 = Grid.OffsetCell(num, rotatedCellOffset);
		int num3;
		int num4;
		Grid.CellToXY(num2, out num3, out num4);
		float num5 = float.MaxValue;
		int num6 = Grid.InvalidCell;
		Vector3 vector = Grid.CellToPos(num2);
		bool flag = false;
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				CellOffset rotatedCellOffset2 = new CellOffset(this.x + j, this.y + i);
				if (this.rotatable)
				{
					rotatedCellOffset2 = this.rotatable.GetRotatedCellOffset(rotatedCellOffset2);
				}
				int num7 = Grid.OffsetCell(num, rotatedCellOffset2);
				if (Grid.IsValidCell(num7))
				{
					int num8;
					int num9;
					Grid.CellToXY(num7, out num8, out num9);
					if (Grid.IsValidCell(num7) && AutoMiner.ValidDigCell(num7) && Grid.TestLineOfSight(num3, num4, num8, num9, new Func<int, bool>(AutoMiner.DigBlockingCB), false))
					{
						if (num7 == this.dig_cell)
						{
							flag = true;
						}
						Vector3 vector2 = Grid.CellToPos(num7);
						float num10 = Vector3.Distance(vector, vector2);
						if (num10 < num5)
						{
							num5 = num10;
							num6 = num7;
						}
					}
				}
			}
		}
		if (!flag && this.dig_cell != num6)
		{
			this.dig_cell = num6;
			this.rotation_complete = false;
		}
	}

	private static bool ValidDigCell(int cell)
	{
		return Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Element[cell].hardness < 150;
	}

	public static bool DigBlockingCB(int cell)
	{
		return Grid.Foundation[cell] || Grid.Element[cell].hardness >= 150;
	}

	private void RotateArm(Vector3 target_dir, bool warp, float dt)
	{
		if (this.rotation_complete)
		{
			return;
		}
		float num = MathUtil.AngleSigned(Vector3.up, target_dir, Vector3.forward) - this.arm_rot;
		num = MathUtil.Wrap(-180f, 180f, num);
		this.rotation_complete = Mathf.Approximately(num, 0f);
		float num2 = num;
		if (warp)
		{
			this.rotation_complete = true;
		}
		else
		{
			num2 = Mathf.Clamp(num2, -this.turn_rate * dt, this.turn_rate * dt);
		}
		this.arm_rot += num2;
		this.arm_rot = MathUtil.Wrap(-180f, 180f, this.arm_rot);
		this.arm_go.transform.rotation = Quaternion.Euler(0f, 0f, this.arm_rot);
		if (!this.rotation_complete)
		{
			this.StartRotateSound();
			this.looping_sounds.SetParameter(this.rotateSound, AutoMiner.HASH_ROTATION, this.arm_rot);
			return;
		}
		this.StopRotateSound();
	}

	private void StartRotateSound()
	{
		if (!this.rotate_sound_playing)
		{
			this.looping_sounds.StartSound(this.rotateSound);
			this.rotate_sound_playing = true;
		}
	}

	private void StopRotateSound()
	{
		if (this.rotate_sound_playing)
		{
			this.looping_sounds.StopSound(this.rotateSound);
			this.rotate_sound_playing = false;
		}
	}

	private static HashedString HASH_ROTATION = "rotation";

	[MyCmpReq]
	private Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private Rotatable rotatable;

	[MyCmpReq]
	private MiningSounds mining_sounds;

	public int x;

	public int y;

	public int width;

	public int height;

	public CellOffset vision_offset;

	private KBatchedAnimController arm_anim_ctrl;

	private GameObject arm_go;

	private LoopingSounds looping_sounds;

	[EventRef]
	private string rotateSound = "AutoMiner_rotate";

	private KAnimLink link;

	private float arm_rot = 45f;

	private float turn_rate = 180f;

	private bool rotation_complete;

	private bool rotate_sound_playing;

	private GameObject hitEffectPrefab;

	private GameObject hitEffect;

	private int dig_cell = Grid.InvalidCell;

	private static readonly EventSystem.IntraObjectHandler<AutoMiner> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<AutoMiner>(delegate(AutoMiner component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public class Instance : GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.GameInstance
	{
		public Instance(AutoMiner master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.off;
			this.root.DoNothing();
			this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (AutoMiner.Instance smi) => smi.GetComponent<Operational>().IsOperational);
			this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsOperational);
			this.on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, this.on.moving, (AutoMiner.Instance smi) => smi.GetComponent<Operational>().IsActive);
			this.on.moving.Exit(delegate(AutoMiner.Instance smi)
			{
				smi.master.StopRotateSound();
			}).PlayAnim("working").EventTransition(GameHashes.ActiveChanged, this.on.idle, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsActive)
				.Update(delegate(AutoMiner.Instance smi, float dt)
				{
					smi.master.UpdateRotation(dt);
				}, UpdateRate.SIM_33ms, false)
				.Transition(this.on.digging, new StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback(AutoMiner.States.RotationComplete), UpdateRate.SIM_200ms);
			this.on.digging.Enter(delegate(AutoMiner.Instance smi)
			{
				smi.master.StartDig();
			}).Exit(delegate(AutoMiner.Instance smi)
			{
				smi.master.StopDig();
			}).PlayAnim("working")
				.EventTransition(GameHashes.ActiveChanged, this.on.idle, (AutoMiner.Instance smi) => !smi.GetComponent<Operational>().IsActive)
				.Update(delegate(AutoMiner.Instance smi, float dt)
				{
					smi.master.UpdateDig(dt);
				}, UpdateRate.SIM_200ms, false)
				.Transition(this.on.moving, GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Not(new StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.Transition.ConditionCallback(AutoMiner.States.RotationComplete)), UpdateRate.SIM_200ms);
		}

		public static bool RotationComplete(AutoMiner.Instance smi)
		{
			return smi.master.RotationComplete;
		}

		public StateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.BoolParameter transferring;

		public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State off;

		public AutoMiner.States.ReadyStates on;

		public class ReadyStates : GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State
		{
			public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State idle;

			public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State moving;

			public GameStateMachine<AutoMiner.States, AutoMiner.Instance, AutoMiner, object>.State digging;
		}
	}
}
