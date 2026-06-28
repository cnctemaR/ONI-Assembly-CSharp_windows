using System;
using System.Collections;
using System.Collections.Generic;
using ProcGen;
using ProcGenGame;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OfflineWorldGen : KMonoBehaviour
{
	private void TrackProgress(string text)
	{
		if (this.trackProgress)
		{
			global::Debug.Log(text, null);
		}
	}

	public static bool CanLoadSave()
	{
		string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
		bool flag = WorldGen.CanLoad(activeSaveFilePath);
		if (!flag)
		{
			SaveLoader.SetActiveSaveFilePath(null);
			flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
		}
		return flag;
	}

	protected override void OnPrefabInit()
	{
		this.doWorldGen = !OfflineWorldGen.CanLoadSave();
		this.updateText = GameObject.Find("Status").GetComponent<LocText>();
		this.updateText.gameObject.SetActive(false);
		this.percentText = GameObject.Find("Percent").GetComponent<LocText>();
		this.percentText.gameObject.SetActive(false);
		this.doWorldGen |= this.debug;
		if (this.doWorldGen)
		{
			GameObject.Find("Title").GetComponent<LocText>().text = UI.FRONTEND.WORLDGENSCREEN.TITLE.ToString();
			GameObject.Find("MainText").GetComponent<LocText>().text = UI.WORLDGEN.CHOOSEWORLDSIZE.ToString();
			for (int i = 0; i < this.validDimensions.Length; i++)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab);
				gameObject.SetActive(true);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(this.buttonRoot);
				component.localScale = Vector3.one;
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				OfflineWorldGen.ValidDimensions validDimensions = this.validDimensions[i];
				componentInChildren.text = validDimensions.name.ToString();
				int idx = i;
				KButton component2 = gameObject.GetComponent<KButton>();
				component2.onClick += delegate
				{
					this.DoWorldGen(idx);
					this.ToggleGenerationUI();
				};
			}
			if (this.validDimensions.Length == 1)
			{
				this.DoWorldGen(0);
				this.ToggleGenerationUI();
			}
			if (KPlayerPrefs.GetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, 0) != 0)
			{
				this.InitSeeds();
				GameObject.Find("Seed").GetComponent<LocText>().text = UI.WORLDGEN.USING_PLAYER_SEED.ToString() + this.worldSeed.ToString();
			}
		}
		else
		{
			GameObject.Find("Title").GetComponent<LocText>().text = UI.FRONTEND.WORLDGENSCREEN.LOADINGGAME.ToString();
			GameObject.Find("MainText").SetActive(false);
			this.currentConvertedCurrentStage = UI.WORLDGEN.COMPLETE.key;
			this.currentPercent = 100f;
			this.updateText.gameObject.SetActive(false);
			this.percentText.gameObject.SetActive(false);
			this.RemoveButtons();
		}
		if (UpdateManager.instance)
		{
			UpdateManager.instance.enabled = false;
		}
		this.buttonPrefab.SetActive(false);
	}

	private void ToggleGenerationUI()
	{
		this.percentText.gameObject.SetActive(true);
		this.updateText.gameObject.SetActive(true);
		GameObject.Find("Title").GetComponent<LocText>().text = UI.FRONTEND.WORLDGENSCREEN.GENERATINGWORLD.ToString();
		if (this.titleText != null && this.titleText.gameObject != null)
		{
			this.titleText.gameObject.SetActive(false);
		}
		if (this.buttonRoot != null && this.buttonRoot.gameObject != null)
		{
			this.buttonRoot.gameObject.SetActive(false);
		}
		if (this.mainText != null && this.mainText.gameObject != null)
		{
			this.mainText.SetActive(false);
		}
	}

	private void ChooseBaseLocation(VoronoiNode startNode)
	{
		WorldGen.ChooseBaseLocation(startNode);
		this.DoRenderWorld();
		this.RemoveLocationButtons();
	}

	private void ShowStartingLocationChoices()
	{
		if (this.titleText != null)
		{
			this.titleText.text = "Choose Starting Location";
		}
		this.startNodes = WorldGen.WorldLayout.GetStartNodes();
		this.startNodes.Shuffle<VoronoiNode>();
		if (this.startNodes.Count > 0)
		{
			this.ChooseBaseLocation(this.startNodes[0]);
		}
		else
		{
			List<SubWorld> list = new List<SubWorld>();
			int i = 0;
			while (i < this.startNodes.Count)
			{
				VoronoiTree voronoiTree = this.startNodes[i] as VoronoiTree;
				if (voronoiTree != null)
				{
					goto IL_00BA;
				}
				voronoiTree = WorldGen.GetOverworldForNode(this.startNodes[i] as VoronoiLeaf);
				if (voronoiTree != null)
				{
					goto IL_00BA;
				}
				IL_01DB:
				i++;
				continue;
				IL_00BA:
				SubWorld subWorldForNode = WorldGen.GetSubWorldForNode(voronoiTree);
				if (subWorldForNode == null || list.Contains(subWorldForNode))
				{
					goto IL_01DB;
				}
				list.Add(subWorldForNode);
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.locationButtonPrefab);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(this.chooseLocationPanel);
				component.localScale = Vector3.one;
				Text componentInChildren = gameObject.GetComponentInChildren<Text>();
				SubWorld subWorld = null;
				VoronoiTree voronoiTree2 = this.startNodes[i].parent;
				while (subWorld == null && voronoiTree2 != null)
				{
					subWorld = WorldGen.GetSubWorldForNode(voronoiTree2);
					if (subWorld == null)
					{
						voronoiTree2 = voronoiTree2.parent;
					}
				}
				TagSet tagSet = new TagSet(this.startNodes[i].tags);
				tagSet.Remove(WorldGenTags.Feature);
				tagSet.Remove(WorldGenTags.StartLocation);
				tagSet.Remove(WorldGenTags.IgnoreCaveOverride);
				componentInChildren.text = tagSet.ToString();
				int idx = i;
				Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
				buttonClickedEvent.AddListener(delegate
				{
					this.ChooseBaseLocation(this.startNodes[idx]);
				});
				Button component2 = gameObject.GetComponent<Button>();
				component2.onClick = buttonClickedEvent;
				goto IL_01DB;
			}
		}
	}

	private void RemoveLocationButtons()
	{
		int childCount = this.chooseLocationPanel.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = this.chooseLocationPanel.GetChild(i);
			global::UnityEngine.Object.Destroy(child.gameObject);
		}
		if (this.titleText != null && this.titleText.gameObject != null)
		{
			global::UnityEngine.Object.DestroyImmediate(this.titleText.gameObject);
		}
	}

	private bool UpdateProgress(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage)
	{
		if (this.currentStage != stage)
		{
			this.currentStage = stage;
		}
		if (this.currentStringKeyRoot.Hash != stringKeyRoot.Hash)
		{
			this.currentConvertedCurrentStage = stringKeyRoot;
			this.currentStringKeyRoot = stringKeyRoot;
		}
		else
		{
			int num = (int)completePercent / 10;
			LocString locString = this.convertList.Find((LocString s) => s.key.Hash == stringKeyRoot.Hash);
			if (num != 0 && locString != null)
			{
				this.currentConvertedCurrentStage = new StringKey(locString.key.String + num.ToString());
			}
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = WorldGenProgressStages.StageWeights[(int)stage].Value * completePercent;
		for (int i = 0; i < WorldGenProgressStages.StageWeights.Length; i++)
		{
			num3 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			if (i < (int)this.currentStage)
			{
				num2 += WorldGenProgressStages.StageWeights[i].Value * 100f;
			}
		}
		float num5 = 100f * ((num2 + num4) / num3);
		this.currentPercent = num5;
		return !this.shouldStop;
	}

	private void Update()
	{
		if (this.loadTriggered)
		{
			return;
		}
		if (this.currentConvertedCurrentStage.String == null)
		{
			return;
		}
		this.updateText.text = Strings.Get(this.currentConvertedCurrentStage.String);
		if (!this.debug && this.currentConvertedCurrentStage.Hash == UI.WORLDGEN.COMPLETE.key.Hash && this.currentPercent >= 100f)
		{
			if (KCrashReporter.terminateOnError && ReportErrorDialog.hasCrash)
			{
				return;
			}
			this.percentText.text = string.Empty;
			this.loadTriggered = true;
			App.LoadScene(this.mainGameLevel);
			return;
		}
		else
		{
			if (this.currentPercent < 0f)
			{
				this.percentText.text = UI.WORLDGEN.RESTARTING.ToString();
				this.loadTriggered = true;
				Sim.Shutdown();
				App.LoadScene(this.frontendGameLevel);
				return;
			}
			if (this.currentPercent > 0f && !this.percentText.gameObject.activeSelf)
			{
				this.percentText.gameObject.SetActive(true);
			}
			this.percentText.text = this.currentPercent.ToString("N1");
			if (this.firstPassGeneration)
			{
				this.generateThreadComplete = this.world.IsGenerateComplete();
				if (!this.generateThreadComplete)
				{
					this.renderThreadComplete = false;
				}
			}
			if (this.secondPassGeneration)
			{
				this.renderThreadComplete = this.world.IsRenderComplete();
			}
			if (!this.shownStartingLocations && this.firstPassGeneration && this.generateThreadComplete)
			{
				this.shownStartingLocations = true;
				this.ShowStartingLocationChoices();
			}
			if (this.renderThreadComplete)
			{
				int num = 0;
				num++;
			}
			return;
		}
	}

	private void RemoveButtons()
	{
		int childCount = this.buttonRoot.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = this.buttonRoot.GetChild(i);
			global::UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	private void DoWorldGen(int selectedDimension)
	{
		this.RemoveButtons();
		Hashtable hashtable = new Hashtable();
		ElementLoader.Load(ref hashtable, this.simElementsSolidsFile.text, this.simElementsLiquidsFile.text, this.simElementsGasesFile.text, null);
		OfflineWorldGen.ValidDimensions validDimensions = this.validDimensions[selectedDimension];
		GridSettings.Reset(validDimensions.width, validDimensions.height);
		this.DoWordGenInitialise();
	}

	public static void SetSeed(int seed)
	{
		KPlayerPrefs.SetInt(OfflineWorldGen.WORLD_SEED_KEY, seed);
		KPlayerPrefs.SetInt(OfflineWorldGen.LAYOUT_SEED_KEY, seed);
		KPlayerPrefs.SetInt(OfflineWorldGen.TERRAIN_SEED_KEY, seed);
		KPlayerPrefs.SetInt(OfflineWorldGen.NOISE_SEED_KEY, seed);
	}

	public static void RemoveSeeds()
	{
		KPlayerPrefs.DeleteKey(OfflineWorldGen.WORLD_SEED_KEY);
		KPlayerPrefs.DeleteKey(OfflineWorldGen.LAYOUT_SEED_KEY);
		KPlayerPrefs.DeleteKey(OfflineWorldGen.TERRAIN_SEED_KEY);
		KPlayerPrefs.DeleteKey(OfflineWorldGen.NOISE_SEED_KEY);
	}

	private void InitSeeds()
	{
		this.worldSeed = KPlayerPrefs.GetInt(OfflineWorldGen.WORLD_SEED_KEY, -1);
		this.layoutSeed = KPlayerPrefs.GetInt(OfflineWorldGen.LAYOUT_SEED_KEY, -1);
		this.terrainSeed = KPlayerPrefs.GetInt(OfflineWorldGen.TERRAIN_SEED_KEY, -1);
		this.noiseSeed = KPlayerPrefs.GetInt(OfflineWorldGen.NOISE_SEED_KEY, -1);
	}

	private void DoWordGenInitialise()
	{
		WorldGen.LoadSettings();
		if (KPlayerPrefs.GetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, 0) != 0)
		{
			global::Debug.Log("Using player defined seed", null);
			this.InitSeeds();
		}
		this.world.Initialise(new WorldGen.OfflineCallbackFunction(this.UpdateProgress), this.worldSeed, this.layoutSeed, this.terrainSeed, this.noiseSeed);
		this.firstPassGeneration = true;
		this.world.GenerateOfflineThreaded();
	}

	private void DoRenderWorld()
	{
		this.firstPassGeneration = false;
		this.secondPassGeneration = true;
		this.world.RenderWorldThreaded();
	}

	public TextAsset simElementsSolidsFile;

	public TextAsset simElementsLiquidsFile;

	public TextAsset simElementsGasesFile;

	[SerializeField]
	private RectTransform buttonRoot;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private RectTransform chooseLocationPanel;

	[SerializeField]
	private GameObject locationButtonPrefab;

	private OfflineWorldGen.ValidDimensions[] validDimensions = new OfflineWorldGen.ValidDimensions[]
	{
		new OfflineWorldGen.ValidDimensions
		{
			width = 256,
			height = 384,
			name = UI.FRONTEND.WORLDGENSCREEN.SIZES.STANDARD.key
		}
	};

	public string frontendGameLevel = "frontend";

	public string mainGameLevel = "backend";

	private bool shouldStop;

	private StringKey currentConvertedCurrentStage;

	private float currentPercent;

	public bool debug;

	public GameObject mainText;

	private bool trackProgress = true;

	private bool doWorldGen;

	private LocText updateText;

	private LocText percentText;

	[SerializeField]
	private Text titleText;

	private WorldGen world = new WorldGen();

	private List<VoronoiNode> startNodes;

	private StringKey currentStringKeyRoot;

	private static LocString[] convertableLocs = new LocString[]
	{
		UI.WORLDGEN.SETTLESIM,
		UI.WORLDGEN.BORDERS,
		UI.WORLDGEN.PROCESSING,
		UI.WORLDGEN.COMPLETELAYOUT,
		UI.WORLDGEN.WORLDLAYOUT,
		UI.WORLDGEN.GENERATENOISE,
		UI.WORLDGEN.BUILDNOISESOURCE,
		UI.WORLDGEN.GENERATESOLARSYSTEM
	};

	private List<LocString> convertList = new List<LocString>(OfflineWorldGen.convertableLocs);

	private WorldGenProgressStages.Stages currentStage;

	private bool loadTriggered;

	private bool shownStartingLocations;

	private bool generateThreadComplete;

	private bool renderThreadComplete;

	private bool firstPassGeneration;

	private bool secondPassGeneration;

	public static string USE_WORLD_SEED_KEY = "UseWorldSeedKey";

	public static string WORLD_SEED_KEY = "WorldSeedKey";

	public static string LAYOUT_SEED_KEY = "LayoutSeedKey";

	public static string TERRAIN_SEED_KEY = "TerrainSeedKey";

	public static string NOISE_SEED_KEY = "NoiseSeedKey";

	private int worldSeed = -1;

	private int layoutSeed = -1;

	private int terrainSeed = -1;

	private int noiseSeed = -1;

	[Serializable]
	private struct ValidDimensions
	{
		public int width;

		public int height;

		public StringKey name;
	}
}
