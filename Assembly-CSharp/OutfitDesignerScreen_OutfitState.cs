using System;
using System.Collections.Generic;
using Database;
using UnityEngine;

public class OutfitDesignerScreen_OutfitState
{
	private OutfitDesignerScreen_OutfitState(ClothingOutfitUtility.OutfitType outfitType, ClothingOutfitTarget sourceTarget, ClothingOutfitTarget destinationTarget)
	{
		this.outfitType = outfitType;
		this.destinationTarget = destinationTarget;
		this.sourceTarget = sourceTarget;
		this.name = sourceTarget.ReadName();
		this.slots = OutfitDesignerScreen_OutfitState.Slots.For(outfitType);
		foreach (ClothingItemResource clothingItemResource in sourceTarget.ReadItemValues())
		{
			this.ApplyItem(clothingItemResource);
		}
	}

	public static OutfitDesignerScreen_OutfitState ForTemplateOutfit(ClothingOutfitTarget outfitTemplate)
	{
		global::Debug.Assert(outfitTemplate.IsTemplateOutfit());
		return new OutfitDesignerScreen_OutfitState(outfitTemplate.OutfitType, outfitTemplate, outfitTemplate);
	}

	public static OutfitDesignerScreen_OutfitState ForMinionInstance(ClothingOutfitTarget sourceTarget, GameObject minionInstance)
	{
		return new OutfitDesignerScreen_OutfitState(sourceTarget.OutfitType, sourceTarget, ClothingOutfitTarget.FromMinion(sourceTarget.OutfitType, minionInstance));
	}

	public unsafe void ApplyItem(ClothingItemResource item)
	{
		*this.slots.GetItemSlotForCategory(item.Category) = item;
	}

	public unsafe Option<ClothingItemResource> GetItemForCategory(PermitCategory category)
	{
		return *this.slots.GetItemSlotForCategory(category);
	}

	public unsafe void SetItemForCategory(PermitCategory category, Option<ClothingItemResource> item)
	{
		if (item.IsSome())
		{
			DebugUtil.DevAssert(item.Unwrap().outfitType == this.outfitType, string.Format("Tried to set clothing item with outfit type \"{0}\" to outfit of type \"{1}\"", item.Unwrap().outfitType, this.outfitType), null);
			DebugUtil.DevAssert(item.Unwrap().Category == category, string.Format("Tried to set clothing item with category \"{0}\" to slot with type \"{1}\"", item.Unwrap().Category, category), null);
		}
		*this.slots.GetItemSlotForCategory(category) = item;
	}

	public void AddItemValuesTo(ICollection<ClothingItemResource> clothingItems)
	{
		for (int i = 0; i < this.slots.array.Length; i++)
		{
			ref Option<ClothingItemResource> ptr = ref this.slots.array[i];
			if (ptr.IsSome())
			{
				clothingItems.Add(ptr.Unwrap());
			}
		}
	}

	public void AddItemsTo(ICollection<string> itemIds)
	{
		for (int i = 0; i < this.slots.array.Length; i++)
		{
			ref Option<ClothingItemResource> ptr = ref this.slots.array[i];
			if (ptr.IsSome())
			{
				itemIds.Add(ptr.Unwrap().Id);
			}
		}
	}

	public string[] GetItems()
	{
		List<string> list = new List<string>();
		this.AddItemsTo(list);
		return list.ToArray();
	}

	public bool DoesContainLockedItems()
	{
		bool flag;
		using (ListPool<string, OutfitDesignerScreen_OutfitState>.PooledList pooledList = PoolsFor<OutfitDesignerScreen_OutfitState>.AllocateList<string>())
		{
			this.AddItemsTo(pooledList);
			flag = ClothingOutfitTarget.DoesContainLockedItems(pooledList);
		}
		return flag;
	}

	public bool IsDirty()
	{
		using (HashSetPool<string, OutfitDesignerScreen>.PooledHashSet pooledHashSet = PoolsFor<OutfitDesignerScreen>.AllocateHashSet<string>())
		{
			this.AddItemsTo(pooledHashSet);
			string[] array = this.destinationTarget.ReadItems();
			if (pooledHashSet.Count != array.Length)
			{
				return true;
			}
			foreach (string text in array)
			{
				if (!pooledHashSet.Contains(text))
				{
					return true;
				}
			}
		}
		return false;
	}

	public string name;

	private OutfitDesignerScreen_OutfitState.Slots slots;

	public ClothingOutfitUtility.OutfitType outfitType;

	public ClothingOutfitTarget sourceTarget;

	public ClothingOutfitTarget destinationTarget;

	public abstract class Slots
	{
		private Slots(int slotsCount)
		{
			this.array = new Option<ClothingItemResource>[slotsCount];
		}

