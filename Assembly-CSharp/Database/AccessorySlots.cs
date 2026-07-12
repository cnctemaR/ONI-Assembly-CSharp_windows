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
			this.Eyes = new AccessorySlot("Eyes", this, anim, null);
			this.Hair = new AccessorySlot("Hair", this, anim4, null);
			this.HeadShape = new AccessorySlot("HeadShape", this, anim, null);
			this.Mouth = new AccessorySlot("Mouth", this, anim, null);
			this.Hat = new AccessorySlot("Hat", this, anim5, null);
			this.HatHair = new AccessorySlot("Hat_Hair", this, anim4, null);
			this.HairAlways = new AccessorySlot("Hair_Always", this, anim4, "hair");
			this.HeadEffects = new AccessorySlot("HeadFX", this, anim, null);
			this.Body = new AccessorySlot("Body", this, anim3, null);
			this.Arm = new AccessorySlot("Arm", this, anim3, null);
			foreach (AccessorySlot accessorySlot in this.resources)
			{
				accessorySlot.AddAccessories(anim2, parent);
			}
		}

		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Body;

		public AccessorySlot Arm;

		public AccessorySlot Hat;

		public AccessorySlot HatHair;

		public AccessorySlot HairAlways;

		public AccessorySlot HeadEffects;
	}
}
