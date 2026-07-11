using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class StylePropertyReader : IStylePropertyReader
	{
		public StyleProperty property { get; private set; }

		public StylePropertyID propertyID { get; private set; }

		public int valueCount { get; private set; }

		public int specificity { get; private set; }

		public float dpiScaling { get; private set; }

		public void SetContext(StyleSheet sheet, StyleComplexSelector selector, StyleVariableContext varContext, float dpiScaling = 1f)
		{
			this.m_Sheet = sheet;
			this.m_Properties = selector.rule.properties;
			this.m_PropertyIDs = StyleSheetCache.GetPropertyIDs(sheet, selector.ruleIndex);
			this.m_Resolver.variableContext = varContext;
			this.specificity = (sheet.isUnityStyleSheet ? (-1) : selector.specificity);
			this.dpiScaling = dpiScaling;
			this.LoadProperties();
		}

		public void SetInlineContext(StyleSheet sheet, StyleRule rule, int ruleIndex, float dpiScaling = 1f)
		{
			this.m_Sheet = sheet;
			this.m_Properties = rule.properties;
			this.m_PropertyIDs = StyleSheetCache.GetPropertyIDs(sheet, ruleIndex);
			this.specificity = int.MaxValue;
			this.dpiScaling = dpiScaling;
			this.LoadProperties();
		}

		public StylePropertyID MoveNextProperty()
		{
			this.m_CurrentPropertyIndex++;
			this.m_CurrentValueIndex += this.valueCount;
			this.SetCurrentProperty();
			return this.propertyID;
		}

		public StylePropertyValue GetValue(int index)
		{
			return this.m_Values[this.m_CurrentValueIndex + index];
		}

		public StyleValueType GetValueType(int index)
		{
			return this.m_Values[this.m_CurrentValueIndex + index].handle.valueType;
		}

		public bool IsValueType(int index, StyleValueType type)
		{
			return this.m_Values[this.m_CurrentValueIndex + index].handle.valueType == type;
		}

		public bool IsKeyword(int index, StyleValueKeyword keyword)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.handle.valueType == StyleValueType.Keyword && stylePropertyValue.handle.valueIndex == (int)keyword;
		}

		public string ReadAsString(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
		}

		public StyleLength ReadStyleLength(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			bool flag = stylePropertyValue.handle.valueType == StyleValueType.Keyword;
			StyleLength styleLength2;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)stylePropertyValue.handle.valueIndex;
				StyleLength styleLength = new StyleLength(valueIndex.ToStyleKeyword())
				{
					specificity = this.specificity
				};
				styleLength2 = styleLength;
			}
			else
			{
				StyleLength styleLength = new StyleLength(stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToLength())
				{
					specificity = this.specificity
				};
				styleLength2 = styleLength;
			}
			return styleLength2;
		}

		public StyleFloat ReadStyleFloat(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return new StyleFloat(stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle))
			{
				specificity = this.specificity
			};
		}

		public StyleInt ReadStyleInt(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return new StyleInt((int)stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle))
			{
				specificity = this.specificity
			};
		}

		public StyleColor ReadStyleColor(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			Color color = Color.clear;
			bool flag = stylePropertyValue.handle.valueType == StyleValueType.Enum;
			if (flag)
			{
				string text = stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
				StyleSheetColor.TryGetColor(text.ToLower(), out color);
			}
			else
			{
				color = stylePropertyValue.sheet.ReadColor(stylePropertyValue.handle);
			}
			return new StyleColor(color)
			{
				specificity = this.specificity
			};
		}

		public StyleInt ReadStyleEnum<T>(int index)
		{
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			return new StyleInt(StyleSheetCache.GetEnumValue<T>(stylePropertyValue.sheet, stylePropertyValue.handle))
			{
				specificity = this.specificity
			};
		}

		public StyleFont ReadStyleFont(int index)
		{
			Font font = null;
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			StyleValueType valueType = stylePropertyValue.handle.valueType;
			if (valueType != StyleValueType.ResourcePath)
			{
				if (valueType != StyleValueType.AssetReference)
				{
					Debug.LogWarning("Invalid value for font " + stylePropertyValue.handle.valueType);
				}
				else
				{
					font = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Font;
					bool flag = font == null;
					if (flag)
					{
						Debug.LogWarning("Invalid font reference");
					}
				}
			}
			else
			{
				string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
				bool flag2 = !string.IsNullOrEmpty(text);
				if (flag2)
				{
					font = Panel.LoadResource(text, typeof(Font), this.dpiScaling) as Font;
				}
				bool flag3 = font == null;
				if (flag3)
				{
					Debug.LogWarning(string.Format("Font not found for path: {0}", text));
				}
			}
			return new StyleFont(font)
			{
				specificity = this.specificity
			};
		}

		public StyleBackground ReadStyleBackground(int index)
		{
			ImageSource imageSource = default(ImageSource);
			StylePropertyValue stylePropertyValue = this.m_Values[this.m_CurrentValueIndex + index];
			bool flag = stylePropertyValue.handle.valueType == StyleValueType.Keyword;
			if (flag)
			{
				bool flag2 = stylePropertyValue.handle.valueIndex != 6;
				if (flag2)
				{
					Debug.LogWarning("Invalid keyword for image source " + (StyleValueKeyword)stylePropertyValue.handle.valueIndex);
				}
			}
			else
			{
				bool flag3 = !StylePropertyReader.TryGetImageSourceFromValue(stylePropertyValue, this.dpiScaling, out imageSource);
				if (flag3)
				{
				}
			}
			bool flag4 = imageSource.texture != null;
			StyleBackground styleBackground2;
			if (flag4)
			{
				StyleBackground styleBackground = new StyleBackground(imageSource.texture)
				{
					specificity = this.specificity
				};
				styleBackground2 = styleBackground;
			}
			else
			{
				bool flag5 = imageSource.vectorImage != null;
				if (flag5)
				{
					StyleBackground styleBackground = new StyleBackground(imageSource.vectorImage)
					{
						specificity = this.specificity
					};
					styleBackground2 = styleBackground;
				}
				else
				{
					StyleBackground styleBackground = new StyleBackground
					{
						specificity = this.specificity
					};
					styleBackground2 = styleBackground;
				}
			}
			return styleBackground2;
		}

		public StyleCursor ReadStyleCursor(int index)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			Texture2D texture2D = null;
			StyleValueType valueType = this.GetValueType(index);
			bool flag = valueType == StyleValueType.ResourcePath || valueType == StyleValueType.AssetReference || valueType == StyleValueType.ScalableImage;
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = this.valueCount < 1;
				if (flag3)
				{
					Debug.LogWarning(string.Format("USS 'cursor' has invalid value at {0}.", index));
				}
				else
				{
					ImageSource imageSource = default(ImageSource);
					StylePropertyValue value = this.GetValue(index);
					bool flag4 = StylePropertyReader.TryGetImageSourceFromValue(value, this.dpiScaling, out imageSource);
					if (flag4)
					{
						texture2D = imageSource.texture;
						bool flag5 = this.valueCount >= 3;
						if (flag5)
						{
							StylePropertyValue value2 = this.GetValue(index + 1);
							StylePropertyValue value3 = this.GetValue(index + 2);
							bool flag6 = value2.handle.valueType != StyleValueType.Float || value3.handle.valueType != StyleValueType.Float;
							if (flag6)
							{
								Debug.LogWarning("USS 'cursor' property requires two integers for the hot spot value.");
							}
							else
							{
								num = value2.sheet.ReadFloat(value2.handle);
								num2 = value3.sheet.ReadFloat(value3.handle);
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = StylePropertyReader.getCursorIdFunc != null;
				if (flag7)
				{
					StylePropertyValue value4 = this.GetValue(index);
					num3 = StylePropertyReader.getCursorIdFunc(value4.sheet, value4.handle);
				}
			}
			Cursor cursor = new Cursor
			{
				texture = texture2D,
				hotspot = new Vector2(num, num2),
				defaultCursorId = num3
			};
			return new StyleCursor(cursor)
			{
				specificity = this.specificity
			};
		}

		private void LoadProperties()
		{
			this.m_CurrentPropertyIndex = 0;
			this.m_CurrentValueIndex = 0;
			this.m_Values.Clear();
			this.m_ValueCount.Clear();
			foreach (StyleProperty styleProperty in this.m_Properties)
			{
				int num = 0;
				bool flag = true;
				bool requireVariableResolve = styleProperty.requireVariableResolve;
				if (requireVariableResolve)
				{
					this.m_Resolver.Init(styleProperty, this.m_Sheet, styleProperty.values);
					int num2 = 0;
					while (num2 < styleProperty.values.Length && flag)
					{
						StyleValueHandle styleValueHandle = styleProperty.values[num2];
						bool flag2 = styleValueHandle.IsVarFunction();
						if (flag2)
						{
							StyleVariableResolver.Result result = this.m_Resolver.ResolveVarFunction(ref num2);
							bool flag3 = result > StyleVariableResolver.Result.Valid;
							if (flag3)
							{
								StyleValueHandle styleValueHandle2 = new StyleValueHandle
								{
									valueType = StyleValueType.Keyword,
									valueIndex = 3
								};
								this.m_Values.Add(new StylePropertyValue
								{
									sheet = this.m_Sheet,
									handle = styleValueHandle2
								});
								num++;
								flag = false;
							}
						}
						else
						{
							this.m_Resolver.AddValue(styleValueHandle);
						}
						num2++;
					}
					bool flag4 = flag;
					if (flag4)
					{
						this.m_Values.AddRange(this.m_Resolver.resolvedValues);
						num += this.m_Resolver.resolvedValues.Count;
					}
				}
				else
				{
					num = styleProperty.values.Length;
					for (int j = 0; j < num; j++)
					{
						StyleValueHandle styleValueHandle3 = styleProperty.values[j];
						this.m_Values.Add(new StylePropertyValue
						{
							sheet = this.m_Sheet,
							handle = styleProperty.values[j]
						});
					}
				}
				this.m_ValueCount.Add(num);
			}
			this.SetCurrentProperty();
		}

		private void SetCurrentProperty()
		{
			bool flag = this.m_CurrentPropertyIndex < this.m_PropertyIDs.Length;
			if (flag)
			{
				this.property = this.m_Properties[this.m_CurrentPropertyIndex];
				this.propertyID = this.m_PropertyIDs[this.m_CurrentPropertyIndex];
				this.valueCount = this.m_ValueCount[this.m_CurrentPropertyIndex];
			}
			else
			{
				this.property = null;
				this.propertyID = StylePropertyID.Unknown;
				this.valueCount = 0;
			}
		}

		internal static bool TryGetImageSourceFromValue(StylePropertyValue propertyValue, float dpiScaling, out ImageSource source)
		{
			source = default(ImageSource);
			StyleValueType valueType = propertyValue.handle.valueType;
			if (valueType != StyleValueType.ResourcePath)
			{
				if (valueType != StyleValueType.AssetReference)
				{
					if (valueType != StyleValueType.ScalableImage)
					{
						Debug.LogWarning("Invalid value for image texture " + propertyValue.handle.valueType);
						return false;
					}
					ScalableImage scalableImage = propertyValue.sheet.ReadScalableImage(propertyValue.handle);
					bool flag = scalableImage.normalImage == null && scalableImage.highResolutionImage == null;
					if (flag)
					{
						Debug.LogWarning("Invalid scalable image specified");
						return false;
					}
					source.texture = scalableImage.normalImage;
					bool flag2 = !Mathf.Approximately(dpiScaling % 1f, 0f);
					if (flag2)
					{
						source.texture.filterMode = FilterMode.Bilinear;
					}
				}
				else
				{
					Object @object = propertyValue.sheet.ReadAssetReference(propertyValue.handle);
					source.texture = @object as Texture2D;
					source.vectorImage = @object as VectorImage;
					bool flag3 = source.texture == null && source.vectorImage == null;
					if (flag3)
					{
						Debug.LogWarning("Invalid image specified");
						return false;
					}
				}
			}
			else
			{
				string text = propertyValue.sheet.ReadResourcePath(propertyValue.handle);
				bool flag4 = !string.IsNullOrEmpty(text);
				if (flag4)
				{
					source.texture = Panel.LoadResource(text, typeof(Texture2D), dpiScaling) as Texture2D;
					bool flag5 = source.texture == null;
					if (flag5)
					{
						source.vectorImage = Panel.LoadResource(text, typeof(VectorImage), dpiScaling) as VectorImage;
					}
				}
				bool flag6 = source.texture == null && source.vectorImage == null;
				if (flag6)
				{
					Debug.LogWarning(string.Format("Image not found for path: {0}", text));
					return false;
				}
			}
			return true;
		}

		internal static StylePropertyReader.GetCursorIdFunction getCursorIdFunc = null;

		private List<StylePropertyValue> m_Values = new List<StylePropertyValue>();

		private List<int> m_ValueCount = new List<int>();

		private StyleVariableResolver m_Resolver = new StyleVariableResolver();

		private StyleSheet m_Sheet;

		private StyleProperty[] m_Properties;

		private StylePropertyID[] m_PropertyIDs;

		private int m_CurrentValueIndex;

		private int m_CurrentPropertyIndex;

		internal delegate int GetCursorIdFunction(StyleSheet sheet, StyleValueHandle handle);
	}
}
