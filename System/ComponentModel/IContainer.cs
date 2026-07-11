using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public interface IContainer : IDisposable
	{
		void Add(IComponent component);

		void Add(IComponent component, string name);

		ComponentCollection Components { get; }

		void Remove(IComponent component);
	}
}
