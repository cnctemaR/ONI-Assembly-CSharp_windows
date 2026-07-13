using System;

namespace UnityEngine.Audio
{
	[Obsolete("DSPConfiguration has been deprecated. Use AudioFormat instead. (UnityUpgradable) -> AudioFormat", true)]
	public struct DSPConfiguration
	{
		[Obsolete("AudioFormat.bufferSize has been deprecated. Use AudioFormat.bufferFrameCount instead. (UnityUpgradable) -> AudioFormat.bufferFrameCount", true)]
		public readonly int bufferSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
