using System;

namespace Database
{
	public class BalloonArtistFacades : ResourceSet<BalloonArtistFacadeResource>
	{
		public BalloonArtistFacades(ResourceSet parent)
			: base("BalloonArtistFacades", parent)
		{
			foreach (BalloonArtistFacadeInfo balloonArtistFacadeInfo in Blueprints.Get().all.balloonArtistFacades)
			{
				this.Add(balloonArtistFacadeInfo.id, balloonArtistFacadeInfo.name, balloonArtistFacadeInfo.desc, balloonArtistFacadeInfo.rarity, balloonArtistFacadeInfo.animFile, balloonArtistFacadeInfo.balloonFacadeType, balloonArtistFacadeInfo.GetRequiredDlcIds(), balloonArtistFacadeInfo.GetForbiddenDlcIds());
			}
		}

		[Obsolete("Please use Add(...) with required/forbidden")]
		public void Add(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType)
		{
			this.Add(id, name, desc, rarity, animFile, balloonFacadeType, null, null);
		}

		[Obsolete("Please use Add(...) with required/forbidden")]
		public void Add(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType, string[] dlcIds)
		{
			this.Add(id, name, desc, rarity, animFile, balloonFacadeType, null, null);
		}

		public void Add(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType, string[] requiredDlcIds, string[] forbiddenDlcIds)
		{
			BalloonArtistFacadeResource balloonArtistFacadeResource = new BalloonArtistFacadeResource(id, name, desc, rarity, animFile, balloonFacadeType, requiredDlcIds, forbiddenDlcIds);
			this.resources.Add(balloonArtistFacadeResource);
		}
	}
}
