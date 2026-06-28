using System;

namespace FileHelpers.Events
{
	public delegate void BeforeReadHandler<T>(EngineBase engine, BeforeReadEventArgs<T> e) where T : class;
}
