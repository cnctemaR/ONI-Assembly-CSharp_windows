using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorInputSystem
	{
		public static void SetupInput()
		{
			NodeEditorInputSystem.eventHandlers = new List<KeyValuePair<EventHandlerAttribute, Delegate>>();
			NodeEditorInputSystem.hotkeyHandlers = new List<KeyValuePair<HotkeyAttribute, Delegate>>();
			NodeEditorInputSystem.contextEntries = new List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>>();
			NodeEditorInputSystem.contextFillers = new List<KeyValuePair<ContextFillerAttribute, Delegate>>();
			foreach (Assembly assembly2 in from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly)
			{
				Type[] types = assembly2.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					for (int j = 0; j < methods.Length; j++)
					{
						MethodInfo methodInfo = methods[j];
						Delegate actionDelegate = null;
						PopupMenu.MenuFunctionData <>9__3;
						foreach (object obj in methodInfo.GetCustomAttributes(true))
						{
							Type type = obj.GetType();
							if (type == typeof(EventHandlerAttribute))
							{
								if (EventHandlerAttribute.AssureValidity(methodInfo, obj as EventHandlerAttribute))
								{
									if (actionDelegate == null)
									{
										actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
									}
									NodeEditorInputSystem.eventHandlers.Add(new KeyValuePair<EventHandlerAttribute, Delegate>(obj as EventHandlerAttribute, actionDelegate));
								}
							}
							else if (type == typeof(HotkeyAttribute))
							{
								if (HotkeyAttribute.AssureValidity(methodInfo, obj as HotkeyAttribute))
								{
									if (actionDelegate == null)
									{
										actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
									}
									NodeEditorInputSystem.hotkeyHandlers.Add(new KeyValuePair<HotkeyAttribute, Delegate>(obj as HotkeyAttribute, actionDelegate));
								}
							}
							else if (type == typeof(ContextEntryAttribute))
							{
								if (ContextEntryAttribute.AssureValidity(methodInfo, obj as ContextEntryAttribute))
								{
									if (actionDelegate == null)
									{
										actionDelegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo>), methodInfo);
									}
									PopupMenu.MenuFunctionData menuFunctionData;
									if ((menuFunctionData = <>9__3) == null)
									{
										menuFunctionData = (<>9__3 = delegate(object callbackObj)
										{
											if (!(callbackObj is NodeEditorInputInfo))
											{
												throw new UnityException("Callback Object passed by context is not of type NodeEditorMenuCallback!");
											}
											actionDelegate.DynamicInvoke(new object[] { callbackObj as NodeEditorInputInfo });
										});
									}
									PopupMenu.MenuFunctionData menuFunctionData2 = menuFunctionData;
									NodeEditorInputSystem.contextEntries.Add(new KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>(obj as ContextEntryAttribute, menuFunctionData2));
								}
							}
							else if (type == typeof(ContextFillerAttribute) && ContextFillerAttribute.AssureValidity(methodInfo, obj as ContextFillerAttribute))
							{
								Delegate @delegate = Delegate.CreateDelegate(typeof(Action<NodeEditorInputInfo, GenericMenu>), methodInfo);
								NodeEditorInputSystem.contextFillers.Add(new KeyValuePair<ContextFillerAttribute, Delegate>(obj as ContextFillerAttribute, @delegate));
							}
						}
					}
				}
			}
			NodeEditorInputSystem.eventHandlers.Sort((KeyValuePair<EventHandlerAttribute, Delegate> handlerA, KeyValuePair<EventHandlerAttribute, Delegate> handlerB) => handlerA.Key.priority.CompareTo(handlerB.Key.priority));
			NodeEditorInputSystem.hotkeyHandlers.Sort((KeyValuePair<HotkeyAttribute, Delegate> handlerA, KeyValuePair<HotkeyAttribute, Delegate> handlerB) => handlerA.Key.priority.CompareTo(handlerB.Key.priority));
		}

		private static void CallEventHandlers(NodeEditorInputInfo inputInfo, bool late)
		{
			object[] array = new object[] { inputInfo };
			foreach (KeyValuePair<EventHandlerAttribute, Delegate> keyValuePair in NodeEditorInputSystem.eventHandlers)
			{
				if (keyValuePair.Key.handledEvent != null)
				{
					EventType? handledEvent = keyValuePair.Key.handledEvent;
					EventType type = inputInfo.inputEvent.type;
					if (!((handledEvent.GetValueOrDefault() == type) & (handledEvent != null)))
					{
						continue;
					}
				}
				if (late ? (keyValuePair.Key.priority >= 100) : (keyValuePair.Key.priority < 100))
				{
					keyValuePair.Value.DynamicInvoke(array);
					if (inputInfo.inputEvent.type == EventType.Used)
					{
						break;
					}
				}
			}
		}

		private static void CallHotkeys(NodeEditorInputInfo inputInfo, KeyCode keyCode, EventModifiers mods)
		{
			object[] array = new object[] { inputInfo };
			foreach (KeyValuePair<HotkeyAttribute, Delegate> keyValuePair in NodeEditorInputSystem.hotkeyHandlers)
			{
				if (keyValuePair.Key.handledHotKey == keyCode)
				{
					if (keyValuePair.Key.modifiers != null)
					{
						EventModifiers? modifiers = keyValuePair.Key.modifiers;
						if (!((modifiers.GetValueOrDefault() == mods) & (modifiers != null)))
						{
							continue;
						}
					}
					if (keyValuePair.Key.limitingEventType != null)
					{
						EventType? limitingEventType = keyValuePair.Key.limitingEventType;
						EventType type = inputInfo.inputEvent.type;
						if (!((limitingEventType.GetValueOrDefault() == type) & (limitingEventType != null)))
						{
							continue;
						}
					}
					keyValuePair.Value.DynamicInvoke(array);
					if (inputInfo.inputEvent.type == EventType.Used)
					{
						break;
					}
				}
			}
		}

		private static void FillContextMenu(NodeEditorInputInfo inputInfo, GenericMenu contextMenu, ContextType contextType)
		{
			foreach (KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData> keyValuePair in NodeEditorInputSystem.contextEntries)
			{
				if (keyValuePair.Key.contextType == contextType)
				{
					contextMenu.AddItem(new GUIContent(keyValuePair.Key.contextPath), false, keyValuePair.Value, inputInfo);
				}
			}
			object[] array = new object[] { inputInfo, contextMenu };
			foreach (KeyValuePair<ContextFillerAttribute, Delegate> keyValuePair2 in NodeEditorInputSystem.contextFillers)
			{
				if (keyValuePair2.Key.contextType == contextType)
				{
					keyValuePair2.Value.DynamicInvoke(array);
				}
			}
		}

		public static void HandleInputEvents(NodeEditorState state)
		{
			if (NodeEditorInputSystem.shouldIgnoreInput(state))
			{
				return;
			}
			NodeEditorInputInfo nodeEditorInputInfo = new NodeEditorInputInfo(state);
			NodeEditorInputSystem.CallEventHandlers(nodeEditorInputInfo, false);
			NodeEditorInputSystem.CallHotkeys(nodeEditorInputInfo, Event.current.keyCode, Event.current.modifiers);
		}

		public static void HandleLateInputEvents(NodeEditorState state)
		{
			if (NodeEditorInputSystem.shouldIgnoreInput(state))
			{
				return;
			}
			NodeEditorInputSystem.CallEventHandlers(new NodeEditorInputInfo(state), true);
		}

		internal static bool shouldIgnoreInput(NodeEditorState state)
		{
			if (OverlayGUI.HasPopupControl())
			{
				return true;
			}
			if (!state.canvasRect.Contains(Event.current.mousePosition))
			{
				return true;
			}
			for (int i = 0; i < state.ignoreInput.Count; i++)
			{
				if (state.ignoreInput[i].Contains(Event.current.mousePosition))
				{
					return true;
				}
			}
			return false;
		}

		[EventHandler(-4)]
		private static void HandleFocussing(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			editorState.focusedNode = NodeEditor.NodeAtPosition(NodeEditor.ScreenToCanvasSpace(inputInfo.inputPos), out editorState.focusedNodeKnob);
			if (NodeEditorInputSystem.unfocusControlsForState == editorState && Event.current.type == EventType.Repaint)
			{
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
				NodeEditorInputSystem.unfocusControlsForState = null;
			}
		}

		[EventHandler(EventType.MouseDown, -2)]
		private static void HandleSelecting(NodeEditorInputInfo inputInfo)
		{
			NodeEditorState editorState = inputInfo.editorState;
			if (inputInfo.inputEvent.button == 0 && editorState.focusedNode != editorState.selectedNode)
			{
				NodeEditorInputSystem.unfocusControlsForState = editorState;
				editorState.selectedNode = editorState.focusedNode;
				NodeEditor.RepaintClients();
			}
		}

		[EventHandler(EventType.MouseDown, 0)]
		private static void HandleContextClicks(NodeEditorInputInfo inputInfo)
		{
			if (Event.current.button == 1)
			{
				GenericMenu genericMenu = new GenericMenu();
				if (inputInfo.editorState.focusedNode != null)
				{
					NodeEditorInputSystem.FillContextMenu(inputInfo, genericMenu, ContextType.Node);
				}
				else
				{
					NodeEditorInputSystem.FillContextMenu(inputInfo, genericMenu, ContextType.Canvas);
				}
				genericMenu.Show(inputInfo.inputPos, 40f);
				Event.current.Use();
			}
		}

		private static List<KeyValuePair<EventHandlerAttribute, Delegate>> eventHandlers;

		private static List<KeyValuePair<HotkeyAttribute, Delegate>> hotkeyHandlers;

		private static List<KeyValuePair<ContextEntryAttribute, PopupMenu.MenuFunctionData>> contextEntries;

		private static List<KeyValuePair<ContextFillerAttribute, Delegate>> contextFillers;

		private static NodeEditorState unfocusControlsForState;
	}
}
