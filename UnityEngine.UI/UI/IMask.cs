using System;
using System.ComponentModel;

namespace UnityEngine.UI
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Not supported anymore.", true)]
	public interface IMask
	{
		bool Enabled();

		RectTransform rectTransform { get; }
	}
}
