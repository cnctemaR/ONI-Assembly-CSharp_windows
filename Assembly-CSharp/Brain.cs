using System;
using System.Diagnostics;

public class Brain : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.running = true;
		Components.Brains.Add(this);
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		return base.GetComponent<ChoreConsumer>().FindNextChore(ref context);
	}

	private void UpdateChores()
	{
		if (base.GetComponent<KPrefabID>().HasTag(GameTags.PreventChoreInterruption))
		{
			return;
		}
		Chore.Precondition.Context context = default(Chore.Precondition.Context);
		if (this.FindBetterChore(ref context))
		{
			base.GetComponent<ChoreDriver>().SetChore(context);
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

	protected override void OnCleanUp()
	{
		this.Stop("OnCleanUp");
		Components.Brains.Remove(this);
	}

	private bool running;

	private bool suspend;
}
