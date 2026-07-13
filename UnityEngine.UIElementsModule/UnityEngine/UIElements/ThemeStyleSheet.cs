using System;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-tss")]
	[Serializable]
	public class ThemeStyleSheet : StyleSheet
	{
		internal override void OnEnable()
		{
			base.isDefaultStyleSheet = true;
			base.OnEnable();
		}
	}
}
