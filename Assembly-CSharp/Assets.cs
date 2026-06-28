using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Assets : KMonoBehaviour, ISerializationCallbackReceiver
{
	protected override void OnPrefabInit()
	{
		RecipeManager.Destroy();
		Assets.AnimMaterial = this.AnimMaterialAsset;
		Assets.Prefabs = new List<KPrefabID>(this.PrefabAssets.Where<KPrefabID>((KPrefabID x) => x != null));
		Assets.RegionPrefabs.Clear();
		Assets.PrefabsByTag.Clear();
		Assets.PrefabsByAdditionalTags.Clear();
		Assets.CountableTags.Clear();
		Assets.Sprites = this.SpriteAssets.Where<Sprite>((Sprite x) => x != null).ToArray<Sprite>();
		Assets.TintedSprites = this.TintedSpriteAssets.Where<TintedSprite>((TintedSprite x) => x != null && x.sprite != null).ToArray<TintedSprite>();
		Assets.Materials = this.MaterialAssets.Where<Material>((Material x) => x != null).ToArray<Material>();
		Assets.Textures = this.TextureAssets.Where<Texture2D>((Texture2D x) => x != null).ToArray<Texture2D>();
		Assets.TextureAtlases = this.TextureAtlasAssets.Where<TextureAtlas>((TextureAtlas x) => x != null).ToArray<TextureAtlas>();
		Assets.BlockTileDecorInfos = this.BlockTileDecorInfoAssets.Where<BlockTileDecorInfo>((BlockTileDecorInfo x) => x != null).ToArray<BlockTileDecorInfo>();
		Assets.Controllers = this.BuildingControllers.Where<RuntimeAnimatorController>((RuntimeAnimatorController x) => x != null).ToArray<RuntimeAnimatorController>();
		Assets.Anims = this.AnimAssets.Where<KAnimFile>((KAnimFile x) => x != null).ToArray<KAnimFile>();
		Assets.UIPrefabs = this.UIPrefabAssets;
		Assets.defaultPhysicsMaterial = this.defaultPhysicsMaterialAsset;
		Assets.DebugFont = this.DebugFontAsset;
		this.SubstanceListHookup();
		Assets.BaseTemplate = this.BaseTemplateAsset;
		Assets.BuildingDefs = new BuildingDef[0];
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
				HashedString hashedString = kanimFile.name;
				Assets.AnimTable[hashedString] = kanimFile;
			}
		}
		GameEntityTypeSet.Destroy();
		this.entityTypeSet = GameEntityTypeSet.Instance;
		LegacyModMain.Load();
	}

	private static void TryAddCountableTag(KPrefabID prefab)
	{
		PrimaryElement component = prefab.GetComponent<PrimaryElement>();
		if (component != null && component.CountableUnits)
		{
			Assets.AddCountableTag(prefab.PrefabTag);
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
		ElementsAudio.Instance.LoadData(this.elementAudio.text);
		ElementLoader.Load(ref hashtable, this.simElementsSolidsFile.text, this.simElementsLiquidsFile.text, this.simElementsGasesFile.text, this.substanceTable);
		Assets.SubstanceTable = this.substanceTable;
	}

	public static string GetSimpleSoundEventName(string path)
	{
		int num = path.LastIndexOf('/');
		return (num == -1) ? path : path.Substring(num + 1);
	}

	private static Def GetDef(Def[] defs, string prefab_id)
	{
		int num = defs.Length;
		for (int i = 0; i < num; i++)
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
		return (BuildingDef)Assets.GetDef(Assets.BuildingDefs, prefab_id);
	}

	public static BaseTemplate GetBaseTemplate()
	{
		return Assets.BaseTemplate;
	}

	public static TintedSprite GetTintedSprite(string name)
	{
		TintedSprite tintedSprite = null;
		if (Assets.TintedSprites != null)
		{
			for (int i = 0; i < Assets.TintedSprites.Length; i++)
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

	public static Sprite GetSprite(string name)
	{
		Sprite sprite = null;
		if (Assets.Sprites != null)
		{
			for (int i = 0; i < Assets.Sprites.Length; i++)
			{
				if (Assets.Sprites[i].name == name)
				{
					sprite = Assets.Sprites[i];
					break;
				}
			}
		}
		return sprite;
	}

	public static Texture2D GetTexture(string name)
	{
		Texture2D texture2D = null;
		if (Assets.Textures != null)
		{
			for (int i = 0; i < Assets.Textures.Length; i++)
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

	public static void AddRegionPrefab(KPrefabID prefab)
	{
		if (prefab == null)
		{
			return;
		}
		Assets.RegionPrefabs.Add(prefab.gameObject);
		Assets.AddPrefab(prefab);
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
			Debug.LogWarning("Tried loading prefab with duplicate tag, ignoring: " + prefab.PrefabTag);
		}
		Assets.PrefabsByTag[prefab.PrefabTag] = prefab;
		for (int i = 0; i < prefab.Tags.Length; i++)
		{
			if (!Assets.PrefabsByAdditionalTags.ContainsKey(prefab.Tags[i]))
			{
				Assets.PrefabsByAdditionalTags[prefab.Tags[i]] = new List<KPrefabID>();
			}
			Assets.PrefabsByAdditionalTags[prefab.Tags[i]].Add(prefab);
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

	public static GameObject GetPrefab(Tag tag)
	{
		KPrefabID kprefabID = null;
		Assets.PrefabsByTag.TryGetValue(tag, out kprefabID);
		if (kprefabID == null)
		{
			Debug.LogWarning("Missing prefab: " + tag);
		}
		return (!(kprefabID != null)) ? null : kprefabID.gameObject;
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

	public static Assets GetInstanceEditorOnly()
	{
		Assets[] array = (Assets[])Resources.FindObjectsOfTypeAll(typeof(Assets));
		if (array == null || array.Length == 0)
		{
			return array[0];
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

	public static RuntimeAnimatorController GetController(string name)
	{
		foreach (RuntimeAnimatorController runtimeAnimatorController in Assets.Controllers)
		{
			if (runtimeAnimatorController.name == name)
			{
				return runtimeAnimatorController;
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
		Output.LogError(new object[] { "Could not find BlockTileDecorInfo named [" + name + "]" });
		return null;
	}

	public static KAnimFile GetAnim(HashedString name)
	{
		if (!name.IsValid())
		{
			Debug.LogWarning("Invalid hash name");
			return null;
		}
		KAnimFile kanimFile = null;
		Assets.AnimTable.TryGetValue(name, out kanimFile);
		if (kanimFile == null)
		{
			Debug.LogWarning("Missing Anim: [" + name.ToString() + "]. You may have to run Collect Anim on the Assets prefab");
		}
		return kanimFile;
	}

	public void OnAfterDeserialize()
	{
		this.TintedSpriteAssets = this.TintedSpriteAssets.Where<TintedSprite>((TintedSprite x) => x != null && x.sprite != null).ToArray<TintedSprite>();
		Array.Sort<TintedSprite>(this.TintedSpriteAssets, (TintedSprite a, TintedSprite b) => a.name.CompareTo(b.name));
	}

	public void OnBeforeSerialize()
	{
	}

	public static void AddBuildingDef(BuildingDef def)
	{
		Assets.BuildingDefs = Assets.BuildingDefs.Where<BuildingDef>((BuildingDef x) => x.PrefabID != def.PrefabID).ToArray<BuildingDef>();
		Assets.BuildingDefs = Assets.BuildingDefs.Append(def);
	}

	private static Action<KPrefabID> OnAddPrefab;

	public BuildingDef[] BuildingDefAssets;

	public static BuildingDef[] BuildingDefs;

	public BaseTemplate BaseTemplateAsset;

	public static BaseTemplate BaseTemplate;

	public List<KPrefabID> PrefabAssets = new List<KPrefabID>();

	public static List<KPrefabID> Prefabs = new List<KPrefabID>();

	private static HashSet<Tag> CountableTags = new HashSet<Tag>();

	public static List<GameObject> RegionPrefabs = new List<GameObject>();

	public Sprite[] SpriteAssets;

	public static Sprite[] Sprites;

	public TintedSprite[] TintedSpriteAssets;

	public static TintedSprite[] TintedSprites;

	public Texture2D[] TextureAssets;

	public static Texture2D[] Textures;

	public static TextureAtlas[] TextureAtlases;

	public TextureAtlas[] TextureAtlasAssets;

	public static Material[] Materials;

	public Material[] MaterialAssets;

	public static Shader[] Shaders;

	public Shader[] ShaderAssets;

	public static BlockTileDecorInfo[] BlockTileDecorInfos;

	public BlockTileDecorInfo[] BlockTileDecorInfoAssets;

	public RuntimeAnimatorController[] BuildingControllers;

	public static RuntimeAnimatorController[] Controllers;

	public Material AnimMaterialAsset;

	public static Material AnimMaterial;

	public Assets.UIPrefabData UIPrefabAssets;

	public static Assets.UIPrefabData UIPrefabs;

	private static Dictionary<Tag, KPrefabID> PrefabsByTag = new Dictionary<Tag, KPrefabID>();

	private static Dictionary<Tag, List<KPrefabID>> PrefabsByAdditionalTags = new Dictionary<Tag, List<KPrefabID>>();

	private static Dictionary<HashedString, KAnimFile> AnimTable = new Dictionary<HashedString, KAnimFile>();

	public PhysicsMaterial2D defaultPhysicsMaterialAsset;

	public static PhysicsMaterial2D defaultPhysicsMaterial;

	public KAnimFile[] AnimAssets;

	public static KAnimFile[] Anims;

	public Font DebugFontAsset;

	public static Font DebugFont;

	public GameEntityTypeSet entityTypeSet;

	public SubstanceTable substanceTable;

	public static SubstanceTable SubstanceTable;

	[SerializeField]
	private TextAsset simElementsSolidsFile;

	[SerializeField]
	private TextAsset simElementsLiquidsFile;

	[SerializeField]
	private TextAsset simElementsGasesFile;

	[SerializeField]
	private TextAsset elementAudio;

	public Assets.PlacementOverrideData[] PlacementOverrides;

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

		public GameObject Label;

		public GameObject LabelHeader;

		public GameObject Checkbox;

		public GameObject BlankCell;

		public GameObject SuperCheckbox_Horizontal;

		public GameObject SuperCheckbox_Vertical;

		public GameObject Spacer;
	}

	[Serializable]
	public struct PlacementOverrideData
	{
		public Tag Building;

		public Tag[] DestroyOnPlaceBuildings;
	}
}
