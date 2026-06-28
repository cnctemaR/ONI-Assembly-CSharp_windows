using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu : KMonoBehaviour
{
	public void AppendToScreen(UserMenuScreen screen)
	{
		this.buttons.Clear();
		this.sliders.Clear();
		this.Trigger(493375141, null);
		screen.AddButtons(this.buttons);
		screen.AddSliders(this.sliders);
	}

	public void Refresh()
	{
		Game.Instance.Trigger(1980521255, base.gameObject);
	}

	public void AddButton(KIconButtonMenu.ButtonInfo button)
	{
		if (button.onClick != null)
		{
			global::System.Action callback = button.onClick;
			button.onClick = delegate
			{
				callback();
				this.Refresh();
			};
		}
		this.buttons.Add(button);
	}

	public void AddSlider(UserMenu.SliderInfo slider)
	{
		this.sliders.Add(slider);
	}

	private List<KIconButtonMenu.ButtonInfo> buttons = new List<KIconButtonMenu.ButtonInfo>();

	private List<UserMenu.SliderInfo> sliders = new List<UserMenu.SliderInfo>();

	public class SliderInfo
	{
		public MinMaxSlider.LockingType lockType = MinMaxSlider.LockingType.Drag;

		public MinMaxSlider.Mode mode;

		public Slider.Direction direction;

		public bool interactable = true;

		public bool lockRange;

		public string toolTip;

		public string toolTipMin;

		public string toolTipMax;

		public float minLimit;

		public float maxLimit = 100f;

		public float currentMinValue = 10f;

		public float currentMaxValue = 90f;

		public GameObject sliderGO;

		public Action<MinMaxSlider> onMinChange;

		public Action<MinMaxSlider> onMaxChange;
	}
}
