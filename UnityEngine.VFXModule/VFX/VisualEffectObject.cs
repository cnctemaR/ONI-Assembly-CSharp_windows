using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[NativeHeader("VFXScriptingClasses.h")]
	[NativeHeader("Modules/VFX/Public/VisualEffectAsset.h")]
	[NativeHeader("Modules/VFX/Public/ScriptBindings/VisualEffectAssetBindings.h")]
	[UsedByNativeCode]
	public abstract class VisualEffectObject : Object
	{
	}
}
