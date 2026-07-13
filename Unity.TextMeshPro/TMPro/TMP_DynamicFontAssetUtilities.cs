using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TMPro
{
	internal class TMP_DynamicFontAssetUtilities
	{
		private void InitializeSystemFontReferenceCache()
		{
			if (this.s_SystemFontLookup == null)
			{
				this.s_SystemFontLookup = new Dictionary<ulong, TMP_DynamicFontAssetUtilities.FontReference>();
			}
			else
			{
				this.s_SystemFontLookup.Clear();
			}
			if (this.s_SystemFontPaths == null)
			{
				this.s_SystemFontPaths = Font.GetPathsToOSFonts();
			}
			for (int i = 0; i < this.s_SystemFontPaths.Length; i++)
			{
				FontEngineError fontEngineError = FontEngine.LoadFontFace(this.s_SystemFontPaths[i]);
				if (fontEngineError != FontEngineError.Success)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Error [",
						fontEngineError.ToString(),
						"] trying to load the font at path [",
						this.s_SystemFontPaths[i],
						"]."
					}));
				}
				else
				{
					string[] fontFaces = FontEngine.GetFontFaces();
					for (int j = 0; j < fontFaces.Length; j++)
					{
						TMP_DynamicFontAssetUtilities.FontReference fontReference = new TMP_DynamicFontAssetUtilities.FontReference(this.s_SystemFontPaths[i], fontFaces[j], j);
						if (!this.s_SystemFontLookup.ContainsKey(fontReference.hashCode))
						{
							this.s_SystemFontLookup.Add(fontReference.hashCode, fontReference);
							Debug.Log(string.Concat(new string[]
							{
								"[",
								i.ToString(),
								"] Family Name [",
								fontReference.familyName,
								"]   Style Name [",
								fontReference.styleName,
								"]   Index [",
								fontReference.faceIndex.ToString(),
								"]   HashCode [",
								fontReference.hashCode.ToString(),
								"]    Path [",
								fontReference.filePath,
								"]."
							}));
						}
					}
					FontEngine.UnloadFontFace();
				}
			}
		}

		public static bool TryGetSystemFontReference(string familyName, out TMP_DynamicFontAssetUtilities.FontReference fontRef)
		{
			return TMP_DynamicFontAssetUtilities.s_Instance.TryGetSystemFontReferenceInternal(familyName, null, out fontRef);
		}

		public static bool TryGetSystemFontReference(string familyName, string styleName, out TMP_DynamicFontAssetUtilities.FontReference fontRef)
		{
			return TMP_DynamicFontAssetUtilities.s_Instance.TryGetSystemFontReferenceInternal(familyName, styleName, out fontRef);
		}

		private bool TryGetSystemFontReferenceInternal(string familyName, string styleName, out TMP_DynamicFontAssetUtilities.FontReference fontRef)
		{
			if (this.s_SystemFontLookup == null)
			{
				this.InitializeSystemFontReferenceCache();
			}
			fontRef = default(TMP_DynamicFontAssetUtilities.FontReference);
			uint hashCodeCaseInSensitive = TMP_TextUtilities.GetHashCodeCaseInSensitive(familyName);
			uint num = (string.IsNullOrEmpty(styleName) ? this.s_RegularStyleNameHashCode : TMP_TextUtilities.GetHashCodeCaseInSensitive(styleName));
			ulong num2 = ((ulong)num << 32) | (ulong)hashCodeCaseInSensitive;
			if (this.s_SystemFontLookup.ContainsKey(num2))
			{
				fontRef = this.s_SystemFontLookup[num2];
				return true;
			}
			if (num != this.s_RegularStyleNameHashCode)
			{
				return false;
			}
			foreach (KeyValuePair<ulong, TMP_DynamicFontAssetUtilities.FontReference> keyValuePair in this.s_SystemFontLookup)
			{
				if (keyValuePair.Value.familyName == familyName)
				{
					fontRef = keyValuePair.Value;
					return true;
				}
			}
			return false;
		}

		private static TMP_DynamicFontAssetUtilities s_Instance = new TMP_DynamicFontAssetUtilities();

		private Dictionary<ulong, TMP_DynamicFontAssetUtilities.FontReference> s_SystemFontLookup;

		private string[] s_SystemFontPaths;

		private uint s_RegularStyleNameHashCode = 1291372090U;

		public struct FontReference
		{
			public FontReference(string fontFilePath, string faceNameAndStyle, int index)
			{
				this.familyName = null;
				this.styleName = null;
				this.faceIndex = index;
				uint num = 0U;
				uint num2 = 0U;
				this.filePath = fontFilePath;
				int length = faceNameAndStyle.Length;
				char[] array = new char[length];
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < length; i++)
				{
					char c = faceNameAndStyle[i];
					if (num3 == 0)
					{
						if (i + 2 < length && c == ' ' && faceNameAndStyle[i + 1] == '-' && faceNameAndStyle[i + 2] == ' ')
						{
							num3 = 1;
							this.familyName = new string(array, 0, num4);
							i += 2;
							num4 = 0;
						}
						else
						{
							num = ((num << 5) + num) ^ (uint)TMP_TextUtilities.ToUpperFast(c);
							array[num4++] = c;
						}
					}
					else if (num3 == 1)
					{
						num2 = ((num2 << 5) + num2) ^ (uint)TMP_TextUtilities.ToUpperFast(c);
						array[num4++] = c;
						if (i + 1 == length)
						{
							this.styleName = new string(array, 0, num4);
						}
					}
				}
				this.hashCode = ((ulong)num2 << 32) | (ulong)num;
			}

			public string familyName;

			public string styleName;

			public int faceIndex;

			public string filePath;

			public ulong hashCode;
		}
	}
}
