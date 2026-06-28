using System;

public static class Singleton<T> where T : class, new()
{
	public static T Instance
	{
		get
		{
			object @lock = Singleton<T>._lock;
			T instance;
			lock (@lock)
			{
				if (Singleton<T>._instance == null)
				{
					Singleton<T>._instance = new T();
				}
				instance = Singleton<T>._instance;
			}
			return instance;
		}
	}

	public static void CreateInstance()
	{
		object @lock = Singleton<T>._lock;
		lock (@lock)
		{
			if (Singleton<T>._instance == null)
			{
				Singleton<T>._instance = new T();
			}
		}
	}

	public static void Destroy()
	{
		object @lock = Singleton<T>._lock;
		lock (@lock)
		{
			Singleton<T>._instance = (T)((object)null);
		}
	}

	private static T _instance;

	private static object _lock = new object();
}
