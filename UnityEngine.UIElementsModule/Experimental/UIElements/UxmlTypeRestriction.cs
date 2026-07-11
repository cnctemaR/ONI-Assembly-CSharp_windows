using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class UxmlTypeRestriction : IEquatable<UxmlTypeRestriction>
	{
		public virtual bool Equals(UxmlTypeRestriction other)
		{
			return this == other;
		}
	}
}
