using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class UserMenu : KMonoBehaviour
{
	public void AppendToScreen(UserMenuScreen screen)
	{
		this.sortedButtons.Clear();
		this.buttons.Clear();
		this.sliders.Clear();
		base.Trigger(493375141, null);
		this.buttons.Sort(delegate(KeyValuePair<KIconButtonMenu.ButtonInfo, float> x, KeyValuePair<KIconButtonMenu.ButtonInfo, float> y)
		{
			int num;
			if (x.Value == y.Value)
			{
				num = 0;
			}
			else if (x.Value > y.Value)
			{
				num = 1;
			}
			else
			{
				num = -1;
			}
			return num;
		});
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.sortedButtons[i] = this.buttons[i].Key;
		}
		screen.AddButtons(this.sortedButtons);
		screen.AddSliders(this.sliders);
	}

	public void Refresh()
	{
		Game.Instance.Trigger(1980521255, base.gameObject);
	}

	public void AddButton(KIconButtonMenu.ButtonInfo button, float sort_order = 1f)
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
		this.buttons.Add(new KeyValuePair<KIconButtonMenu.ButtonInfo, float>(button, sort_order));
		this.sortedButtons.Add(null);
	}

	public void AddSlider(UserMenu.SliderInfo slider)
	{
		this.sliders.Add(slider);
	}

	private List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>> buttons = new List<KeyValuePair<KIconButtonMenu.ButtonInfo, float>>();

	private List<KIconButtonMenu.ButtonInfo> sortedButtons = new List<KIconButtonMenu.ButtonInfo>();

	private List<UserMenu.SliderInfo> sliders = new List<UserMenu.SliderInfo>();

	public class SliderInfo
	{
		public MinMaxSlider.LockingType lockType = MinMaxSlider.LockingType.Drag;

		public MinMaxSlider.Mode mode = MinMaxSlider.Mode.Single;

		public Slider.Direction direction = Slider.Direction.LeftToRight;

		public bool interactable = true;

		public bool lockRange = false;

		public string toolTip;

		public string toolTipMin;

		public string toolTipMax;

		public float minLimit = 0f;

		public float maxLimit = 100f;

		public float currentMinValue = 10f;

		public float currentMaxValue = 90f;

		public GameObject sliderGO;

		public Action<MinMaxSlider> onMinChange;

		public Action<MinMaxSlider> onMaxChange;
	}
}
