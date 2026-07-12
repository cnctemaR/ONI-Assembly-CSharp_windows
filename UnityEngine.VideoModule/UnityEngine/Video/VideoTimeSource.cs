using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	[Obsolete("VideoTimeSource is deprecated. Use TimeUpdateMode instead. (UnityUpgradable) -> VideoTimeUpdateMode")]
	[RequiredByNativeCode]
	public enum VideoTimeSource
	{
		[Obsolete("AudioDSPTimeSource is deprecated. Use DSPTime instead. (UnityUpgradable) -> DSPTime")]
		AudioDSPTimeSource,
		[Obsolete("GameTimeSource is deprecated. Use GameTime instead. (UnityUpgradable) -> GameTime")]
		GameTimeSource
	}
}
