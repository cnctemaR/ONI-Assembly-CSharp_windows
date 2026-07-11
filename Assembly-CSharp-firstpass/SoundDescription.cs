using System;

public struct SoundDescription
{
	public int GetParameterIdx(HashedString name)
	{
		foreach (SoundDescription.Parameter parameter in this.parameters)
		{
			if (parameter.name == name)
			{
				return parameter.idx;
			}
		}
		return -1;
	}

	public string path;

	public float falloffDistanceSq;

	public SoundDescription.Parameter[] parameters;

	public OneShotSoundParameterUpdater[] oneShotParameterUpdaters;

	public struct Parameter
	{
		public HashedString name;

		public int idx;

		public const int INVALID_IDX = -1;
	}
}
