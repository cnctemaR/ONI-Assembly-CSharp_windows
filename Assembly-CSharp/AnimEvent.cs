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
		this.Name = name;
		this.Frame = frame;
	}

	public void Play(IAnimBehaviour behaviour)
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

	public virtual void OnPlay(IAnimBehaviour behaviour)
	{
	}

	public virtual void OnUpdate(IAnimBehaviour behaviour)
	{
	}

	public virtual void Stop(IAnimBehaviour behaviour)
	{
	}

	protected bool IsFilteredOut(IAnimBehaviour behaviour)
	{
		return this.File != null && behaviour.currentAnimFile != null && this.File != behaviour.currentAnimFile.ToLower();
	}

	public string Name;

	public string File;

	public int Frame;

	public bool OnExit;
}
