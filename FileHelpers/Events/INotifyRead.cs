using System;

namespace FileHelpers.Events
{
	public interface INotifyRead<T> where T : class
	{
		void AfterRead(AfterReadEventArgs<T> e);

		void BeforeRead(BeforeReadEventArgs<T> e);
	}
}
