using System;

namespace Database
{
	public class AccessorySlots : ResourceSet<AccessorySlot>
	{
		public AccessorySlots(ResourceSet parent)
			: base("AccessorySlots", parent)
		{
			parent = Db.Get().Accessories;
			KAnimFile anim = Assets.GetAnim("head_swap_kanim");
			KAnimFile anim2 = Assets.GetAnim("body_comp_default_kanim");
			KAnimFile anim3 = Assets.GetAnim("body_swap_kanim");
			KAnimFile anim4 = Assets.GetAnim("hair_swap_kanim");
			KAnimFile anim5 = Assets.GetAnim("hat_swap_kanim");
			this.Eyes = new AccessorySlot("Eyes", this, anim, true);
			this.Hair = new AccessorySlot("Hair", this, anim4, true);
			this.HeadShape = new AccessorySlot("HeadShape", this, anim, true);
			this.Mouth = new AccessorySlot("Mouth", this, anim, true);
			this.Hat = new AccessorySlot("Hat", this, anim5, false);
			this.HatHair = new AccessorySlot("Hat_Hair", this, anim4, false);
			this.HeadEffects = new AccessorySlot("HeadFX", this, anim, false);
			this.Body = new AccessorySlot("Torso", this, new KAnimHashedString("torso"), anim3, true, null);
			this.Arm = new AccessorySlot("Arm_Sleeve", this, new KAnimHashedString("arm_sleeve"), anim3, false, null);
			this.ArmLower = new AccessorySlot("Arm_Lower_Sleeve", this, new KAnimHashedString("arm_lower_sleeve"), anim3, false, null);
			this.Belt = new AccessorySlot("Belt", this, new KAnimHashedString("belt"), anim2, false, null);
			this.Neck = new AccessorySlot("Neck", this, new KAnimHashedString("neck"), anim2, false, null);
			this.Pelvis = new AccessorySlot("Pelvis", this, new KAnimHashedString("pelvis"), anim2, true, null);
			this.Foot = new AccessorySlot("Foot", this, new KAnimHashedString("foot"), anim2, true, Assets.GetAnim("shoes_basic_black_kanim"));
			this.Leg = new AccessorySlot("Leg", this, new KAnimHashedString("leg"), anim2, false, null);
			this.Necklace = new AccessorySlot("Necklace", this, new KAnimHashedString("necklace"), anim2, false, null);
			this.Cuff = new AccessorySlot("Cuff", this, new KAnimHashedString("cuff"), anim2, true, null);
			this.Skirt = new AccessorySlot("Skirt", this, new KAnimHashedString("skirt"), anim3, false, null);
			this.ArmLowerSkin = new AccessorySlot("Arm_Lower", this, new KAnimHashedString("arm_lower"), anim3, true, null);
			this.ArmUpperSkin = new AccessorySlot("Arm_Upper", this, new KAnimHashedString("arm_upper"), anim3, true, null);
			this.LegSkin = new AccessorySlot("Leg_Skin", this, new KAnimHashedString("leg_skin"), anim3, true, null);
			this.Hand = new AccessorySlot("Hand", this, new KAnimHashedString("hand_paint"), anim2, true, null);
			foreach (AccessorySlot accessorySlot in this.resources)
			{
				accessorySlot.AddAccessories(accessorySlot.AnimFile, parent);
			}
		}

		public AccessorySlot Find(KAnimHashedString symbol_name)
		{
			foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
			{
				if (symbol_name == accessorySlot.targetSymbolId)
				{
					return accessorySlot;
				}
			}
			return null;
		}

		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Body;

		public AccessorySlot Arm;

		public AccessorySlot ArmLower;

		public AccessorySlot Hat;

		public AccessorySlot HatHair;

		public AccessorySlot HeadEffects;

		public AccessorySlot Belt;

		public AccessorySlot Neck;

		public AccessorySlot Pelvis;

		public AccessorySlot Leg;

		public AccessorySlot Foot;

		public AccessorySlot Skirt;

		public AccessorySlot Necklace;

		public AccessorySlot Cuff;

		public AccessorySlot Hand;

		public AccessorySlot ArmLowerSkin;

		public AccessorySlot ArmUpperSkin;

		public AccessorySlot LegSkin;
	}
}
