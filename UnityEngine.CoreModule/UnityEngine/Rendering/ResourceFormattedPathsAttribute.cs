using System;

namespace UnityEngine.Rendering
{
	public sealed class ResourceFormattedPathsAttribute : ResourcePathsBaseAttribute
	{
		public ResourceFormattedPathsAttribute(string pathFormat, int rangeMin, int rangeMax, SearchType location = SearchType.ProjectPath)
			: base(null, false, location)
		{
		}

		private static string[] CreateFormattedPaths(string format, int rangeMin, int rangeMax)
		{
			string[] array = new string[rangeMax - rangeMin];
			int i = rangeMin;
			int num = 0;
			while (i < rangeMax)
			{
				array[num] = string.Format(format, i);
				i++;
				num++;
			}
			return array;
		}
	}
}
