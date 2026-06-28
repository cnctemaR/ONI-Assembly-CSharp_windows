using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class GameInputMapping
{
	public static HashSet<KeyCode> GetKeyCodes()
	{
		HashSet<KeyCode> hashSet = new HashSet<KeyCode>();
		foreach (BindingEntry bindingEntry in GameInputMapping.GetBindingEntries())
		{
			if (bindingEntry.mKeyCode < KKeyCode.KleiKeys)
			{
				hashSet.Add((KeyCode)bindingEntry.mKeyCode);
			}
		}
		hashSet.Add(KeyCode.LeftAlt);
		hashSet.Add(KeyCode.LeftControl);
		hashSet.Add(KeyCode.LeftShift);
		hashSet.Add(KeyCode.CapsLock);
		return hashSet;
	}

	public static HashSet<string> GetAxis()
	{
		return new HashSet<string> { "Mouse X", "Mouse Y", "Mouse ScrollWheel" };
	}

	public static BindingEntry[] GetBindingEntries()
	{
		return GameInputMapping.KeyBindings;
	}

	public static BindingEntry FindEntry(global::Action mAction)
	{
		foreach (BindingEntry bindingEntry in GameInputMapping.KeyBindings)
		{
			if (bindingEntry.mAction == mAction)
			{
				return bindingEntry;
			}
		}
		return GameInputMapping.KeyBindings[0];
	}

	public static bool CompareActionKeyCodes(global::Action a, global::Action b)
	{
		BindingEntry bindingEntry = GameInputMapping.FindEntry(a);
		BindingEntry bindingEntry2 = GameInputMapping.FindEntry(b);
		return bindingEntry.mKeyCode == bindingEntry2.mKeyCode && bindingEntry.mModifier == bindingEntry2.mModifier;
	}

	public static BindingEntry[] FindEntriesByKeyCode(KKeyCode keycode)
	{
		List<BindingEntry> list = new List<BindingEntry>();
		foreach (BindingEntry bindingEntry in GameInputMapping.KeyBindings)
		{
			if (bindingEntry.mKeyCode == keycode)
			{
				list.Add(bindingEntry);
			}
		}
		return list.ToArray();
	}

	private static string BindingsFilename
	{
		get
		{
			return Path.Combine(Util.RootFolder(), "keybindings.json");
		}
	}

	public static void SaveBindings()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		List<BindingEntry> list = new List<BindingEntry>();
		foreach (BindingEntry bindingEntry in GameInputMapping.KeyBindings)
		{
			bool flag = false;
			foreach (BindingEntry bindingEntry2 in GameInputMapping.DefaultBindings)
			{
				if (bindingEntry == bindingEntry2)
				{
					flag = true;
					break;
				}
			}
			if (!flag && bindingEntry.mRebindable)
			{
				list.Add(bindingEntry);
			}
		}
		if (list.Count > 0)
		{
			string text = JsonConvert.SerializeObject(list);
			File.WriteAllText(GameInputMapping.BindingsFilename, text);
		}
		else if (File.Exists(GameInputMapping.BindingsFilename))
		{
			File.Delete(GameInputMapping.BindingsFilename);
		}
	}

	public static void LoadBindings()
	{
		GameInputMapping.KeyBindings = (BindingEntry[])GameInputMapping.DefaultBindings.Clone();
		if (!File.Exists(GameInputMapping.BindingsFilename))
		{
			return;
		}
		string text = File.ReadAllText(GameInputMapping.BindingsFilename);
		if (text == null || text == string.Empty)
		{
			return;
		}
		BindingEntry[] array = null;
		try
		{
			array = JsonConvert.DeserializeObject<BindingEntry[]>(text);
		}
		catch
		{
			Output.LogError(new object[]
			{
				"Error parsing",
				GameInputMapping.BindingsFilename
			});
		}
		if (array == null || array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			BindingEntry bindingEntry = GameInputMapping.KeyBindings[i];
			foreach (BindingEntry bindingEntry2 in array)
			{
				if (bindingEntry2.mAction == bindingEntry.mAction && bindingEntry.mRebindable)
				{
					BindingEntry bindingEntry3 = bindingEntry;
					bindingEntry3.mButton = bindingEntry2.mButton;
					bindingEntry3.mKeyCode = bindingEntry2.mKeyCode;
					bindingEntry3.mModifier = bindingEntry2.mModifier;
					GameInputMapping.KeyBindings[i] = bindingEntry3;
					break;
				}
			}
		}
	}

	public static readonly BindingEntry[] DefaultBindings = new BindingEntry[]
	{
		new BindingEntry(null, GamepadButton.NumButtons, KKeyCode.Escape, Modifier.None, global::Action.Escape, false, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.PanUp, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.PanDown, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.PanLeft, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.PanRight, true, false),
		new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.RotateBuilding, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, global::Action.ManagePeople, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.ManageConsumables, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.ManageVitals, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.ManageResearch, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.ManageReport, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, global::Action.ManageCodex, true, false),
		new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, global::Action.ManageRoles, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.Dig, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, global::Action.Mop, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, global::Action.Clear, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.Disinfect, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.Attack, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, global::Action.Harvest, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, global::Action.Prioritize, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.Alt, global::Action.ToggleScreenshotMode, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildingCancel, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, global::Action.BuildingDeconstruct, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Tab, Modifier.None, global::Action.CycleSpeed, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, global::Action.CameraHome, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.None, global::Action.MouseLeft, false, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse0, Modifier.Shift, global::Action.ShiftMouseLeft, false, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse1, Modifier.None, global::Action.MouseRight, false, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Mouse2, Modifier.None, global::Action.MouseMiddle, false, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.None, global::Action.Plan1, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.None, global::Action.Plan2, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.None, global::Action.Plan3, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.None, global::Action.Plan4, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.None, global::Action.Plan5, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.None, global::Action.Plan6, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.None, global::Action.Plan7, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.None, global::Action.Plan8, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.None, global::Action.Plan9, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.None, global::Action.Plan10, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.None, global::Action.Plan11, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.None, global::Action.Plan12, true, false),
		new BindingEntry("BaseMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.BuildCategoryLadders, true, true),
		new BindingEntry("BaseMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.BuildCategoryTiles, true, true),
		new BindingEntry("BaseMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.BuildCategoryDoors, true, true),
		new BindingEntry("BaseMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.BuildCategoryStorage, true, true),
		new BindingEntry("BaseMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildCategoryResearch, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.BuildCategoryGenerators, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.BuildCategoryWires, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildCategoryPowerControl, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, global::Action.BuildCategoryPlumbingStructures, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.BuildCategoryPipes, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.BuildCategoryVentilationStructures, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.BuildCategoryTubes, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildCategoryLogicWiring, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.BuildCategoryLogicGates, true, true),
		new BindingEntry("InfrastructureMenu", GamepadButton.NumButtons, KKeyCode.Q, Modifier.None, global::Action.BuildCategoryLogicSwitches, true, true),
		new BindingEntry("FoodAgricultureMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildCategoryCooking, true, true),
		new BindingEntry("FoodAgricultureMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.BuildCategoryFarming, true, true),
		new BindingEntry("FoodAgricultureMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildCategoryRanching, true, true),
		new BindingEntry("HealthHappinessMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.BuildCategoryHygiene, true, true),
		new BindingEntry("HealthHappinessMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildCategoryMedical, true, true),
		new BindingEntry("HealthHappinessMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildCategoryRecreation, true, true),
		new BindingEntry("HealthHappinessMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.BuildCategoryFurniture, true, true),
		new BindingEntry("HealthHappinessMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.BuildCategoryDecor, true, true),
		new BindingEntry("IndustrialMenu", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, global::Action.BuildCategoryOxygen, true, true),
		new BindingEntry("IndustrialMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.BuildCategoryUtilities, true, true),
		new BindingEntry("IndustrialMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildCategoryRefining, true, true),
		new BindingEntry("IndustrialMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.BuildCategoryEquipment, true, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.BuildMenuKeyA, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, global::Action.BuildMenuKeyB, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.C, Modifier.None, global::Action.BuildMenuKeyC, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.BuildMenuKeyD, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.BuildMenuKeyE, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.BuildMenuKeyF, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.BuildMenuKeyG, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.H, Modifier.None, global::Action.BuildMenuKeyH, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.BuildMenuKeyI, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, global::Action.BuildMenuKeyJ, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, global::Action.BuildMenuKeyK, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, global::Action.BuildMenuKeyL, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, global::Action.BuildMenuKeyM, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, global::Action.BuildMenuKeyN, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.BuildMenuKeyO, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.P, Modifier.None, global::Action.BuildMenuKeyP, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Q, Modifier.None, global::Action.BuildMenuKeyQ, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.BuildMenuKeyR, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.BuildMenuKeyS, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.BuildMenuKeyT, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, global::Action.BuildMenuKeyU, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.BuildMenuKeyV, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.BuildMenuKeyW, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.X, Modifier.None, global::Action.BuildMenuKeyX, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, global::Action.BuildMenuKeyY, false, true),
		new BindingEntry("BuildingsMenu", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, global::Action.BuildMenuKeyZ, false, true),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollUp, Modifier.None, global::Action.ZoomIn, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.MouseScrollDown, Modifier.None, global::Action.ZoomOut, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.None, global::Action.Overlay1, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.None, global::Action.Overlay2, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.None, global::Action.Overlay3, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F4, Modifier.None, global::Action.Overlay4, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F5, Modifier.None, global::Action.Overlay5, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F6, Modifier.None, global::Action.Overlay6, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F7, Modifier.None, global::Action.Overlay7, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F8, Modifier.None, global::Action.Overlay8, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F9, Modifier.None, global::Action.Overlay9, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F10, Modifier.None, global::Action.Overlay10, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F11, Modifier.None, global::Action.Overlay11, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Shift, global::Action.Overlay12, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Shift, global::Action.Overlay13, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadPlus, Modifier.None, global::Action.SpeedUp, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadMinus, Modifier.None, global::Action.SlowDown, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Space, Modifier.None, global::Action.TogglePause, true, false),
		new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Slash, Modifier.None, global::Action.ToggleOpen, true, false),
		new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, global::Action.ToggleEnabled, true, false),
		new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.Backslash, Modifier.None, global::Action.BuildingUtility1, true, false),
		new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.LeftBracket, Modifier.None, global::Action.BuildingUtility2, true, false),
		new BindingEntry("Building", GamepadButton.NumButtons, KKeyCode.RightBracket, Modifier.None, global::Action.BuildingUtility3, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.LeftAlt, Modifier.Alt, global::Action.AlternateView, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.RightAlt, Modifier.Alt, global::Action.AlternateView, true, false),
		new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.LeftShift, Modifier.Shift, global::Action.DragStraight, true, false),
		new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.RightShift, Modifier.Shift, global::Action.DragStraight, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Ctrl, global::Action.DebugFocus, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.U, Modifier.Ctrl, global::Action.DebugUltraTestMode, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Alt, global::Action.DebugToggleUI, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Alt, global::Action.DebugCollectGarbage, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, Modifier.Alt, global::Action.DebugInvincible, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Alt, global::Action.DebugForceLightEverywhere, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Shift, global::Action.DebugVisualTest, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Shift, global::Action.DebugElementTest, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Shift, global::Action.DebugRiverTest, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F5, Modifier.Shift, global::Action.DebugTileTest, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.N, Modifier.Alt, global::Action.DebugRefreshNavCell, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Ctrl, global::Action.DebugGotoTarget, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.S, Modifier.Ctrl, global::Action.DebugSelectMaterial, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.M, Modifier.Ctrl, global::Action.DebugToggleMusic, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Backspace, Modifier.None, global::Action.DebugToggle, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Q, Modifier.Alt, global::Action.DebugTeleport, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F2, Modifier.Ctrl, global::Action.DebugSpawnMinion, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Ctrl, global::Action.DebugPlace, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F4, Modifier.Ctrl, global::Action.DebugInstantBuildMode, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F5, Modifier.Ctrl, global::Action.DebugSlowTestMode, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F6, Modifier.Ctrl, global::Action.DebugDig, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F8, Modifier.Ctrl, global::Action.DebugExplosion, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F9, Modifier.Ctrl, global::Action.DebugDiscoverAllElements, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.T, Modifier.Alt, global::Action.DebugToggleSelectInEditor, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.P, Modifier.Alt, global::Action.DebugPathFinding, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.M, Modifier.Alt, global::Action.DebugReloadMods, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.L, Modifier.Alt, global::Action.DebugReloadLevel, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Alt, global::Action.DebugSuperSpeed, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Alt, global::Action.DebugGameStep, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Alt, global::Action.DebugSimStep, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.X, Modifier.Alt, global::Action.DebugNotification, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Alt, global::Action.DebugNotificationMessage, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.None, global::Action.ToggleProfiler, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.Ctrl, global::Action.ToggleChromeProfiler, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Ctrl, global::Action.DebugDumpSceneParitionerLeakData, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Ctrl, global::Action.DebugTriggerException, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, (Modifier)6, global::Action.DebugTriggerError, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Ctrl, global::Action.DebugDumpGarbageReferences, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Ctrl, global::Action.DebugDumpEventData, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Alt, global::Action.DebugCrashSim, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Alt, global::Action.SreenShot1x, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Alt, global::Action.SreenShot2x, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Alt, global::Action.SreenShot8x, true, false),
		new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Alt, global::Action.SreenShot32x, true, false),
		new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, global::Action.DialogSubmit, false, false)
	};

	public static BindingEntry[] KeyBindings = (BindingEntry[])GameInputMapping.DefaultBindings.Clone();
}
