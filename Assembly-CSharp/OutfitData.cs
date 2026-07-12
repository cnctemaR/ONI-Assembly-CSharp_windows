using System;
using System.Collections.Generic;

public class OutfitData
{
	public Dictionary<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> DuplicantOutfits = new Dictionary<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>();

	public Dictionary<string, string[]> CustomOutfits = new Dictionary<string, string[]>();
}
