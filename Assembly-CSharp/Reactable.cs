using System;
using UnityEngine;

public abstract class Reactable : ISim1000ms
{
	public Reactable(GameObject gameObject, ChoreType chore_type, int range_width = 15, int range_height = 8, bool follow_transform = false)
	{
		this.rangeHeight = range_height;
		this.rangeWidth = range_width;
		this.gameObject = gameObject;
		this.choreType = chore_type;
		this.UpdateLocation();
		if (follow_transform)
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}
	}

	public void Begin(GameObject reactor)
	{
		this.reactor = reactor;
		this.InternalBegin();
	}

	public void End()
	{
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
		ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
		if (component == null)
		{
			return false;
		}
		Chore currentChore = component.choreDriver.GetCurrentChore();
		return currentChore != null && this.choreType.priority > currentChore.choreType.priority && this.InternalCanBegin(reactor, transition);
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
		SimAndRenderScheduler.instance.Remove(this);
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

	private GameScenePartitionerEntry partitionerEntry;

	protected GameObject gameObject;

	public bool preventChoreInterruption = true;

	public int sourceCell;

	private int rangeWidth;

	private int rangeHeight;

	protected GameObject reactor;

	private ChoreType choreType;

	protected LoggerFSS log;
}
