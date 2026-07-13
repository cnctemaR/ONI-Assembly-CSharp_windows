using System;
using System.ComponentModel;

namespace UnityEngine
{
	[Obsolete("GUILayer has been removed.", true)]
	[ExcludeFromObjectFactory]
	[ExcludeFromPreset]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class GUILayer
	{
		[Obsolete("GUILayer has been removed.", true)]
		public GUIElement HitTest(Vector3 screenPosition)
		{
			throw new Exception("GUILayer has been removed from Unity.");
		}
	}
}
