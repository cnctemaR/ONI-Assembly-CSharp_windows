using System;

namespace FileHelpers.Events
{
	public delegate void BeforeWriteHandler<T>(EngineBase engine, BeforeWriteEventArgs<T> e) where T : class;
}
