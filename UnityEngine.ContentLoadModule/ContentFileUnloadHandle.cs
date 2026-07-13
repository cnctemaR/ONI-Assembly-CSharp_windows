using System;

namespace Unity.Loading
{
	public struct ContentFileUnloadHandle
	{
		public bool IsCompleted
		{
			get
			{
				return ContentLoadInterface.ContentFile_IsUnloadComplete(this.Id);
			}
		}

		public bool WaitForCompletion(int timeoutMs)
		{
			return ContentLoadInterface.WaitForUnloadCompletion(this.Id, timeoutMs);
		}

		internal ContentFile Id;
	}
}
