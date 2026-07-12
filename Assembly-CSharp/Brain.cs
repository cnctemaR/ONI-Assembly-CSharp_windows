using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Brain")]
public class Brain : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.prefabId = base.GetComponent<KPrefabID>();
		this.choreConsumer = base.GetComponent<ChoreConsumer>();
		this.running = true;
		Components.Brains.Add(this);
	}

	public event global::System.Action onPreUpdate;

	public virtual void UpdateBrain()
	{
		if (this.onPreUpdate != null)
		{
			this.onPreUpdate();
		}
		if (this.IsRunning())
		{
			this.UpdateChores();
		}
	}

	private bool FindBetterChore(ref Chore.Precondition.Context context)
	{
		return this.choreConsumer.FindNextChore(ref context);
	}

	private void UpdateChores()
	{
		if (this.prefabId.HasTag(GameTags.PreventChoreInterruption))
		{
			return;
		}
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		if (this.FindBetterChore(ref context))
		{
			if (this.prefabId.HasTag(GameTags.PerformingWorkRequest))
			{
				base.Trigger(1485595942, null);
				return;
			}
			this.choreConsumer.choreDriver.SetChore(context);
		}
	}

	public bool IsRunning()
	{
		return this.running && !this.suspend;
	}

	public void Reset(string reason)
	{
		this.Stop("Reset");
		this.running = true;
	}

	public void Stop(string reason)
	{
		base.GetComponent<ChoreDriver>().StopChore();
		this.running = false;
	}

	public void Resume(string caller)
	{
		this.suspend = false;
	}

	public void Suspend(string caller)
	{
		this.suspend = true;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.Stop("OnCmpDisable");
	}

	protected override void OnCleanUp()
	{
		this.Stop("OnCleanUp");
		Components.Brains.Remove(this);
	}

	private bool running;

	private bool suspend;

	protected KPrefabID prefabId;

	protected ChoreConsumer choreConsumer;
}
