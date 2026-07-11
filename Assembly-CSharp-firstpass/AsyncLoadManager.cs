using System;
using System.Collections.Generic;
using System.Reflection;

public static class AsyncLoadManager<AsyncLoaderType>
{
	public static void Run()
	{
		List<AsyncLoader> list = new List<AsyncLoader>();
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (!type.IsAbstract)
				{
					if (typeof(AsyncLoaderType).IsAssignableFrom(type))
					{
						AsyncLoader asyncLoader = (AsyncLoader)Activator.CreateInstance(type);
						list.Add(asyncLoader);
						AsyncLoadManager<AsyncLoaderType>.loaders[type] = asyncLoader;
						asyncLoader.CollectLoaders(list);
					}
				}
			}
		}
		if (AsyncLoadManager<AsyncLoaderType>.loaders.Count > 0)
		{
			WorkItemCollection<AsyncLoadManager<AsyncLoaderType>.RunLoader, object> workItemCollection = new WorkItemCollection<AsyncLoadManager<AsyncLoaderType>.RunLoader, object>();
			workItemCollection.Reset(null);
			foreach (AsyncLoader asyncLoader2 in list)
			{
				workItemCollection.Add(new AsyncLoadManager<AsyncLoaderType>.RunLoader
				{
					loader = asyncLoader2
				});
			}
			GlobalJobManager.Run(workItemCollection);
		}
	}

	public static AsyncLoader GetLoader(Type type)
	{
		return AsyncLoadManager<AsyncLoaderType>.loaders[type];
	}

	private static Dictionary<Type, AsyncLoader> loaders = new Dictionary<Type, AsyncLoader>();

	public abstract class AsyncLoader<LoaderType> : AsyncLoader where LoaderType : class
	{
		public static LoaderType Get()
		{
			return AsyncLoadManager<AsyncLoaderType>.GetLoader(typeof(LoaderType)) as LoaderType;
		}
	}

	private struct RunLoader : IWorkItem<object>
	{
		public void Run(object shared_data)
		{
			this.loader.Run();
		}

		public AsyncLoader loader;
	}
}
