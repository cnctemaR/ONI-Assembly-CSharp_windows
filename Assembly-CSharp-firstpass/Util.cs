using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using KSerialization;
using UnityEngine;

public static class Util
{
	[Conditional("UNITY_EDITOR")]
	public static void WarnIfCapturing<T>(Predicate<T> pred)
	{
		if (pred.Target != null || !pred.Method.IsStatic)
		{
			global::Debug.LogWarning("Non-static or capturing predicate used");
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void WarnIfCapturing<T>(Action<T> action)
	{
		if (action.Target != null || !action.Method.IsStatic)
		{
			global::Debug.LogWarning("Non-static or capturing predicate used");
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void WarnIfCapturing<T1, T2>(Action<T1, T2> action)
	{
		if (action.Target != null || !action.Method.IsStatic)
		{
			global::Debug.LogWarning("Non-static or capturing predicate used");
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void WarnIfCapturing<T, Y>(Func<T, Y, bool> pred)
	{
		if (pred.Target != null || !pred.Method.IsStatic)
		{
			global::Debug.LogWarning("Non-static or capturing predicate used");
		}
	}

	public static void RemoveAtSwap<T>(this List<T> list, int index)
	{
		list[index] = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
	}

	public static void RemoveAllSwap<T>(this List<T> list, Predicate<T> pred)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (pred(list[i]))
			{
				list.RemoveAtSwap<T>(i);
			}
		}
	}

	public static void RemoveAllSwap<T, Y>(this List<T> list, Func<T, Y, bool> pred, Y context)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (pred(list[i], context))
			{
				list.RemoveAtSwap<T>(i);
			}
		}
	}

	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	public static IEnumerable Reverse(IList list)
	{
		int num;
		for (int i = list.Count - 1; i >= 0; i = num)
		{
			yield return list[i];
			num = i - 1;
		}
		yield break;
	}

	public static void Swap(IList list_or_array, in int index_a, in int index_b)
	{
		object obj = list_or_array[index_a];
		list_or_array[index_a] = list_or_array[index_b];
		list_or_array[index_b] = obj;
	}

	public static float Remap(this float value, [TupleElementNames(new string[] { "min", "max" })] ValueTuple<float, float> fromRange, [TupleElementNames(new string[] { "min", "max" })] ValueTuple<float, float> toRange)
	{
		return (value - fromRange.Item1) / (fromRange.Item2 - fromRange.Item1) * (toRange.Item2 - toRange.Item1) + toRange.Item1;
	}

	public static void InitializeComponent(Component cmp)
	{
		if (cmp != null)
		{
			KMonoBehaviour kmonoBehaviour = cmp as KMonoBehaviour;
			if (kmonoBehaviour != null)
			{
				kmonoBehaviour.InitializeComponent();
			}
		}
	}

	public static void SpawnComponent(Component cmp)
	{
		if (cmp != null)
		{
			KMonoBehaviour kmonoBehaviour = cmp as KMonoBehaviour;
			if (kmonoBehaviour != null)
			{
				kmonoBehaviour.Spawn();
			}
		}
	}

	public static Component FindComponent(this Component cmp, string targetName)
	{
		return cmp.gameObject.FindComponent(targetName);
	}

	public static Component FindComponent(this GameObject go, string targetName)
	{
		Component component = go.GetComponent(targetName);
		Util.InitializeComponent(component);
		return component;
	}

	public static T FindComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.FindComponent<T>();
	}

	public static T FindComponent<T>(this GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		Util.InitializeComponent(component);
		return component;
	}

	public static T FindOrAddUnityComponent<T>(this Component cmp) where T : Component
	{
		return cmp.gameObject.FindOrAddUnityComponent<T>();
	}

	public static T FindOrAddUnityComponent<T>(this GameObject go) where T : Component
	{
		T t = go.GetComponent<T>();
		if (t == null)
		{
			t = go.AddComponent<T>();
		}
		return t;
	}

	public static Component RequireComponent(this Component cmp, string name)
	{
		return cmp.gameObject.RequireComponent(name);
	}

	public static Component RequireComponent(this GameObject go, string name)
	{
		Component component = go.GetComponent(name);
		if (component == null)
		{
			global::Debug.LogErrorFormat(go, "{0} '{1}' requires a component of type {2}!", new object[]
			{
				go.GetType().ToString(),
				go.name,
				name
			});
			return null;
		}
		Util.InitializeComponent(component);
		return component;
	}

	public static T RequireComponent<T>(this Component cmp) where T : Component
	{
		T component = cmp.gameObject.GetComponent<T>();
		if (component == null)
		{
			global::Debug.LogErrorFormat(cmp.gameObject, "{0} '{1}' requires a component of type {2} as requested by {3}!", new object[]
			{
				cmp.gameObject.GetType().ToString(),
				cmp.gameObject.name,
				typeof(T).ToString(),
				cmp.GetType().ToString()
			});
			return default(T);
		}
		Util.InitializeComponent(component);
		return component;
	}

	public static T RequireComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (component == null)
		{
			global::Debug.LogErrorFormat(gameObject, "{0} '{1}' requires a component of type {2}!", new object[]
			{
				gameObject.GetType().ToString(),
				gameObject.name,
				typeof(T).ToString()
			});
			return default(T);
		}
		Util.InitializeComponent(component);
		return component;
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		Util.SetLayer(go.transform, layer);
	}

