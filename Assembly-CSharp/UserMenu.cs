using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu
{
	public void Refresh(GameObject go)
	{
		Game.Instance.Trigger(1980521255, go);
	}

	public void AddButton(GameObject go, KIconButtonMenu.ButtonInfo button, float sort_order = 1f)
	{
		if (button.onClick != null)
		{
			global::System.Action callback = button.onClick;
			button.onClick = delegate
			{
				callback();
				Game.Instance.Trigger(1980521255, go);
			};
		}
		this.buttons.Add(new KeyValuePair<KIconButtonMenu.ButtonInfo, float>(button, sort_order));
	}

	public void AddSlider(GameObject go, UserMenu.SliderInfo slider)
	{
		this.sliders.Add(slider);
	}

	public void AppendToScreen(GameObject go, UserMenuScreen screen)
	{
		this.buttons.Clear();
		this.sliders.Clear();
		go.Trigger(493375141, null);
		if (this.buttons.Count > 0)
		{
			this.buttons.Sort(delegate(KeyValuePair<KIconButtonMenu.ButtonInfo, float> x, KeyValuePair<KIconButtonMenu.ButtonInfo, float> y)
			{
				if (x.Value == y.Value)
				{
					return 0;
				}
				if (x.Value > y.Value)
				{
					return 1;
				}
				return -1;
			});
			for (int i = 0; i < this.buttons.Count; i++)
			{
				this.sortedButtons.Add(this.buttons[i].Key);
			}
			screen.AddButtons(this.sortedButtons);
			this.sortedButtons.Clear();
		}
		if (this.sliders.Count > 0)
		{
			screen.AddSliders(this.sliders);
		}
	}

	private List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>> buttons = new List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>>();

	private List<UserMenu.SliderInfo> sliders = new List<UserMenu.SliderInfo>();

	private List<KIconButtonMenu.ButtonInfo> sortedButtons = new List<KIconButtonMenu.ButtonInfo>();

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
