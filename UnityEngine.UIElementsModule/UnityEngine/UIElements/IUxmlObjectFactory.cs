using System;

namespace UnityEngine.UIElements
{
	[Obsolete("IUxmlObjectFactory<out T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal interface IUxmlObjectFactory<out T> : IBaseUxmlObjectFactory, IBaseUxmlFactory where T : new()
	{
		T CreateObject(IUxmlAttributes bag, CreationContext cc);
	}
}
