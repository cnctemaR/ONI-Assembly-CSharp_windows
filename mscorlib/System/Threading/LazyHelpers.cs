using System;

namespace System.Threading
{
	internal static class LazyHelpers<T>
	{
		private static T ActivatorFactorySelector()
		{
			T t;
			try
			{
				t = (T)((object)Activator.CreateInstance(typeof(T)));
			}
			catch (MissingMethodException)
			{
				throw new MissingMemberException(Environment.GetResourceString("The lazily-initialized type does not have a public, parameterless constructor."));
			}
			return t;
		}

		internal static Func<T> s_activatorFactorySelector = new Func<T>(LazyHelpers<T>.ActivatorFactorySelector);
	}
}
