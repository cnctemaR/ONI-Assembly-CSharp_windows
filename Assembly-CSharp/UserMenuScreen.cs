using System;
using System.Collections.Generic;
using UnityEngine;

public class UserMenuScreen : KIconButtonMenu
{
	protected override void OnPrefabInit()
	{
		this.keepMenuOpen = true;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(1980521255, new Action<object>(this.OnUIRefresh));
	}

	public void SetSelected(GameObject go)
	{
		this.priorityScreen.SetTarget(go);
		this.selected = go;
	}

	public void Refresh(GameObject go)
	{
		if (!(go != this.selected))
		{
			this.priorityScreen.SetTarget(go);
			this.buttonInfos.Clear();
			this.slidersInfos.Clear();
			UserMenu[] components = go.GetComponents<UserMenu>();
			if (components != null)
			{
				foreach (UserMenu userMenu in components)
				{
					userMenu.AppendToScreen(this);
				}
			}
			base.SetButtons(this.buttonInfos);
			base.RefreshButtons();
			this.RefreshSliders();
			if ((this.sliders == null || this.sliders.Count == 0) && (this.buttonInfos == null || this.buttonInfos.Count == 0))
			{
				base.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				base.transform.parent.gameObject.SetActive(true);
			}
		}
	}

	public void AddSliders(IList<UserMenu.SliderInfo> sliders)
	{
		this.slidersInfos.AddRange(sliders);
	}

	public void AddButtons(IList<KIconButtonMenu.ButtonInfo> buttons)
	{
		this.buttonInfos.AddRange(buttons);
	}

	private void OnUIRefresh(object data)
	{
		this.Refresh(data as GameObject);
	}

	public void RefreshSliders()
	{
		if (this.sliders != null)
		{
			for (int i = 0; i < this.sliders.Count; i++)
			{
				global::UnityEngine.Object.Destroy(this.sliders[i].gameObject);
			}
			this.sliders = null;
		}
		if (this.slidersInfos != null && this.slidersInfos.Count != 0)
		{
			this.sliders = new List<MinMaxSlider>();
			for (int j = 0; j < this.slidersInfos.Count; j++)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.sliderPrefab.gameObject, Vector3.zero, Quaternion.identity);
				this.slidersInfos[j].sliderGO = gameObject;
				MinMaxSlider component = gameObject.GetComponent<MinMaxSlider>();
				this.sliders.Add(component);
				Transform transform = ((!(this.sliderParent != null)) ? base.transform : this.sliderParent.transform);
				gameObject.transform.SetParent(transform, false);
				gameObject.SetActive(true);
				gameObject.name = "Slider";
				if (component.toolTip)
				{
					component.toolTip.toolTip = this.slidersInfos[j].toolTip;
				}
				component.lockType = this.slidersInfos[j].lockType;
				component.interactable = this.slidersInfos[j].interactable;
				component.minLimit = this.slidersInfos[j].minLimit;
				component.maxLimit = this.slidersInfos[j].maxLimit;
				component.currentMinValue = this.slidersInfos[j].currentMinValue;
				component.currentMaxValue = this.slidersInfos[j].currentMaxValue;
				component.onMinChange = this.slidersInfos[j].onMinChange;
				component.onMaxChange = this.slidersInfos[j].onMaxChange;
				component.direction = this.slidersInfos[j].direction;
				component.SetMode(this.slidersInfos[j].mode);
				component.SetMinMaxValue(this.slidersInfos[j].currentMinValue, this.slidersInfos[j].currentMaxValue, this.slidersInfos[j].minLimit, this.slidersInfos[j].maxLimit);
			}
		}
	}

	private GameObject selected = null;

	public MinMaxSlider sliderPrefab;

	public GameObject sliderParent;

	private List<MinMaxSlider> sliders = new List<MinMaxSlider>();

	private List<UserMenu.SliderInfo> slidersInfos = new List<UserMenu.SliderInfo>();

	private List<KIconButtonMenu.ButtonInfo> buttonInfos = new List<KIconButtonMenu.ButtonInfo>();

	[SerializeField]
	private InfoPriorityScreen priorityScreen;
}
