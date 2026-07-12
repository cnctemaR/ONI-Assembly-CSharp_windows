using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Klei;
using STRINGS;

namespace Database
{
	public class ClothingOutfits : ResourceSet<ClothingOutfitResource>
	{
		public ClothingOutfits(ResourceSet parent, ClothingItems items_resource)
			: base("ClothingOutfits", parent)
		{
			base.Initialize();
			this.Add("BasicBlack", new string[] { "TopBasicBlack", "BottomBasicBlack", "GlovesBasicBlack", "ShoesBasicBlack" }, UI.OUTFITS.BASIC_BLACK.NAME);
			this.Add("BasicWhite", new string[] { "TopBasicWhite", "BottomBasicWhite", "GlovesBasicWhite", "ShoesBasicWhite" }, UI.OUTFITS.BASIC_WHITE.NAME);
			this.Add("BasicRed", new string[] { "TopBasicRed", "BottomBasicRed", "GlovesBasicRed", "ShoesBasicRed" }, UI.OUTFITS.BASIC_RED.NAME);
			this.Add("BasicOrange", new string[] { "TopBasicOrange", "BottomBasicOrange", "GlovesBasicOrange", "ShoesBasicOrange" }, UI.OUTFITS.BASIC_ORANGE.NAME);
			this.Add("BasicYellow", new string[] { "TopBasicYellow", "BottomBasicYellow", "GlovesBasicYellow", "ShoesBasicYellow" }, UI.OUTFITS.BASIC_YELLOW.NAME);
			this.Add("BasicGreen", new string[] { "TopBasicGreen", "BottomBasicGreen", "GlovesBasicGreen", "ShoesBasicGreen" }, UI.OUTFITS.BASIC_GREEN.NAME);
			this.Add("BasicAqua", new string[] { "TopBasicAqua", "BottomBasicAqua", "GlovesBasicAqua", "ShoesBasicAqua" }, UI.OUTFITS.BASIC_AQUA.NAME);
			this.Add("BasicPurple", new string[] { "TopBasicPurple", "BottomBasicPurple", "GlovesBasicPurple", "ShoesBasicPurple" }, UI.OUTFITS.BASIC_PURPLE.NAME);
			this.Add("BasicPinkOrchid", new string[] { "TopBasicPinkOrchid", "BottomBasicPinkOrchid", "GlovesBasicPinkOrchid", "ShoesBasicPinkOrchid" }, UI.OUTFITS.BASIC_PINK_ORCHID.NAME);
			this.Load(items_resource);
			ClothingOutfitUtility.LoadClothingOutfitData(this);
		}

		public void Add(string id, string[] items_in_outfit, LocString name)
		{
			ClothingOutfitResource clothingOutfitResource = new ClothingOutfitResource(id, items_in_outfit, name);
			this.resources.Add(clothingOutfitResource);
		}

		public void Load(ClothingItems items_resource)
		{
			ListPool<YamlIO.Error, ClothingItemResource>.PooledList errors = ListPool<YamlIO.Error, ClothingItemResource>.Allocate();
			List<FileHandle> list = new List<FileHandle>();
			DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(Path.Combine(new string[] { Db.GetPath("", "clothing") })));
			if (directoryInfo.Exists)
			{
				FileSystem.GetFiles(directoryInfo.FullName, "*.yaml", list);
				YamlIO.ErrorHandler <>9__0;
				foreach (FileHandle fileHandle in list)
				{
					try
					{
						FileHandle fileHandle2 = fileHandle;
						YamlIO.ErrorHandler errorHandler;
						if ((errorHandler = <>9__0) == null)
						{
							errorHandler = (<>9__0 = delegate(YamlIO.Error error, bool force_log_as_warning)
							{
								errors.Add(error);
							});
						}
						ClothingOutfits.ClothingOutfitInfo clothingOutfitInfo = YamlIO.LoadFile<ClothingOutfits.ClothingOutfitInfo>(fileHandle2, errorHandler, null);
						if (errors.Count == 0)
						{
							string[] array = new string[clothingOutfitInfo.items.Count];
							for (int i = 0; i < clothingOutfitInfo.items.Count; i++)
							{
								ClothingOutfits.ClothingOutfitInfo.ClothingItem clothingItem = clothingOutfitInfo.items[i];
								if (Assets.GetAnim(clothingItem.animFilename) == null)
								{
									Debug.LogError(string.Concat(new string[] { "missing anim file ", clothingItem.animFilename, " for ClothingItem ", clothingItem.id, " in ", fileHandle.full_path }));
								}
								else if (clothingItem.id == null)
								{
									Debug.LogError("missing clothing item id in " + fileHandle.full_path);
								}
								else
								{
									if (items_resource.TryGet(clothingItem.id) == null)
									{
										items_resource.Add(clothingItem.id, clothingItem.name, clothingItem.description, PermitCategories.GetCategoryForId(clothingItem.category), PermitRarity.Unknown, clothingItem.animFilename);
									}
									array[i] = clothingItem.id;
								}
							}
							if (clothingOutfitInfo.id != null)
							{
								this.Add(clothingOutfitInfo.id, array, clothingOutfitInfo.id + " (yaml)");
							}
						}
						else
						{
							Debug.LogError("Failed to load clothing outfit " + fileHandle.full_path + " \n " + errors[0].message);
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("Failed to load clothing outfit {0} error {1}", fileHandle.full_path, ex));
					}
				}
			}
			this.resources = this.resources.Distinct<ClothingOutfitResource>().ToList<ClothingOutfitResource>();
			errors.Recycle();
		}

		public void SetDuplicantPersonalityOutfit(string personalityId, Option<string> outfit_id, ClothingOutfitUtility.OutfitType outfit_type = ClothingOutfitUtility.OutfitType.Clothing)
		{
			Db.Get().Personalities.Get(personalityId).SetOutfit(outfit_type, outfit_id);
			CustomClothingOutfits.Instance.SetDuplicantPerosonalityOutfit(personalityId, outfit_id, outfit_type);
		}

		public class ClothingOutfitInfo
		{
			public string id { get; set; }

			public string name { get; set; }

			public List<ClothingOutfits.ClothingOutfitInfo.ClothingItem> items { get; set; }

			public class ClothingItem
			{
				public string id { get; set; }

				public string name { get; set; }

				public string description { get; set; }

				public string category { get; set; }

				public string animFilename { get; set; }
			}
		}
	}
}
