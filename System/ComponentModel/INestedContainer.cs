using System;

namespace System.ComponentModel
{
	public interface INestedContainer : IDisposable, IContainer
	{
		IComponent Owner { get; }
	}
}
