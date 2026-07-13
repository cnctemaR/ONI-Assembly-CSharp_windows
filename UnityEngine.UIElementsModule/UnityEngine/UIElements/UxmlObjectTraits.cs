using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal abstract class UxmlObjectTraits<T> : BaseUxmlTraits
	{
		public virtual void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
		{
		}
	}
}
