using System;

namespace Unity.Loading
{
	public enum SceneLoadingStatus
	{
		InProgress,
		WaitingForIntegrate,
		WillIntegrateNextFrame,
		Complete,
		Failed
	}
}
