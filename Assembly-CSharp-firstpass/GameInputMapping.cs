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

	public static BindingEntry[] DefaultBindings { get; private set; }

	public static void SetDefaultKeyBindings(BindingEntry[] default_keybindings)
	{
		GameInputMapping.DefaultBindings = default_keybindings;
		GameInputMapping.KeyBindings = (BindingEntry[])default_keybindings.Clone();
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
		global::Debug.Assert(false, "Unbound action " + mAction.ToString());
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
			return;
		}
		if (File.Exists(GameInputMapping.BindingsFilename))
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
		if (text == null || text == "")
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
			DebugUtil.LogErrorArgs(new object[]
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

	public static BindingEntry[] KeyBindings;
}
