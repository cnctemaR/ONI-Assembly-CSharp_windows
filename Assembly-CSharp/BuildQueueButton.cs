using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueButton : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		int num = (int)this.texture.rectTransform.rect.width;
		int num2 = (int)this.texture.rectTransform.rect.height;
		if (num == 0 || num2 == 0)
		{
			LayoutElement component = base.GetComponent<LayoutElement>();
			num = (int)component.minWidth;
			num2 = (int)component.minHeight;
		}
		this.texture.enabled = false;
		this.BG = base.GetComponent<Image>();
		KButton component2 = base.GetComponent<KButton>();
		component2.onClick += this.ButtonClicked;
		KButton kbutton = component2;
		kbutton.onPointerEnter = (global::System.Action)Delegate.Combine(kbutton.onPointerEnter, new global::System.Action(this.PointerEntered));
		KButton kbutton2 = component2;
		kbutton2.onPointerExit = (global::System.Action)Delegate.Combine(kbutton2.onPointerExit, new global::System.Action(this.PointerLeft));
	}

	public void SetAvailability(string recipeName, bool currentAvailability, string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			str = recipeName + "\n";
		}
		else
		{
			str = recipeName + "\n" + str;
		}
		this.texture.color = ((!currentAvailability) ? this.unavailableSpriteColor : this.order.IconColor);
		this.texture.GetComponent<Image>().material = ((!currentAvailability) ? GlobalResources.Instance().AnimMaterialUIDesaturated : null);
		this.BG.color = ((!currentAvailability) ? this.unavailableBGColor : Color.white);
		str = str + "\n" + UI.UISIDESCREENS.FABRICATORSIDESCREEN.CANCEL;
		this.toolTip.toolTip = str;
	}

	private void ButtonClicked()
	{
		if (this.BG.sprite == this.filledBG)
		{
			this.ResetGraphics();
		}
	}

	private void ResetGraphics()
	{
		this.BG.sprite = this.emptyBG;
		this.BG.color = this.defaultBGColor;
		this.texture.color = Color.white;
		this.closeImg.SetActive(false);
		this.infiniteImg.SetActive(false);
	}

	private void PointerEntered()
	{
		if (this.BG.sprite == this.filledBG)
		{
			this.closeImg.SetActive(true);
		}
	}

	private void PointerLeft()
	{
		if (this.BG.sprite == this.filledBG && this.closeImg.activeInHierarchy)
		{
			this.closeImg.SetActive(false);
		}
	}

	private bool CheckMaterialAvailability(IBuildQueueOrder order, out string newTooltip)
	{
		newTooltip = string.Empty;
		Dictionary<Tag, float> dictionary = order.CheckMaterialRequirements();
		bool flag = true;
		foreach (KeyValuePair<Tag, float> keyValuePair in dictionary)
		{
			bool flag2 = keyValuePair.Value <= 0f;
			if (!flag2 && GameTags.LiquidElements.Contains(keyValuePair.Key))
			{
				Element element = ElementLoader.GetElement(keyValuePair.Key);
				if (element != null && LiquidPumpingStation.IsLiquidAccessible(element))
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				string text;
				if (GameTags.DisplayAsCalories.Contains(keyValuePair.Key))
				{
					EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(keyValuePair.Key.Name);
					float num = foodInfo.CaloriesPerUnit * keyValuePair.Value;
					text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.CALS, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true));
				}
				else if (GameTags.DisplayAsUnits.Contains(keyValuePair.Key))
				{
					text = GameUtil.GetFormattedUnits(keyValuePair.Value, GameUtil.TimeSlice.None, true);
				}
				else
				{
					text = GameUtil.GetFormattedMass(keyValuePair.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				}
				newTooltip += string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.QUEUED_MISSING_INGREDIENTS_TOOLTIP, text, keyValuePair.Key.ProperName());
			}
			flag = flag && flag2;
		}
		return flag;
	}

	public void SetOrder(IBuildQueueOrder order)
	{
		if (this.order != order)
		{
			this.ResetGraphics();
			if (this.visualizer != null)
			{
				global::UnityEngine.Object.Destroy(this.visualizer);
				this.visualizer = null;
			}
			this.texture.enabled = false;
			if (order != null)
			{
				this.texture.enabled = true;
				this.texture.sprite = order.Icon;
				this.texture.color = order.IconColor;
				this.BG.sprite = this.filledBG;
				this.order = order;
				this.infiniteImg.SetActive(order.Infinite);
			}
			else
			{
				this.BG.sprite = this.emptyBG;
			}
		}
	}

	[SerializeField]
	private Image texture;

	[SerializeField]
	private GameObject closeImg;

	[SerializeField]
	private GameObject infiniteImg;

	[SerializeField]
	private GameObject unavailableImg;

	[SerializeField]
	private Sprite emptyBG;

	[SerializeField]
	private Sprite filledBG;

	[SerializeField]
	private Color unavailableSpriteColor = new Color32(120, 120, 120, byte.MaxValue);

	[SerializeField]
	private Color unavailableBGColor = new Color32(120, 120, 120, byte.MaxValue);

	[SerializeField]
	private Color defaultBGColor = new Color32(135, 69, 102, byte.MaxValue);

	public ToolTip toolTip;

	private IBuildQueueOrder order;

	private GameObject visualizer;

	private Image BG;
}
