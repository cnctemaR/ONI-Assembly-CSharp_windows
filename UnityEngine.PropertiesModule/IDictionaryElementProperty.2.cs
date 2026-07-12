using System;

namespace Unity.Properties
{
	public interface IDictionaryElementProperty<out TKey> : IDictionaryElementProperty, ICollectionElementProperty
	{
		TKey Key { get; }
	}
}
