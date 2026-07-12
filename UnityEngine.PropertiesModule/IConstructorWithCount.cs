using System;

namespace Unity.Properties
{
	internal interface IConstructorWithCount<out T> : IConstructor
	{
		T InstantiateWithCount(int count);
	}
}
