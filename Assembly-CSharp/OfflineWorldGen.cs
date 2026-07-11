using System;
using System.Collections.Generic;
using System.Threading;
using Klei.CustomSettings;
using ProcGen;
using ProcGenGame;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoronoiTree;

public class OfflineWorldGen : KMonoBehaviour
{
	private void TrackProgress(string text)
	{
		if (this.trackProgress)
		{
			global::Debug.Log(text);
		}
	}

	public static bool CanLoadSave()
	{
		bool flag = WorldGen.CanLoad(SaveLoader.GetActiveSaveFilePath());
		if (!flag)
		{
			SaveLoader.SetActiveSaveFilePath(null);
			flag = WorldGen.CanLoad(WorldGen.SIM_SAVE_FILENAME);
		}
		return flag;
	}

	public void Generate()
	{
		this.doWorldGen = !OfflineWorldGen.CanLoadSave();
		this.updateText.gameObject.SetActive(false);
		this.percentText.gameObject.SetActive(false);
		this.doWorldGen |= this.debug;
		if (this.doWorldGen)
		{
			this.seedText.text = string.Format(UI.WORLDGEN.USING_PLAYER_SEED, this.seed);
			this.titleText.text = UI.FRONTEND.WORLDGENSCREEN.TITLE.ToString();
			this.mainText.text = UI.WORLDGEN.CHOOSEWORLDSIZE.ToString();
			for (int i = 0; i < this.validDimensions.Length; i++)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab);
				gameObject.SetActive(true);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(this.buttonRoot);
				component.localScale = Vector3.one;
				TMP_Text componentInChildren = gameObject.GetComponentInChildren<LocText>();
				OfflineWorldGen.ValidDimensions validDimensions = this.validDimensions[i];
				componentInChildren.text = validDimensions.name.ToString();
				int idx = i;
				gameObject.GetComponent<KButton>().onClick += delegate
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
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
			this.OnResize();
		}
		else
		{
			this.titleText.text = UI.FRONTEND.WORLDGENSCREEN.LOADINGGAME.ToString();
			this.mainText.gameObject.SetActive(false);
			this.currentConvertedCurrentStage = UI.WORLDGEN.COMPLETE.key;
			this.currentPercent = 100f;
			this.updateText.gameObject.SetActive(false);
			this.percentText.gameObject.SetActive(false);
			this.RemoveButtons();
		}
		this.buttonPrefab.SetActive(false);
	}

	private void OnResize()
	{
		float canvasScale = base.GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if (this.asteriodAnim != null)
		{
			this.asteriodAnim.animScale = 0.005f * (1f / canvasScale);
		}
	}

	private void ToggleGenerationUI()
	{
		this.percentText.gameObject.SetActive(false);
		this.updateText.gameObject.SetActive(true);
		this.titleText.text = UI.FRONTEND.WORLDGENSCREEN.GENERATINGWORLD.ToString();
		if (this.titleText != null && this.titleText.gameObject != null)
		{
			this.titleText.gameObject.SetActive(false);
		}
		if (this.buttonRoot != null && this.buttonRoot.gameObject != null)
		{
			this.buttonRoot.gameObject.SetActive(false);
		}
	}

	private void ChooseBaseLocation(global::VoronoiTree.Node startNode)
	{
		this.worldGen.ChooseBaseLocation(startNode);
		this.DoRenderWorld();
		this.RemoveLocationButtons();
	}

	private void ShowStartingLocationChoices()
	{
		if (this.titleText != null)
		{
			this.titleText.text = "Choose Starting Location";
		}
		this.startNodes = this.worldGen.WorldLayout.GetStartNodes();
		this.startNodes.Shuffle<global::VoronoiTree.Node>();
		if (this.startNodes.Count == 0)
		{
			this.DoRenderWorld();
			this.RemoveLocationButtons();
			return;
		}
		if (this.startNodes.Count > 0)
		{
			this.ChooseBaseLocation(this.startNodes[0]);
			return;
		}
		List<SubWorld> list = new List<SubWorld>();
		int i = 0;
		while (i < this.startNodes.Count)
		{
			Tree tree = this.startNodes[i] as Tree;
			if (tree != null)
			{
				goto IL_00CC;
			}
			tree = this.worldGen.GetOverworldForNode(this.startNodes[i] as Leaf);
			if (tree != null)
			{
				goto IL_00CC;
			}
			IL_01E1:
			i++;
			continue;
			IL_00CC:
			SubWorld subWorldForNode = this.worldGen.GetSubWorldForNode(tree);
			if (subWorldForNode != null && !list.Contains(subWorldForNode))
			{
				list.Add(subWorldForNode);
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.locationButtonPrefab);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.SetParent(this.chooseLocationPanel);
				component.localScale = Vector3.one;
				Text componentInChildren = gameObject.GetComponentInChildren<Text>();
				SubWorld subWorld = null;
				Tree tree2 = this.startNodes[i].parent;
				while (subWorld == null && tree2 != null)
				{
					subWorld = this.worldGen.GetSubWorldForNode(tree2);
					if (subWorld == null)
					{
						tree2 = tree2.parent;
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
				gameObject.GetComponent<Button>().onClick = buttonClickedEvent;
				goto IL_01E1;
			}
			goto IL_01E1;
		}
	}

	private void RemoveLocationButtons()
	{
		for (int i = this.chooseLocationPanel.childCount - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.chooseLocationPanel.GetChild(i).gameObject);
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
		this.errorMutex.WaitOne();
		int count = this.errors.Count;
		this.errorMutex.ReleaseMutex();
		if (count > 0)
		{
			this.DoExitFlow();
			return;
		}
		this.updateText.text = Strings.Get(this.currentConvertedCurrentStage.String);
		if (!this.debug && this.currentConvertedCurrentStage.Hash == UI.WORLDGEN.COMPLETE.key.Hash && this.currentPercent >= 100f)
		{
			if (KCrashReporter.terminateOnError && ReportErrorDialog.hasCrash)
			{
				return;
			}
			this.percentText.text = "";
			this.loadTriggered = true;
			App.LoadScene(this.mainGameLevel);
			return;
		}
		else
		{
			if (this.currentPercent < 0f)
			{
				this.DoExitFlow();
				return;
			}
			if (this.currentPercent > 0f && !this.percentText.gameObject.activeSelf)
			{
				this.percentText.gameObject.SetActive(false);
			}
			this.percentText.text = GameUtil.GetFormattedPercent(this.currentPercent, GameUtil.TimeSlice.None);
			this.meterAnim.SetPositionPercent(this.currentPercent / 100f);
			if (this.firstPassGeneration)
			{
				this.generateThreadComplete = this.worldGen.IsGenerateComplete();
				if (!this.generateThreadComplete)
				{
					this.renderThreadComplete = false;
				}
			}
			if (this.secondPassGeneration)
			{
				this.renderThreadComplete = this.worldGen.IsRenderComplete();
			}
			if (!this.shownStartingLocations && this.firstPassGeneration && this.generateThreadComplete)
			{
				this.shownStartingLocations = true;
				this.ShowStartingLocationChoices();
			}
			if (this.renderThreadComplete)
			{
				int num = 0 + 1;
			}
			return;
		}
	}

	private void DisplayErrors()
	{
		this.errorMutex.WaitOne();
		if (this.errors.Count > 0)
		{
			foreach (OfflineWorldGen.ErrorInfo errorInfo in this.errors)
			{
				global::Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, FrontEndManager.Instance.gameObject, true).PopupConfirmDialog(errorInfo.errorDesc, new global::System.Action(this.OnConfirmExit), null, null, null, null, null, null, null, true);
			}
		}
		this.errorMutex.ReleaseMutex();
	}

	private void DoExitFlow()
	{
		if (this.startedExitFlow)
		{
			return;
		}
		this.startedExitFlow = true;
		this.percentText.text = UI.WORLDGEN.RESTARTING.ToString();
		this.loadTriggered = true;
		Sim.Shutdown();
		this.DisplayErrors();
	}

	private void OnConfirmExit()
	{
		App.LoadScene(this.frontendGameLevel);
	}

	private void RemoveButtons()
	{
		for (int i = this.buttonRoot.childCount - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.buttonRoot.GetChild(i).gameObject);
		}
	}

	private void DoWorldGen(int selectedDimension)
	{
		this.RemoveButtons();
		this.DoWorldGenInitialize();
	}

	private void DoWorldGenInitialize()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World);
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		this.seed = int.Parse(currentQualitySetting2.id);
		List<string> randomTraits = SettingsCache.GetRandomTraits(this.seed);
		this.worldGen = new WorldGen(currentQualitySetting.id, randomTraits, true);
		Vector2I worldsize = this.worldGen.Settings.world.worldsize;
		GridSettings.Reset(worldsize.x, worldsize.y);
		this.worldGen.Initialise(new WorldGen.OfflineCallbackFunction(this.UpdateProgress), new Action<OfflineWorldGen.ErrorInfo>(this.OnError), this.seed, this.seed, this.seed, this.seed);
		this.firstPassGeneration = true;
		this.worldGen.GenerateOfflineThreaded();
	}

	private void DoRenderWorld()
	{
		this.firstPassGeneration = false;
		this.secondPassGeneration = true;
		this.worldGen.RenderWorldThreaded();
	}

	private void OnError(OfflineWorldGen.ErrorInfo error)
	{
		this.errorMutex.WaitOne();
		this.errors.Add(error);
		this.errorMutex.ReleaseMutex();
	}

	[SerializeField]
	private RectTransform buttonRoot;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private RectTransform chooseLocationPanel;

	[SerializeField]
	private GameObject locationButtonPrefab;

	private const float baseScale = 0.005f;

	private Mutex errorMutex = new Mutex();

	private List<OfflineWorldGen.ErrorInfo> errors = new List<OfflineWorldGen.ErrorInfo>();

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

	private bool trackProgress = true;

	private bool doWorldGen;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText mainText;

	[SerializeField]
	private LocText updateText;

	[SerializeField]
	private LocText percentText;

	[SerializeField]
	private LocText seedText;

	[SerializeField]
	private KBatchedAnimController meterAnim;

	[SerializeField]
	private KBatchedAnimController asteriodAnim;

	private WorldGen worldGen;

	private List<global::VoronoiTree.Node> startNodes;

	private StringKey currentStringKeyRoot;

	private List<LocString> convertList = new List<LocString>
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

	private WorldGenProgressStages.Stages currentStage;

	private bool loadTriggered;

	private bool shownStartingLocations;

	private bool startedExitFlow;

	private bool generateThreadComplete;

	private bool renderThreadComplete;

	private bool firstPassGeneration;

	private bool secondPassGeneration;

	private int seed;

	public struct ErrorInfo
	{
		public string errorDesc;

		public Exception exception;
	}

	[Serializable]
	private struct ValidDimensions
	{
		public int width;

		public int height;

		public StringKey name;
	}
}
