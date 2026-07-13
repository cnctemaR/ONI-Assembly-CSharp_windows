using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	[Obsolete("Derive collection attributes from 'PropertyAttribute' and set its 'applyToCollection' property to 'true'.", false)]
	public abstract class PropertyCollectionAttribute : PropertyAttribute
	{
		protected PropertyCollectionAttribute()
			: base(true)
		{
		}
	}
}
