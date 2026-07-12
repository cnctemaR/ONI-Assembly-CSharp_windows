using System;
using System.Runtime.CompilerServices;
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

	public static Sprite GetNoneBalloonArtistIcon()
	{
		return Assets.GetSprite("NoTraits");
	}

	[return: TupleElementNames(new string[] { "name", "desc" })]
	public static ValueTuple<string, string> GetNoneClothingItemStrings(PermitCategory category)
	{
		switch (category)
		{
		case PermitCategory.DupeTops:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_TOPS.NAME, EQUIPMENT.PREFABS.CLOTHING_TOPS.DESC);
		case PermitCategory.DupeBottoms:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.NAME, EQUIPMENT.PREFABS.CLOTHING_BOTTOMS.DESC);
		case PermitCategory.DupeGloves:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_GLOVES.NAME, EQUIPMENT.PREFABS.CLOTHING_GLOVES.DESC);
		case PermitCategory.DupeShoes:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_SHOES.NAME, EQUIPMENT.PREFABS.CLOTHING_SHOES.DESC);
		case PermitCategory.DupeHats:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_HATS.NAME, EQUIPMENT.PREFABS.CLOTHING_HATS.DESC);
		case PermitCategory.DupeAccessories:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.CLOTHING_ACCESORIES.NAME, EQUIPMENT.PREFABS.CLOTHING_ACCESORIES.DESC);
		case PermitCategory.AtmoSuitHelmet:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_HELMET.DESC);
		case PermitCategory.AtmoSuitBody:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.ATMO_SUIT_BODY.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BODY.DESC);
		case PermitCategory.AtmoSuitGloves:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_GLOVES.DESC);
		case PermitCategory.AtmoSuitBelt:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.ATMO_SUIT_BELT.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_BELT.DESC);
		case PermitCategory.AtmoSuitShoes:
			return new ValueTuple<string, string>(EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.NAME, EQUIPMENT.PREFABS.ATMO_SUIT_SHOES.DESC);
		case PermitCategory.JoyResponse:
			return new ValueTuple<string, string>(UI.OUTFIT_DESCRIPTION.NO_JOY_RESPONSE_NAME, UI.OUTFIT_DESCRIPTION.NO_JOY_RESPONSE_DESC);
		}
		DebugUtil.DevAssert(false, string.Format("Couldn't find \"no item\" string for category {0}", category), null);
		return new ValueTuple<string, string>("-", "-");
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
		string text = KleiItemsUI.WrapAsToolTipTitle(permit.Name);
		if (!string.IsNullOrWhiteSpace(permit.Description))
		{
			text = text + "\n" + permit.Description;
		}
		string text2 = UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", permit.Rarity.GetLocStringName());
		if (!string.IsNullOrWhiteSpace(text2))
		{
			text = text + "\n\n" + text2;
		}
		if (PermitItems.GetOwnedCount(permit) <= 0)
		{
			text = text + "\n\n" + KleiItemsUI.WrapWithColor(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED);
		}
		return text;
	}

	public static string GetNoneTooltipStringFor(PermitCategory category)
	{
		ValueTuple<string, string> noneClothingItemStrings = KleiItemsUI.GetNoneClothingItemStrings(category);
		string item = noneClothingItemStrings.Item1;
		string item2 = noneClothingItemStrings.Item2;
		return KleiItemsUI.WrapAsToolTipTitle(item) + "\n" + item2;
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