		public static OutfitDesignerScreen_OutfitState.Slots For(ClothingOutfitUtility.OutfitType outfitType)
		{
			switch (outfitType)
			{
			case ClothingOutfitUtility.OutfitType.Clothing:
				return new OutfitDesignerScreen_OutfitState.Slots.Clothing();
			case ClothingOutfitUtility.OutfitType.JoyResponse:
				throw new NotSupportedException("OutfitType.JoyResponse cannot be used with OutfitDesignerScreen_OutfitState. Use JoyResponseOutfitTarget instead.");
			case ClothingOutfitUtility.OutfitType.AtmoSuit:
				return new OutfitDesignerScreen_OutfitState.Slots.Atmosuit();
			case ClothingOutfitUtility.OutfitType.JetSuit:
				return new OutfitDesignerScreen_OutfitState.Slots.Jetsuit();
			default:
				throw new NotImplementedException();
			}
		}

		public abstract ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category);

		private ref Option<ClothingItemResource> FallbackSlot(OutfitDesignerScreen_OutfitState.Slots self, PermitCategory category)
		{
			DebugUtil.DevAssert(false, string.Format("Couldn't get a {0}<{1}> for {2} \"{3}\" on {4}.{5}", new object[]
			{
				"Option",
				"ClothingItemResource",
				"PermitCategory",
				category,
				"Slots",
				self.GetType().Name
			}), null);
			return ref OutfitDesignerScreen_OutfitState.Slots.dummySlot;
		}

		public Option<ClothingItemResource>[] array;

		private static Option<ClothingItemResource> dummySlot;

		public class Clothing : OutfitDesignerScreen_OutfitState.Slots
		{
			public Clothing()
				: base(6)
			{
			}

			public ref Option<ClothingItemResource> hatSlot
			{
				get
				{
					return ref this.array[0];
				}
			}

			public ref Option<ClothingItemResource> topSlot
			{
				get
				{
					return ref this.array[1];
				}
			}

			public ref Option<ClothingItemResource> glovesSlot
			{
				get
				{
					return ref this.array[2];
				}
			}

			public ref Option<ClothingItemResource> bottomSlot
			{
				get
				{
					return ref this.array[3];
				}
			}

			public ref Option<ClothingItemResource> shoesSlot
			{
				get
				{
					return ref this.array[4];
				}
			}

			public ref Option<ClothingItemResource> accessorySlot
			{
				get
				{
					return ref this.array[5];
				}
			}

			public override ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category)
			{
				if (category == PermitCategory.DupeHats)
				{
					return this.hatSlot;
				}
				if (category == PermitCategory.DupeTops)
				{
					return this.topSlot;
				}
				if (category == PermitCategory.DupeGloves)
				{
					return this.glovesSlot;
				}
				if (category == PermitCategory.DupeBottoms)
				{
					return this.bottomSlot;
				}
				if (category == PermitCategory.DupeShoes)
				{
					return this.shoesSlot;
				}
				if (category == PermitCategory.DupeAccessories)
				{
					return this.accessorySlot;
				}
				return base.FallbackSlot(this, category);
			}
		}

		public class Atmosuit : OutfitDesignerScreen_OutfitState.Slots
		{
			public Atmosuit()
				: base(5)
			{
			}

			public ref Option<ClothingItemResource> helmetSlot
			{
				get
				{
					return ref this.array[0];
				}
			}

			public ref Option<ClothingItemResource> bodySlot
			{
				get
				{
					return ref this.array[1];
				}
			}

			public ref Option<ClothingItemResource> glovesSlot
			{
				get
				{
					return ref this.array[2];
				}
			}

			public ref Option<ClothingItemResource> beltSlot
			{
				get
				{
					return ref this.array[3];
				}
			}

			public ref Option<ClothingItemResource> shoesSlot
			{
				get
				{
					return ref this.array[4];
				}
			}

			public override ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category)
			{
				if (category == PermitCategory.AtmoSuitHelmet)
				{
					return this.helmetSlot;
				}
				if (category == PermitCategory.AtmoSuitBody)
				{
					return this.bodySlot;
				}
				if (category == PermitCategory.AtmoSuitGloves)
				{
					return this.glovesSlot;
				}
				if (category == PermitCategory.AtmoSuitBelt)
				{
					return this.beltSlot;
				}
				if (category == PermitCategory.AtmoSuitShoes)
				{
					return this.shoesSlot;
				}
				return base.FallbackSlot(this, category);
			}
		}

		public class Jetsuit : OutfitDesignerScreen_OutfitState.Slots
		{
			public Jetsuit()
				: base(4)
			{
			}

			public ref Option<ClothingItemResource> helmetSlot
			{
				get
				{
					return ref this.array[0];
				}
			}

			public ref Option<ClothingItemResource> bodySlot
			{
				get
				{
					return ref this.array[1];
				}
			}

			public ref Option<ClothingItemResource> glovesSlot
			{
				get
				{
					return ref this.array[2];
				}
			}

			public ref Option<ClothingItemResource> shoesSlot
			{
				get
				{
					return ref this.array[3];
				}
			}

			public override ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category)
			{
				if (category == PermitCategory.JetSuitHelmet)
				{
					return this.helmetSlot;
				}
				if (category == PermitCategory.JetSuitBody)
				{
					return this.bodySlot;
				}
				if (category == PermitCategory.JetSuitGloves)
				{
					return this.glovesSlot;
				}
				if (category == PermitCategory.JetSuitShoes)
				{
					return this.shoesSlot;
				}
				return base.FallbackSlot(this, category);
			}
		}
	}
}
