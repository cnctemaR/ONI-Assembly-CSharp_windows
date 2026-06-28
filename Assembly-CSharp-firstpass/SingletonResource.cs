using System;
using UnityEngine;

public static class SingletonResource<T> where T : ResourceFile
{
	public static T Get()
	{
		if (SingletonResource<T>.StaticInstance == null)
		{
			SingletonResource<T>.StaticInstance = Resources.Load<T>(typeof(T).Name);
			SingletonResource<T>.StaticInstance.Initialize();
		}
		return SingletonResource<T>.StaticInstance;
	}

	private static T StaticInstance;
}
