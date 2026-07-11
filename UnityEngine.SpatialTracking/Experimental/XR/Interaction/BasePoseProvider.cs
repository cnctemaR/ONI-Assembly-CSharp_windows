using System;

namespace UnityEngine.Experimental.XR.Interaction
{
	[Serializable]
	public abstract class BasePoseProvider : MonoBehaviour
	{
		public abstract bool TryGetPoseFromProvider(out Pose output);
	}
}
