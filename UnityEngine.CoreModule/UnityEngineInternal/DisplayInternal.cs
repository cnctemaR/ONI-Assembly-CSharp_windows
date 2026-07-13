using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace UnityEngineInternal
{
	internal class DisplayInternal
	{
		[FreeFunction("UnityDisplayManager_PrimaryDisplayIndex")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int PrimaryDisplayIndex();

		internal static bool IsASecondaryDisplayIndex(int displayIndex)
		{
			return displayIndex >= 0 && displayIndex < Display.displays.Length && displayIndex != DisplayInternal.PrimaryDisplayIndex();
		}
	}
}
