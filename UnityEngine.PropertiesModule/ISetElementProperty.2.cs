using System;

namespace Unity.Properties
{
	public interface ISetElementProperty<out TKey> : ISetElementProperty, ICollectionElementProperty
	{
		TKey Key { get; }
	}
}
