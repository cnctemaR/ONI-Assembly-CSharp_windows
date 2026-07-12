using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public class DynamicAtlasSettings
	{
		public int minAtlasSize
		{
			get
			{
				return this.m_MinAtlasSize;
			}
			set
			{
				this.m_MinAtlasSize = value;
			}
		}

		public int maxAtlasSize
		{
			get
			{
				return this.m_MaxAtlasSize;
			}
			set
			{
				this.m_MaxAtlasSize = value;
			}
		}

		public int maxSubTextureSize
		{
			get
			{
				return this.m_MaxSubTextureSize;
			}
			set
			{
				this.m_MaxSubTextureSize = value;
			}
		}

		public DynamicAtlasFilters activeFilters
		{
			get
			{
				return this.m_ActiveFilters;
			}
			set
			{
				this.m_ActiveFilters = value;
			}
		}

		public static DynamicAtlasFilters defaultFilters
		{
			get
			{
				return DynamicAtlas.defaultFilters;
			}
		}

		public DynamicAtlasCustomFilter customFilter
		{
			get
			{
				return this.m_CustomFilter;
			}
			set
			{
				this.m_CustomFilter = value;
			}
		}

		public static DynamicAtlasSettings defaults
		{
			get
			{
				return new DynamicAtlasSettings
				{
					minAtlasSize = 64,
					maxAtlasSize = 4096,
					maxSubTextureSize = 64,
					activeFilters = DynamicAtlasSettings.defaultFilters,
					customFilter = null
				};
			}
		}

		[HideInInspector]
		[SerializeField]
		private int m_MinAtlasSize;

		[HideInInspector]
		[SerializeField]
		private int m_MaxAtlasSize;

		[HideInInspector]
		[SerializeField]
		private int m_MaxSubTextureSize;

		[HideInInspector]
		[SerializeField]
		private DynamicAtlasFilters m_ActiveFilters;

		private DynamicAtlasCustomFilter m_CustomFilter;
	}
}
