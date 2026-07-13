using System;

namespace UnityEngine.Rendering
{
	public sealed class ResourcePathAttribute : ResourcePathsBaseAttribute
	{
		public ResourcePathAttribute(string path, SearchType location = SearchType.ProjectPath)
			: base(null, true, location)
		{
		}
	}
}
