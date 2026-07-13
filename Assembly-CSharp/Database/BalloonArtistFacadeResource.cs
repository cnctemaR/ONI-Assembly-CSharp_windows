using System;
using STRINGS;

namespace Database
{
	public class BalloonArtistFacadeResource : PermitResource
	{
		public string animFilename { get; private set; }

		public KAnimFile AnimFile { get; private set; }

		public BalloonArtistFacadeResource(string id, string name, string desc, PermitRarity rarity, string animFile, BalloonArtistFacadeType balloonFacadeType, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: base(id, name, desc, PermitCategory.JoyResponse, rarity, requiredDlcIds, forbiddenDlcIds)
		{
			this.AnimFile = Assets.GetAnim(animFile);
			this.animFilename = animFile;
			this.balloonFacadeType = balloonFacadeType;
			Db.Get().Accessories.AddAccessories(id, this.AnimFile);
			this.balloonOverrideSymbolIDs = this.GetBalloonOverrideSymbolIDs();
			Debug.Assert(this.balloonOverrideSymbolIDs.Length != 0);
		}

		public override PermitPresentationInfo GetPermitPresentationInfo()
		{
			PermitPresentationInfo permitPresentationInfo = default(PermitPresentationInfo);
			permitPresentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(this.AnimFile, "ui", false, "");
			permitPresentationInfo.SetFacadeForText(UI.KLEI_INVENTORY_SCREEN.BALLOON_ARTIST_FACADE_FOR);
			return permitPresentationInfo;
		}

		public BalloonOverrideSymbol GetNextOverride()
		{
			int num = this.nextSymbolIndex;
			this.nextSymbolIndex = (this.nextSymbolIndex + 1) % this.balloonOverrideSymbolIDs.Length;
			return new BalloonOverrideSymbol(this.animFilename, this.balloonOverrideSymbolIDs[num]);
		}

		public BalloonOverrideSymbolIter GetSymbolIter()
		{
			return new BalloonOverrideSymbolIter(this);
		}

		public BalloonOverrideSymbol GetOverrideAt(int index)
		{
			return new BalloonOverrideSymbol(this.animFilename, this.balloonOverrideSymbolIDs[index]);
		}

		private string[] GetBalloonOverrideSymbolIDs()
		{
			KAnim.Build build = this.AnimFile.GetData().build;
			BalloonArtistFacadeType balloonArtistFacadeType = this.balloonFacadeType;
			string[] array;
			if (balloonArtistFacadeType != BalloonArtistFacadeType.Single)
			{
				if (balloonArtistFacadeType != BalloonArtistFacadeType.ThreeSet)
				{
					throw new NotImplementedException();
				}
				array = new string[] { "body1", "body2", "body3" };
			}
			else
			{
				array = new string[] { "body" };
			}
			return array;
		}

		private BalloonArtistFacadeType balloonFacadeType;

		public readonly string[] balloonOverrideSymbolIDs;

		public int nextSymbolIndex;
	}
}
