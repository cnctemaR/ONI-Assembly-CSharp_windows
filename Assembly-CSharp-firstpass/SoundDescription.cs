using System;
using FMOD.Studio;

public struct SoundDescription
{
	public PARAMETER_ID GetParameterId(HashedString name)
	{
		foreach (SoundDescription.Parameter parameter in this.parameters)
		{
			if (parameter.name == name)
			{
				return parameter.id;
			}
		}
		return SoundDescription.Parameter.INVALID_ID;
	}

	public string path;

	public float falloffDistanceSq;

	public SoundDescription.Parameter[] parameters;

	public OneShotSoundParameterUpdater[] oneShotParameterUpdaters;

	public struct Parameter
	{
		public HashedString name;

		public PARAMETER_ID id;

		public static readonly PARAMETER_ID INVALID_ID;
	}
}
