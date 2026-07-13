using System;
using UnityEngine;

namespace Unity.Loading
{
	public struct ContentFile
	{
		public ContentFileUnloadHandle UnloadAsync()
		{
			this.ThrowIfInvalidHandle();
			ContentLoadInterface.ContentFile_UnloadAsync(this);
			return new ContentFileUnloadHandle
			{
				Id = this
			};
		}

		public Object[] GetObjects()
		{
			this.ThrowIfNotComplete();
			return ContentLoadInterface.ContentFile_GetObjects(this);
		}

		public Object GetObject(ulong localIdentifierInFile)
		{
			this.ThrowIfNotComplete();
			return ContentLoadInterface.ContentFile_GetObject(this, localIdentifierInFile);
		}

		private void ThrowIfInvalidHandle()
		{
			bool flag = !this.IsValid;
			if (flag)
			{
				throw new Exception("The ContentFile operation cannot be performed because the handle is invalid. Did you already unload it?");
			}
		}

		private void ThrowIfNotComplete()
		{
			LoadingStatus loadingStatus = this.LoadingStatus;
			bool flag = loadingStatus == LoadingStatus.Failed;
			if (flag)
			{
				throw new Exception("Cannot use a failed ContentFile operation.");
			}
			bool flag2 = loadingStatus == LoadingStatus.InProgress;
			if (flag2)
			{
				throw new Exception("This ContentFile functionality is not supported while loading is in progress");
			}
		}

		public bool IsValid
		{
			get
			{
				return ContentLoadInterface.ContentFile_IsHandleValid(this);
			}
		}

		public LoadingStatus LoadingStatus
		{
			get
			{
				this.ThrowIfInvalidHandle();
				return ContentLoadInterface.ContentFile_GetLoadingStatus(this);
			}
		}

		public bool WaitForCompletion(int timeoutMs)
		{
			this.ThrowIfInvalidHandle();
			return ContentLoadInterface.WaitForLoadCompletion(this, timeoutMs);
		}

		public static ContentFile GlobalTableDependency
		{
			get
			{
				return new ContentFile
				{
					Id = 1UL
				};
			}
		}

		internal ulong Id;
	}
}
