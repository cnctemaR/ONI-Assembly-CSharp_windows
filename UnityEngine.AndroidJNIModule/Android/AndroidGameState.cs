using System;
using UnityEngine.Bindings;

namespace UnityEngine.Android
{
	[NativeType(Header = "Modules/AndroidJNI/Public/GameStateHelper.h")]
	public enum AndroidGameState
	{
		Unknown,
		None,
		GamePlayInterruptible,
		GamePlayUninterruptible,
		Content
	}
}
