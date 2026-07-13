using System;
using UnityEngine.Audio;

namespace UnityEngine
{
	internal struct PlayableSettings
	{
		public readonly AudioContainerElement element { get; }

		public readonly double scheduledTime { get; }

		public readonly float pitchOffset { get; }

		public readonly float volumeOffset { get; }

		public readonly double triggerTimeOffset { get; }
	}
}
