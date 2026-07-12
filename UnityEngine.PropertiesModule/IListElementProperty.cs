using System;

namespace Unity.Properties
{
	public interface IListElementProperty : ICollectionElementProperty
	{
		int Index { get; }
	}
}
