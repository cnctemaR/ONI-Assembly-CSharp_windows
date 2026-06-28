using System;
using UnityEngine;

public class Audio : ScriptableObject
{
	public static Audio Get()
	{
		if (Audio._Instance == null)
		{
			Audio._Instance = Resources.Load<Audio>("Audio");
		}
		return Audio._Instance;
	}

	private static Audio _Instance;

	public float listenerMinZ;

	public float listenerMinOrthographicSize;

	public float listenerReferenceZ;

	public float listenerReferenceOrthographicSize;
}
