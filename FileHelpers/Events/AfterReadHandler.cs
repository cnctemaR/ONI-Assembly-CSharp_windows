using System;

namespace FileHelpers.Events
{
	public delegate void AfterReadHandler<T>(EngineBase engine, AfterReadEventArgs<T> e) where T : class;
}
