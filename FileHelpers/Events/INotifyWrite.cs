using System;

namespace FileHelpers.Events
{
	public interface INotifyWrite<T> where T : class
	{
		void BeforeWrite(BeforeWriteEventArgs<T> e);

		void AfterWrite(AfterWriteEventArgs<T> e);
	}
}
