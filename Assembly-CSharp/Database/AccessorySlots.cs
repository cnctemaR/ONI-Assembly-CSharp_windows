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
			this.Eyes = new AccessorySlot("Eyes", this, swap_build);
			this.Hair = new AccessorySlot("Hair", this, swap_build);
			this.HeadShape = new AccessorySlot("HeadShape", this, swap_build);
			this.Mouth = new AccessorySlot("Mouth", this, swap_build);
			this.Neck = new AccessorySlot("Neck", this, swap_build);
			this.Body = new AccessorySlot("Body", this, torso_swap_build);
			this.Arm = new AccessorySlot("Arm", this, torso_swap_build);
			foreach (AccessorySlot accessorySlot in this)
			{
				accessorySlot.AddAccessories(default_build, parent);
			}
		}

		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Neck;

		public AccessorySlot Body;

		public AccessorySlot Arm;
	}
}
