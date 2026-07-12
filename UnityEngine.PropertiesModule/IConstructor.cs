using System;

namespace Unity.Properties
{
	internal interface IConstructor
	{
		InstantiationKind InstantiationKind { get; }
	}
}
