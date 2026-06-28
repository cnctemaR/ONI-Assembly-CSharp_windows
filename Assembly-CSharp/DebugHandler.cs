using System;
using System.IO;
using UnityEngine;

public class DebugHandler : IInputHandler
{
	public DebugHandler()
	{
		DebugHandler.enabled = File.Exists(Path.Combine(Application.dataPath, "debug_enable.txt"));
	}

	public static bool enabled { get; private set; }

	public KInputHandler inputHandler { get; set; }

	public static int GetMouseCell()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
		return Grid.PosToCell(vector);
	}

	public static Vector3 GetMousePos()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
		return Camera.main.ScreenToWorldPoint(mousePosition);
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.MinionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Entities), null);
		gameObject.name = EntityPrefabs.Instance.MinionPrefab.name;
		Vector3 vector = Grid.CellToPosCBC(DebugHandler.GetMouseCell(), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		MinionStartingStats minionStartingStats = new MinionStartingStats(false);
		minionStartingStats.Apply(gameObject);
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (!DebugHandler.enabled)
		{
			return;
		}
		if (e.TryConsume(global::Action.DebugSpawnMinion))
		{
			this.SpawnMinion();
		}
		else if (e.TryConsume(global::Action.DebugSpawnStressTest))
		{
			for (int i = 0; i < 60; i++)
			{
				this.SpawnMinion();
			}
		}
		else if (e.TryConsume(global::Action.DebugSuperTestMode))
		{
			if (!this.superTestMode)
			{
				Time.timeScale = 15f;
				this.superTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.superTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugUltraTestMode))
		{
			if (!this.ultraTestMode)
			{
				Time.timeScale = 30f;
				this.ultraTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.ultraTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugSlowTestMode))
		{
			if (!this.slowTestMode)
			{
				Time.timeScale = 0.06f;
				this.slowTestMode = true;
			}
			else
			{
				Time.timeScale = 1f;
				this.slowTestMode = false;
			}
		}
		else if (e.TryConsume(global::Action.DebugDig))
		{
			int mouseCell = DebugHandler.GetMouseCell();
			SimMessages.Dig(mouseCell, -1);
		}
		else if (e.TryConsume(global::Action.DebugInstantBuildMode))
		{
			DebugHandler.InstantBuildMode = !DebugHandler.InstantBuildMode;
			if (PlanScreen.Instance != null)
			{
				PlanScreen.Instance.Refresh();
			}
			if (BuildMenu.Instance != null)
			{
				BuildMenu.Instance.Refresh();
			}
			if (OverlayMenu.Instance != null)
			{
				OverlayMenu.Instance.Refresh();
			}
			ConsumerManager.instance.RefreshDiscovered(null);
			if (ManagementMenu.Instance != null)
			{
				ManagementMenu.Instance.CheckResearch(null);
			}
		}
		else if (e.TryConsume(global::Action.DebugExplosion))
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
			Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
			GameUtil.CreateExplosion(vector);
		}
		else if (e.TryConsume(global::Action.DebugDiscoverAllElements))
		{
			foreach (Element element in ElementLoader.elements)
			{
				WorldInventory.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
			}
		}
		else if (e.TryConsume(global::Action.DebugToggleUI))
		{
			DebugHandler.ToggleScreenshotMode();
		}
		else if (e.TryConsume(global::Action.SreenShot1x))
		{
			string text = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			Application.CaptureScreenshot(text, 1);
		}
		else if (e.TryConsume(global::Action.SreenShot2x))
		{
			string text2 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			Application.CaptureScreenshot(text2, 2);
		}
		else if (e.TryConsume(global::Action.SreenShot8x))
		{
			string text3 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			Application.CaptureScreenshot(text3, 8);
		}
		else if (e.TryConsume(global::Action.SreenShot32x))
		{
			string text4 = Path.ChangeExtension(SaveLoader.GetActiveSaveFilePath(), ".png");
			Application.CaptureScreenshot(text4, 32);
		}
		else if (e.TryConsume(global::Action.DebugCellInfo))
		{
			DebugHandler.DebugCellInfo = !DebugHandler.DebugCellInfo;
		}
		else if (e.TryConsume(global::Action.DebugToggle))
		{
			PropertyTextures.FogOfWarScale = 1f - PropertyTextures.FogOfWarScale;
			DebugHandler.FreeCameraMode = !DebugHandler.FreeCameraMode;
			if (Game.Instance != null)
			{
				Game.Instance.UpdateGameActiveRegion(0, 0, Grid.WidthInCells, Grid.HeightInCells);
				WorldGenSpawner.Instance.SpawnEverything();
			}
			if (DebugPaintElementScreen.Instance != null)
			{
				bool activeSelf = DebugPaintElementScreen.Instance.gameObject.activeSelf;
				DebugPaintElementScreen.Instance.gameObject.SetActive(!activeSelf);
				if (DebugElementMenu.Instance && DebugElementMenu.Instance.root.activeSelf)
				{
					DebugElementMenu.Instance.root.SetActive(false);
				}
				DebugBaseTemplateButton.Instance.gameObject.SetActive(!activeSelf);
			}
		}
		else if (e.TryConsume(global::Action.DebugCollectGarbage))
		{
			GC.Collect();
		}
		else if (e.TryConsume(global::Action.DebugInvincible))
		{
			DebugHandler.InvincibleMode = !DebugHandler.InvincibleMode;
		}
		else if (e.TryConsume(global::Action.DebugVisualTest))
		{
			Scenario.Instance.SetupVisualTest();
		}
		else if (e.TryConsume(global::Action.DebugGameplayTest))
		{
			Scenario.Instance.SetupGameplayTest();
		}
		else if (e.TryConsume(global::Action.DebugElementTest))
		{
			Scenario.Instance.SetupElementTest();
		}
		else if (e.TryConsume(global::Action.ToggleProfiler))
		{
			Sim.SIM_HandleMessage(-409964931, 0, null);
		}
		else if (e.TryConsume(global::Action.DebugRefreshNavCell))
		{
			Pathfinding.Instance.RefreshNavCell(DebugHandler.GetMouseCell());
		}
		else if (e.TryConsume(global::Action.DebugToggleSelectInEditor))
		{
			DebugHandler.SetSelectInEditor(!DebugHandler.SelectInEditor);
		}
		else if (e.TryConsume(global::Action.DebugGotoTarget))
		{
			global::Debug.Log("Debug GoTo", null);
			Game.Instance.Trigger(775300118, null);
			foreach (Brain brain in Components.Brains)
			{
				DebugGoToMonitor component = brain.GetComponent<DebugGoToMonitor>();
				if (component != null)
				{
					component.GoToCursor();
				}
			}
		}
		else if (e.TryConsume(global::Action.DebugTeleport))
		{
			KSelectable selected = SelectTool.Instance.selected;
			if (selected != null)
			{
				int mouseCell2 = DebugHandler.GetMouseCell();
				if (!Grid.IsValidCell(mouseCell2))
				{
					return;
				}
				selected.transform.SetPosition(Grid.CellToPosCBC(mouseCell2, Grid.SceneLayer.Move));
			}
		}
		else if (!e.TryConsume(global::Action.DebugPlace))
		{
			if (!e.TryConsume(global::Action.DebugSelectMaterial))
			{
				if (e.TryConsume(global::Action.DebugNotification))
				{
					Tutorial.Instance.DebugNotification();
				}
				else if (e.TryConsume(global::Action.DebugNotificationMessage))
				{
					Tutorial.Instance.DebugNotificationMessage();
				}
				else if (e.TryConsume(global::Action.DebugSuperSpeed))
				{
					SpeedControlScreen.Instance.ToggleRidiculousSpeed();
				}
				else if (e.TryConsume(global::Action.DebugGameStep))
				{
					SpeedControlScreen.Instance.DebugStepFrame();
				}
				else if (e.TryConsume(global::Action.DebugSimStep))
				{
					Game.Instance.ForceSimStep();
				}
				else if (e.TryConsume(global::Action.DebugToggleMusic))
				{
					AudioDebug.Get().ToggleMusic();
				}
				else if (e.TryConsume(global::Action.DebugRiverTest))
				{
					Scenario.Instance.SetupRiverTest();
				}
				else if (e.TryConsume(global::Action.DebugTileTest))
				{
					Scenario.Instance.SetupTileTest();
				}
				else if (e.TryConsume(global::Action.DebugForceLightEverywhere))
				{
					PropertyTextures.instance.ForceLightEverywhere = !PropertyTextures.instance.ForceLightEverywhere;
				}
				else if (e.TryConsume(global::Action.DebugPathFinding))
				{
					DebugHandler.DebugPathFinding = !DebugHandler.DebugPathFinding;
					global::Debug.Log("DebugPathFinding=" + DebugHandler.DebugPathFinding, null);
				}
				else if (!e.TryConsume(global::Action.DebugFocus))
				{
					if (e.TryConsume(global::Action.DebugReportBug))
					{
						int num = 0;
						string validSaveFilename;
						for (;;)
						{
							validSaveFilename = SaveScreen.GetValidSaveFilename("bug_report_savefile_" + num.ToString());
							if (!File.Exists(validSaveFilename))
							{
								break;
							}
							num++;
						}
						string text5 = "No save file (front end)";
						if (SaveLoader.Instance != null)
						{
							text5 = SaveLoader.Instance.Save(validSaveFilename, false, false);
						}
						KCrashReporter.ReportBug("Bug Report", text5);
					}
					else if (e.TryConsume(global::Action.DebugReloadLevel))
					{
						global::Debug.Log("Reloading Level.", null);
						SaveLoader.Instance.InitialSave();
						LoadScreen.ForceStopGame();
						SaveLoader.SetActiveSaveFilePath(SaveLoader.GetAutosaveFilePath());
						LoadingOverlay.Load(delegate
						{
							App.LoadScene("frontend");
						});
					}
					else if (e.TryConsume(global::Action.DebugTriggerException))
					{
						string text6 = Guid.NewGuid().ToString();
						KCrashReporter.ReportError("Debug crash with random stack", text6, null, ScreenPrefabs.Instance.ConfirmDialogScreen, string.Empty);
					}
					else if (e.TryConsume(global::Action.DebugTriggerError))
					{
						global::Debug.LogError("Oooops! Testing error!", null);
					}
					else if (e.TryConsume(global::Action.DebugDumpGarbageReferences))
					{
						GarbageProfiler.DebugDumpGarbageStats();
					}
					else if (e.TryConsume(global::Action.DebugDumpEventData))
					{
						KObjectManager.Instance.DumpEventData();
					}
					else if (!e.TryConsume(global::Action.DebugDumpSceneParitionerLeakData))
					{
						if (e.TryConsume(global::Action.DebugCrashSim))
						{
							Sim.SIM_DebugCrash();
						}
						else if (e.TryConsume(global::Action.DebugNextCall))
						{
							DebugHandler.DebugNextCall = true;
						}
					}
				}
			}
		}
		if (e.Consumed && Game.Instance != null)
		{
			Game.Instance.debugWasUsed = true;
			KCrashReporter.debugWasUsed = true;
		}
	}

	public static void SetSelectInEditor(bool select_in_editor)
	{
	}

	public static void ToggleScreenshotMode()
	{
		DebugHandler.SetHideUI(!DebugHandler.HideUI);
		DebugHandler.FreeCameraMode = !DebugHandler.FreeCameraMode;
	}

	public static void SetHideUI(bool hide)
	{
		DebugHandler.HideUI = hide;
		foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
		{
			CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
			}
			if (DebugHandler.HideUI)
			{
				canvasGroup.alpha = 0f;
			}
			else
			{
				canvasGroup.alpha = 1f;
			}
		}
	}

	public static bool InstantBuildMode;

	public static bool FreeCameraMode;

	public static bool InvincibleMode;

	public static bool SelectInEditor;

	public static bool DebugPathFinding;

	public static bool HideUI;

	public static bool DebugCellInfo;

	public static bool DebugNextCall;

	private bool superTestMode;

	private bool ultraTestMode;

	private bool slowTestMode;

	public enum PaintMode
	{
		None,
		Element,
		Hot,
		Cold
	}
}
