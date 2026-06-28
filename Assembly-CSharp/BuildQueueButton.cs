using System;
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

	private void Update()
	{
		if (this.order != null)
		{
			string empty = string.Empty;
			bool flag = this.CheckMaterialAvailability(this.order, out empty);
			if (this.materialsAvailable != flag)
			{
				this.SetAvailability(this.order.recipe.Result.GetProperName(), flag, empty);
			}
		}
	}

	private void SetAvailability(string recipeName, bool currentAvailability, string str)
	{
		str = recipeName + "\n" + str;
		this.unavailableImg.SetActive(!currentAvailability);
		this.texture.color = ((!currentAvailability) ? this.unavailableSpriteColor : this.order.recipe.IconColor);
		this.BG.color = ((!currentAvailability) ? this.unavailableBGColor : Color.white);
		this.materialsAvailable = currentAvailability;
		if (!currentAvailability)
		{
			str += UI.UISIDESCREENS.FABRICATORSIDESCREEN.CANCEL;
		}
		else
		{
			str = UI.UISIDESCREENS.FABRICATORSIDESCREEN.CANCEL;
		}
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
		this.BG.color = Color.white;
		this.texture.color = Color.white;
		this.closeImg.SetActive(false);
		this.infiniteImg.SetActive(false);
		this.unavailableImg.SetActive(false);
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

	private bool CheckMaterialAvailability(Fabricator.UserOrder order, out string newTooltip)
	{
		newTooltip = string.Empty;
		Recipe recipe = order.recipe;
		Element[] elements = recipe.GetElements();
		bool flag = true;
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			float amount = recipe.Ingredients[i].amount;
			if (i < elements.Length)
			{
				Element element = elements[i];
				if (element != null)
				{
					Tag tag = TagManager.Create(element.id);
					float num = WorldInventory.Instance.GetAmount(tag);
					if (num < amount && element.IsLiquid && LiquidSourceDetector2.Instance.IsLiquidAccessible(element))
					{
						num = amount + 1f;
					}
					if (amount > num)
					{
						string text = tag.UnitOfMeasurement();
						string text2 = newTooltip;
						newTooltip = string.Concat(new string[]
						{
							text2,
							"Missing ",
							Mathf.Abs(amount - num).ToString(),
							text,
							" of ",
							element.name,
							"\n"
						});
					}
					flag = flag && num >= amount;
				}
			}
		}
		return flag;
	}

	public void SetOrder(Fabricator.UserOrder order)
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
				KBatchedAnimController component = order.recipe.Result.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					this.texture.enabled = true;
					this.texture.preserveAspect = true;
					this.texture.sprite = ((!(order.recipe.Icon == null)) ? order.recipe.Icon : Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], component.initialAnim));
					if (order.recipe.Icon != null)
					{
						this.texture.color = order.recipe.IconColor;
					}
					else
					{
						this.texture.color = Color.white;
					}
					this.BG.sprite = this.filledBG;
				}
				else
				{
					this.texture.enabled = order.recipe.Icon != null;
					this.texture.sprite = order.recipe.Icon;
					this.texture.color = order.recipe.IconColor;
				}
				this.order = order;
				if (this.toolTip != null)
				{
					string text = "Cancel ";
					if (order.infinite)
					{
						text += "repeating ";
					}
					text += order.recipe.Name;
					this.toolTip.toolTip = text;
				}
				this.infiniteImg.SetActive(order.infinite);
				string empty = string.Empty;
				bool flag = this.CheckMaterialAvailability(order, out empty);
				this.SetAvailability(order.recipe.Result.GetProperName(), flag, empty);
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
	private Color unavailableSpriteColor = Color.gray;

	[SerializeField]
	private Color unavailableBGColor = Color.gray;

	public ToolTip toolTip;

	private Fabricator.UserOrder order;

	private GameObject visualizer;

	private Image BG;

	private bool materialsAvailable = true;
}
