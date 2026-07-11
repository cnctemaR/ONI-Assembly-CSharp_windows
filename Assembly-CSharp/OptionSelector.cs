using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelector : MonoBehaviour
{
	private void Start()
	{
		this.selectedItem.GetComponent<KButton>().onBtnClick += this.OnClick;
	}

	public void Initialize(object id)
	{
		this.id = id;
	}

	private void OnClick(KKeyCode button)
	{
		if (button == KKeyCode.Mouse0)
		{
			this.OnChangePriority(this.id, 1);
			return;
		}
		if (button != KKeyCode.Mouse1)
		{
			return;
		}
		this.OnChangePriority(this.id, -1);
	}

	public void ConfigureItem(bool disabled, OptionSelector.DisplayOptionInfo display_info)
	{
		HierarchyReferences component = this.selectedItem.GetComponent<HierarchyReferences>();
		KImage kimage = component.GetReference("BG") as KImage;
		if (display_info.bgOptions == null)
		{
			kimage.gameObject.SetActive(false);
		}
		else
		{
			kimage.sprite = display_info.bgOptions[display_info.bgIndex];
		}
		KImage kimage2 = component.GetReference("FG") as KImage;
		if (display_info.fgOptions == null)
		{
			kimage2.gameObject.SetActive(false);
		}
		else
		{
			kimage2.sprite = display_info.fgOptions[display_info.fgIndex];
		}
		KImage kimage3 = component.GetReference("Fill") as KImage;
		if (kimage3 != null)
		{
			kimage3.enabled = !disabled;
			kimage3.color = display_info.fillColour;
		}
		KImage kimage4 = component.GetReference("Outline") as KImage;
		if (kimage4 != null)
		{
			kimage4.enabled = !disabled;
		}
	}

	private object id;

	public Action<object, int> OnChangePriority;

	[SerializeField]
	private KImage selectedItem;

	[SerializeField]
	private KImage itemTemplate;

	public class DisplayOptionInfo
	{
		public IList<Sprite> bgOptions;

		public IList<Sprite> fgOptions;

		public int bgIndex;

		public int fgIndex;

		public Color32 fillColour;
	}
}
