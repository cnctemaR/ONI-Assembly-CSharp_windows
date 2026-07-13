using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedCreature : GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.freeMovement.idle;
		this.root.Enter(new StateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State.Callback(this.SetRetractedPath));
		this.retracted.DefaultState(this.retracted.pre).Enter(delegate(SegmentedCreature.Instance smi)
		{
			this.PlayBodySegmentsAnim(smi, "idle_loop", KAnim.PlayMode.Loop, false, 0);
		}).Exit(new StateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State.Callback(this.SetRetractedPath));
		this.retracted.pre.Update(new Action<SegmentedCreature.Instance, float>(this.UpdateRetractedPre), UpdateRate.SIM_EVERY_TICK, false);
		this.retracted.loop.ParamTransition<bool>(this.isRetracted, this.freeMovement, (SegmentedCreature.Instance smi, bool p) => !this.isRetracted.Get(smi)).Update(new Action<SegmentedCreature.Instance, float>(this.UpdateRetractedLoop), UpdateRate.SIM_EVERY_TICK, false);
		this.freeMovement.DefaultState(this.freeMovement.idle).ParamTransition<bool>(this.isRetracted, this.retracted, (SegmentedCreature.Instance smi, bool p) => this.isRetracted.Get(smi)).Update(new Action<SegmentedCreature.Instance, float>(this.UpdateFreeMovement), UpdateRate.SIM_EVERY_TICK, false);
		this.freeMovement.idle.Transition(this.freeMovement.moving, (SegmentedCreature.Instance smi) => smi.navigator.IsMoving(), UpdateRate.SIM_200ms).Enter(delegate(SegmentedCreature.Instance smi)
		{
			this.PlayBodySegmentsAnim(smi, "idle_loop", KAnim.PlayMode.Loop, true, 0);
		});
		this.freeMovement.moving.Transition(this.freeMovement.idle, (SegmentedCreature.Instance smi) => !smi.navigator.IsMoving(), UpdateRate.SIM_200ms).Enter(delegate(SegmentedCreature.Instance smi)
		{
			this.PlayBodySegmentsAnim(smi, "walking_pre", KAnim.PlayMode.Once, false, 0);
			this.PlayBodySegmentsAnim(smi, "walking_loop", KAnim.PlayMode.Loop, false, smi.def.animFrameOffset);
		}).Exit(delegate(SegmentedCreature.Instance smi)
		{
			this.PlayBodySegmentsAnim(smi, "walking_pst", KAnim.PlayMode.Once, true, 0);
		});
	}

	private void PlayBodySegmentsAnim(SegmentedCreature.Instance smi, string animName, KAnim.PlayMode playMode, bool queue = false, int frameOffset = 0)
	{
		LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode();
		int num = 0;
		while (linkedListNode != null)
		{
			if (queue)
			{
				linkedListNode.Value.animController.Queue(animName, playMode, 1f, 0f);
			}
			else
			{
				linkedListNode.Value.animController.Play(animName, playMode, 1f, 0f);
			}
			if (frameOffset > 0)
			{
				float num2 = (float)linkedListNode.Value.animController.GetCurrentNumFrames();
				float num3 = (float)num * ((float)frameOffset / num2);
				linkedListNode.Value.animController.SetElapsedTime(num3);
			}
			num++;
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateRetractedPre(SegmentedCreature.Instance smi, float dt)
	{
		if (this.UpdateHeadPosition(smi) == 0f)
		{
			return;
		}
		bool flag = true;
		for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.distanceToPreviousSegment = Mathf.Max(smi.def.minSegmentSpacing, linkedListNode.Value.distanceToPreviousSegment - dt * smi.def.retractionSegmentSpeed);
			if (linkedListNode.Value.distanceToPreviousSegment > smi.def.minSegmentSpacing)
			{
				flag = false;
			}
		}
		SegmentedCreature.CreatureSegment value = smi.GetHeadSegmentNode().Value;
		LinkedListNode<SegmentedCreature.PathNode> linkedListNode2 = smi.path.First;
		Vector3 forward = value.Forward;
		Quaternion rotation = value.Rotation;
		int num = 0;
		while (linkedListNode2 != null)
		{
			Vector3 vector = value.Position - smi.def.pathSpacing * (float)num * forward;
			linkedListNode2.Value.position = Vector3.Lerp(linkedListNode2.Value.position, vector, dt * smi.def.retractionPathSpeed);
			linkedListNode2.Value.rotation = Quaternion.Slerp(linkedListNode2.Value.rotation, rotation, dt * smi.def.retractionPathSpeed);
			num++;
			linkedListNode2 = linkedListNode2.Next;
		}
		this.UpdateBodyPosition(smi);
		if (flag)
		{
			smi.GoTo(this.retracted.loop);
		}
	}

	private void UpdateRetractedLoop(SegmentedCreature.Instance smi, float dt)
	{
		if (this.UpdateHeadPosition(smi) != 0f)
		{
			this.SetRetractedPath(smi);
			this.UpdateBodyPosition(smi);
		}
	}

	private void SetRetractedPath(SegmentedCreature.Instance smi)
	{
		SegmentedCreature.CreatureSegment value = smi.GetHeadSegmentNode().Value;
		LinkedListNode<SegmentedCreature.PathNode> linkedListNode = smi.path.First;
		Vector3 position = value.Position;
		Quaternion rotation = value.Rotation;
		Vector3 forward = value.Forward;
		int num = 0;
		while (linkedListNode != null)
		{
			linkedListNode.Value.position = position - smi.def.pathSpacing * (float)num * forward;
			linkedListNode.Value.rotation = rotation;
			num++;
			linkedListNode = linkedListNode.Next;
		}
	}

	private void UpdateFreeMovement(SegmentedCreature.Instance smi, float dt)
	{
		float num = this.UpdateHeadPosition(smi);
		if (num != 0f)
		{
			this.AdjustBodySegmentsSpacing(smi, num);
			this.UpdateBodyPosition(smi);
		}
	}

	private float UpdateHeadPosition(SegmentedCreature.Instance smi)
	{
		SegmentedCreature.CreatureSegment value = smi.GetHeadSegmentNode().Value;
		if (value.Position == smi.previousHeadPosition)
		{
			return 0f;
		}
		SegmentedCreature.PathNode value2 = smi.path.First.Value;
		SegmentedCreature.PathNode pathNode = smi.path.First.Next.Value;
		float magnitude = (value2.position - pathNode.position).magnitude;
		float magnitude2 = (value.Position - pathNode.position).magnitude;
		float num = magnitude2 - magnitude;
		value2.position = value.Position;
		value2.rotation = value.Rotation;
		smi.previousHeadPosition = value2.position;
		Vector3 normalized = (value2.position - pathNode.position).normalized;
		int num2 = Mathf.FloorToInt(magnitude2 / smi.def.pathSpacing);
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector = pathNode.position + normalized * smi.def.pathSpacing;
			LinkedListNode<SegmentedCreature.PathNode> last = smi.path.Last;
			last.Value.position = vector;
			last.Value.rotation = value2.rotation;
			float num3 = magnitude2 - (float)i * smi.def.pathSpacing;
			float num4 = num3 - smi.def.pathSpacing / num3;
			last.Value.rotation = Quaternion.Lerp(value2.rotation, pathNode.rotation, num4);
			smi.path.RemoveLast();
			smi.path.AddAfter(smi.path.First, last);
			pathNode = last.Value;
		}
		return num;
	}

	private void AdjustBodySegmentsSpacing(SegmentedCreature.Instance smi, float spacing)
	{
		for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value.distanceToPreviousSegment += spacing;
			if (linkedListNode.Value.distanceToPreviousSegment < smi.def.minSegmentSpacing)
			{
				spacing = linkedListNode.Value.distanceToPreviousSegment - smi.def.minSegmentSpacing;
				linkedListNode.Value.distanceToPreviousSegment = smi.def.minSegmentSpacing;
			}
			else
			{
				if (linkedListNode.Value.distanceToPreviousSegment <= smi.def.maxSegmentSpacing)
				{
					break;
				}
				spacing = linkedListNode.Value.distanceToPreviousSegment - smi.def.maxSegmentSpacing;
				linkedListNode.Value.distanceToPreviousSegment = smi.def.maxSegmentSpacing;
			}
		}
	}

	private void UpdateBodyPosition(SegmentedCreature.Instance smi)
	{
		LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = smi.GetFirstBodySegmentNode();
		LinkedListNode<SegmentedCreature.PathNode> linkedListNode2 = smi.path.First;
		float num = 0f;
		float num2 = smi.LengthPercentage();
		int num3 = 0;
		while (linkedListNode != null)
		{
			float num4 = linkedListNode.Value.distanceToPreviousSegment;
			float num5 = 0f;
			while (linkedListNode2.Next != null)
			{
				num5 = (linkedListNode2.Value.position - linkedListNode2.Next.Value.position).magnitude - num;
				if (num4 < num5)
				{
					break;
				}
				num4 -= num5;
				num = 0f;
				linkedListNode2 = linkedListNode2.Next;
			}
			if (linkedListNode2.Next == null)
			{
				linkedListNode.Value.SetPosition(linkedListNode2.Value.position);
				linkedListNode.Value.SetRotation(smi.path.Last.Value.rotation);
			}
			else
			{
				SegmentedCreature.PathNode value = linkedListNode2.Value;
				SegmentedCreature.PathNode value2 = linkedListNode2.Next.Value;
				linkedListNode.Value.SetPosition(linkedListNode2.Value.position + (linkedListNode2.Next.Value.position - linkedListNode2.Value.position).normalized * num4);
				linkedListNode.Value.SetRotation(Quaternion.Slerp(value.rotation, value2.rotation, num4 / num5));
				num = num4;
			}
			linkedListNode.Value.animController.FlipX = linkedListNode.Previous.Value.Position.x < linkedListNode.Value.Position.x;
			linkedListNode.Value.animController.animScale = smi.baseAnimScale + smi.baseAnimScale * smi.def.compressedMaxScale * ((float)(smi.def.numBodySegments - num3) / (float)smi.def.numBodySegments) * (1f - num2);
			linkedListNode = linkedListNode.Next;
			num3++;
		}
	}

	private void DrawDebug(SegmentedCreature.Instance smi, float dt)
	{
		SegmentedCreature.CreatureSegment value = smi.GetHeadSegmentNode().Value;
		DrawUtil.Arrow(value.Position, value.Position + value.Up, 0.05f, Color.red, 0f);
		DrawUtil.Arrow(value.Position, value.Position + value.Forward * 0.06f, 0.05f, Color.cyan, 0f);
		int num = 0;
		foreach (SegmentedCreature.PathNode pathNode in smi.path)
		{
			Color color = Color.HSVToRGB((float)num / (float)smi.def.numPathNodes, 1f, 1f);
			DrawUtil.Gnomon(pathNode.position, 0.05f, Color.cyan, 0f);
			DrawUtil.Arrow(pathNode.position, pathNode.position + pathNode.rotation * Vector3.up * 0.5f, 0.025f, color, 0f);
			num++;
		}
		for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = smi.segments.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			DrawUtil.Circle(linkedListNode.Value.Position, 0.05f, Color.white, new Vector3?(Vector3.forward), 0f);
			DrawUtil.Gnomon(linkedListNode.Value.Position, 0.05f, Color.white, 0f);
		}
	}

	public SegmentedCreature.RectractStates retracted;

	public SegmentedCreature.FreeMovementStates freeMovement;

	private StateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.BoolParameter isRetracted;

	public class Def : StateMachine.BaseDef
	{
		public HashedString segmentTrackerSymbol;

		public Vector3 headOffset = Vector3.zero;

		public Vector3 bodyPivot = Vector3.zero;

		public Vector3 tailPivot = Vector3.zero;

		public int numBodySegments;

		public float minSegmentSpacing;

		public float maxSegmentSpacing;

		public int numPathNodes;

		public float pathSpacing;

		public KAnimFile midAnim;

		public KAnimFile tailAnim;

		public string movingAnimName;

		public string idleAnimName;

		public float retractionSegmentSpeed = 1f;

		public float retractionPathSpeed = 1f;

		public float compressedMaxScale = 1.2f;

		public int animFrameOffset;

		public HashSet<HashedString> hideBoddyWhenStartingAnimNames = new HashSet<HashedString> { "rocket_biological" };

		public HashSet<HashedString> retractWhenStartingAnimNames = new HashSet<HashedString> { "trapped", "trussed", "escape", "drown_pre", "drown_loop", "drown_pst", "rocket_biological" };

		public HashSet<HashedString> retractWhenEndingAnimNames = new HashSet<HashedString> { "floor_floor_2_0", "grooming_pst", "fall" };
	}

	public class RectractStates : GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State
	{
		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State pre;

		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State loop;
	}

	public class FreeMovementStates : GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State
	{
		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State idle;

		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State moving;

		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State layEgg;

		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State poop;

		public GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.State dead;
	}

	public new class Instance : GameStateMachine<SegmentedCreature, SegmentedCreature.Instance, IStateMachineTarget, SegmentedCreature.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, SegmentedCreature.Def def)
			: base(master, def)
		{
			global::Debug.Assert((float)def.numBodySegments * def.maxSegmentSpacing < (float)def.numPathNodes * def.pathSpacing);
			this.CreateSegments();
			this.navigator = master.GetComponent<Navigator>();
		}

		private void CreateSegments()
		{
			float num = (float)SegmentedCreature.Instance.creatureBatchSlot * 0.01f;
			SegmentedCreature.Instance.creatureBatchSlot = (SegmentedCreature.Instance.creatureBatchSlot + 1) % 10;
			SegmentedCreature.CreatureSegment value = this.segments.AddFirst(new SegmentedCreature.CreatureSegment(base.GetComponent<KBatchedAnimController>(), base.gameObject, num, base.smi.def.headOffset, Vector3.zero)).Value;
			base.gameObject.SetActive(false);
			value.animController = base.GetComponent<KBatchedAnimController>();
			value.animController.SetSymbolVisiblity(base.smi.def.segmentTrackerSymbol, false);
			value.symbol = base.smi.def.segmentTrackerSymbol;
			value.SetPosition(base.transform.position);
			base.gameObject.SetActive(true);
			this.baseAnimScale = value.animController.animScale;
			value.animController.onAnimEnter += this.AnimEntered;
			value.animController.onAnimComplete += this.AnimComplete;
			for (int i = 0; i < base.def.numBodySegments; i++)
			{
				GameObject gameObject = new GameObject(base.gameObject.GetProperName() + string.Format(" Segment {0}", i));
				gameObject.SetActive(false);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = value.Position;
				KAnimFile kanimFile = base.def.midAnim;
				Vector3 vector = base.def.bodyPivot;
				if (i == base.def.numBodySegments - 1)
				{
					kanimFile = base.def.tailAnim;
					vector = base.def.tailPivot;
				}
				KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
				kbatchedAnimController.AnimFiles = new KAnimFile[] { kanimFile };
				kbatchedAnimController.isMovable = true;
				kbatchedAnimController.SetSymbolVisiblity(base.smi.def.segmentTrackerSymbol, false);
				kbatchedAnimController.sceneLayer = value.animController.sceneLayer;
				SegmentedCreature.CreatureSegment creatureSegment = new SegmentedCreature.CreatureSegment(value.animController, gameObject, num + (float)(i + 1) * 0.0001f, Vector3.zero, vector);
				creatureSegment.animController = kbatchedAnimController;
				creatureSegment.symbol = base.smi.def.segmentTrackerSymbol;
				creatureSegment.distanceToPreviousSegment = base.smi.def.minSegmentSpacing;
				creatureSegment.animLink = new KAnimLink(value.animController, kbatchedAnimController);
				this.segments.AddLast(creatureSegment);
				gameObject.SetActive(true);
			}
			for (int j = 0; j < base.def.numPathNodes; j++)
			{
				this.path.AddLast(new SegmentedCreature.PathNode(value.Position));
			}
		}

		public void AnimEntered(HashedString name)
		{
			if (base.smi.def.retractWhenStartingAnimNames.Contains(name))
			{
				base.smi.sm.isRetracted.Set(true, base.smi, false);
			}
			else
			{
				base.smi.sm.isRetracted.Set(false, base.smi, false);
			}
			if (base.smi.def.hideBoddyWhenStartingAnimNames.Contains(name))
			{
				this.SetBodySegmentsVisibility(false);
				return;
			}
			this.SetBodySegmentsVisibility(true);
		}

		public void SetBodySegmentsVisibility(bool visible)
		{
			for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = base.smi.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.animController.SetVisiblity(visible);
			}
		}

		public void AnimComplete(HashedString name)
		{
			if (base.smi.def.retractWhenEndingAnimNames.Contains(name))
			{
				base.smi.sm.isRetracted.Set(true, base.smi, false);
			}
		}

		public LinkedListNode<SegmentedCreature.CreatureSegment> GetHeadSegmentNode()
		{
			return base.smi.segments.First;
		}

		public LinkedListNode<SegmentedCreature.CreatureSegment> GetFirstBodySegmentNode()
		{
			return base.smi.segments.First.Next;
		}

		public float LengthPercentage()
		{
			float num = 0f;
			for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = this.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				num += linkedListNode.Value.distanceToPreviousSegment;
			}
			float num2 = this.MinLength();
			float num3 = this.MaxLength();
			return Mathf.Clamp(num - num2, 0f, num3) / (num3 - num2);
		}

		public float MinLength()
		{
			return base.smi.def.minSegmentSpacing * (float)base.smi.def.numBodySegments;
		}

		public float MaxLength()
		{
			return base.smi.def.maxSegmentSpacing * (float)base.smi.def.numBodySegments;
		}

		protected override void OnCleanUp()
		{
			this.GetHeadSegmentNode().Value.animController.onAnimEnter -= this.AnimEntered;
			this.GetHeadSegmentNode().Value.animController.onAnimComplete -= this.AnimComplete;
			for (LinkedListNode<SegmentedCreature.CreatureSegment> linkedListNode = this.GetFirstBodySegmentNode(); linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.CleanUp();
			}
		}

		private const int NUM_CREATURE_SLOTS = 10;

		private static int creatureBatchSlot;

		public float baseAnimScale;

		public Vector3 previousHeadPosition;

		public float previousDist;

		public LinkedList<SegmentedCreature.PathNode> path = new LinkedList<SegmentedCreature.PathNode>();

		public LinkedList<SegmentedCreature.CreatureSegment> segments = new LinkedList<SegmentedCreature.CreatureSegment>();

		public Navigator navigator;
	}

	public class PathNode
	{
		public PathNode(Vector3 position)
		{
			this.position = position;
			this.rotation = Quaternion.identity;
		}

		public Vector3 position;

		public Quaternion rotation;
	}

	public class CreatureSegment
	{
		public float ZOffset
		{
			get
			{
				return Grid.GetLayerZ(this.head.sceneLayer) + this.zRelativeOffset;
			}
		}

		public CreatureSegment(KBatchedAnimController head, GameObject go, float zRelativeOffset, Vector3 offset, Vector3 pivot)
		{
			this.head = head;
			this.m_transform = go.transform;
			this.zRelativeOffset = zRelativeOffset;
			this.offset = offset;
			this.pivot = pivot;
			this.SetPosition(go.transform.position);
		}

		public Vector3 Position
		{
			get
			{
				Vector3 vector = this.offset;
				vector.x *= (float)(this.animController.FlipX ? (-1) : 1);
				if (vector != Vector3.zero)
				{
					vector = this.Rotation * vector;
				}
				if (this.symbol.IsValid)
				{
					bool flag;
					Vector3 vector2 = this.animController.GetSymbolTransform(this.symbol, out flag).GetColumn(3);
					vector2.z = this.ZOffset;
					return vector2 + vector;
				}
				return this.m_transform.position + vector;
			}
		}

		public void SetPosition(Vector3 value)
		{
			bool flag = false;
			if (this.animController != null && this.animController.sceneLayer != this.head.sceneLayer)
			{
				this.animController.SetSceneLayer(this.head.sceneLayer);
				flag = true;
			}
			value.z = this.ZOffset;
			this.m_transform.position = value;
			if (flag)
			{
				this.animController.enabled = false;
				this.animController.enabled = true;
			}
		}

		public void SetRotation(Quaternion rotation)
		{
			this.m_transform.rotation = rotation;
		}

		public Quaternion Rotation
		{
			get
			{
				if (this.symbol.IsValid)
				{
					bool flag;
					Vector3 vector = this.animController.GetSymbolLocalTransform(this.symbol, out flag).MultiplyVector(Vector3.right);
					if (!this.animController.FlipX)
					{
						vector.y *= -1f;
					}
					return Quaternion.FromToRotation(Vector3.right, vector);
				}
				return this.m_transform.rotation;
			}
		}

		public Vector3 Forward
		{
			get
			{
				return this.Rotation * (this.animController.FlipX ? Vector3.left : Vector3.right);
			}
		}

		public Vector3 Up
		{
			get
			{
				return this.Rotation * Vector3.up;
			}
		}

		public void CleanUp()
		{
			global::UnityEngine.Object.Destroy(this.m_transform.gameObject);
		}

		public KBatchedAnimController animController;

		public KAnimLink animLink;

		public float distanceToPreviousSegment;

		public HashedString symbol;

		public Vector3 offset;

		public Vector3 pivot;

		public KBatchedAnimController head;

		private float zRelativeOffset;

		private Transform m_transform;
	}
}
