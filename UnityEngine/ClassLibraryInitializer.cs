using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Serialization;

namespace UnityEngine
{
	internal static class ClassLibraryInitializer
	{
		private static void Init()
		{
			UnityLogWriter.Init();
			if (Application.platform.ToString().Contains("WebPlayer"))
			{
				BinaryFormatter.DefaultSurrogateSelector = new UnitySurrogateSelector();
			}
		}
	}
}
