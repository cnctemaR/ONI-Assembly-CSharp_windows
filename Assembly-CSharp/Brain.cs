using System;
using UnityEngine;

public abstract class Brain : KMonoBehaviour
{
	public event global::System.Action onPreUpdate;

	public bool clearDebugStatus { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.log = new LoggerFSS("Brain");
	}

	protected override void OnSpawn()
	{
		this.running = true;
		Components.Brains.Add(this);
	}

	public virtual void UpdateBrain()
	{
		if (this.onPreUpdate != null)
		{
			this.onPreUpdate();
		}
		int instanceID = base.gameObject.GetInstanceID();
		if (instanceID == Brain.stopID)
		{
			Output.Log(new object[] { "Break" });
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
		this.lastResumer = caller + "@" + Time.realtimeSinceStartup.ToString();
	}

	public void Suspend(string caller)
	{
		this.suspend = true;
		this.lastSuspender = caller + "@" + Time.realtimeSinceStartup.ToString();
	}

	protected override void OnCleanUp()
	{
		this.Stop("OnCleanUp");
		Components.Brains.Remove(this);
	}

	private bool running;

	private bool suspend;

	public string lastSuspender = "Nothing";

	public string lastResumer = "Nothing";

	private static int stopID = -1;

	protected LoggerFSS log;
}
