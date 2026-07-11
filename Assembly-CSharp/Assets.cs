using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Assets : KMonoBehaviour, ISerializationCallbackReceiver
{
	protected override void OnPrefabInit()
	{
		Assets.instance = this;
		if (KPlayerPrefs.HasKey("TemperatureUnit"))
		{
			GameUtil.temperatureUnit = (GameUtil.TemperatureUnit)KPlayerPrefs.GetInt("TemperatureUnit");
		}
		if (KPlayerPrefs.HasKey("MassUnit"))
		{
			GameUtil.massUnit = (GameUtil.MassUnit)KPlayerPrefs.GetInt("MassUnit");
		}
		RecipeManager.DestroyInstance();
		RecipeManager.Get();
		Assets.AnimMaterial = this.AnimMaterialAsset;
		Assets.Prefabs = new List<KPrefabID>(this.PrefabAssets.Where<KPrefabID>((KPrefabID x) => x != null));
		Assets.PrefabsByTag.Clear();
		Assets.PrefabsByAdditionalTags.Clear();
		Assets.CountableTags.Clear();
		Assets.Sprites = new Dictionary<HashedString, Sprite>();
		foreach (Sprite sprite in this.SpriteAssets)
		{
			if (!(sprite == null))
			{
				HashedString hashedString = new HashedString(sprite.name);
				Assets.Sprites.Add(hashedString, sprite);
			}
		}
		Assets.TintedSprites = this.TintedSpriteAssets.Where<TintedSprite>((TintedSprite x) => x != null && x.sprite != null).ToList<TintedSprite>();
		Assets.Materials = this.MaterialAssets.Where<Material>((Material x) => x != null).ToList<Material>();
		Assets.Textures = this.TextureAssets.Where<Texture2D>((Texture2D x) => x != null).ToList<Texture2D>();
		Assets.TextureAtlases = this.TextureAtlasAssets.Where<TextureAtlas>((TextureAtlas x) => x != null).ToList<TextureAtlas>();
		Assets.BlockTileDecorInfos = this.BlockTileDecorInfoAssets.Where<BlockTileDecorInfo>((BlockTileDecorInfo x) => x != null).ToList<BlockTileDecorInfo>();
		Assets.Anims = this.AnimAssets.Where<KAnimFile>((KAnimFile x) => x != null).ToList<KAnimFile>();
		Assets.Anims.AddRange(Assets.ModLoadedKAnims);
		Assets.UIPrefabs = this.UIPrefabAssets;
		Assets.DebugFont = this.DebugFontAsset;
		AsyncLoadManager<IGlobalAsyncLoader>.Run();
		GameAudioSheets.Get().Initialize();
		this.SubstanceListHookup();
		Assets.BuildingDefs = new List<BuildingDef>();
		foreach (KPrefabID kprefabID in this.PrefabAssets)
		{
			if (!(kprefabID == null))
			{
				Assets.AddPrefab(kprefabID);
			}
		}
		Assets.AnimTable.Clear();
		foreach (KAnimFile kanimFile in Assets.Anims)
		{
			if (kanimFile != null)
			{
				HashedString hashedString2 = kanimFile.name;
				Assets.AnimTable[hashedString2] = kanimFile;
			}
		}
		this.CreatePrefabs();
	}

	private void CreatePrefabs()
	{
		LegacyModMain.Load();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Db.Get();
	}

	private static void TryAddCountableTag(KPrefabID prefab)
	{
		foreach (Tag tag in GameTags.DisplayAsUnits)
		{
			if (prefab.HasTag(tag))
			{
				Assets.AddCountableTag(prefab.PrefabTag);
				break;
			}
		}
	}

	public static void AddCountableTag(Tag tag)
	{
		Assets.CountableTags.Add(tag);
	}

	public static bool IsTagCountable(Tag tag)
	{
		return Assets.CountableTags.Contains(tag);
	}

	private void SubstanceListHookup()
	{
		Hashtable hashtable = new Hashtable();
		ElementsAudio.Instance.LoadData(AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<ElementAudioFileLoader>.Get().entries);
		ElementLoader.Load(ref hashtable, this.substanceTable);
		Assets.SubstanceTable = this.substanceTable;
	}

	public static string GetSimpleSoundEventName(string path)
	{
		string text = null;
		if (!Assets.simpleSoundEventNames.TryGetValue(path, out text))
		{
			int num = path.LastIndexOf('/');
			text = ((num != -1) ? path.Substring(num + 1) : path);
			Assets.simpleSoundEventNames[path] = text;
		}
		return text;
	}

	private static BuildingDef GetDef(IList<BuildingDef> defs, string prefab_id)
	{
		int count = defs.Count;
		for (int i = 0; i < count; i++)
		{
			if (defs[i].PrefabID == prefab_id)
			{
				return defs[i];
			}
		}
		return null;
	}

	public static BuildingDef GetBuildingDef(string prefab_id)
	{
		return Assets.GetDef(Assets.BuildingDefs, prefab_id);
	}

	public static TintedSprite GetTintedSprite(string name)
	{
		TintedSprite tintedSprite = null;
		if (Assets.TintedSprites != null)
		{
			for (int i = 0; i < Assets.TintedSprites.Count; i++)
			{
				if (Assets.TintedSprites[i].sprite.name == name)
				{
					tintedSprite = Assets.TintedSprites[i];
					break;
				}
			}
		}
		return tintedSprite;
	}

	public static Sprite GetSprite(HashedString name)
	{
		Sprite sprite = null;
		if (Assets.Sprites != null)
		{
			Assets.Sprites.TryGetValue(name, out sprite);
		}
		return sprite;
	}

	public static VideoClip GetVideo(string name)
	{
		return Resources.Load<VideoClip>("video_webm/" + name);
	}

	public static Texture2D GetTexture(string name)
	{
		Texture2D texture2D = null;
		if (Assets.Textures != null)
		{
			for (int i = 0; i < Assets.Textures.Count; i++)
			{
				if (Assets.Textures[i].name == name)
				{
					texture2D = Assets.Textures[i];
					break;
				}
			}
		}
		return texture2D;
	}

	public static ComicData GetComic(string id)
	{
		foreach (ComicData comicData in Assets.instance.comics)
		{
			if (comicData.name == id)
			{
				return comicData;
			}
		}
		return null;
	}

	public static void AddPrefab(KPrefabID prefab)
	{
		if (prefab == null)
		{
			return;
		}
		prefab.UpdateSaveLoadTag();
		if (Assets.PrefabsByTag.ContainsKey(prefab.PrefabTag))
		{
			global::Debug.LogWarning("Tried loading prefab with duplicate tag, ignoring: " + prefab.PrefabTag);
		}
		Assets.PrefabsByTag[prefab.PrefabTag] = prefab;
		foreach (Tag tag in prefab.Tags)
		{
			if (!Assets.PrefabsByAdditionalTags.ContainsKey(tag))
			{
				Assets.PrefabsByAdditionalTags[tag] = new List<KPrefabID>();
			}
			Assets.PrefabsByAdditionalTags[tag].Add(prefab);
		}
		Assets.Prefabs.Add(prefab);
		Assets.TryAddCountableTag(prefab);
		if (Assets.OnAddPrefab != null)
		{
			Assets.OnAddPrefab(prefab);
		}
	}

	public static void RegisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		Assets.OnAddPrefab = (Action<KPrefabID>)Delegate.Combine(Assets.OnAddPrefab, on_add);
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			on_add(kprefabID);
		}
	}

	public static void UnregisterOnAddPrefab(Action<KPrefabID> on_add)
	{
		Assets.OnAddPrefab = (Action<KPrefabID>)Delegate.Remove(Assets.OnAddPrefab, on_add);
	}

	public static void ClearOnAddPrefab()
	{
		Assets.OnAddPrefab = null;
	}

	public static GameObject GetPrefab(Tag tag)
	{
		GameObject gameObject = Assets.TryGetPrefab(tag);
		if (gameObject == null)
		{
			global::Debug.LogWarning("Missing prefab: " + tag);
		}
		return gameObject;
	}

	public static GameObject TryGetPrefab(Tag tag)
	{
		KPrefabID kprefabID = null;
		Assets.PrefabsByTag.TryGetValue(tag, out kprefabID);
		if (!(kprefabID != null))
		{
			return null;
		}
		return kprefabID.gameObject;
	}

	public static List<GameObject> GetPrefabsWithTag(Tag tag)
	{
		List<GameObject> list = new List<GameObject>();
		if (Assets.PrefabsByAdditionalTags.ContainsKey(tag))
		{
			for (int i = 0; i < Assets.PrefabsByAdditionalTags[tag].Count; i++)
			{
				list.Add(Assets.PrefabsByAdditionalTags[tag][i].gameObject);
			}
		}
		return list;
	}

	public static List<GameObject> GetPrefabsWithComponent<Type>()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < Assets.Prefabs.Count; i++)
		{
			if (Assets.Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Assets.Prefabs[i].gameObject);
			}
		}
		return list;
	}

	public static List<Tag> GetPrefabTagsWithComponent<Type>()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < Assets.Prefabs.Count; i++)
		{
			if (Assets.Prefabs[i].GetComponent<Type>() != null)
			{
				list.Add(Assets.Prefabs[i].PrefabID());
			}
		}
		return list;
	}

	public static Assets GetInstanceEditorOnly()
	{
		Assets[] array = (Assets[])Resources.FindObjectsOfTypeAll(typeof(Assets));
		if (array != null)
		{
			int num = array.Length;
		}
		return array[0];
	}

	public static TextureAtlas GetTextureAtlas(string name)
	{
		foreach (TextureAtlas textureAtlas in Assets.TextureAtlases)
		{
			if (textureAtlas.name == name)
			{
				return textureAtlas;
			}
		}
		return null;
	}

	public static Material GetMaterial(string name)
	{
		foreach (Material material in Assets.Materials)
		{
			if (material.name == name)
			{
				return material;
			}
		}
		return null;
	}

	public static BlockTileDecorInfo GetBlockTileDecorInfo(string name)
	{
		foreach (BlockTileDecorInfo blockTileDecorInfo in Assets.BlockTileDecorInfos)
		{
			if (blockTileDecorInfo.name == name)
			{
				return blockTileDecorInfo;
			}
		}
		global::Debug.LogError("Could not find BlockTileDecorInfo named [" + name + "]");
		return null;
	}

	public static KAnimFile GetAnim(HashedString name)
	{
		if (!name.IsValid)
		{
			global::Debug.LogWarning("Invalid hash name");
			return null;
		}
		KAnimFile kanimFile = null;
		Assets.AnimTable.TryGetValue(name, out kanimFile);
		if (kanimFile == null)
		{
			global::Debug.LogWarning("Missing Anim: [" + name.ToString() + "]. You may have to run Collect Anim on the Assets prefab");
		}
		return kanimFile;
	}

	public void OnAfterDeserialize()
	{
		this.TintedSpriteAssets = this.TintedSpriteAssets.Where<TintedSprite>((TintedSprite x) => x != null && x.sprite != null).ToList<TintedSprite>();
		this.TintedSpriteAssets.Sort((TintedSprite a, TintedSprite b) => a.name.CompareTo(b.name));
	}

	public void OnBeforeSerialize()
	{
	}

	public static void AddBuildingDef(BuildingDef def)
	{
		Assets.BuildingDefs = Assets.BuildingDefs.Where<BuildingDef>((BuildingDef x) => x.PrefabID != def.PrefabID).ToList<BuildingDef>();
		Assets.BuildingDefs.Add(def);
	}

	public static List<KAnimFile> ModLoadedKAnims = new List<KAnimFile>();

	private static Action<KPrefabID> OnAddPrefab;

	public static List<BuildingDef> BuildingDefs;

	public List<KPrefabID> PrefabAssets = new List<KPrefabID>();

	public static List<KPrefabID> Prefabs = new List<KPrefabID>();

	private static HashSet<Tag> CountableTags = new HashSet<Tag>();

	public List<Sprite> SpriteAssets;

	public static Dictionary<HashedString, Sprite> Sprites;

	public List<string> videoClipNames;

	private const string VIDEO_ASSET_PATH = "video_webm";

	public List<TintedSprite> TintedSpriteAssets;

	public static List<TintedSprite> TintedSprites;

	public List<Texture2D> TextureAssets;

	public static List<Texture2D> Textures;

	public static List<TextureAtlas> TextureAtlases;

	public List<TextureAtlas> TextureAtlasAssets;

	public static List<Material> Materials;

	public List<Material> MaterialAssets;

	public static List<Shader> Shaders;

	public List<Shader> ShaderAssets;

	public static List<BlockTileDecorInfo> BlockTileDecorInfos;

	public List<BlockTileDecorInfo> BlockTileDecorInfoAssets;

	public Material AnimMaterialAsset;

	public static Material AnimMaterial;

	public DiseaseVisualization DiseaseVisualization;

	public Sprite LegendColourBox;

	public Texture2D invalidAreaTex;

	public Assets.UIPrefabData UIPrefabAssets;

	public static Assets.UIPrefabData UIPrefabs;

	private static Dictionary<Tag, KPrefabID> PrefabsByTag = new Dictionary<Tag, KPrefabID>();

	private static Dictionary<Tag, List<KPrefabID>> PrefabsByAdditionalTags = new Dictionary<Tag, List<KPrefabID>>();

	private static Dictionary<HashedString, KAnimFile> AnimTable = new Dictionary<HashedString, KAnimFile>();

	public List<KAnimFile> AnimAssets;

	public static List<KAnimFile> Anims;

	public Font DebugFontAsset;

	public static Font DebugFont;

	public SubstanceTable substanceTable;

	public static SubstanceTable SubstanceTable;

	[SerializeField]
	public TextAsset elementAudio;

	[SerializeField]
	public TextAsset personalitiesFile;

	public LogicModeUI logicModeUIData;

	public CommonPlacerConfig.CommonPlacerAssets commonPlacerAssets;

	public DigPlacerConfig.DigPlacerAssets digPlacerAssets;

	public MopPlacerConfig.MopPlacerAssets mopPlacerAssets;

	public ComicData[] comics;

	public static Assets instance;

	private static Dictionary<string, string> simpleSoundEventNames = new Dictionary<string, string>();

	[Serializable]
	public struct UIPrefabData
	{
		public ProgressBar ProgressBar;

		public HealthBar HealthBar;

		public GameObject ResourceVisualizer;

		public Image RegionCellBlocked;

		public RectTransform PriorityOverlayIcon;

		public RectTransform HarvestWhenReadyOverlayIcon;

		public Assets.TableScreenAssets TableScreenWidgets;
	}

	[Serializable]
	public struct TableScreenAssets
	{
		public Material DefaultUIMaterial;

		public Material DesaturatedUIMaterial;

		public GameObject MinionPortrait;

		public GameObject GenericPortrait;

		public GameObject TogglePortrait;

		public GameObject ButtonLabel;

		public GameObject ButtonLabelWhite;

		public GameObject Label;

		public GameObject LabelHeader;

		public GameObject Checkbox;

		public GameObject BlankCell;

		public GameObject SuperCheckbox_Horizontal;

		public GameObject SuperCheckbox_Vertical;

		public GameObject Spacer;

		public GameObject NumericDropDown;

		public GameObject DropDownHeader;

		public GameObject PriorityGroupSelector;

		public GameObject PriorityGroupSelectorHeader;

		public GameObject PrioritizeRowWidget;

		public GameObject PrioritizeRowHeaderWidget;
	}
}
