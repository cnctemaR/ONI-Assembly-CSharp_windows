using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DetailLabelWithButton")]
public class DetailLabelWithButton : KMonoBehaviour
{
	public void RefreshLabelsVisibility()
	{
		if (this.label.gameObject.activeInHierarchy != !string.IsNullOrEmpty(this.label.text))
		{
			this.label.gameObject.SetActive(!string.IsNullOrEmpty(this.label.text));
		}
		if (this.label2.gameObject.activeInHierarchy != !string.IsNullOrEmpty(this.label2.text))
		{
			this.label2.gameObject.SetActive(!string.IsNullOrEmpty(this.label2.text));
		}
		if (this.label3.gameObject.activeInHierarchy != !string.IsNullOrEmpty(this.label3.text))
		{
			this.label3.gameObject.SetActive(!string.IsNullOrEmpty(this.label3.text));
		}
	}

	public LocText label;

	public LocText label2;

	public LocText label3;

	public ToolTip toolTip;

	public KButton button;
}
