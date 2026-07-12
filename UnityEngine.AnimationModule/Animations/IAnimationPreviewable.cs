using System;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[UsedByNativeCode]
	internal interface IAnimationPreviewable
	{
		void OnPreviewUpdate();
	}
}
