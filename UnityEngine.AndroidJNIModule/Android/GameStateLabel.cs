using System;
using UnityEngine.Bindings;

namespace UnityEngine.Android
{
	[NativeType(Header = "Modules/AndroidJNI/Public/GameStateHelper.h")]
	internal enum GameStateLabel
	{
		Default = -1,
		InitialLoading = -2,
		AssetPacksLoading = -3,
		WebRequest = -4
	}
}
