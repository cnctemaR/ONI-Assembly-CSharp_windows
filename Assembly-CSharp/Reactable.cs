using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reactable
{
	public bool IsValid
	{
		get
		{
			return this.partitionerEntry.IsValid();
		}
	}

	public float creationTime { get; private set; }

	public bool IsReacting
	{
		get
		{
			return this.reactor != null;
		}
	}

	public Reactable(GameObject gameObject, HashedString id, ChoreType chore_type, int range_width = 15, int range_height = 8, bool follow_transform = false, float globalCooldown = 0f, float localCooldown = 0f, float lifeSpan = float.PositiveInfinity, float max_initial_delay = 0f, ObjectLayer overrideLayer = ObjectLayer.NumLayers)
	{
		this.rangeHeight = range_height;
		this.rangeWidth = range_width;
		this.id = id;
		this.gameObject = gameObject;
		this.choreType = chore_type;
		this.globalCooldown = globalCooldown;
		this.localCooldown = localCooldown;
		this.lifeSpan = lifeSpan;
		this.initialDelay = ((max_initial_delay > 0f) ? global::UnityEngine.Random.Range(0f, max_initial_delay) : 0f);
		this.creationTime = GameClock.Instance.GetTime();
		ObjectLayer objectLayer = ((overrideLayer == ObjectLayer.NumLayers) ? this.reactionLayer : overrideLayer);
		ReactionMonitor.Def def = gameObject.GetDef<ReactionMonitor.Def>();
		if (overrideLayer != objectLayer && def != null)
		{
			objectLayer = def.ReactionLayer;
		}
		this.reactionLayer = objectLayer;
		this.Initialize(follow_transform);
	}

	public void Initialize(bool followTransform)
	{
		this.UpdateLocation();
		if (followTransform)
		{
			this.transformId = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.gameObject.transform, new global::System.Action(this.UpdateLocation), "Reactable follow transform");
		}
	}

	public void Begin(GameObject reactor)
	{
		this.reactor = reactor;
		this.lastTriggerTime = GameClock.Instance.GetTime();
		this.InternalBegin();
	}

	public void End()
	{
		this.InternalEnd();
		if (this.reactor != null)
		{
			GameObject gameObject = this.reactor;
			this.InternalEnd();
			this.reactor = null;
			if (gameObject != null)
			{
				ReactionMonitor.Instance smi = gameObject.GetSMI<ReactionMonitor.Instance>();
				if (smi != null)
				{
					smi.StopReaction();
				}
			}
		}
	}

	public bool CanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		float time = GameClock.Instance.GetTime();
		float num = time - this.creationTime;
		float num2 = time - this.lastTriggerTime;
		if (num < this.initialDelay || num2 < this.globalCooldown)
		{
			return false;
		}
		ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
		Chore chore = ((component != null) ? component.choreDriver.GetCurrentChore() : null);
		if (chore == null || this.choreType.priority <= chore.choreType.priority)
		{
			return false;
		}
		int num3 = 0;
		while (this.additionalPreconditions != null && num3 < this.additionalPreconditions.Count)
		{
			if (!this.additionalPreconditions[num3](reactor, transition))
			{
				return false;
			}
			num3++;
		}
		return this.InternalCanBegin(reactor, transition);
	}

	public bool IsExpired()
	{
		return GameClock.Instance.GetTime() - this.creationTime > this.lifeSpan;
	}

	public abstract bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition);

	public abstract void Update(float dt);

	protected abstract void InternalBegin();

	protected abstract void InternalEnd();

	protected abstract void InternalCleanup();

	public void Cleanup()
	{
		this.End();
		this.InternalCleanup();
		if (this.transformId != -1)
		{
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transformId, new global::System.Action(this.UpdateLocation));
			this.transformId = -1;
		}
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	private void UpdateLocation()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		if (this.gameObject != null)
		{
			this.sourceCell = Grid.PosToCell(this.gameObject);
			Extents extents = new Extents(Grid.PosToXY(this.gameObject.transform.GetPosition()).x - this.rangeWidth / 2, Grid.PosToXY(this.gameObject.transform.GetPosition()).y - this.rangeHeight / 2, this.rangeWidth, this.rangeHeight);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Reactable", this, extents, GameScenePartitioner.Instance.objectLayers[(int)this.reactionLayer], null);
		}
	}

	public Reactable AddPrecondition(Reactable.ReactablePrecondition precondition)
	{
		if (this.additionalPreconditions == null)
		{
			this.additionalPreconditions = new List<Reactable.ReactablePrecondition>();
		}
		this.additionalPreconditions.Add(precondition);
		return this;
	}

	public void InsertPrecondition(int index, Reactable.ReactablePrecondition precondition)
	{
		if (this.additionalPreconditions == null)
		{
			this.additionalPreconditions = new List<Reactable.ReactablePrecondition>();
		}
		index = Math.Min(index, this.additionalPreconditions.Count);
		this.additionalPreconditions.Insert(index, precondition);
	}

	private HandleVector<int>.Handle partitionerEntry;

	protected GameObject gameObject;

	public HashedString id;

	public bool preventChoreInterruption = true;

	public int sourceCell;

	private int rangeWidth;

	private int rangeHeight;

	private int transformId = -1;

	public float globalCooldown;

	public float localCooldown;

	public float lifeSpan = float.PositiveInfinity;

	private float lastTriggerTime = -2.1474836E+09f;

	private float initialDelay;

	protected GameObject reactor;

	private ChoreType choreType;

	protected LoggerFSS log;

	private List<Reactable.ReactablePrecondition> additionalPreconditions;

	private ObjectLayer reactionLayer;

	public delegate bool ReactablePrecondition(GameObject go, Navigator.ActiveTransition transition);
}
