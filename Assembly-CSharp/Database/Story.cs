using System;
using ProcGen;

namespace Database
{
	public class Story : Resource, IComparable<Story>
	{
		public int HashId { get; private set; }

		public WorldTrait StoryTrait
		{
			get
			{
				if (this._cachedStoryTrait == null)
				{
					this._cachedStoryTrait = SettingsCache.GetCachedStoryTrait(this.worldgenStoryTraitKey, false);
				}
				return this._cachedStoryTrait;
			}
		}

		public Story(string id, string worldgenStoryTraitKey, int displayOrder)
		{
			this.Id = id;
			this.worldgenStoryTraitKey = worldgenStoryTraitKey;
			this.displayOrder = displayOrder;
			this.kleiUseOnlyCoordinateOrder = -1;
			this.updateNumber = -1;
			this.sandboxStampTemplateId = null;
			this.HashId = Hash.SDBMLower(id);
		}

		public Story(string id, string worldgenStoryTraitKey, int displayOrder, int kleiUseOnlyCoordinateOrder, int updateNumber, string sandboxStampTemplateId)
		{
			this.Id = id;
			this.worldgenStoryTraitKey = worldgenStoryTraitKey;
			this.displayOrder = displayOrder;
			this.updateNumber = updateNumber;
			this.sandboxStampTemplateId = sandboxStampTemplateId;
			this.kleiUseOnlyCoordinateOrder = kleiUseOnlyCoordinateOrder;
			this.HashId = Hash.SDBMLower(id);
		}

		public int CompareTo(Story other)
		{
			return this.displayOrder.CompareTo(other.displayOrder);
		}

		public bool IsNew()
		{
			return this.updateNumber == LaunchInitializer.UpdateNumber();
		}

		public Story AutoStart()
		{
			this.autoStart = true;
			return this;
		}

		public Story SetKeepsake(string prefabId)
		{
			this.keepsakePrefabId = prefabId;
			return this;
		}

		public const int MODDED_STORY = -1;

		public int kleiUseOnlyCoordinateOrder;

		public bool autoStart;

		public string keepsakePrefabId;

		public readonly string worldgenStoryTraitKey;

		private readonly int displayOrder;

		private readonly int updateNumber;

		public string sandboxStampTemplateId;

		private WorldTrait _cachedStoryTrait;
	}
}
