using System;

namespace Unity.Properties
{
	internal interface IConstructor<out T> : IConstructor
	{
		T Instantiate();
	}
}
