using System;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true)]
	public abstract class ResourcePathsBaseAttribute : Attribute
	{
		public SearchType location
		{
			get
			{
				Debug.LogWarning("ResourcePathsBaseAttribute.location cannot be reliable at runtime as data is not stored.");
				return SearchType.ProjectPath;
			}
		}

		public string[] paths
		{
			get
			{
				Debug.LogWarning("ResourcePathsBaseAttribute.paths cannot be reliable at runtime as data is not stored.");
				return null;
			}
		}

		public bool isField
		{
			get
			{
				Debug.LogWarning("ResourcePathsBaseAttribute.isField cannot be reliable at runtime as data is not stored.");
				return false;
			}
		}

		protected ResourcePathsBaseAttribute(string[] paths, bool isField, SearchType location)
		{
		}
	}
}
