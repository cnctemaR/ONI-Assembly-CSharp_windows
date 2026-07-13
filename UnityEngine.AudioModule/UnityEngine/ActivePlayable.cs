using System;
using UnityEngine.Playables;

namespace UnityEngine
{
	internal struct ActivePlayable
	{
		public readonly PlayableSettings settings { get; }

		public readonly PlayableHandle clipPlayableHandle { get; }
	}
}
