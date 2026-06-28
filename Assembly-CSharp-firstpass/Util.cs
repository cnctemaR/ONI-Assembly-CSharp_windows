using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public static class Util
{
	public static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	public static Vector3 Position(GameObject o)
	{
		return o.transform.position;
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
			Output.LogErrorWithObj(go, new object[] { string.Format("{0} '{1}' requires a component of type {2}!", go.GetType().ToString(), go.name, name) });
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
			Output.LogErrorWithObj(cmp.gameObject, new object[] { string.Format("{0} '{1}' requires a component of type {2} as requested by {3}!", new object[]
			{
				cmp.gameObject.GetType().ToString(),
				cmp.gameObject.name,
				typeof(T).ToString(),
				cmp.GetType().ToString()
			}) });
			return (T)((object)null);
		}
		Util.InitializeComponent(component);
		return component;
	}

	public static T RequireComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (component == null)
		{
			Output.LogErrorWithObj(gameObject, new object[] { string.Format("{0} '{1}' requires a component of type {2}!", gameObject.GetType().ToString(), gameObject.name, typeof(T).ToString()) });
			return (T)((object)null);
		}
		Util.InitializeComponent(component);
		return component;
	}

	public static T GetFirstChildComponent<T>(this Component c) where T : Component
	{
		for (int i = 0; i < c.transform.childCount; i++)
		{
			GameObject gameObject = c.transform.GetChild(i).gameObject;
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
		}
		return (T)((object)null);
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		Util.SetLayer(go.transform, layer);
	}

	public static void SetLayer(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		IEnumerator enumerator = t.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				Util.SetLayer(transform, layer);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void KDestroyGameObject(Component original)
	{
		Util.KDestroyGameObject(original.gameObject);
	}

	public static void KDestroyGameObject(GameObject original)
	{
		original.DeleteObject();
	}

	public static T FindOrAddComponent<T>(this Component cmp) where T : KMonoBehaviour
	{
		return cmp.gameObject.FindOrAddComponent<T>();
	}

	public static T FindOrAddComponent<T>(this GameObject go) where T : KMonoBehaviour
	{
		T t = go.GetComponent<T>();
		if (t == null)
		{
			t = go.AddComponent<T>();
			if (!KMonoBehaviour.isPoolPreInit && !t.IsInitialized())
			{
				DebugUtil.Assert(t.IsInitialized(), "Could not find component " + typeof(T).ToString() + " on object " + go.ToString());
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
		foreach (KMonoBehaviour kmonoBehaviour in go.GetComponents<KMonoBehaviour>())
		{
			kmonoBehaviour.InitializeComponent();
		}
		KMonoBehaviour.isPoolPreInit = false;
	}

	public static void SetActive(GameObject go, bool active)
	{
		if (go.activeSelf != active)
		{
			go.SetActive(active);
		}
	}

	public static GameObject KInstantiateClearTransform(GameObject original, GameObject parent = null, string name = null)
	{
		GameObject gameObject = Util.KInstantiate(original, parent, name);
		Util.Reset(gameObject.transform);
		return gameObject;
	}

	public static GameObject KInstantiate(GameObject original, Folder folder)
	{
		return Util.KInstantiate(original, Vector3.zero, Quaternion.identity, SceneOrganizer.Instance.GetFolder(folder), null, true, 0);
	}

	public static GameObject KInstantiate(Component original, Folder folder)
	{
		return Util.KInstantiate(original.gameObject, folder);
	}

	public static T KInstantiate<T>(Component original, Folder folder)
	{
		return Util.KInstantiate<T>(original.gameObject, folder);
	}

	public static T KInstantiate<T>(GameObject original, Folder folder)
	{
		return Util.KInstantiate(original, folder).GetComponent<T>();
	}

	public static T KInstantiate<T>(Component original, GameObject parent)
	{
		return Util.KInstantiate(original.gameObject, parent, null).GetComponent<T>();
	}

	public static T KInstantiate<T>(GameObject original, GameObject parent)
	{
		return Util.KInstantiate(original, parent, null).GetComponent<T>();
	}

	public static GameObject KInstantiate(Component original, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original.gameObject, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiatePrecise(GameObject original, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original, original.transform.position, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject original, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original, Vector3.zero, Quaternion.identity, parent, name, true, 0);
	}

	public static GameObject KInstantiate(Component original, Vector3 position, Quaternion rotation, GameObject parent = null, string name = null)
	{
		return Util.KInstantiate(original.gameObject, position, rotation, parent, name, true, 0);
	}

	public static GameObject KInstantiate(GameObject go, Vector3 position, Quaternion rotation, Folder folder)
	{
		return Util.KInstantiate(go, position, rotation, SceneOrganizer.Instance.GetFolder(folder), null, true, 0);
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
			Output.LogWarning(new object[] { "Missing prefab" });
		}
		if (gameObject == null)
		{
			gameObject = global::UnityEngine.Object.Instantiate<GameObject>(original, position, rotation);
			if (gameLayer != 0)
			{
				gameObject.SetLayerRecursively(gameLayer);
			}
			if (parent != null)
			{
				if (gameObject.GetComponent<RectTransform>() != null)
				{
					gameObject.transform.SetParent(parent.transform, true);
				}
				else
				{
					gameObject.transform.parent = parent.transform;
				}
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
			}
			KPrefabIDTracker.Get().Register(component, original.GetComponent<KPrefabID>());
			KPrefabID component2 = original.GetComponent<KPrefabID>();
			component.CopyInitFunctions(component2);
			component.RunInstantiateFn();
		}
		return gameObject;
	}

	public static T KInstantiateUI<T>(GameObject original, GameObject parent = null, bool force_active = false) where T : MonoBehaviour
	{
		GameObject gameObject = Util.KInstantiateUI(original, parent, force_active);
		return gameObject.GetComponent<T>();
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
			Output.LogWarning(new object[] { "Missing prefab" });
		}
		if (gameObject == null)
		{
			gameObject = global::UnityEngine.Object.Instantiate<GameObject>(original);
			if (parent != null)
			{
				gameObject.transform.SetParent(parent.transform, false);
			}
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

	public static T KAddComponent<T>(this Component c) where T : KMonoBehaviour
	{
		return c.gameObject.KAddComponent<T>();
	}

	public static T KAddComponent<T>(this GameObject owner) where T : KMonoBehaviour
	{
		T t = owner.AddComponent<T>();
		t.InitializeComponent();
		return t;
	}

	public static T UpdateComponentRequirement<T>(this GameObject go, bool required = true) where T : Component
	{
		T t = go.GetComponent(typeof(T)) as T;
		if (!required && t != null)
		{
			global::UnityEngine.Object.DestroyImmediate(t, true);
			t = (T)((object)null);
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

	public static bool IsInputCharacterValid(char _char, bool isPath = false)
	{
		return !Util.defaultInvalidUserInputChars.Contains(_char) && (isPath || !Util.additionalInvalidUserInputChars.Contains(_char));
	}

	public static void ScrubInputField(TMP_InputField inputField, bool isPath = false)
	{
		for (int i = inputField.text.Length - 1; i >= 0; i--)
		{
			if (i < inputField.text.Length)
			{
				if (!Util.IsInputCharacterValid(inputField.text[i], isPath))
				{
					inputField.text = inputField.text.Remove(i, 1);
				}
			}
		}
	}

	public static string StripTextFormatting(string original)
	{
		return Regex.Replace(original, "<[^>]*>([^<]*)<[^>]*>", "$1");
	}

	public static bool Contains(this Bounds parentBounds, Bounds queryBounds)
	{
		return parentBounds.Contains(queryBounds.max) && parentBounds.Contains(queryBounds.min);
	}

	public static Bounds DeepRenderBounds(GameObject obj)
	{
		Bounds bounds = default(Bounds);
		Renderer[] componentsInChildren = obj.transform.GetComponentsInChildren<Renderer>();
		if (componentsInChildren.Length >= 1)
		{
			bounds = componentsInChildren[0].bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
		}
		return bounds;
	}

	public static GameObject FindChildGameObject(this Transform root, string name)
	{
		if (root == null)
		{
			return null;
		}
		if (root.name == name)
		{
			return root.gameObject;
		}
		GameObject gameObject = null;
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				gameObject = transform.FindChildGameObject(name);
				if (gameObject != null)
				{
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		return gameObject;
	}

	public static bool IsChildOf(this GameObject testChild, GameObject testParent)
	{
		if (testChild == testParent)
		{
			return true;
		}
		Transform transform = testChild.transform;
		while (transform.parent != null)
		{
			if (transform.parent.gameObject == testParent)
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	public static bool HasMethod(this object obj, string method)
	{
		return obj.GetType().GetMethod(method) != null;
	}

	public static GameObject CreateFolder(string name, Component c)
	{
		return Util.CreateFolder(name, c.gameObject);
	}

	public static GameObject CreateFolder(string name, GameObject parent)
	{
		return new GameObject
		{
			name = name,
			transform = 
			{
				parent = parent.transform
			}
		};
	}

	public static void ZeroTransform(Transform child)
	{
		child.localPosition = Vector3.zero;
		child.localRotation = Quaternion.identity;
		child.localScale = Vector3.one;
	}

	public static void Parent(GameObject child, GameObject parent)
	{
		child.transform.parent = parent.transform;
	}

	public static GameObject GetSubFolder(GameObject go, string name)
	{
		if (go != null)
		{
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					if (transform.gameObject.name == name)
					{
						return transform.gameObject;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
		return null;
	}

	public static void Reset(Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static int RandomInt(int b, int e)
	{
		return Util.random.Next(b, e);
	}

	public static int RandomInt()
	{
		return Util.random.Next();
	}

	public static float GaussianRandom(float mu = 0f, float sigma = 1f)
	{
		double num = Util.random.NextDouble();
		double num2 = Util.random.NextDouble();
		double num3 = (double)(Mathf.Sqrt(-2f * Mathf.Log((float)num)) * Mathf.Sin(6.2831855f * (float)num2));
		double num4 = (double)mu + (double)sigma * num3;
		return (float)num4;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		list.ShuffleSeeded<T>(Util.random);
	}

	public static void CopyTransform(Transform source, Transform dest)
	{
		dest.localPosition = source.localPosition;
		dest.rotation = source.rotation;
		dest.localScale = source.localScale;
	}

	public static Vector2 ScreenToGUI(Vector2 pos)
	{
		pos.x = pos.x / (float)Screen.width * 1280f;
		pos.y = pos.y / (float)Screen.height / 1.7777778f * 1280f;
		return pos;
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
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					Util.GetBounds(transform.gameObject, ref bounds, ref first);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}

	public static bool IsOnLeftSideOfScreen(Vector3 position)
	{
		return position.x < (float)Screen.width;
	}

	public static void Write(this BinaryWriter writer, Vector2 v)
	{
		writer.Write(v.x);
		writer.Write(v.y);
	}

	public static void Write(this BinaryWriter writer, Vector3 v)
	{
		writer.Write(v.x);
		writer.Write(v.y);
		writer.Write(v.z);
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
		writer.Write(q.x);
		writer.Write(q.y);
		writer.Write(q.z);
		writer.Write(q.w);
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

	public static string RootFolder()
	{
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return Path.Combine(folderPath, "Klei/OxygenNotIncluded");
		}
		return Util.defaultRootFolder;
	}

	public static T GetComponentInChildren<T>(this GameObject go, bool include_inactive) where T : Component
	{
		if (!include_inactive)
		{
			return go.GetComponentInChildren<T>();
		}
		T[] componentsInChildren = go.GetComponentsInChildren<T>(true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			return componentsInChildren[0];
		}
		return (T)((object)null);
	}

	public static T GetComponentInChildren<T>(this Component cmp, bool include_inactive) where T : Component
	{
		return cmp.gameObject.GetComponentInChildren<T>(include_inactive);
	}

	public static Transform FindTransformRecursive(Transform node, string name)
	{
		if (node.name == name)
		{
			return node;
		}
		IEnumerator enumerator = node.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				Transform transform2 = Util.FindTransformRecursive(transform, name);
				if (transform2 != null)
				{
					return transform2;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static void SkipKleiString(this BinaryReader reader)
	{
		int num = reader.ReadInt32();
		reader.Read(Util.chars, 0, num);
	}

	public static string ReadKleiString(this BinaryReader reader)
	{
		int num = reader.ReadInt32();
		reader.Read(Util.chars, 0, num);
		return new string(Util.chars, 0, num);
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

	public static float RandomVariance(float center, float plusminus)
	{
		return center + global::UnityEngine.Random.Range(-plusminus, plusminus);
	}

	public const float OneOverRoot2 = 0.70710677f;

	private static HashSet<char> defaultInvalidUserInputChars = new HashSet<char>(Path.GetInvalidPathChars());

	private static HashSet<char> additionalInvalidUserInputChars = new HashSet<char>(new char[] { '<', '>', ':', '"', '/', '?', '*', '\\', '!' });

	private static global::System.Random random = new global::System.Random();

	private static string defaultRootFolder = Application.persistentDataPath;

	private static char[] chars = new char[128];
}
