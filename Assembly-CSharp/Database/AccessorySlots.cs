using System;

namespace Database
{
	public class AccessorySlots : ResourceSet<AccessorySlot>
	{
		public AccessorySlots(ResourceSet parent, KAnimFile default_build = null, KAnimFile swap_build = null, KAnimFile torso_swap_build = null)
			: base("AccessorySlots", parent)
		{
			if (swap_build == null)
			{
				swap_build = Assets.GetAnim("head_swap_kanim");
				parent = Db.Get().Accessories;
			}
			if (default_build == null)
			{
				default_build = Assets.GetAnim("body_comp_default_kanim");
			}
			if (torso_swap_build == null)
			{
				torso_swap_build = Assets.GetAnim("body_swap_kanim");
			}
			this.Eyes = new AccessorySlot("Eyes", this, swap_build, null);
			this.Hair = new AccessorySlot("Hair", this, swap_build, null);
			this.HeadShape = new AccessorySlot("HeadShape", this, swap_build, null);
			this.Mouth = new AccessorySlot("Mouth", this, swap_build, null);
			this.Hat = new AccessorySlot("Hat", this, swap_build, null);
			this.HatHair = new AccessorySlot("Hat_Hair", this, swap_build, null);
			this.HairAlways = new AccessorySlot("Hair_Always", this, swap_build, "hair");
			this.Body = new AccessorySlot("Body", this, torso_swap_build, null);
			this.Arm = new AccessorySlot("Arm", this, torso_swap_build, null);
			foreach (AccessorySlot accessorySlot in this)
			{
				accessorySlot.AddAccessories(default_build, parent);
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
	}
}
