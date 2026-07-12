using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	internal static class AtomicCompositionExtensions
	{
		internal static T GetValueAllowNull<T>(this AtomicComposition atomicComposition, T defaultResultAndKey) where T : class
		{
			Assumes.NotNull<T>(defaultResultAndKey);
			return atomicComposition.GetValueAllowNull(defaultResultAndKey, defaultResultAndKey);
		}

		internal static T GetValueAllowNull<T>(this AtomicComposition atomicComposition, object key, T defaultResult)
		{
			T t;
			if (atomicComposition != null && atomicComposition.TryGetValue<T>(key, out t))
			{
				return t;
			}
			return defaultResult;
		}

		internal static void AddRevertActionAllowNull(this AtomicComposition atomicComposition, Action action)
		{
			Assumes.NotNull<Action>(action);
			if (atomicComposition == null)
			{
				action();
				return;
			}
			atomicComposition.AddRevertAction(action);
		}

		internal static void AddCompleteActionAllowNull(this AtomicComposition atomicComposition, Action action)
		{
			Assumes.NotNull<Action>(action);
			if (atomicComposition == null)
			{
				action();
				return;
			}
			atomicComposition.AddCompleteAction(action);
		}
	}
}
