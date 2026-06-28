using System;

[Serializable]
public class AnimEvent
{
	public AnimEvent()
	{
	}

	public AnimEvent(string file, string name, int frame)
	{
		this.File = ((!(file == string.Empty)) ? file : null);
		if (this.File != null)
		{
			this.FileHash = new KAnimHashedString(this.File);
		}
		this.Name = name;
		this.Frame = frame;
	}

	public string File { get; private set; }

	public void Play(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.IsFilteredOut(behaviour))
		{
			return;
		}
		if (behaviour.previousFrame < behaviour.currentFrame)
		{
			if (behaviour.previousFrame < this.Frame && behaviour.currentFrame >= this.Frame)
			{
				this.OnPlay(behaviour);
			}
		}
		else if (behaviour.previousFrame > behaviour.currentFrame && (behaviour.previousFrame < this.Frame || this.Frame <= behaviour.currentFrame))
		{
			this.OnPlay(behaviour);
		}
	}

	private void DebugAnimEvent(string ev_name, AnimEventManager.EventPlayerData behaviour)
	{
	}

	public virtual void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
	}

	public virtual void OnUpdate(AnimEventManager.EventPlayerData behaviour)
	{
	}

	public virtual void Stop(AnimEventManager.EventPlayerData behaviour)
	{
	}

	protected bool IsFilteredOut(AnimEventManager.EventPlayerData behaviour)
	{
		return this.File != null && behaviour.currentAnimFile != null && this.FileHash != behaviour.currentAnimFileHash;
	}

	public virtual bool ShouldPlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		CameraController instance = CameraController.Instance;
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		return (!(instance2 != null) || !instance2.IsPaused) && (!(instance != null) || instance.IsAudibleSound(behaviour.position, 0f));
	}

	public virtual void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
	}

	public string Name;

	private KAnimHashedString FileHash;

	public int Frame;

	public bool OnExit;
}
