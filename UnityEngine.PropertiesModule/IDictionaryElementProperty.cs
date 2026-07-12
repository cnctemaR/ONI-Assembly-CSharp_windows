using System;

namespace Unity.Properties
{
	public interface IDictionaryElementProperty : ICollectionElementProperty
	{
		object ObjectKey { get; }
	}
}
