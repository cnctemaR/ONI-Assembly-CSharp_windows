using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reactable
{
	public Reactable(GameObject gameObject, HashedString id, ChoreType chore_type, int range_width = 15, int range_height = 8, bool follow_transform = false, float min_reactable_time = 0f, float min_reactor_time = 0f, float max_trigger_time = float.PositiveInfinity)
	{
		this.rangeHeight = range_height;
		this.rangeWidth = range_width;
		this.id = id;
		this.gameObject = gameObject;
		this.choreType = chore_type;
		this.minReactableTime = min_reactable_time;
		this.minReactorTime = min_reactor_time;
		this.maxTriggerTime = max_trigger_time;
		this.creationTime = GameClock.Instance.GetTime();
		this.UpdateLocation();
		if (follow_transform)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(gameObject.transform, new global::System.Action(this.UpdateLocation), "Reactable follow transform");
		}
	}

	public bool IsReacting
	{
		get
		{
			return this.reactor != null;
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
		if (GameClock.Instance.GetTime() - this.lastTriggerTime < this.minReactableTime)
		{
			return false;
		}
		ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
		if (component == null)
		{
			return false;
		}
		Chore currentChore = component.choreDriver.GetCurrentChore();
		if (currentChore == null)
		{
			return false;
		}
		if (this.choreType.priority <= currentChore.choreType.priority)
		{
			return false;
		}
		if (this.additionalPreconditions != null)
		{
			foreach (Reactable.ReactablePrecondition reactablePrecondition in this.additionalPreconditions)
			{
				if (!reactablePrecondition(reactor, transition))
				{
					return false;
				}
			}
		}
		return this.InternalCanBegin(reactor, transition);
	}

	public bool IsExpired()
	{
		return GameClock.Instance.GetTime() - this.creationTime > this.maxTriggerTime;
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
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.gameObject.transform, new global::System.Action(this.UpdateLocation));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateLocation();
	}

	private void UpdateLocation()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		if (this.gameObject != null)
		{
			this.sourceCell = Grid.PosToCell(this.gameObject);
			Extents extents = new Extents(Grid.PosToXY(this.gameObject.transform.GetPosition()).x - this.rangeWidth / 2, Grid.PosToXY(this.gameObject.transform.GetPosition()).y - this.rangeHeight / 2, this.rangeWidth, this.rangeHeight);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Reactable", this, extents, GameScenePartitioner.Instance.objectLayers[0], null);
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

	private GameScenePartitionerEntry partitionerEntry;

	protected GameObject gameObject;

	public HashedString id;

	public bool preventChoreInterruption = true;

	public int sourceCell;

	private int rangeWidth;

	private int rangeHeight;

	public float minReactableTime;

	public float minReactorTime;

	public float maxTriggerTime = float.PositiveInfinity;

	private float lastTriggerTime = -2.1474836E+09f;

	private float creationTime;

	protected GameObject reactor;

	private ChoreType choreType;

	protected LoggerFSS log;

	private List<Reactable.ReactablePrecondition> additionalPreconditions;

	public delegate bool ReactablePrecondition(GameObject go, Navigator.ActiveTransition transition);
}
