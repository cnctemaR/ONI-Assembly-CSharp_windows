using System;
using FMOD.Studio;

public abstract class OneShotSoundParameterUpdater
{
	public HashedString parameter { get; private set; }

	public OneShotSoundParameterUpdater(HashedString parameter)
	{
		this.parameter = parameter;
	}

	public virtual void Update(float dt)
	{
	}

	public abstract void Play(OneShotSoundParameterUpdater.Sound sound);

	public struct Sound
	{
		public EventInstance ev;

		public SoundDescription description;

		public HashedString path;
	}
}
