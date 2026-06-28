using System;

namespace FileHelpers.Events
{
	public delegate void AfterWriteHandler<T>(EngineBase engine, AfterWriteEventArgs<T> e) where T : class;
}
