using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Jobs
{
	public class EarlyInitHelpers
	{
		static EarlyInitHelpers()
		{
			EarlyInitHelpers.FlushEarlyInits();
		}

		public static void FlushEarlyInits()
		{
			while (EarlyInitHelpers.s_PendingDelegates != null)
			{
				List<EarlyInitHelpers.EarlyInitFunction> list = EarlyInitHelpers.s_PendingDelegates;
				EarlyInitHelpers.s_PendingDelegates = null;
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						list[i]();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
		}

		public static void AddEarlyInitFunction(EarlyInitHelpers.EarlyInitFunction func)
		{
			if (EarlyInitHelpers.s_PendingDelegates == null)
			{
				EarlyInitHelpers.s_PendingDelegates = new List<EarlyInitHelpers.EarlyInitFunction>();
			}
			EarlyInitHelpers.s_PendingDelegates.Add(func);
		}

		public static void JobReflectionDataCreationFailed(Exception ex)
		{
			Debug.LogError("Failed to create job reflection data. Please refer to callstack of exception for information on which job could not produce its reflection data.");
			Debug.LogException(ex);
		}

		private static List<EarlyInitHelpers.EarlyInitFunction> s_PendingDelegates;

		public delegate void EarlyInitFunction();
	}
}
