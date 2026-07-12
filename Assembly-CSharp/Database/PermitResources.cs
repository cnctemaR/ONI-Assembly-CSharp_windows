using System;
using System.Collections.Generic;

namespace Database
{
	public class PermitResources : ResourceSet<PermitResource>
	{
		public PermitResources(ResourceSet parent)
			: base("PermitResources", parent)
		{
			this.Root = new ResourceSet<Resource>("Root", null);
			this.Permits = new Dictionary<string, IEnumerable<PermitResource>>();
			this.BuildingFacades = new BuildingFacades(this.Root);
			this.Permits.Add(this.BuildingFacades.Id, this.BuildingFacades.resources);
			this.EquippableFacades = new EquippableFacades(this.Root);
			this.Permits.Add(this.EquippableFacades.Id, this.EquippableFacades.resources);
			this.ArtableStages = new ArtableStages(this.Root);
			this.Permits.Add(this.ArtableStages.Id, this.ArtableStages.resources);
			this.StickerBombs = new StickerBombs(this.Root);
			this.Permits.Add(this.StickerBombs.Id, this.StickerBombs.resources);
			this.ClothingItems = new ClothingItems(this.Root);
			this.ClothingOutfits = new ClothingOutfits(this.Root, this.ClothingItems);
			this.Permits.Add(this.ClothingItems.Id, this.ClothingItems.resources);
			this.MonumentParts = new MonumentParts(this.Root);
			foreach (IEnumerable<PermitResource> enumerable in this.Permits.Values)
			{
				this.resources.AddRange(enumerable);
			}
		}

		public void PostProcess()
		{
			this.BuildingFacades.PostProcess();
		}

		public static bool ShouldDisplayPermitInSupplyCloset(string permitId)
		{
			return !PermitResources.PermitIdsToExcludeFromSupplyCloset.Contains(permitId);
		}

		public ResourceSet Root;

		public BuildingFacades BuildingFacades;

		public EquippableFacades EquippableFacades;

		public ArtableStages ArtableStages;

		public StickerBombs StickerBombs;

		public ClothingItems ClothingItems;

		public ClothingOutfits ClothingOutfits;

		public MonumentParts MonumentParts;

		public Dictionary<string, IEnumerable<PermitResource>> Permits;

		public static readonly HashSet<string> PermitIdsToExcludeFromSupplyCloset = new HashSet<string>
		{
			"Canvas_Bad", "Canvas_Average", "Canvas_Good", "Canvas_Good2", "Canvas_Good3", "Canvas_Good4", "Canvas_Good5", "Canvas_Good6", "CanvasTall_Bad", "CanvasTall_Average",
			"CanvasTall_Good", "CanvasTall_Good2", "CanvasTall_Good3", "CanvasTall_Good4", "CanvasWide_Bad", "CanvasWide_Average", "CanvasWide_Good", "CanvasWide_Good2", "CanvasWide_Good3", "CanvasWide_Good4",
			"Sculpture_Bad", "Sculpture_Average", "Sculpture_Good1", "Sculpture_Good2", "Sculpture_Good3", "SmallSculpture_Bad", "SmallSculpture_Average", "SmallSculpture_Good", "SmallSculpture_Good2", "SmallSculpture_Good3",
			"IceSculpture_Bad", "IceSculpture_Average", "MarbleSculpture_Bad", "MarbleSculpture_Average", "MarbleSculpture_Good1", "MarbleSculpture_Good2", "MarbleSculpture_Good3", "MetalSculpture_Bad", "MetalSculpture_Average", "MetalSculpture_Good1",
			"MetalSculpture_Good2", "MetalSculpture_Good3", "a", "b", "c", "d", "e", "f", "g", "h",
			"rocket", "paperplane", "plant", "plantpot", "mushroom", "mermaid", "spacepet", "spacepet2", "spacepet3", "spacepet4",
			"spacepet5", "unicorn"
		};
	}
}
