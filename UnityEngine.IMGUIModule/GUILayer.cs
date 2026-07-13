using System;
using System.ComponentModel;

namespace UnityEngine
{
	[Obsolete("GUILayer has been removed.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromObjectFactory]
	[ExcludeFromPreset]
	public sealed class GUILayer
	{
		[Obsolete("GUILayer has been removed.", true)]
		public GUIElement HitTest(Vector3 screenPosition)
		{
			throw new Exception("GUILayer has been removed from Unity.");
		}
	}
}
