using System;

namespace Database
{
	public class AccessorySlots : ResourceSet<AccessorySlot>
	{
		public AccessorySlots(ResourceSet parent, KAnimFile default_build = null, KAnimFile swap_build = null)
			: base("AccessorySlots", parent)
		{
			this.Eyes = new AccessorySlot("Eyes", this);
			this.Hair = new AccessorySlot("Hair", this);
			this.HeadShape = new AccessorySlot("HeadShape", this);
			this.Mouth = new AccessorySlot("Mouth", this);
			this.Neck = new AccessorySlot("Neck", this);
			if (swap_build == null)
			{
				swap_build = Assets.GetAnim("head_swap");
				parent = Db.Get().Accessories;
			}
			if (default_build == null)
			{
				default_build = Assets.GetAnim("body_comp_default");
			}
			foreach (AccessorySlot accessorySlot in this)
			{
				accessorySlot.AddAccessories(default_build, swap_build, parent);
			}
		}

		public AccessorySlot Eyes;

		public AccessorySlot Hair;

		public AccessorySlot HeadShape;

		public AccessorySlot Mouth;

		public AccessorySlot Neck;
	}
}
