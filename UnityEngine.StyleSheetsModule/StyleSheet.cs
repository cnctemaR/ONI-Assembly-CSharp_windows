using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal class StyleSheet : ScriptableObject
	{
		public StyleRule[] rules
		{
			get
			{
				return this.m_Rules;
			}
			internal set
			{
				this.m_Rules = value;
				this.SetupReferences();
			}
		}

		public StyleComplexSelector[] complexSelectors
		{
			get
			{
				return this.m_ComplexSelectors;
			}
			internal set
			{
				this.m_ComplexSelectors = value;
				this.SetupReferences();
			}
		}

		private static bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle[] handles, int index, out T value)
		{
			bool flag = false;
			value = default(T);
			if (index < handles.Length)
			{
				StyleValueHandle styleValueHandle = handles[index];
				if (styleValueHandle.valueType == type && styleValueHandle.valueIndex >= 0 && styleValueHandle.valueIndex < list.Length)
				{
					value = list[styleValueHandle.valueIndex];
					flag = true;
				}
				else
				{
					Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", new object[] { type, styleValueHandle.valueType });
				}
			}
			return flag;
		}

		private static T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
		{
			T t = default(T);
			if (handle.valueType != type)
			{
				Debug.LogErrorFormat("Trying to read value of type {0} while reading a value of type {1}", new object[] { type, handle.valueType });
			}
			else if (handle.valueIndex < 0 && handle.valueIndex >= list.Length)
			{
				Debug.LogError("Accessing invalid property");
			}
			else
			{
				t = list[handle.valueIndex];
			}
			return t;
		}

		private void OnEnable()
		{
			this.SetupReferences();
		}

		private void SetupReferences()
		{
			if (this.complexSelectors != null && this.rules != null)
			{
				for (int i = 0; i < this.complexSelectors.Length; i++)
				{
					StyleComplexSelector styleComplexSelector = this.complexSelectors[i];
					if (styleComplexSelector.ruleIndex < this.rules.Length)
					{
						styleComplexSelector.rule = this.rules[styleComplexSelector.ruleIndex];
					}
				}
			}
		}

		public StyleValueKeyword ReadKeyword(StyleValueHandle handle)
		{
			return (StyleValueKeyword)handle.valueIndex;
		}

		public float ReadFloat(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<float>(this.floats, StyleValueType.Float, handle);
		}

		public bool TryReadFloat(StyleValueHandle[] handles, int index, out float value)
		{
			return StyleSheet.TryCheckAccess<float>(this.floats, StyleValueType.Float, handles, index, out value);
		}

		public Color ReadColor(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<Color>(this.colors, StyleValueType.Color, handle);
		}

		public bool TryReadColor(StyleValueHandle[] handles, int index, out Color value)
		{
			return StyleSheet.TryCheckAccess<Color>(this.colors, StyleValueType.Color, handles, index, out value);
		}

		public string ReadString(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.String, handle);
		}

		public bool TryReadString(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.String, handles, index, out value);
		}

		public string ReadEnum(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.Enum, handle);
		}

		public bool TryReadEnum(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.Enum, handles, index, out value);
		}

		public string ReadResourcePath(StyleValueHandle handle)
		{
			return StyleSheet.CheckAccess<string>(this.strings, StyleValueType.ResourcePath, handle);
		}

		public bool TryReadResourcePath(StyleValueHandle[] handles, int index, out string value)
		{
			return StyleSheet.TryCheckAccess<string>(this.strings, StyleValueType.ResourcePath, handles, index, out value);
		}

		[SerializeField]
		private StyleRule[] m_Rules;

		[SerializeField]
		private StyleComplexSelector[] m_ComplexSelectors;

		[SerializeField]
		internal float[] floats;

		[SerializeField]
		internal Color[] colors;

		[SerializeField]
		internal string[] strings;
	}
}
