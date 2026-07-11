using System;

namespace System.ComponentModel
{
	public interface INestedSite : IServiceProvider, ISite
	{
		string FullName { get; }
	}
}
