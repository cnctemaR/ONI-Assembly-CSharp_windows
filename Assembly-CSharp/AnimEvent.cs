using System;
using UnityEngine;

[Serializable]
public class AnimEvent
{
	[SerializeField]
	public string name { get; private set; }

	[SerializeField]
	public string file { get; private set; }

	[SerializeField]
	public int frame { get; private set; }

	public AnimEvent()
	{
	}

	public AnimEvent(string file, string name, int frame)
	{
		this.file = ((file == "") ? null : file);
		if (this.file != null)
		{
			this.fileHash = new KAnimHashedString(this.file);
		}
		this.name = name;
		this.frame = frame;
	}

	public void Play(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.IsFilteredOut(behaviour))
		{
			return;
		}
		if (behaviour.previousFrame < behaviour.currentFrame)
		{
			if (behaviour.previousFrame < this.frame && behaviour.currentFrame >= this.frame)
			{
				this.OnPlay(behaviour);
				return;
			}
		}
		else if (behaviour.previousFrame > behaviour.currentFrame && (behaviour.previousFrame < this.frame || this.frame <= behaviour.currentFrame))
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
		return this.file != null && !behaviour.controller.HasAnimationFile(this.fileHash);
	}

	[SerializeField]
	private KAnimHashedString fileHash;

	public bool OnExit;
}