	public static void SetLayer(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		for (int i = 0; i < t.childCount; i++)
		{
			Util.SetLayer(t.GetChild(i), layer);
		}
	}

	public static void KDestroyGameObject(Component original)
	{
		global::Debug.Assert(original != null, "Attempted to destroy a GameObject that is already destroyed.");
		Util.KDestroyGameObject(original.gameObject);
	}

	public static void KDestroyGameObject(GameObject original)
	{
		global::Debug.Assert(original != null, "Attempted to destroy a GameObject that is already destroyed.");
		original.DeleteObject();
	}

	public static T FindOrAddComponent<T>(this Component cmp) where T : Component
	{
		return cmp.gameObject.FindOrAddComponent<T>();
	}

	public static T FindOrAddComponent<T>(this GameObject go) where T : Component
	{
		T t = go.GetComponent<T>();
		if (t == null)
		{
			t = go.AddComponent<T>();
			KMonoBehaviour kmonoBehaviour = t as KMonoBehaviour;
			if (kmonoBehaviour != null && !KMonoBehaviour.isPoolPreInit && !kmonoBehaviour.IsInitialized())
			{
				global::Debug.LogErrorFormat("Could not find component " + typeof(T).ToString() + " on object " + go.ToString(), Array.Empty<object>());
			}
		}
		else
		{
			Util.InitializeComponent(t);
		}
		return t;
	}

