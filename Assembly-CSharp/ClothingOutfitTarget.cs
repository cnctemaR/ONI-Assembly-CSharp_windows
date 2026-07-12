using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using STRINGS;
using UnityEngine;

public readonly struct ClothingOutfitTarget : IEquatable<ClothingOutfitTarget>
{
	public string Id
	{
		get
		{
			return this.impl.OutfitId;
		}
	}

	public string[] ReadItems()
	{
		return this.impl.ReadItems().Where<string>(new Func<string, bool>(ClothingOutfitTarget.DoesClothingItemExist)).ToArray<string>();
	}

	public void WriteItems(string[] items)
	{
		this.impl.WriteItems(items);
	}

	public bool CanWriteItems
	{
		get
		{
			return this.impl.CanWriteItems;
		}
	}

	public string ReadName()
	{
		return this.impl.ReadName();
	}

	public void WriteName(string name)
	{
		this.impl.WriteName(name);
	}

	public bool CanWriteName
	{
		get
		{
			return this.impl.CanWriteName;
		}
	}

	public void Delete()
	{
		this.impl.Delete();
	}

	public bool CanDelete
	{
		get
		{
			return this.impl.CanDelete;
		}
	}

	public bool DoesExist()
	{
		return this.impl.DoesExist();
	}

	public ClothingOutfitTarget(ClothingOutfitTarget.Implementation impl)
	{
		this.impl = impl;
	}

	public bool DoesContainNonOwnedItems()
	{
		return ClothingOutfitTarget.DoesContainNonOwnedItems(this.ReadItems());
	}

	public static bool DoesContainNonOwnedItems(IList<string> itemIds)
	{
		foreach (string text in itemIds)
		{
			bool flag;
			int num;
			PermitItems.GetOwnedCount(text).Deconstruct(out flag, out num);
			bool flag2 = flag;
			int num2 = num;
			if (flag2 && num2 <= 0)
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<ClothingItemResource> ReadItemValues()
	{
		return from i in this.ReadItems()
			select Db.Get().Permits.ClothingItems.Get(i);
	}

	public static bool DoesClothingItemExist(string clothingItemId)
	{
		return !Db.Get().Permits.ClothingItems.TryGet(clothingItemId).IsNullOrDestroyed();
	}

	public bool Is<T>() where T : ClothingOutfitTarget.Implementation
	{
		return this.impl is T;
	}

	public bool Is<T>(out T value) where T : ClothingOutfitTarget.Implementation
	{
		ClothingOutfitTarget.Implementation implementation = this.impl;
		if (implementation is T)
		{
			T t = (T)((object)implementation);
			value = t;
			return true;
		}
		value = default(T);
		return false;
	}

	public bool IsTemplateOutfit()
	{
		return this.Is<ClothingOutfitTarget.KleiAuthored>() || this.Is<ClothingOutfitTarget.UserAuthored>();
	}

	public static ClothingOutfitTarget ForNewOutfit()
	{
		return new ClothingOutfitTarget(new ClothingOutfitTarget.UserAuthored(ClothingOutfitTarget.GetUniqueNameIdFrom(UI.OUTFIT_NAME.NEW)));
	}

	public static ClothingOutfitTarget ForNewOutfit(string id)
	{
		if (ClothingOutfitTarget.DoesExist(id))
		{
			throw new ArgumentException("Can not create a new target with id " + id + ", an outfit with that id already exists");
		}
		return new ClothingOutfitTarget(new ClothingOutfitTarget.UserAuthored(id));
	}

	public static ClothingOutfitTarget ForCopyOf(ClothingOutfitTarget sourceTarget)
	{
		return new ClothingOutfitTarget(new ClothingOutfitTarget.UserAuthored(ClothingOutfitTarget.GetUniqueNameIdFrom(UI.OUTFIT_NAME.COPY_OF.Replace("{OutfitName}", sourceTarget.ReadName()))));
	}

	public static ClothingOutfitTarget FromMinion(GameObject minionInstance)
	{
		return new ClothingOutfitTarget(new ClothingOutfitTarget.MinionInstance(minionInstance));
	}

	public static ClothingOutfitTarget FromId(string outfitId)
	{
		return ClothingOutfitTarget.TryFromId(outfitId).Value;
	}

	public static Option<ClothingOutfitTarget> TryFromId(string outfitId)
	{
		if (outfitId == null)
		{
			return Option.None;
		}
		if (CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(outfitId))
		{
			return new ClothingOutfitTarget(new ClothingOutfitTarget.UserAuthored(outfitId));
		}
		if (Db.Get().Permits.ClothingOutfits.TryGet(outfitId) != null)
		{
			return new ClothingOutfitTarget(new ClothingOutfitTarget.KleiAuthored(outfitId));
		}
		return Option.None;
	}

	public static bool DoesExist(string outfitId)
	{
		return Db.Get().Permits.ClothingOutfits.TryGet(outfitId) != null || CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(outfitId);
	}

	public static IEnumerable<ClothingOutfitTarget> GetAll()
	{
		foreach (ClothingOutfitResource clothingOutfitResource in Db.Get().Permits.ClothingOutfits.resources)
		{
			yield return new ClothingOutfitTarget(new ClothingOutfitTarget.KleiAuthored(clothingOutfitResource));
		}
		List<ClothingOutfitResource>.Enumerator enumerator = default(List<ClothingOutfitResource>.Enumerator);
		foreach (KeyValuePair<string, string[]> keyValuePair in CustomClothingOutfits.Instance.OutfitData.CustomOutfits)
		{
			string text;
			string[] array;
			keyValuePair.Deconstruct<string, string[]>(out text, out array);
			string text2 = text;
			yield return new ClothingOutfitTarget(new ClothingOutfitTarget.UserAuthored(text2));
		}
		Dictionary<string, string[]>.Enumerator enumerator2 = default(Dictionary<string, string[]>.Enumerator);
		yield break;
		yield break;
	}

	public static ClothingOutfitTarget GetRandom()
	{
		return ClothingOutfitTarget.GetAll().GetRandom<ClothingOutfitTarget>();
	}

	public static string GetUniqueNameIdFrom(string preferredName)
	{
		if (!ClothingOutfitTarget.DoesExist(preferredName))
		{
			return preferredName;
		}
		string text = "testOutfit";
		string text2 = UI.OUTFIT_NAME.RESOLVE_CONFLICT.Replace("{OutfitName}", text).Replace("{ConflictNumber}", 1.ToString());
		string text3 = UI.OUTFIT_NAME.RESOLVE_CONFLICT.Replace("{OutfitName}", text).Replace("{ConflictNumber}", 2.ToString());
		string text4;
		if (text2 != text3)
		{
			text4 = UI.OUTFIT_NAME.RESOLVE_CONFLICT;
		}
		else
		{
			text4 = "{OutfitName} ({ConflictNumber})";
		}
		for (int i = 1; i < 10000; i++)
		{
			string text5 = text4.Replace("{OutfitName}", preferredName).Replace("{ConflictNumber}", i.ToString());
			if (!ClothingOutfitTarget.DoesExist(text5))
			{
				return text5;
			}
		}
		throw new Exception("Couldn't get a unique name for preferred name: " + preferredName);
	}

	public static bool operator ==(ClothingOutfitTarget a, ClothingOutfitTarget b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(ClothingOutfitTarget a, ClothingOutfitTarget b)
	{
		return !a.Equals(b);
	}

	public override bool Equals(object obj)
	{
		if (obj is ClothingOutfitTarget)
		{
			ClothingOutfitTarget clothingOutfitTarget = (ClothingOutfitTarget)obj;
			return this.Equals(clothingOutfitTarget);
		}
		return false;
	}

	public bool Equals(ClothingOutfitTarget other)
	{
		if (this.impl == null || other.impl == null)
		{
			return this.impl == null == (other.impl == null);
		}
		return this.Id == other.Id;
	}

	public override int GetHashCode()
	{
		return Hash.SDBMLower(this.impl.OutfitId);
	}

	public readonly ClothingOutfitTarget.Implementation impl;

	public interface Implementation
	{
		string OutfitId { get; }

		string[] ReadItems();

		void WriteItems(string[] items);

		bool CanWriteItems { get; }

		string ReadName();

		void WriteName(string name);

		bool CanWriteName { get; }

		void Delete();

		bool CanDelete { get; }

		bool DoesExist();
	}

	public readonly struct MinionInstance : ClothingOutfitTarget.Implementation
	{
		public bool CanWriteItems
		{
			get
			{
				return true;
			}
		}

		public bool CanWriteName
		{
			get
			{
				return false;
			}
		}

		public bool CanDelete
		{
			get
			{
				return false;
			}
		}

		public bool DoesExist()
		{
			return !this.minionInstance.IsNullOrDestroyed();
		}

		public string OutfitId
		{
			get
			{
				return this.minionInstance.GetInstanceID().ToString() + "_outfit";
			}
		}

		public MinionInstance(GameObject minionInstance)
		{
			this.minionInstance = minionInstance;
			this.accessorizer = minionInstance.GetComponent<Accessorizer>();
		}

		public string[] ReadItems()
		{
			return this.accessorizer.GetClothingItemIds();
		}

		public void WriteItems(string[] items)
		{
			this.accessorizer.ApplyClothingItems(items.Select<string, ClothingItemResource>((string i) => Db.Get().Permits.ClothingItems.Get(i)), true);
		}

		public string ReadName()
		{
			return UI.OUTFIT_NAME.MINIONS_OUTFIT.Replace("{MinionName}", this.minionInstance.GetProperName());
		}

		public void WriteName(string name)
		{
			throw new InvalidOperationException("Can not change change the outfit id for a minion instance");
		}

		public void Delete()
		{
			throw new InvalidOperationException("Can not delete a minion instance outfit");
		}

		public readonly GameObject minionInstance;

		public readonly Accessorizer accessorizer;
	}

	public readonly struct UserAuthored : ClothingOutfitTarget.Implementation
	{
		public bool CanWriteItems
		{
			get
			{
				return true;
			}
		}

		public bool CanWriteName
		{
			get
			{
				return true;
			}
		}

		public bool CanDelete
		{
			get
			{
				return true;
			}
		}

		public bool DoesExist()
		{
			return CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(this.OutfitId);
		}

		public string OutfitId
		{
			get
			{
				return this.m_outfitId[0];
			}
		}

		public UserAuthored(string outfitId)
		{
			this.m_outfitId = new string[] { outfitId };
		}

		public string[] ReadItems()
		{
			string[] array;
			if (CustomClothingOutfits.Instance.OutfitData.CustomOutfits.TryGetValue(this.OutfitId, out array))
			{
				return array;
			}
			return ClothingOutfitTargetExtensions.NO_ITEMS;
		}

		public void WriteItems(string[] items)
		{
			CustomClothingOutfits.Instance.EditOutfit(this.OutfitId, items);
		}

		public string ReadName()
		{
			return this.OutfitId;
		}

		public void WriteName(string name)
		{
			if (this.OutfitId == name)
			{
				return;
			}
			if (ClothingOutfitTarget.DoesExist(name))
			{
				throw new Exception(string.Concat(new string[] { "Can not change outfit name from \"", this.OutfitId, "\" to \"", name, "\", \"", name, "\" already exists" }));
			}
			if (CustomClothingOutfits.Instance.OutfitData.CustomOutfits.ContainsKey(this.OutfitId))
			{
				CustomClothingOutfits.Instance.RenameOutfit(this.OutfitId, name);
			}
			else
			{
				CustomClothingOutfits.Instance.EditOutfit(name, ClothingOutfitTargetExtensions.NO_ITEMS);
			}
			this.m_outfitId[0] = name;
		}

		public void Delete()
		{
			CustomClothingOutfits.Instance.RemoveOutfit(this.OutfitId);
		}

		private readonly string[] m_outfitId;
	}

	public readonly struct KleiAuthored : ClothingOutfitTarget.Implementation
	{
		public bool CanWriteItems
		{
			get
			{
				return false;
			}
		}

		public bool CanWriteName
		{
			get
			{
				return false;
			}
		}

		public bool CanDelete
		{
			get
			{
				return false;
			}
		}

		public bool DoesExist()
		{
			return true;
		}

		public string OutfitId
		{
			get
			{
				return this.m_outfitId;
			}
		}

		public KleiAuthored(string outfitId)
		{
			this.m_outfitId = outfitId;
			this.resource = Db.Get().Permits.ClothingOutfits.Get(outfitId);
		}

		public KleiAuthored(ClothingOutfitResource outfit)
		{
			this.m_outfitId = outfit.Id;
			this.resource = outfit;
		}

		public string[] ReadItems()
		{
			return this.resource.itemsInOutfit;
		}

		public void WriteItems(string[] items)
		{
			throw new InvalidOperationException("Can not set items on a Db authored outfit");
		}

		public string ReadName()
		{
			return this.resource.Name;
		}

		public void WriteName(string name)
		{
			throw new InvalidOperationException("Can not set name on a Db authored outfit");
		}

		public void Delete()
		{
			throw new InvalidOperationException("Can not delete a Db authored outfit");
		}

		public readonly ClothingOutfitResource resource;

		private readonly string m_outfitId;
	}
}
