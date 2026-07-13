using System;
using System.ComponentModel;
using UnityEngine.Internal;

namespace UnityEngine
{
	[Obsolete("GUIElement has been removed. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromObjectFactory]
	[ExcludeFromPreset]
	public sealed class GUIElement
	{
		private static void FeatureRemoved()
		{
			throw new Exception("GUIElement has been removed from Unity. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.");
		}

		[Obsolete("GUIElement has been removed. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.", true)]
		public bool HitTest(Vector3 screenPosition)
		{
			GUIElement.FeatureRemoved();
			return false;
		}

		[Obsolete("GUIElement has been removed. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.", true)]
		public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera)
		{
			GUIElement.FeatureRemoved();
			return false;
		}

		[Obsolete("GUIElement has been removed. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.", true)]
		public Rect GetScreenRect([DefaultValue("null")] Camera camera)
		{
			GUIElement.FeatureRemoved();
			return new Rect(0f, 0f, 0f, 0f);
		}

		[Obsolete("GUIElement has been removed. Consider using https://docs.unity3d.com/ScriptReference/UIElements.Image.html, https://docs.unity3d.com/ScriptReference/UIElements.TextElement.html or TextMeshPro instead.", true)]
		public Rect GetScreenRect()
		{
			GUIElement.FeatureRemoved();
			return new Rect(0f, 0f, 0f, 0f);
		}
	}
}