	public static void PreInit(this GameObject go)
	{
		KMonoBehaviour.isPoolPreInit = true;
		KMonoBehaviour[] components = go.GetComponents<KMonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].InitializeComponent();
		}
		KMonoBehaviour.isPoolPreInit = false;
	}

	public static T KInstantiate<T>(GameObject original, GameObject parent = null, string name = null) where T : global::UnityEngine.Object
	{
		GameObject gameObject = Util.KInstantiate(original, parent, null);
		if (!(gameObject == null))
		{
			return gameObject.GetComponent<T>();
		}
		return default(T);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position)
	{
		return Util.KInstantiate(original, position, Quaternion.identity, null, null, true, 0);
	}

	public static GameObject KInstantiate(Component original, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original.gameObject, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject original, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject original, Vector3 position, Quaternion rotation, GameObject parent = null, string name = null, bool initialize_id = true, int gameLayer = 0)
	{
		if (App.IsExiting)
		{
			return null;
		}
		GameObject gameObject = null;
		if (original == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "Missing prefab" });
		}
		if (gameObject == null)
		{
			if (original.GetComponent<RectTransform>() != null && parent != null)
			{
				gameObject = global::UnityEngine.Object.Instantiate<GameObject>(original, position, rotation);
				gameObject.transform.SetParent(parent.transform, true);
			}
			else
			{
				Transform transform = null;
				if (parent != null)
				{
					transform = parent.transform;
				}
				gameObject = global::UnityEngine.Object.Instantiate<GameObject>(original, position, rotation, transform);
			}
			if (gameLayer != 0)
			{
				gameObject.SetLayerRecursively(gameLayer);
			}
		}
		if (name != null)
		{
			gameObject.name = name;
		}
		else
		{
			gameObject.name = original.name;
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		if (component != null)
		{
			if (initialize_id)
			{
				component.InstanceID = KPrefabID.GetUniqueID();
				KPrefabIDTracker.Get().Register(component);
			}
			component.InitializeTags(true);
			KPrefabID component2 = original.GetComponent<KPrefabID>();
			component.CopyTags(component2);
			component.CopyInitFunctions(component2);
			component.RunInstantiateFn();
		}
		return gameObject;
	}

	public static T KInstantiateUI<T>(GameObject original, GameObject parent = null, bool force_active = false) where T : Component
	{
		return Util.KInstantiateUI(original, parent, force_active).GetComponent<T>();
	}

	public static GameObject KInstantiateUI(GameObject original, GameObject parent = null, bool force_active = false)
	{
		if (App.IsExiting)
		{
			return null;
		}
		GameObject gameObject = null;
		if (original == null)
		{
			DebugUtil.LogWarningArgs(new object[] { "Missing prefab" });
		}
		if (gameObject == null)
		{
			gameObject = global::UnityEngine.Object.Instantiate<GameObject>(original, (parent != null) ? parent.transform : null, false);
		}
		gameObject.name = original.name;
		if (force_active)
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public static GameObject NewGameObject(GameObject parent, string name)
	{
		GameObject gameObject = new GameObject();
		if (parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
		if (name != null)
		{
			gameObject.name = name;
		}
		return gameObject;
	}

	public static T UpdateComponentRequirement<T>(this GameObject go, bool required = true) where T : Component
	{
		T t = go.GetComponent(typeof(T)) as T;
		if (!required && t != null)
		{
			global::UnityEngine.Object.DestroyImmediate(t, true);
			t = default(T);
		}
		else if (required && t == null)
		{
			t = go.AddComponent(typeof(T)) as T;
		}
		return t;
	}

	public static string FormatTwoDecimalPlace(float value)
	{
		return string.Format("{0:0.00}", value);
	}

	public static string FormatOneDecimalPlace(float value)
	{
		return string.Format("{0:0.0}", value);
	}

	public static string FormatWholeNumber(float value)
	{
		return string.Format("{0:0}", value);
	}

	public static bool IsInputCharacterValid(char _char, bool isPath = false, bool allowNewLine = false)
	{
		return (!isPath && allowNewLine && _char == '\n') || (!Util.defaultInvalidUserInputChars.Contains(_char) && (isPath || !Util.additionalInvalidUserInputChars.Contains(_char)));
	}

	public static bool IsInputStringValid(string input, bool isPath = false, bool allowNewLine = false)
	{
		for (int i = 0; i < input.Length; i++)
		{
			if (!Util.IsInputCharacterValid(input[i], isPath, allowNewLine))
			{
				return false;
			}
		}
		return true;
	}

	public static void ScrubInputField(KInputTextField inputField, bool isPath = false, bool allowNewLine = false)
	{
		for (int i = inputField.text.Length - 1; i >= 0; i--)
		{
			if (i < inputField.text.Length && !Util.IsInputCharacterValid(inputField.text[i], isPath, allowNewLine))
			{
				inputField.text = inputField.text.Remove(i, 1);
			}
		}
	}

	public static string StripTextFormatting(string original)
	{
		return Regex.Replace(original, "<[^>]*>([^<]*)<[^>]*>", "$1");
	}

	public static void Reset(Transform transform)
	{
		transform.SetLocalPosition(Vector3.zero);
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static float GaussianRandom(float mu = 0f, float sigma = 1f)
	{
		double num = Util.random.NextDouble();
		double num2 = Util.random.NextDouble();
		double num3 = (double)(Mathf.Sqrt(-2f * Mathf.Log((float)num)) * Mathf.Sin(6.2831855f * (float)num2));
		return (float)((double)mu + (double)sigma * num3);
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		list.ShuffleSeeded<T>(Util.random);
	}

	public static Bounds GetBounds(GameObject go)
	{
		Bounds bounds = default(Bounds);
		bool flag = true;
		Util.GetBounds(go, ref bounds, ref flag);
		return bounds;
	}

	private static void GetBounds(GameObject go, ref Bounds bounds, ref bool first)
	{
		if (go != null)
		{
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				if (first)
				{
					bounds = component.bounds;
					first = false;
				}
				else
				{
					bounds.Encapsulate(component.bounds);
				}
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				Util.GetBounds(go.transform.GetChild(i).gameObject, ref bounds, ref first);
			}
		}
	}

	public static bool IsOnLeftSideOfScreen(Vector3 position)
	{
		return position.x < (float)Screen.width;
	}

	public static void Write(this BinaryWriter writer, Vector2 v)
	{
		writer.WriteSingleFast(v.x);
		writer.WriteSingleFast(v.y);
	}

	public static void Write(this BinaryWriter writer, Vector3 v)
	{
		writer.WriteSingleFast(v.x);
		writer.WriteSingleFast(v.y);
		writer.WriteSingleFast(v.z);
	}

	public static Vector2 ReadVector2(this BinaryReader reader)
	{
		return new Vector2
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle()
		};
	}

	public static Vector3 ReadVector3(this BinaryReader reader)
	{
		return new Vector3
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle()
		};
	}

	public static void Write(this BinaryWriter writer, Quaternion q)
	{
		writer.WriteSingleFast(q.x);
		writer.WriteSingleFast(q.y);
		writer.WriteSingleFast(q.z);
		writer.WriteSingleFast(q.w);
	}

	public static Quaternion ReadQuaternion(this BinaryReader reader)
	{
		return new Quaternion
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle(),
			w = reader.ReadSingle()
		};
	}

	public static Color ColorFromHex(string hex)
	{
		int num = Convert.ToInt32(hex, 16);
		float num2 = 1f;
		float num3 = 1f;
		float num4 = 1f;
		float num5 = 1f;
		if (hex.Length == 6)
		{
			num2 = (float)((num >> 16) & 255);
			num2 /= 255f;
			num3 = (float)((num >> 8) & 255);
			num3 /= 255f;
			num4 = (float)(num & 255);
			num4 /= 255f;
		}
		else if (hex.Length == 8)
		{
			num2 = (float)((num >> 24) & 255);
			num2 /= 255f;
			num3 = (float)((num >> 16) & 255);
			num3 /= 255f;
			num4 = (float)((num >> 8) & 255);
			num4 /= 255f;
			num5 = (float)(num & 255);
			num5 /= 255f;
		}
		return new Color(num2, num3, num4, num5);
	}

	public static string ToHexString(this Color c)
	{
		return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
		{
			(int)(c.r * 255f),
			(int)(c.g * 255f),
			(int)(c.b * 255f),
			(int)(c.a * 255f)
		});
	}

	public static string ToHexString(this Color32 c)
	{
		return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
		{
			(int)c.r,
			(int)c.g,
			(int)c.b,
			(int)c.a
		});
	}

	public static void Signal(this global::System.Action action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void Signal<T>(this Action<T> action, T parameter)
	{
		if (action != null)
		{
			action(parameter);
		}
	}

	public static RectTransform rectTransform(this GameObject go)
	{
		return go.GetComponent<RectTransform>();
	}

	public static RectTransform rectTransform(this Component cmp)
	{
		return cmp.GetComponent<RectTransform>();
	}

	public static T FindFirst<T>(this T[] array, Func<T, bool> condition) where T : class
	{
		if (condition != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && condition(array[i]))
				{
					return array[i];
				}
			}
			return default(T);
		}
		if (array != null && array.Length != 0)
		{
			return array[0];
		}
		return default(T);
	}

	public static T[] Append<T>(this T[] array, T item)
	{
		T[] array2 = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		array2[array.Length] = item;
		return array2;
	}

	public static T[] Append<T>(this T[] array, T[] items)
	{
		T[] array2 = new T[array.Length + items.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		for (int j = 0; j < items.Length; j++)
		{
			array2[array.Length + j] = items[j];
		}
		return array2;
	}

	public static T[] Append<T>(this T[] array, List<T> items)
	{
		if (items == null)
		{
			return array;
		}
		T[] array2 = new T[array.Length + items.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		for (int j = 0; j < items.Count; j++)
		{
			array2[array.Length + j] = items[j];
		}
		return array2;
	}

	public static T[] Concat<T>(this T[] a1, T[] a2)
	{
		T[] array = new T[a1.Length + a2.Length];
		a1.CopyTo(array, 0);
		a2.CopyTo(array, a1.Length);
		return array;
	}

	public static string GetOperatingSystem()
	{
		return Util.operatingSystem;
	}

	public static string GetKleiRootPath()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Klei");
		}
		return Util.defaultRootFolder;
	}

	public static string GetTitleFolderName()
	{
		return "OxygenNotIncluded";
	}

	public static string GetRetiredColoniesFolderName()
	{
		return "RetiredColonies";
	}

	public static string GetKleiItemUserDataFolderName()
	{
		return "KleiItemData";
	}

	public static string RootFolder()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			return Path.Combine(Util.GetKleiRootPath(), Util.GetTitleFolderName());
		}
		return Util.GetKleiRootPath();
	}

	public static string LogFilePath()
	{
		return Util.consoleLogPath;
	}

	public static string LogsFolder()
	{
		return Path.GetDirectoryName(Util.consoleLogPath);
	}

	public static string CacheFolder()
	{
		return Path.Combine(Util.defaultRootFolder, "cache");
	}

	public static Transform FindTransformRecursive(Transform node, string name)
	{
		if (node.name == name)
		{
			return node;
		}
		for (int i = 0; i < node.childCount; i++)
		{
			Transform transform = Util.FindTransformRecursive(node.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static Vector3 ReadVector3(this IReader reader)
	{
		return new Vector3
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle()
		};
	}

	public static Quaternion ReadQuaternion(this IReader reader)
	{
		return new Quaternion
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle(),
			w = reader.ReadSingle()
		};
	}

	public static T GetRandom<T>(this T[] tArray)
	{
		return tArray[global::UnityEngine.Random.Range(0, tArray.Length)];
	}

	public static T GetRandom<T>(this List<T> tList)
	{
		return tList[global::UnityEngine.Random.Range(0, tList.Count)];
	}

	public static T GetRandom<T>(this IEnumerable<T> tEnumerable)
	{
		return tEnumerable.Shuffle<T>().First<T>();
	}

	public static void ShuffleList(IList list_or_array, global::System.Random random)
	{
		for (int i = list_or_array.Count - 1; i > 0; i--)
		{
			int num = random.Next(i + 1);
			Util.Swap(list_or_array, in i, in num);
		}
	}

	public static void ShiftLeft<T>(this T[] array, T new_ending_value = default(T))
	{
		Array.Copy(array, 1, array, 0, array.Length - 1);
		array[array.Length - 1] = new_ending_value;
	}

	public static void ShiftRight<T>(this T[] array, T new_starting_value = default(T))
	{
		Array.Copy(array, 0, array, 1, array.Length - 1);
		array[0] = new_starting_value;
	}

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, global::System.Random random)
	{
		List<T> list = enumerable.ToList<T>();
		Util.ShuffleList(list, random);
		return list;
	}

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
	{
		return enumerable.Shuffle<T>(new global::System.Random());
	}

	public static IOrderedEnumerable<T> StableSort<T>(this IEnumerable<T> enumerable)
	{
		return enumerable.OrderBy<T, T>((T t) => t);
	}

	public static IOrderedEnumerable<T> StableSort<T>(this IEnumerable<T> enumerable, Comparer<T> comparer)
	{
		return enumerable.OrderBy<T, T>((T t) => t, comparer);
	}

	public static IOrderedEnumerable<T> StableSort<T>(this IEnumerable<T> enumerable, Comparison<T> comparer)
	{
		return enumerable.OrderBy<T, T>((T t) => t, Comparer<T>.Create(comparer));
	}

	public static IOrderedEnumerable<T> StableSort<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> key_selector)
	{
		return enumerable.OrderBy<T, TKey>(key_selector);
	}

	public static IOrderedEnumerable<T> StableSort<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> key_selector, Comparer<TKey> comparer)
	{
		return enumerable.OrderBy<T, TKey>(key_selector, comparer);
	}

	public static IOrderedEnumerable<T> StableSort<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> key_selector, Comparison<TKey> comparer)
	{
		return enumerable.OrderBy<T, TKey>(key_selector, Comparer<TKey>.Create(comparer));
	}

	public static float RandomVariance(float center, float plusminus)
	{
		return center + global::UnityEngine.Random.Range(-plusminus, plusminus);
	}

	public static bool IsNullOrWhiteSpace(this string str)
	{
		return string.IsNullOrEmpty(str) || str == " ";
	}

	public static void ApplyInvariantCultureToThread(Thread thread)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor)
		{
			thread.CurrentCulture = CultureInfo.InvariantCulture;
		}
	}

	public static bool IsNullOrDestroyed(this object obj)
	{
		if (obj == null)
		{
			return true;
		}
		global::UnityEngine.Object @object = obj as global::UnityEngine.Object;
		return @object != null && @object == null;
	}

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> self, out TKey key, out TValue value)
	{
		key = self.Key;
		value = self.Value;
	}

	public static void Deconstruct<T, U>(this Pair<T, U> self, out T first, out U second)
	{
		first = self.first;
		second = self.second;
	}

	public static int IntPow(int x, int pow)
	{
		int num = 1;
		while (pow != 0)
		{
			if ((pow & 1) == 1)
			{
				num *= x;
			}
			x *= x;
			pow >>= 1;
		}
		return num;
	}

	public static string DebugToCommaSeparatedList(this string[] list)
	{
		if (list == null || list.Length == 0)
		{
			return string.Empty;
		}
		return string.Join(", ", list);
	}

	private static HashSet<char> defaultInvalidUserInputChars = new HashSet<char>(Path.GetInvalidPathChars());

	private static HashSet<char> additionalInvalidUserInputChars = new HashSet<char>(new char[] { '<', '>', ':', '"', '/', '?', '*', '\\', '!', '.' });

	private static KRandom random = new KRandom();

	private static string defaultRootFolder = Application.persistentDataPath;

	private static string consoleLogPath = Application.consoleLogPath;

	private static string operatingSystem = SystemInfo.operatingSystem;

	public enum IterationInstruction : byte
	{
		Continue,
		Halt
	}
}
