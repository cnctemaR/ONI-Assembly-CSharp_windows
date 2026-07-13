using System;

namespace UnityEngine.Rendering
{
	public sealed class ResourcePathsAttribute : ResourcePathsBaseAttribute
	{
		public ResourcePathsAttribute(string[] paths, SearchType location = SearchType.ProjectPath)
			: base(paths, false, location)
		{
		}
	}
}
