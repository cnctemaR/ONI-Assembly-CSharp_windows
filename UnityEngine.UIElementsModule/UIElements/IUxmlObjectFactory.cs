using System;

namespace UnityEngine.UIElements
{
	internal interface IUxmlObjectFactory<out T> : IBaseUxmlObjectFactory, IBaseUxmlFactory where T : new()
	{
		T CreateObject(IUxmlAttributes bag, CreationContext cc);
	}
}
