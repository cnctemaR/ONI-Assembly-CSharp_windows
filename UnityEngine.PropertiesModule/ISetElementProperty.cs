using System;

namespace Unity.Properties
{
	public interface ISetElementProperty : ICollectionElementProperty
	{
		object ObjectKey { get; }
	}
}
