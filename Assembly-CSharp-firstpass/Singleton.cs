using System;

public static class Singleton<T> where T : new()
{
	public static T Instance
	{
		get
		{
			if (Singleton<T>.StaticInstance == null)
			{
				Singleton<T>.StaticInstance = ((default(T) == null) ? new T() : default(T));
			}
			return Singleton<T>.StaticInstance;
		}
	}

	public static void Destroy()
	{
		Singleton<T>.StaticInstance = default(T);
	}

	private static T StaticInstance;
}
