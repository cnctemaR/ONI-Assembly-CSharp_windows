using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Klei;
using KSerialization;
using Steamworks;
using UnityEngine;
using UnityEngine.U2D;

public class Global : MonoBehaviour
{
	public static Global Instance { get; private set; }

	public static BindingEntry[] GenerateDefaultBindings()
	{
		List<BindingEntry> list = new List<BindingEntry>
		{
			new BindingEntry(null, GamepadButton.NumButtons, KKeyCode.Escape, Modifier.None, global::Action.Escape, false, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.W, Modifier.None, global::Action.PanUp, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.S, Modifier.None, global::Action.PanDown, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.A, Modifier.None, global::Action.PanLeft, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.D, Modifier.None, global::Action.PanRight, true, false),
			new BindingEntry("Tool", GamepadButton.NumButtons, KKeyCode.O, Modifier.None, global::Action.RotateBuilding, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.L, Modifier.None, global::Action.ManagePeople, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.F, Modifier.None, global::Action.ManageConsumables, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.V, Modifier.None, global::Action.ManageVitals, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.R, Modifier.None, global::Action.ManageResearch, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.E, Modifier.None, global::Action.ManageReport, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.U, Modifier.None, global::Action.ManageCodex, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.J, Modifier.None, global::Action.ManageRoles, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Period, Modifier.None, global::Action.ManageSchedule, true, false),
			new BindingEntry("Management", GamepadButton.NumButtons, KKeyCode.Z, Modifier.None, global::Action.ManageStarmap, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.G, Modifier.None, global::Action.Dig, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.M, Modifier.None, global::Action.Mop, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.K, Modifier.None, global::Action.Clear, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.I, Modifier.None, global::Action.Disinfect, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.T, Modifier.None, global::Action.Attack, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.N, Modifier.None, global::Action.Capture, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Y, Modifier.None, global::Action.Harvest, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Insert, Modifier.None, global::Action.EmptyPipe, true, false),
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
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.B, Modifier.None, global::Action.CopyBuilding, true, false),
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
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.F3, Modifier.Shift, global::Action.Overlay14, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadPlus, Modifier.None, global::Action.SpeedUp, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.KeypadMinus, Modifier.None, global::Action.SlowDown, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Space, Modifier.None, global::Action.TogglePause, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Ctrl, global::Action.SetUserNav1, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Ctrl, global::Action.SetUserNav2, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Ctrl, global::Action.SetUserNav3, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Ctrl, global::Action.SetUserNav4, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Ctrl, global::Action.SetUserNav5, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Ctrl, global::Action.SetUserNav6, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Ctrl, global::Action.SetUserNav7, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Ctrl, global::Action.SetUserNav8, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Ctrl, global::Action.SetUserNav9, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Ctrl, global::Action.SetUserNav10, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Shift, global::Action.GotoUserNav1, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Shift, global::Action.GotoUserNav2, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Shift, global::Action.GotoUserNav3, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Shift, global::Action.GotoUserNav4, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Shift, global::Action.GotoUserNav5, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha6, Modifier.Shift, global::Action.GotoUserNav6, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha7, Modifier.Shift, global::Action.GotoUserNav7, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha8, Modifier.Shift, global::Action.GotoUserNav8, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Shift, global::Action.GotoUserNav9, true, false),
			new BindingEntry("Navigation", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Shift, global::Action.GotoUserNav10, true, false),
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
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Shift, global::Action.DebugElementTest, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Shift, global::Action.DebugRiverTest, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Shift, global::Action.DebugTileTest, true, false),
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
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Z, Modifier.Alt, global::Action.DebugSuperSpeed, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Equals, Modifier.Alt, global::Action.DebugGameStep, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Minus, Modifier.Alt, global::Action.DebugSimStep, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.X, Modifier.Alt, global::Action.DebugNotification, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.C, Modifier.Alt, global::Action.DebugNotificationMessage, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.None, global::Action.ToggleProfiler, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.BackQuote, Modifier.Alt, global::Action.ToggleChromeProfiler, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F1, Modifier.Ctrl, global::Action.DebugDumpSceneParitionerLeakData, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, Modifier.Ctrl, global::Action.DebugTriggerException, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F12, (Modifier)6, global::Action.DebugTriggerError, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, Modifier.Ctrl, global::Action.DebugDumpGCRoots, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F10, (Modifier)3, global::Action.DebugDumpGarbageReferences, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F11, Modifier.Ctrl, global::Action.DebugDumpEventData, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.F7, (Modifier)3, global::Action.DebugCrashSim, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha9, Modifier.Alt, global::Action.DebugNextCall, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha1, Modifier.Alt, global::Action.SreenShot1x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha2, Modifier.Alt, global::Action.SreenShot2x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha3, Modifier.Alt, global::Action.SreenShot8x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha4, Modifier.Alt, global::Action.SreenShot32x, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha5, Modifier.Alt, global::Action.DebugLockCursor, true, false),
			new BindingEntry("Debug", GamepadButton.NumButtons, KKeyCode.Alpha0, Modifier.Alt, global::Action.DebugTogglePersonalPriorityComparison, true, false),
			new BindingEntry("Root", GamepadButton.NumButtons, KKeyCode.Return, Modifier.None, global::Action.DialogSubmit, false, false),
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
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.B, Modifier.Shift, global::Action.SandboxBrush, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.N, Modifier.Shift, global::Action.SandboxSprinkle, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.F, Modifier.Shift, global::Action.SandboxFlood, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.K, Modifier.Shift, global::Action.SandboxSample, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.H, Modifier.Shift, global::Action.SandboxHeatGun, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.C, Modifier.Shift, global::Action.SandboxClearFloor, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.X, Modifier.Shift, global::Action.SandboxDestroy, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.E, Modifier.Shift, global::Action.SandboxSpawnEntity, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.S, Modifier.Shift, global::Action.ToggleSandboxTools, true, false),
			new BindingEntry("Sandbox", GamepadButton.NumButtons, KKeyCode.R, Modifier.Shift, global::Action.SandboxReveal, true, false)
		};
		IList<BuildMenu.DisplayInfo> list2 = (IList<BuildMenu.DisplayInfo>)BuildMenu.OrderedBuildings.data;
		foreach (BuildMenu.DisplayInfo displayInfo in list2)
		{
			Global.AddBindings(HashedString.Invalid, displayInfo, list);
		}
		return list.ToArray();
	}

	private static void AddBindings(HashedString parent_category, BuildMenu.DisplayInfo display_info, List<BindingEntry> bindings)
	{
		if (display_info.data != null)
		{
			Type type = display_info.data.GetType();
			if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
			{
				IList<BuildMenu.DisplayInfo> list = (IList<BuildMenu.DisplayInfo>)display_info.data;
				foreach (BuildMenu.DisplayInfo displayInfo in list)
				{
					Global.AddBindings(display_info.category, displayInfo, bindings);
				}
			}
			else if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(type))
			{
				string text = HashCache.Get().Get(parent_category);
				TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
				string text2 = textInfo.ToTitleCase(text) + " Menu";
				BindingEntry bindingEntry = new BindingEntry(text2, GamepadButton.NumButtons, display_info.keyCode, Modifier.None, display_info.hotkey, true, true);
				bindings.Add(bindingEntry);
			}
		}
	}

	private void Awake()
	{
		this.globalCanvas = GameObject.Find("Canvas");
		global::UnityEngine.Object.DontDestroyOnLoad(this.globalCanvas.gameObject);
		this.OutputSystemInfo();
		Global.Instance = this;
		if (this.forcedAtlasInitializationList != null)
		{
			foreach (SpriteAtlas spriteAtlas in this.forcedAtlasInitializationList)
			{
				int spriteCount = spriteAtlas.spriteCount;
				Sprite[] array2 = new Sprite[spriteCount];
				spriteAtlas.GetSprites(array2);
				foreach (Sprite sprite in array2)
				{
					Texture2D texture = sprite.texture;
					if (texture != null)
					{
						texture.filterMode = FilterMode.Bilinear;
						texture.anisoLevel = 4;
						texture.mipMapBias = 0f;
					}
				}
			}
		}
		LayeredFileSystem.CreateInstance();
		this.layeredFileSystem = LayeredFileSystem.instance;
		this.standardFS = new StandardFileSystem();
		this.layeredFileSystem.AddFileSystem(this.standardFS);
		Singleton<StateMachineUpdater>.CreateInstance();
		Singleton<StateMachineManager>.CreateInstance();
		this.modManager = new ModManager();
		this.modManager.Start();
		Manager.Initialize();
		this.mInputManager = new GameInputManager(Global.GenerateDefaultBindings());
		Audio.Get();
		KAnimBatchManager.CreateInstance();
		Singleton<SoundEventVolumeCache>.CreateInstance();
		this.mAnimEventManager = new AnimEventManager();
		Singleton<KBatchedAnimUpdater>.CreateInstance();
		DistributionPlatform.Initialize();
		Localization.Initialize(false);
		KProfiler.main_thread = Thread.CurrentThread;
		this.RestoreLegacyMetricsSetting();
		if (DistributionPlatform.Initialized)
		{
			if (!KPrivacyPrefs.instance.disableDataCollection)
			{
				global::Debug.Log(string.Concat(new object[]
				{
					"Logged into ",
					DistributionPlatform.Inst.Name,
					" with ID:",
					DistributionPlatform.Inst.LocalUser.Id,
					", NAME:",
					DistributionPlatform.Inst.LocalUser.Name
				}), null);
				ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.OnGetUserIdKey));
			}
		}
		else
		{
			global::Debug.LogWarning("Can't init " + DistributionPlatform.Inst.Name + " distribution platform...", null);
			this.OnGetUserIdKey();
		}
		GlobalResources.Instance();
	}

	private void RestoreLegacyMetricsSetting()
	{
		if (KPlayerPrefs.GetInt("ENABLE_METRICS", 1) == 0)
		{
			KPlayerPrefs.DeleteKey("ENABLE_METRICS");
			KPlayerPrefs.Save();
			KPrivacyPrefs.instance.disableDataCollection = true;
			KPrivacyPrefs.Save();
		}
	}

	public GameInputManager GetInputManager()
	{
		return this.mInputManager;
	}

	public AnimEventManager GetAnimEventManager()
	{
		if (App.IsExiting)
		{
			return null;
		}
		return this.mAnimEventManager;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (this.mInputManager != null)
		{
			this.mInputManager.OnApplicationFocus(focus);
		}
	}

	private void OnGetUserIdKey()
	{
		this.gotKleiUserID = true;
	}

	private void Update()
	{
		this.mInputManager.Update();
		if (this.mAnimEventManager != null)
		{
			this.mAnimEventManager.Update();
		}
		if (DistributionPlatform.Initialized && SteamUGCService.Instance == null)
		{
			SteamUGCService.Initialize();
			this.modManager.RegisterUGCEventHandlers(SteamUGCService.Instance);
		}
		if (this.gotKleiUserID)
		{
			this.gotKleiUserID = false;
			ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(new global::System.Action(this.SetONIStaticSessionVariables), new Action<Dictionary<string, object>>(this.SetONIDynamicSessionVariables));
			ThreadedHttps<KleiMetrics>.Instance.StartSession();
		}
		ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
	}

	private void SetONIStaticSessionVariables()
	{
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", "release");
		ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", 302293U);
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, ((GameUtil.MassUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey)).ToString());
		}
		if (KPlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
		{
			ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, ((GameUtil.TemperatureUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey)).ToString());
		}
		if (SteamManager.Initialized)
		{
			PublishedFileId_t publishedFileId_t;
			string installedLanguageCode = LanguageOptionsScreen.GetInstalledLanguageCode(out publishedFileId_t);
			if (publishedFileId_t != PublishedFileId_t.Invalid)
			{
				ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguagePackKey, publishedFileId_t.m_PublishedFileId);
			}
			if (!string.IsNullOrEmpty(installedLanguageCode))
			{
				ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageCodeKey, installedLanguageCode);
			}
		}
	}

	private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
	{
		if (Game.Instance != null && GameClock.Instance != null)
		{
			data.Add("GameTimeSeconds", (int)GameClock.Instance.GetTime());
		}
	}

	private void LateUpdate()
	{
		Singleton<KBatchedAnimUpdater>.Instance.LateUpdate();
	}

	private void OnDestroy()
	{
		if (this.modManager != null)
		{
			this.modManager.Shutdown();
		}
		Global.Instance = null;
		if (this.mAnimEventManager != null)
		{
			this.mAnimEventManager.FreeResources();
		}
		Singleton<KBatchedAnimUpdater>.DestroyInstance();
	}

	private void OnApplicationQuit()
	{
		KGlobalAnimParser.DestroyInstance();
		ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
	}

	private void OutputSystemInfo()
	{
		try
		{
			Console.WriteLine("SYSTEM INFO:");
			Dictionary<string, object> hardwareStats = KleiMetrics.GetHardwareStats();
			foreach (KeyValuePair<string, object> keyValuePair in hardwareStats)
			{
				try
				{
					Console.WriteLine(string.Format("    {0}={1}", keyValuePair.Key.ToString(), keyValuePair.Value.ToString()));
				}
				catch
				{
				}
			}
			Console.WriteLine(string.Format("    {0}={1}", "System Language", Application.systemLanguage.ToString()));
		}
		catch
		{
		}
	}

	public SpriteAtlas[] forcedAtlasInitializationList;

	public GameObject modErrorsPrefab;

	public GameObject globalCanvas;

	private GameInputManager mInputManager;

	private AnimEventManager mAnimEventManager;

	public ModManager modManager;

	public LayeredFileSystem layeredFileSystem;

	public StandardFileSystem standardFS;

	public ZipFileSystem worldGenZipFS;

	private bool gotKleiUserID;

	private Thread mainThread;

	public static readonly string LanguagePackKey = "LanguagePack";

	public static readonly string LanguageCodeKey = "LanguageCode";
}
