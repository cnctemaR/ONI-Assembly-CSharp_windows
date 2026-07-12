using System;

namespace System.ComponentModel
{
	public interface IContainer : IDisposable
	{
		void Add(IComponent component);

		void Add(IComponent component, string name);

		ComponentCollection Components { get; }

		void Remove(IComponent component);
	}
}
