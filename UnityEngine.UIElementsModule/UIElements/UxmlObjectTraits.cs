using System;

namespace UnityEngine.UIElements
{
	internal abstract class UxmlObjectTraits<T> : BaseUxmlTraits
	{
		public virtual void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
		{
		}
	}
}
