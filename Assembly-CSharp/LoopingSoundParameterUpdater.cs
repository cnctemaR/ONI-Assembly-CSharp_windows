using System;
using FMOD.Studio;
using UnityEngine;

public abstract class LoopingSoundParameterUpdater
{
	public HashedString parameter { get; private set; }

	public LoopingSoundParameterUpdater(HashedString parameter)
	{
		this.parameter = parameter;
	}

	public abstract void Add(LoopingSoundParameterUpdater.Sound sound);

	public abstract void Update(float dt);

	public abstract void Remove(LoopingSoundParameterUpdater.Sound sound);

	public struct Sound
	{
		public EventInstance ev;

		public HashedString path;

		public Transform transform;

		public SoundDescription description;

		public bool objectIsSelectedAndVisible;
	}
}
