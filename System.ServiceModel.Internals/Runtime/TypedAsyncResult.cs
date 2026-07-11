using System;

namespace System.Runtime
{
	internal abstract class TypedAsyncResult<T> : AsyncResult
	{
		public TypedAsyncResult(AsyncCallback callback, object state)
			: base(callback, state)
		{
		}

		public T Data
		{
			get
			{
				return this.data;
			}
		}

		protected void Complete(T data, bool completedSynchronously)
		{
			this.data = data;
			base.Complete(completedSynchronously);
		}

		public static T End(IAsyncResult result)
		{
			return AsyncResult.End<TypedAsyncResult<T>>(result).Data;
		}

		private T data;
	}
}
