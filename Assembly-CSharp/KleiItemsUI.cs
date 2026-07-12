using System;
using Database;
using STRINGS;
using UnityEngine;

public static class KleiItemsUI
{
	public static string WrapAsToolTipTitle(string text)
	{
		return "<b><style=\"KLink\">" + text + "</style></b>";
	}

	public static string WrapWithColor(string text, Color color)
	{
		return string.Concat(new string[]
		{
			"<color=#",
			color.ToHexString(),
			">",
			text,
			"</color>"
		});
	}

	public static Sprite GetNoneOutfitIcon()
	{
		return Assets.GetSprite("NoTraits");
	}

	public static Sprite GetNoneClothingItemIcon(PermitCategory category)
	{
		return Assets.GetSprite("NoTraits");
	}

	public static string GetNoneClothingItemString(PermitCategory category)
	{
		switch (category)
		{
		case PermitCategory.DupeTops:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_TOPS;
		case PermitCategory.DupeBottoms:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_BOTTOMS;
		case PermitCategory.DupeGloves:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_GLOVES;
		case PermitCategory.DupeShoes:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_SHOES;
		case PermitCategory.DupeHats:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_HATS;
		case PermitCategory.DupeAccessories:
			return UI.OUTFIT_DESCRIPTION.NO_DUPE_ACCESSORIES;
		default:
			DebugUtil.DevAssert(false, string.Format("Couldn't find \"no item\" string for category {0}", category), null);
			return "-";
		}
	}

	public static void ConfigureTooltipOn(GameObject gameObject, Option<LocString> tooltipText = default(Option<LocString>))
	{
		KleiItemsUI.ConfigureTooltipOn(gameObject, tooltipText.HasValue ? Option.Some<string>(tooltipText.Value) : Option.None);
	}

	public static void ConfigureTooltipOn(GameObject gameObject, Option<string> tooltipText = default(Option<string>))
	{
		ToolTip toolTip = gameObject.GetComponent<ToolTip>();
		if (toolTip.IsNullOrDestroyed())
		{
			toolTip = gameObject.AddComponent<ToolTip>();
			toolTip.tooltipPivot = new Vector2(0.5f, 1f);
			if (gameObject.GetComponent<KButton>())
			{
				toolTip.tooltipPositionOffset = new Vector2(0f, 22f);
			}
			else
			{
				toolTip.tooltipPositionOffset = new Vector2(0f, 0f);
			}
			toolTip.parentPositionAnchor = new Vector2(0.5f, 0f);
			toolTip.toolTipPosition = ToolTip.TooltipPosition.Custom;
		}
		if (!tooltipText.HasValue)
		{
			toolTip.ClearMultiStringTooltip();
			return;
		}
		toolTip.SetSimpleTooltip(tooltipText.Value);
	}

	public static string GetTooltipStringFor(PermitResource permit)
	{
		return KleiItemsUI.GetTooltipStringFor(permit.GetPermitPresentationInfo());
	}

	public static string GetTooltipStringFor(PermitPresentationInfo permitPresInfo)
	{
		string text = KleiItemsUI.WrapAsToolTipTitle(permitPresInfo.name);
		if (!string.IsNullOrWhiteSpace(permitPresInfo.description))
		{
			text = text + "\n" + permitPresInfo.description;
		}
		if (!string.IsNullOrWhiteSpace(permitPresInfo.rarityDetails))
		{
			text = text + "\n\n" + permitPresInfo.rarityDetails;
		}
		if (!permitPresInfo.IsUnlocked())
		{
			text = text + "\n\n" + KleiItemsUI.WrapWithColor(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED);
		}
		return text;
	}

	public static Color GetColor(string input)
	{
		if (input[0] == '#')
		{
			return Util.ColorFromHex(input.Substring(1));
		}
		return Util.ColorFromHex(input);
	}

	public static readonly Color TEXT_COLOR__PERMIT_NOT_OWNED = KleiItemsUI.GetColor("#DD992F");
}
