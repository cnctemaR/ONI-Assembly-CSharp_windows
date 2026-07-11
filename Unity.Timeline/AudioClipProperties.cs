using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[NotKeyable]
	[Serializable]
	internal class AudioClipProperties : PlayableBehaviour
	{
		[Range(0f, 1f)]
		public float volume = 1f;
	}
}
