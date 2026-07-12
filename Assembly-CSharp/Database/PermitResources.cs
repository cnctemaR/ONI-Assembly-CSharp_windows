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
			this.BalloonArtistFacades = new BalloonArtistFacades(this.Root);
			this.Permits.Add(this.BalloonArtistFacades.Id, this.BalloonArtistFacades.resources);
			this.MonumentParts = new MonumentParts(this.Root);
			this.Permits.Add(this.MonumentParts.Id, this.MonumentParts.resources);
			foreach (IEnumerable<PermitResource> enumerable in this.Permits.Values)
			{
				this.resources.AddRange(enumerable);
			}
		}

		public void PostProcess()
		{
			this.BuildingFacades.PostProcess();
		}

		public ResourceSet Root;

		public BuildingFacades BuildingFacades;

		public EquippableFacades EquippableFacades;

		public ArtableStages ArtableStages;

		public StickerBombs StickerBombs;

		public ClothingItems ClothingItems;

		public ClothingOutfits ClothingOutfits;

		public MonumentParts MonumentParts;

		public BalloonArtistFacades BalloonArtistFacades;

		public Dictionary<string, IEnumerable<PermitResource>> Permits;
	}
}
