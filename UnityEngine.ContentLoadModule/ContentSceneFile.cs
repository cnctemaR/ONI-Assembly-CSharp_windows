using System;
using UnityEngine.SceneManagement;

namespace Unity.Loading
{
	public struct ContentSceneFile
	{
		public Scene Scene
		{
			get
			{
				this.ThrowIfInvalidHandle();
				return ContentLoadInterface.ContentSceneFile_GetScene(this);
			}
		}

		public void IntegrateAtEndOfFrame()
		{
			this.ThrowIfInvalidHandle();
			ContentLoadInterface.ContentSceneFile_IntegrateAtEndOfFrame(this);
		}

		public SceneLoadingStatus Status
		{
			get
			{
				this.ThrowIfInvalidHandle();
				return ContentLoadInterface.ContentSceneFile_GetStatus(this);
			}
		}

		public bool UnloadAtEndOfFrame()
		{
			this.ThrowIfInvalidHandle();
			return ContentLoadInterface.ContentSceneFile_UnloadAtEndOfFrame(this);
		}

		public bool WaitForLoadCompletion(int timeoutMs)
		{
			this.ThrowIfInvalidHandle();
			return ContentLoadInterface.ContentSceneFile_WaitForCompletion(this, timeoutMs);
		}

		public bool IsValid
		{
			get
			{
				return ContentLoadInterface.ContentSceneFile_IsHandleValid(this);
			}
		}

		private void ThrowIfInvalidHandle()
		{
			bool flag = !this.IsValid;
			if (flag)
			{
				throw new Exception("The ContentSceneFile operation cannot be performed because the handle is invalid. Did you already unload it?");
			}
		}

		internal ulong Id;
	}
}
