using System;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal abstract class ReflectionItem
	{
		public abstract string Name { get; }

		public abstract string GetDisplayName();

		public abstract Type ReturnType { get; }

		public abstract ReflectionItemType ItemType { get; }
	}
}
