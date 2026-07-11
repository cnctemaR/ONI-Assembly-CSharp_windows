using System;

namespace UnityEngine.Networking
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Network/NetworkStartPosition")]
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class NetworkStartPosition : MonoBehaviour
	{
		public void Awake()
		{
			NetworkManager.RegisterStartPosition(base.transform);
		}

		public void OnDestroy()
		{
			NetworkManager.UnRegisterStartPosition(base.transform);
		}
	}
}
