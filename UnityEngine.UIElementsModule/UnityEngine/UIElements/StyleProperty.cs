using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal class StyleProperty
	{
		internal StylePropertyId id
		{
			get
			{
				return this.m_Id;
			}
		}

		public string name
		{
			get
			{
				StylePropertyId id = this.id;
				StylePropertyId stylePropertyId = id;
				string text;
				if (stylePropertyId - StylePropertyId.Custom > 1)
				{
					text = StylePropertyUtil.stylePropertyIdToPropertyName[this.id];
				}
				else
				{
					text = this.m_CustomName;
				}
				return text;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.CacheId(value);
			}
		}

		public int line
		{
			get
			{
				return this.m_Line;
			}
			internal set
			{
				this.m_Line = value;
			}
		}

		public StyleValueHandle[] values
		{
			get
			{
				return this.m_Values;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_Values = value;
			}
		}

		internal int handleCount
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				StyleValueHandle[] values = this.m_Values;
				return (values != null) ? values.Length : 0;
			}
		}

		internal bool isCustomProperty
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.id == StylePropertyId.Custom;
			}
		}

		internal StyleProperty()
		{
		}

		internal void CacheId(string value)
		{
			this.m_Id = StylePropertyId.Unknown;
			this.m_CustomName = value;
			bool flag = string.IsNullOrEmpty(value);
			if (flag)
			{
				this.m_Id = StylePropertyId.Unknown;
			}
			else
			{
				bool flag2 = StringUtils.StartsWith(value, "--");
				if (flag2)
				{
					this.m_Id = StylePropertyId.Custom;
				}
				else
				{
					StylePropertyId stylePropertyId;
					bool flag3 = StylePropertyUtil.propertyNameToStylePropertyId.TryGetValue(value, out stylePropertyId);
					if (flag3)
					{
						this.m_Id = stylePropertyId;
						this.m_CustomName = null;
					}
				}
			}
		}

		public bool ContainsVariable()
		{
			foreach (StyleValueHandle styleValueHandle in this.values)
			{
				bool flag = styleValueHandle.IsVarFunction();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasValue()
		{
			return this.handleCount != 0;
		}

		public void ClearValue()
		{
			this.m_Values = Array.Empty<StyleValueHandle>();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetKeyword(StyleSheet styleSheet, StyleValueKeyword value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteKeyword(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetKeyword(StyleSheet styleSheet, out StyleValueKeyword value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadKeyword(this.m_Values[0], out value);
			}
			else
			{
				value = StyleValueKeyword.Inherit;
				flag2 = false;
			}
			return flag2;
		}

		public void SetFloat(StyleSheet styleSheet, float value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteFloat(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetFloat(StyleSheet styleSheet, out float value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadFloat(this.m_Values[0], out value);
			}
			else
			{
				value = 0f;
				flag2 = false;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetDimension(StyleSheet styleSheet, Dimension value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteDimension(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetDimension(StyleSheet styleSheet, out Dimension value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadDimension(this.m_Values[0], out value);
			}
			else
			{
				value = default(Dimension);
				flag2 = false;
			}
			return flag2;
		}

		public void SetColor(StyleSheet styleSheet, Color value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteColor(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetColor(StyleSheet styleSheet, out Color value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadColor(this.m_Values[0], out value);
			}
			else
			{
				value = default(Color);
				flag2 = false;
			}
			return flag2;
		}

		public void SetString(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteString(ref this.values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetString(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadString(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public void SetEnum(StyleSheet styleSheet, Enum value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteEnum<Enum>(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SetEnumAsString(StyleSheet styleSheet, string enumStr)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteEnumAsString(ref this.m_Values[0], enumStr);
			this.requireVariableResolve = false;
		}

		public void SetEnum<TEnum>(StyleSheet styleSheet, TEnum value) where TEnum : struct, Enum
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteEnum<TEnum>(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetEnumString(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadEnum(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public bool TryGetEnum<TEnum>(StyleSheet styleSheet, out TEnum value) where TEnum : struct, Enum
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadEnum<TEnum>(this.m_Values[0], out value);
			}
			else
			{
				value = default(TEnum);
				flag2 = false;
			}
			return flag2;
		}

		public void SetVariableReference(StyleSheet styleSheet, string variableName)
		{
			StyleProperty.SetSize(ref this.m_Values, 3);
			styleSheet.WriteFunction(ref this.m_Values[0], StyleValueFunction.Var);
			styleSheet.WriteFloat(ref this.m_Values[1], 1f);
			styleSheet.WriteVariable(ref this.m_Values[2], variableName);
			this.requireVariableResolve = true;
		}

		public bool TryGetVariableReference(StyleSheet styleSheet, out string variableName)
		{
			StyleValueFunction styleValueFunction;
			float num;
			bool flag = this.handleCount == 3 && styleSheet.TryReadFunction(this.m_Values[0], out styleValueFunction) && styleValueFunction == StyleValueFunction.Var && styleSheet.TryReadFloat(this.m_Values[1], out num) && (int)num == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadVariable(this.m_Values[2], out variableName);
			}
			else
			{
				variableName = null;
				flag2 = false;
			}
			return flag2;
		}

		public void SetResourcePath(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteResourcePath(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetResourcePath(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadResourcePath(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public void SetAssetReference(StyleSheet styleSheet, Object value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteAssetReference(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetAssetReference(StyleSheet styleSheet, out Object value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadAssetReference(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public bool TryGetAssetReference<TObject>(StyleSheet styleSheet, out TObject value) where TObject : Object
		{
			Object @object;
			TObject tobject;
			bool flag;
			if (this.TryGetAssetReference(styleSheet, out @object))
			{
				tobject = @object as TObject;
				flag = tobject != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (flag2)
			{
				value = tobject;
				flag3 = true;
			}
			else
			{
				value = default(TObject);
				flag3 = false;
			}
			return flag3;
		}

		public void SetMissingAssetReferenceUrl(StyleSheet styleSheet, string value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteMissingAssetReferenceUrl(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetMissingAssetReferenceUrl(StyleSheet styleSheet, out string value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadMissingAssetReferenceUrl(this.m_Values[0], out value);
			}
			else
			{
				value = null;
				flag2 = false;
			}
			return flag2;
		}

		public void SetScalableImage(StyleSheet styleSheet, ScalableImage value)
		{
			StyleProperty.SetSize(ref this.m_Values, 1);
			styleSheet.WriteScalableImage(ref this.m_Values[0], value);
			this.requireVariableResolve = false;
		}

		public bool TryGetScalableImage(StyleSheet styleSheet, out ScalableImage value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = styleSheet.TryReadScalableImage(this.m_Values[0], out value);
			}
			else
			{
				value = default(ScalableImage);
				flag2 = false;
			}
			return flag2;
		}

		public void SetKeyword(StyleSheet styleSheet, StyleKeyword value)
		{
			this.SetKeyword(styleSheet, value.ToStyleValueKeyword());
			this.requireVariableResolve = false;
		}

		public bool TryGetKeyword(StyleSheet styleSheet, out StyleKeyword value)
		{
			bool flag = this.handleCount == 1;
			bool flag2;
			if (flag)
			{
				flag2 = StyleProperty.TryReadKeyword(styleSheet, ref this.m_Values[0], out value);
			}
			else
			{
				value = StyleKeyword.Undefined;
				flag2 = false;
			}
			return flag2;
		}

		public void SetBackgroundRepeat(StyleSheet styleSheet, BackgroundRepeat value)
		{
			StyleProperty.SetSize(ref this.m_Values, 2);
			styleSheet.WriteEnum<Repeat>(ref this.values[0], value.x);
			styleSheet.WriteEnum<Repeat>(ref this.values[1], value.y);
			this.requireVariableResolve = false;
		}

		public bool TryGetBackgroundRepeat(StyleSheet styleSheet, out BackgroundRepeat value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool flag2;
			if (flag)
			{
				value = default(BackgroundRepeat);
				flag2 = false;
			}
			else
			{
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadBackgroundRepeat(this.handleCount, stylePropertyValue, stylePropertyValue2);
				flag2 = true;
			}
			return flag2;
		}

		public void SetBackgroundSize(StyleSheet styleSheet, BackgroundSize value)
		{
			switch (value.sizeType)
			{
			case BackgroundSizeType.Length:
				StyleProperty.SetSize(ref this.m_Values, 2);
				styleSheet.WriteLength(ref this.values[0], value.x);
				styleSheet.WriteLength(ref this.values[1], value.y);
				break;
			case BackgroundSizeType.Cover:
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.Cover);
				break;
			case BackgroundSizeType.Contain:
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.Contain);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.requireVariableResolve = false;
		}

		public bool TryGetBackgroundSize(StyleSheet styleSheet, out BackgroundSize value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool flag2;
			if (flag)
			{
				value = default(BackgroundSize);
				flag2 = false;
			}
			else
			{
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadBackgroundSize(this.handleCount, stylePropertyValue, stylePropertyValue2);
				flag2 = true;
			}
			return flag2;
		}

		public void SetBackgroundPosition(StyleSheet styleSheet, BackgroundPosition value)
		{
			bool flag = value.keyword == BackgroundPositionKeyword.Center;
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteEnum<BackgroundPositionKeyword>(ref this.values[0], value.keyword);
				this.requireVariableResolve = false;
			}
			else
			{
				StyleProperty.SetSize(ref this.m_Values, 2);
				styleSheet.WriteEnum<BackgroundPositionKeyword>(ref this.values[0], value.keyword);
				styleSheet.WriteDimension(ref this.values[1], value.offset.ToDimension());
				this.requireVariableResolve = false;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool TryGetBackgroundPosition(StyleSheet styleSheet, out BackgroundPosition value, BackgroundPosition.Axis axis)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 2;
			bool flag2;
			if (flag)
			{
				value = default(BackgroundPosition);
				flag2 = false;
			}
			else
			{
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((this.handleCount > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadBackgroundPosition(this.handleCount, stylePropertyValue, stylePropertyValue2, (axis == BackgroundPosition.Axis.Horizontal) ? BackgroundPositionKeyword.Left : BackgroundPositionKeyword.Top);
				flag2 = true;
			}
			return flag2;
		}

		public void SetInt(StyleSheet styleSheet, int value)
		{
			this.SetFloat(styleSheet, (float)value);
		}

		public bool TryGetInt(StyleSheet styleSheet, out int value)
		{
			float num;
			bool flag = this.TryGetFloat(styleSheet, out num);
			bool flag2;
			if (flag)
			{
				value = (int)num;
				flag2 = true;
			}
			else
			{
				value = 0;
				flag2 = false;
			}
			return flag2;
		}

		public void SetLength(StyleSheet styleSheet, Length value)
		{
			bool flag = value.IsAuto();
			if (flag)
			{
				this.SetKeyword(styleSheet, StyleValueKeyword.Auto);
			}
			else
			{
				bool flag2 = value.IsNone();
				if (flag2)
				{
					this.SetKeyword(styleSheet, StyleValueKeyword.None);
				}
				else
				{
					this.SetDimension(styleSheet, value.ToDimension());
				}
			}
		}

		public bool TryGetLength(StyleSheet styleSheet, out Length value)
		{
			bool flag = this.handleCount != 1;
			bool flag2;
			if (flag)
			{
				value = default(Length);
				flag2 = false;
			}
			else
			{
				StyleValueKeyword styleValueKeyword;
				bool flag3 = styleSheet.TryReadKeyword(this.m_Values[0], out styleValueKeyword);
				if (flag3)
				{
					StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
					StyleValueKeyword styleValueKeyword3 = styleValueKeyword2;
					if (styleValueKeyword3 != StyleValueKeyword.Initial)
					{
						if (styleValueKeyword3 != StyleValueKeyword.Auto)
						{
							if (styleValueKeyword3 != StyleValueKeyword.None)
							{
								value = default(Length);
								flag2 = false;
							}
							else
							{
								value = Length.None();
								flag2 = true;
							}
						}
						else
						{
							value = Length.Auto();
							flag2 = true;
						}
					}
					else
					{
						value = default(Length);
						flag2 = true;
					}
				}
				else
				{
					Dimension dimension;
					bool flag4 = styleSheet.TryReadDimension(this.m_Values[0], out dimension) && dimension.IsLength();
					if (flag4)
					{
						value = dimension.ToLength();
						flag2 = true;
					}
					else
					{
						value = default(Length);
						flag2 = false;
					}
				}
			}
			return flag2;
		}

		public void SetTranslate(StyleSheet styleSheet, Translate value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.m_Values[0], StyleValueKeyword.None);
			}
			else
			{
				bool flag2 = value.z == 0f;
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 2);
					styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 3);
					styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
					styleSheet.WriteDimension(ref this.m_Values[2], new Length(value.z).ToDimension());
					this.requireVariableResolve = false;
				}
			}
		}

		public bool TryGetTranslate(StyleSheet styleSheet, out Translate value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool flag2;
			if (flag)
			{
				value = default(Translate);
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadTranslate(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3);
				flag2 = true;
			}
			return flag2;
		}

		public void SetRatio(StyleSheet styleSheet, Ratio value)
		{
			bool flag = value.IsAuto();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.m_Values[0], StyleValueKeyword.Auto);
			}
			else
			{
				this.SetFloat(styleSheet, value.value);
			}
		}

		public bool TryGetRatio(StyleSheet styleSheet, out Ratio value)
		{
			float num;
			bool flag = this.TryGetFloat(styleSheet, out num);
			if (flag)
			{
				bool flag2 = num != 0f;
				if (flag2)
				{
					value = num;
					return true;
				}
			}
			value = Ratio.Auto();
			return false;
		}

		public void SetRotate(StyleSheet styleSheet, Rotate value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.None);
				this.requireVariableResolve = false;
			}
			else
			{
				bool flag2 = value.axis == Vector3.forward;
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					styleSheet.WriteAngle(ref this.values[0], value.angle);
					this.requireVariableResolve = false;
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 4);
					Vector3 axis = value.axis;
					styleSheet.WriteFloat(ref this.values[0], axis.x);
					styleSheet.WriteFloat(ref this.values[1], axis.y);
					styleSheet.WriteFloat(ref this.values[2], axis.z);
					styleSheet.WriteAngle(ref this.values[3], value.angle);
					this.requireVariableResolve = false;
				}
			}
		}

		public bool TryGetRotate(StyleSheet styleSheet, out Rotate value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 4;
			bool flag2;
			if (flag)
			{
				value = default(Rotate);
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue4 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[3],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadRotate(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3, stylePropertyValue4);
				flag2 = true;
			}
			return flag2;
		}

		public void SetScale(StyleSheet styleSheet, Scale value)
		{
			bool flag = value.IsNone();
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteKeyword(ref this.values[0], StyleValueKeyword.None);
				this.requireVariableResolve = false;
			}
			else
			{
				bool flag2 = Mathf.Approximately(value.value.z, 1f);
				if (flag2)
				{
					StyleProperty.SetSize(ref this.m_Values, 2);
					styleSheet.WriteFloat(ref this.values[0], value.value.x);
					styleSheet.WriteFloat(ref this.values[1], value.value.y);
					this.requireVariableResolve = false;
				}
				else
				{
					StyleProperty.SetSize(ref this.m_Values, 3);
					styleSheet.WriteFloat(ref this.values[0], value.value.x);
					styleSheet.WriteFloat(ref this.values[1], value.value.y);
					styleSheet.WriteFloat(ref this.values[2], value.value.z);
					this.requireVariableResolve = false;
				}
			}
		}

		public bool TryGetScale(StyleSheet styleSheet, out Scale value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool flag2;
			if (flag)
			{
				value = default(Scale);
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadScale(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3);
				flag2 = true;
			}
			return flag2;
		}

		public void SetTextShadow(StyleSheet styleSheet, TextShadow value)
		{
			StyleProperty.SetSize(ref this.m_Values, 4);
			styleSheet.WriteDimension(ref this.values[0], new Dimension
			{
				value = value.offset.x,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteDimension(ref this.values[1], new Dimension
			{
				value = value.offset.y,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteDimension(ref this.values[2], new Dimension
			{
				value = value.blurRadius,
				unit = Dimension.Unit.Pixel
			});
			styleSheet.WriteColor(ref this.values[3], value.color);
			this.requireVariableResolve = false;
		}

		public bool TryGetTextShadow(StyleSheet styleSheet, out TextShadow value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 4;
			bool flag2;
			if (flag)
			{
				value = default(TextShadow);
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue4 = ((handleCount2 > 3) ? new StylePropertyValue
				{
					handle = this.values[3],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadTextShadow(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3, stylePropertyValue4);
				flag2 = true;
			}
			return flag2;
		}

		public void SetTextAutoSize(StyleSheet styleSheet, TextAutoSize value)
		{
			bool flag = value.mode == TextAutoSizeMode.None;
			if (flag)
			{
				StyleProperty.SetSize(ref this.m_Values, 1);
				styleSheet.WriteEnum<TextAutoSizeMode>(ref this.m_Values[0], value.mode);
			}
			else
			{
				StyleProperty.SetSize(ref this.m_Values, 3);
				styleSheet.WriteEnum<TextAutoSizeMode>(ref this.m_Values[0], value.mode);
				styleSheet.WriteDimension(ref this.values[1], new Dimension
				{
					value = value.minSize.value,
					unit = Dimension.Unit.Pixel
				});
				styleSheet.WriteDimension(ref this.values[2], new Dimension
				{
					value = value.maxSize.value,
					unit = Dimension.Unit.Pixel
				});
			}
		}

		public bool TryGetTextAutoSize(StyleSheet styleSheet, out TextAutoSize value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool flag2;
			if (flag)
			{
				value = TextAutoSize.None();
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadTextAutoSize(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3);
				flag2 = true;
			}
			return flag2;
		}

		public void SetTransformOrigin(StyleSheet styleSheet, TransformOrigin value)
		{
			TransformOriginOffset? transformOriginOffset = StyleProperty.GetTransformOriginOffset(value.x, true);
			TransformOriginOffset? transformOriginOffset2 = StyleProperty.GetTransformOriginOffset(value.y, false);
			bool flag = value.z != 0f;
			bool flag2 = !flag;
			if (flag2)
			{
				bool flag3 = transformOriginOffset2 == TransformOriginOffset.Center;
				if (flag3)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					bool flag4 = transformOriginOffset != null;
					if (flag4)
					{
						styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset.Value);
					}
					else
					{
						styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
					}
					this.requireVariableResolve = false;
					return;
				}
				bool flag5 = transformOriginOffset != null && transformOriginOffset2 != null && transformOriginOffset.Value == TransformOriginOffset.Center;
				if (flag5)
				{
					StyleProperty.SetSize(ref this.m_Values, 1);
					styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset2.Value);
					this.requireVariableResolve = false;
					return;
				}
			}
			StyleProperty.SetSize(ref this.m_Values, 2 + (flag ? 1 : 0));
			bool flag6 = transformOriginOffset != null;
			if (flag6)
			{
				styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[0], transformOriginOffset.Value);
			}
			else
			{
				styleSheet.WriteDimension(ref this.m_Values[0], value.x.ToDimension());
			}
			bool flag7 = transformOriginOffset2 != null;
			if (flag7)
			{
				styleSheet.WriteEnum<TransformOriginOffset>(ref this.m_Values[1], transformOriginOffset2.Value);
			}
			else
			{
				styleSheet.WriteDimension(ref this.m_Values[1], value.y.ToDimension());
			}
			bool flag8 = flag;
			if (flag8)
			{
				styleSheet.WriteDimension(ref this.m_Values[2], new Dimension(value.z, Dimension.Unit.Pixel));
			}
			this.requireVariableResolve = false;
		}

		public bool TryGetTransformOrigin(StyleSheet styleSheet, out TransformOrigin value)
		{
			int handleCount = this.handleCount;
			bool flag = handleCount <= 0 || handleCount > 3;
			bool flag2;
			if (flag)
			{
				value = default(TransformOrigin);
				flag2 = false;
			}
			else
			{
				int handleCount2 = this.handleCount;
				StylePropertyValue stylePropertyValue = new StylePropertyValue
				{
					handle = this.values[0],
					sheet = styleSheet
				};
				StylePropertyValue stylePropertyValue2 = ((handleCount2 > 1) ? new StylePropertyValue
				{
					handle = this.values[1],
					sheet = styleSheet
				} : default(StylePropertyValue));
				StylePropertyValue stylePropertyValue3 = ((handleCount2 > 2) ? new StylePropertyValue
				{
					handle = this.values[2],
					sheet = styleSheet
				} : default(StylePropertyValue));
				value = StylePropertyReader.ReadTransformOrigin(handleCount2, stylePropertyValue, stylePropertyValue2, stylePropertyValue3);
				flag2 = true;
			}
			return flag2;
		}

		internal static int ArgumentCountForMaterialPropertyValueType(MaterialPropertyValueType type)
		{
			int num;
			switch (type)
			{
			case MaterialPropertyValueType.Float:
			case MaterialPropertyValueType.Color:
			case MaterialPropertyValueType.Texture:
				num = 1;
				break;
			case MaterialPropertyValueType.Vector:
				num = 4;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return num;
		}

		public void SetMaterialDefinition(StyleSheet styleSheet, MaterialDefinition value)
		{
			int num = 1;
			bool flag = value.propertyValues != null;
			if (flag)
			{
				foreach (MaterialPropertyValue materialPropertyValue in value.propertyValues)
				{
					num += StyleProperty.ArgumentCountForMaterialPropertyValueType(materialPropertyValue.type) + 3;
				}
			}
			StyleProperty.SetSize(ref this.m_Values, num);
			styleSheet.WriteAssetReference(ref this.values[0], value.material);
			bool flag2 = value.propertyValues == null;
			if (!flag2)
			{
				int num2 = 1;
				foreach (MaterialPropertyValue materialPropertyValue2 in value.propertyValues)
				{
					styleSheet.WriteFunction(ref this.values[num2++], StyleValueFunction.MaterialProperty);
					styleSheet.WriteFloat(ref this.values[num2++], (float)(StyleProperty.ArgumentCountForMaterialPropertyValueType(materialPropertyValue2.type) + 1));
					styleSheet.WriteString(ref this.values[num2++], materialPropertyValue2.name);
					switch (materialPropertyValue2.type)
					{
					case MaterialPropertyValueType.Float:
					{
						float @float = value.GetFloat(materialPropertyValue2.name);
						styleSheet.WriteFloat(ref this.values[num2++], @float);
						break;
					}
					case MaterialPropertyValueType.Vector:
					{
						Vector4 vector = value.GetVector(materialPropertyValue2.name);
						styleSheet.WriteFloat(ref this.values[num2++], vector.x);
						styleSheet.WriteFloat(ref this.values[num2++], vector.y);
						styleSheet.WriteFloat(ref this.values[num2++], vector.z);
						styleSheet.WriteFloat(ref this.values[num2++], vector.w);
						break;
					}
					case MaterialPropertyValueType.Color:
					{
						Color color = value.GetColor(materialPropertyValue2.name);
						styleSheet.WriteColor(ref this.values[num2++], color);
						break;
					}
					case MaterialPropertyValueType.Texture:
					{
						Texture texture = value.GetTexture(materialPropertyValue2.name);
						styleSheet.WriteAssetReference(ref this.values[num2++], texture);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		public bool TryGetMaterialDefinition(StyleSheet styleSheet, out MaterialDefinition value)
		{
			value = default(MaterialDefinition);
			bool flag = this.handleCount < 1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Object @object;
				bool flag3 = !styleSheet.TryReadAssetReference(this.values[0], out @object);
				if (flag3)
				{
					string text;
					bool flag4 = !styleSheet.TryReadResourcePath(this.values[0], out text);
					if (flag4)
					{
						return false;
					}
					@object = (Material)Panel.LoadResource(text, typeof(Material), 1f);
				}
				List<MaterialPropertyValue> list = new List<MaterialPropertyValue>();
				int i = 1;
				while (i < this.values.Length)
				{
					StyleValueFunction valueIndex = (StyleValueFunction)this.values[i++].valueIndex;
					bool flag5 = valueIndex != StyleValueFunction.MaterialProperty;
					if (flag5)
					{
						break;
					}
					float num;
					bool flag6 = !styleSheet.TryReadFloat(this.values[i++], out num);
					if (flag6)
					{
						return false;
					}
					int num2 = (int)num;
					string text2;
					bool flag7 = !styleSheet.TryReadString(this.values[i++], out text2);
					if (flag7)
					{
						return false;
					}
					StyleValueType valueType = this.values[i].valueType;
					bool flag8 = valueType == StyleValueType.Float;
					if (flag8)
					{
						int num3 = num2 - 1;
						Vector4 zero = Vector4.zero;
						int j = 0;
						while (j < num3)
						{
							float num4;
							bool flag9 = !styleSheet.TryReadFloat(this.values[i++], out num4);
							if (flag9)
							{
								return false;
							}
							zero[j++] = num4;
						}
						MaterialPropertyValueType materialPropertyValueType = ((num3 > 1) ? MaterialPropertyValueType.Vector : MaterialPropertyValueType.Float);
						list.Add(new MaterialPropertyValue
						{
							name = text2,
							type = materialPropertyValueType,
							packedValue = zero
						});
					}
					else
					{
						bool flag10 = valueType == StyleValueType.Color || valueType == StyleValueType.Enum;
						if (flag10)
						{
							Color color;
							bool flag11 = !styleSheet.TryReadColor(this.values[i++], out color);
							if (flag11)
							{
								return false;
							}
							list.Add(new MaterialPropertyValue
							{
								name = text2,
								type = MaterialPropertyValueType.Color,
								packedValue = new Vector4(color.r, color.g, color.b, color.a)
							});
						}
						else
						{
							bool flag12 = valueType == StyleValueType.AssetReference || valueType == StyleValueType.ResourcePath || valueType == StyleValueType.MissingAssetReference;
							if (!flag12)
							{
								Debug.LogError(string.Format("Unexpected value type {0} in material property argument", valueType));
								return false;
							}
							Object object2 = null;
							bool flag13 = valueType != StyleValueType.MissingAssetReference;
							if (flag13)
							{
								bool flag14 = this.values[i].valueType == StyleValueType.AssetReference;
								if (flag14)
								{
									bool flag15 = !styleSheet.TryReadAssetReference(this.values[i++], out object2);
									if (flag15)
									{
										return false;
									}
								}
								else
								{
									bool flag16 = this.values[i].valueType == StyleValueType.ResourcePath;
									if (flag16)
									{
										string text3;
										bool flag17 = !styleSheet.TryReadResourcePath(this.values[i++], out text3);
										if (flag17)
										{
											return false;
										}
										object2 = (Texture)Panel.LoadResource(text3, typeof(Texture), 1f);
									}
								}
							}
							else
							{
								i++;
							}
							list.Add(new MaterialPropertyValue
							{
								name = text2,
								type = MaterialPropertyValueType.Texture,
								textureValue = (object2 as Texture)
							});
						}
					}
				}
				value = new MaterialDefinition
				{
					material = (@object as Material),
					propertyValues = list
				};
				flag2 = true;
			}
			return flag2;
		}

		public void SetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteDimension(ref this.values[num], value[i].ToDimension());
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
			this.requireVariableResolve = false;
		}

		public bool TryGetTimeValue(StyleSheet styleSheet, out List<TimeValue> value)
		{
			bool flag = this.ContainsVariable();
			bool flag2;
			if (flag)
			{
				value = null;
				flag2 = false;
			}
			else
			{
				value = new List<TimeValue>();
				flag2 = this.TryGetTimeValue(styleSheet, value);
			}
			return flag2;
		}

		public bool TryGetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					TimeValue timeValue;
					bool flag4 = !styleSheet.TryReadTimeValue(this.m_Values[i], out timeValue) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag4)
					{
						value.Clear();
						return false;
					}
					value.Add(timeValue);
				}
				flag3 = true;
			}
			return flag3;
		}

		public void SetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteStylePropertyName(ref this.values[num], value[i]);
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
			this.requireVariableResolve = false;
		}

		public bool TryGetStylePropertyName(StyleSheet styleSheet, out List<StylePropertyName> value)
		{
			bool flag = this.ContainsVariable();
			bool flag2;
			if (flag)
			{
				value = null;
				flag2 = false;
			}
			else
			{
				value = new List<StylePropertyName>();
				flag2 = this.TryGetStylePropertyName(styleSheet, value);
			}
			return flag2;
		}

		public bool TryGetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					StylePropertyName stylePropertyName;
					bool flag4 = !styleSheet.TryReadStylePropertyName(this.m_Values[i], out stylePropertyName) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag4)
					{
						value.Clear();
						return false;
					}
					value.Add(stylePropertyName);
				}
				flag3 = true;
			}
			return flag3;
		}

		public void SetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
		{
			StyleProperty.SetSize(ref this.m_Values, value.Count * 2 - 1);
			for (int i = 0; i < value.Count; i++)
			{
				int num = i * 2;
				styleSheet.WriteEnum<EasingMode>(ref this.values[num], value[i].mode);
				bool flag = i < value.Count - 1;
				if (flag)
				{
					styleSheet.WriteCommaSeparator(ref this.values[num + 1]);
				}
			}
			this.requireVariableResolve = false;
		}

		public bool TryGetEasingFunction(StyleSheet styleSheet, out List<EasingFunction> value)
		{
			bool flag = this.ContainsVariable();
			bool flag2;
			if (flag)
			{
				value = null;
				flag2 = false;
			}
			else
			{
				value = new List<EasingFunction>();
				flag2 = this.TryGetEasingFunction(styleSheet, value);
			}
			return flag2;
		}

		public bool TryGetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				for (int i = 0; i < this.m_Values.Length; i += 2)
				{
					int num = i + 1;
					EasingMode easingMode;
					bool flag4 = !styleSheet.TryReadEnum<EasingMode>(this.m_Values[i], out easingMode) || (num < this.m_Values.Length && this.values[num].valueType != StyleValueType.CommaSeparator);
					if (flag4)
					{
						value.Clear();
						return false;
					}
					value.Add(new EasingFunction(easingMode));
				}
				flag3 = true;
			}
			return flag3;
		}

		public void SetFilter(StyleSheet styleSheet, List<FilterFunction> filterFunctions)
		{
			int num = 0;
			foreach (FilterFunction filterFunction in filterFunctions)
			{
				num += StyleProperty.GetNumberOfValuesForFilterFunction(filterFunction);
			}
			StyleProperty.SetSize(ref this.m_Values, num);
			int num2 = 0;
			foreach (FilterFunction filterFunction2 in filterFunctions)
			{
				styleSheet.WriteFunction(ref this.values[num2++], StyleProperty.ToStyleValueFunction(filterFunction2.type));
				int num3 = filterFunction2.parameterCount;
				bool flag = filterFunction2.customDefinition != null;
				if (flag)
				{
					num3++;
				}
				styleSheet.WriteFloat(ref this.values[num2++], (float)num3);
				bool flag2 = filterFunction2.customDefinition != null;
				if (flag2)
				{
					styleSheet.WriteAssetReference(ref this.values[num2++], filterFunction2.customDefinition);
				}
				for (int i = 0; i < filterFunction2.parameterCount; i++)
				{
					FilterParameter parameter = filterFunction2.GetParameter(i);
					bool flag3 = parameter.type == FilterParameterType.Float;
					if (flag3)
					{
						styleSheet.WriteFloat(ref this.values[num2++], parameter.floatValue);
					}
					else
					{
						bool flag4 = parameter.type == FilterParameterType.Color;
						if (flag4)
						{
							styleSheet.WriteColor(ref this.values[num2++], parameter.colorValue);
						}
					}
				}
			}
		}

		public bool TryGetFilter(StyleSheet styleSheet, out List<FilterFunction> value)
		{
			bool flag = this.ContainsVariable();
			bool flag2;
			if (flag)
			{
				value = null;
				flag2 = false;
			}
			else
			{
				value = new List<FilterFunction>();
				flag2 = this.TryGetFilter(styleSheet, value);
			}
			return flag2;
		}

		public unsafe bool TryGetFilter(StyleSheet styleSheet, List<FilterFunction> value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value.Clear();
			bool flag2 = this.ContainsVariable();
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				int i = 0;
				while (i < this.m_Values.Length)
				{
					StyleValueFunction styleValueFunction;
					bool flag4 = !styleSheet.TryReadFunction(this.m_Values[i++], out styleValueFunction);
					if (flag4)
					{
						value.Clear();
						return false;
					}
					float num;
					bool flag5 = !styleSheet.TryReadFloat(this.m_Values[i++], out num);
					if (flag5)
					{
						value.Clear();
						return false;
					}
					int num2 = (int)num;
					FilterFunctionDefinition filterFunctionDefinition = null;
					bool flag6 = styleValueFunction == StyleValueFunction.CustomFilter && num2 > 0;
					if (flag6)
					{
						Object @object;
						bool flag7 = !styleSheet.TryReadAssetReference(this.m_Values[i++], out @object);
						if (flag7)
						{
							value.Clear();
							return false;
						}
						filterFunctionDefinition = @object as FilterFunctionDefinition;
						bool flag8 = filterFunctionDefinition == null;
						if (flag8)
						{
							value.Clear();
							return false;
						}
						num2--;
						bool flag9 = filterFunctionDefinition.parameters.Length != num2;
						if (flag9)
						{
							value.Clear();
							return false;
						}
					}
					FixedBuffer4<FilterParameter> fixedBuffer = default(FixedBuffer4<FilterParameter>);
					for (int j = 0; j < num2; j++)
					{
						StyleValueHandle styleValueHandle = this.m_Values[i++];
						Dimension dimension;
						bool flag10 = styleSheet.TryReadDimension(styleValueHandle, out dimension);
						if (flag10)
						{
							*fixedBuffer[j] = new FilterParameter(StyleProperty.ConvertDimensionToFilterFloat(dimension));
						}
						else
						{
							float num3;
							bool flag11 = styleSheet.TryReadFloat(styleValueHandle, out num3);
							if (flag11)
							{
								*fixedBuffer[j] = new FilterParameter(num3);
							}
							else
							{
								Color color;
								bool flag12 = styleSheet.TryReadColor(styleValueHandle, out color);
								if (!flag12)
								{
									value.Clear();
									return false;
								}
								*fixedBuffer[j] = new FilterParameter(color);
							}
						}
					}
					bool flag13 = styleValueFunction == StyleValueFunction.CustomFilter;
					if (flag13)
					{
						value.Add(new FilterFunction(filterFunctionDefinition, fixedBuffer, num2));
					}
					else
					{
						value.Add(new FilterFunction(StyleProperty.ToFilterFunctionType(styleValueFunction), fixedBuffer, num2));
					}
				}
				flag3 = true;
			}
			return flag3;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static int GetNumberOfValuesForFilterFunction(FilterFunction ff)
		{
			int num = 0;
			FilterFunctionDefinition definition = ff.GetDefinition();
			int num2 = ((definition != null) ? definition.parameters.Length : 0);
			bool flag = ff.customDefinition != null;
			if (flag)
			{
				num++;
			}
			return num + (num2 + 2);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static FilterFunctionType ToFilterFunctionType(StyleValueFunction function)
		{
			FilterFunctionType filterFunctionType;
			switch (function)
			{
			case StyleValueFunction.CustomFilter:
				filterFunctionType = FilterFunctionType.Custom;
				break;
			case StyleValueFunction.FilterTint:
				filterFunctionType = FilterFunctionType.Tint;
				break;
			case StyleValueFunction.FilterOpacity:
				filterFunctionType = FilterFunctionType.Opacity;
				break;
			case StyleValueFunction.FilterInvert:
				filterFunctionType = FilterFunctionType.Invert;
				break;
			case StyleValueFunction.FilterGrayscale:
				filterFunctionType = FilterFunctionType.Grayscale;
				break;
			case StyleValueFunction.FilterSepia:
				filterFunctionType = FilterFunctionType.Sepia;
				break;
			case StyleValueFunction.FilterBlur:
				filterFunctionType = FilterFunctionType.Blur;
				break;
			case StyleValueFunction.FilterContrast:
				filterFunctionType = FilterFunctionType.Contrast;
				break;
			case StyleValueFunction.FilterHueRotate:
				filterFunctionType = FilterFunctionType.HueRotate;
				break;
			default:
				filterFunctionType = FilterFunctionType.None;
				break;
			}
			return filterFunctionType;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static StyleValueFunction ToStyleValueFunction(FilterFunctionType type)
		{
			switch (type)
			{
			case FilterFunctionType.None:
				return StyleValueFunction.NoneFilter;
			case FilterFunctionType.Tint:
				return StyleValueFunction.FilterTint;
			case FilterFunctionType.Opacity:
				return StyleValueFunction.FilterOpacity;
			case FilterFunctionType.Invert:
				return StyleValueFunction.FilterInvert;
			case FilterFunctionType.Grayscale:
				return StyleValueFunction.FilterGrayscale;
			case FilterFunctionType.Sepia:
				return StyleValueFunction.FilterSepia;
			case FilterFunctionType.Blur:
				return StyleValueFunction.FilterBlur;
			case FilterFunctionType.Contrast:
				return StyleValueFunction.FilterContrast;
			case FilterFunctionType.HueRotate:
				return StyleValueFunction.FilterHueRotate;
			}
			return StyleValueFunction.CustomFilter;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static float ConvertDimensionToFilterFloat(Dimension dim)
		{
			switch (dim.unit)
			{
			case Dimension.Unit.Percent:
				return dim.value * 0.01f;
			case Dimension.Unit.Millisecond:
				return dim.value * 0.001f;
			case Dimension.Unit.Degree:
				return dim.value * 0.017453292f;
			case Dimension.Unit.Gradian:
				return dim.value * 3.1415927f / 200f;
			case Dimension.Unit.Turn:
				return dim.value * 3.1415927f * 2f;
			}
			return dim.value;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static Dimension ConvertFilterFloatToDimension(float value, Dimension.Unit unit)
		{
			switch (unit)
			{
			case Dimension.Unit.Percent:
				value *= 100f;
				break;
			case Dimension.Unit.Millisecond:
				value *= 1000f;
				break;
			case Dimension.Unit.Degree:
				value *= 57.29578f;
				break;
			case Dimension.Unit.Gradian:
				value /= 0.015707964f;
				break;
			case Dimension.Unit.Turn:
				value /= 6.2831855f;
				break;
			}
			return new Dimension(value, unit);
		}

		private static void SetSize(ref StyleValueHandle[] store, int size)
		{
			StyleValueHandle[] array = store;
			bool flag = array != null && array.Length == size;
			if (!flag)
			{
				store = new StyleValueHandle[size];
			}
		}

		internal static bool TryReadKeyword(StyleSheet styleSheet, ref StyleValueHandle handle, out StyleKeyword value)
		{
			bool flag = handle.valueType == StyleValueType.Keyword;
			if (flag)
			{
				StyleValueKeyword valueIndex = (StyleValueKeyword)handle.valueIndex;
				StyleValueKeyword styleValueKeyword = valueIndex;
				StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
				if (styleValueKeyword2 == StyleValueKeyword.Initial)
				{
					value = StyleKeyword.Initial;
					return true;
				}
				if (styleValueKeyword2 == StyleValueKeyword.Auto)
				{
					value = StyleKeyword.Auto;
					return true;
				}
				if (styleValueKeyword2 == StyleValueKeyword.None)
				{
					value = StyleKeyword.None;
					return true;
				}
			}
			value = StyleKeyword.Undefined;
			return false;
		}

		private static TransformOriginOffset? GetTransformOriginOffset(Length dim, bool horizontal)
		{
			TransformOriginOffset? transformOriginOffset = null;
			bool flag = Mathf.Approximately(dim.value, 0f);
			if (flag)
			{
				transformOriginOffset = new TransformOriginOffset?(horizontal ? TransformOriginOffset.Left : TransformOriginOffset.Top);
			}
			else
			{
				bool flag2 = dim.unit == LengthUnit.Percent;
				if (flag2)
				{
					bool flag3 = Mathf.Approximately(dim.value, 50f);
					if (flag3)
					{
						transformOriginOffset = new TransformOriginOffset?(TransformOriginOffset.Center);
					}
					else
					{
						bool flag4 = Mathf.Approximately(dim.value, 100f);
						if (flag4)
						{
							transformOriginOffset = new TransformOriginOffset?(horizontal ? TransformOriginOffset.Right : TransformOriginOffset.Bottom);
						}
					}
				}
			}
			return transformOriginOffset;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleProperty.Manipulator GetManipulator(StyleSheet styleSheet)
		{
			return new StyleProperty.Manipulator(styleSheet, this);
		}

		[SerializeField]
		private StylePropertyId m_Id;

		[SerializeField]
		private string m_CustomName;

		[SerializeField]
		private int m_Line;

		[SerializeField]
		private StyleValueHandle[] m_Values = Array.Empty<StyleValueHandle>();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[NonSerialized]
		internal bool requireVariableResolve;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal readonly struct Manipulator
		{
			internal Manipulator(StyleSheet styleSheet, StyleProperty property)
			{
				this.m_StyleSheet = styleSheet;
				this.m_Property = property;
			}

			public int GetValueCount()
			{
				List<int> list;
				int count;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					StyleSheetUtility.GetValueOffsets(this.m_StyleSheet, this.m_Property.values, list);
					count = list.Count;
				}
				return count;
			}

			public void AddKeyword(StyleValueKeyword value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteKeyword(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetKeyword(int index, StyleValueKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteKeyword(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertKeyword(int index, StyleValueKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteKeyword(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetKeyword(int index, out StyleValueKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = StyleValueKeyword.Inherit;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadKeyword(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddFloat(float value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteFloat(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetFloat(int index, float value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertFloat(int index, float value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetFloat(int index, out float value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = 0f;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadFloat(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddDimension(Dimension value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteDimension(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetDimension(int index, Dimension value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteDimension(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertDimension(int index, Dimension value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteDimension(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetDimension(int index, out Dimension value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(Dimension);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadDimension(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddColor(Color value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteColor(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetColor(int index, Color value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteColor(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertColor(int index, Color value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteColor(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetColor(int index, out Color value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(Color);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadColor(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddString(string value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteString(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetString(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteString(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertString(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteString(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetString(int index, out string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadString(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddEnum(Enum value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteEnum<Enum>(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void AddEnum<TEnum>(TEnum value) where TEnum : struct, Enum
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteEnum<TEnum>(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void AddEnum(string value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteEnumAsString(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetEnum(int index, Enum value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteEnum<Enum>(ref this.m_Property.values[valueSpan.start], value);
			}

			public void SetEnum<TEnum>(int index, TEnum value) where TEnum : struct, Enum
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteEnum<TEnum>(ref this.m_Property.values[valueSpan.start], value);
			}

			public void SetEnum(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteEnumAsString(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertEnum(int index, Enum value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteEnum<Enum>(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertEnum<TEnum>(int index, TEnum value) where TEnum : struct, Enum
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteEnum<TEnum>(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertEnum(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteEnumAsString(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetEnum<TEnum>(int index, out TEnum value) where TEnum : struct, Enum
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(TEnum);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadEnum<TEnum>(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public bool TryGetEnum(int index, out string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadEnum(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddVariableReference(string value)
			{
				int num = this.m_Property.values.Length;
				StyleProperty.Manipulator.Insert(this.m_Property, this.m_Property.values.Length, 3);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[num], StyleValueFunction.Var);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num + 1], 1f);
				this.m_StyleSheet.WriteVariable(ref this.m_Property.values[num + 2], value);
				this.m_Property.requireVariableResolve = true;
			}

			public void SetVariableReference(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 3);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleValueFunction.Var);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], 1f);
				this.m_StyleSheet.WriteVariable(ref this.m_Property.values[valueSpan.start + 2], value);
				this.m_Property.requireVariableResolve = true;
			}

			public void InsertVariableReference(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 3);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleValueFunction.Var);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], 1f);
				this.m_StyleSheet.WriteVariable(ref this.m_Property.values[valueSpan.start + 2], value);
				this.m_Property.requireVariableResolve = true;
			}

			public bool TryGetVariableReference(int index, out string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length >= 3 && this.m_Property.values[valueSpan.start].valueType == StyleValueType.Function && this.m_Property.values[valueSpan.start + 1].valueType == StyleValueType.Float && this.m_Property.values[valueSpan.start + 2].valueType == StyleValueType.Variable;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadVariable(this.m_Property.values[valueSpan.start + 2], out value);
				}
				return flag2;
			}

			public void AddResourcePath(string value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteResourcePath(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetResourcePath(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteResourcePath(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertResourcePath(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteResourcePath(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetResourcePath(int index, out string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadResourcePath(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddAssetReference(Object value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteAssetReference(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetAssetReference(int index, Object value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertAssetReference(int index, Object value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetAssetReference(int index, out Object value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadAssetReference(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public bool TryGetAssetReference<TObject>(int index, out TObject value) where TObject : Object
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(TObject);
					flag2 = false;
				}
				else
				{
					Object @object;
					TObject tobject;
					bool flag3;
					if (this.m_StyleSheet.TryReadAssetReference(this.m_Property.values[valueSpan.start], out @object))
					{
						tobject = @object as TObject;
						flag3 = tobject != null;
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						value = tobject;
						flag2 = true;
					}
					else
					{
						value = default(TObject);
						flag2 = false;
					}
				}
				return flag2;
			}

			public void AddMissingAssetReferenceUrl(string value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteMissingAssetReferenceUrl(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetMissingAssetReferenceUrl(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteMissingAssetReferenceUrl(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertMissingAssetReferenceUrl(int index, string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteMissingAssetReferenceUrl(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetMissingAssetReferenceUrl(int index, out string value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = null;
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadMissingAssetReferenceUrl(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddScalableImage(ScalableImage value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteScalableImage(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetScalableImage(int index, ScalableImage value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteScalableImage(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertScalableImage(int index, ScalableImage value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteScalableImage(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetScalableImage(int index, out ScalableImage value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(ScalableImage);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadScalableImage(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddAngle(Angle value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteAngle(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetAngle(int index, Angle value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteAngle(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertAngle(int index, Angle value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteAngle(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetAngle(int index, out Angle value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(Angle);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadAngle(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddKeyword(StyleKeyword value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteKeyword(ref styleValueHandle, value.ToStyleValueKeyword());
				this.AddHandle(styleValueHandle);
			}

			public void SetKeyword(int index, StyleKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteKeyword(ref this.m_Property.values[valueSpan.start], value.ToStyleValueKeyword());
			}

			public void InsertKeyword(int index, StyleKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteKeyword(ref this.m_Property.values[valueSpan.start], value.ToStyleValueKeyword());
			}

			public bool TryGetKeyword(int index, out StyleKeyword value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = StyleKeyword.Undefined;
					flag2 = false;
				}
				else
				{
					flag2 = StyleProperty.TryReadKeyword(this.m_StyleSheet, ref this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddInt(int value)
			{
				this.AddFloat((float)value);
			}

			public void SetInt(int index, int value)
			{
				this.SetFloat(index, (float)value);
			}

			public void InsertInt(int index, int value)
			{
				this.InsertFloat(index, (float)value);
			}

			public bool TryGetInt(int index, out int value)
			{
				float num;
				bool flag = this.TryGetFloat(index, out num);
				bool flag2;
				if (flag)
				{
					value = (int)num;
					flag2 = true;
				}
				else
				{
					value = 0;
					flag2 = false;
				}
				return flag2;
			}

			public void AddLength(Length value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteLength(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetLength(int index, Length value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteLength(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertLength(int index, Length value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteLength(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetLength(int index, out Length value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(Length);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadLength(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddTimeValue(TimeValue value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteTimeValue(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetTimeValue(int index, TimeValue value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteTimeValue(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertTimeValue(int index, TimeValue value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteTimeValue(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetTimeValue(int index, out TimeValue value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(TimeValue);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadTimeValue(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddStylePropertyName(StylePropertyName value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteStylePropertyName(ref styleValueHandle, value);
				this.AddHandle(styleValueHandle);
			}

			public void SetStylePropertyName(int index, StylePropertyName value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteStylePropertyName(ref this.m_Property.values[valueSpan.start], value);
			}

			public void InsertStylePropertyName(int index, StylePropertyName value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteStylePropertyName(ref this.m_Property.values[valueSpan.start], value);
			}

			public bool TryGetStylePropertyName(int index, out StylePropertyName value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(StylePropertyName);
					flag2 = false;
				}
				else
				{
					flag2 = this.m_StyleSheet.TryReadStylePropertyName(this.m_Property.values[valueSpan.start], out value);
				}
				return flag2;
			}

			public void AddEasingFunction(EasingFunction value)
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteEnum<EasingMode>(ref styleValueHandle, value.mode);
				this.AddHandle(styleValueHandle);
			}

			public void SetEasingFunction(int index, EasingFunction value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, 1);
				this.m_StyleSheet.WriteEnum<EasingMode>(ref this.m_Property.values[valueSpan.start], value.mode);
			}

			public void InsertEasingFunction(int index, EasingFunction value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, 1);
				this.m_StyleSheet.WriteEnum<EasingMode>(ref this.m_Property.values[valueSpan.start], value.mode);
			}

			public bool TryGetEasingFunction(int index, out EasingFunction value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length != 1;
				bool flag2;
				if (flag)
				{
					value = default(EasingFunction);
					flag2 = false;
				}
				else
				{
					EasingMode easingMode;
					bool flag3 = this.m_StyleSheet.TryReadEnum<EasingMode>(this.m_Property.values[valueSpan.start], out easingMode);
					if (flag3)
					{
						value = new EasingFunction(easingMode);
						flag2 = true;
					}
					else
					{
						value = default(EasingFunction);
						flag2 = false;
					}
				}
				return flag2;
			}

			public void AddFilterFunction(FilterFunction value)
			{
				bool flag = value.type == FilterFunctionType.Custom && value.customDefinition;
				int num = value.parameterCount + (flag ? 1 : 0);
				int num2 = num + 2;
				int num3 = this.m_Property.values.Length;
				StyleProperty.Manipulator.Insert(this.m_Property, this.m_Property.values.Length, num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[num3], StyleProperty.ToStyleValueFunction(value.type));
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num3 + 1], (float)num);
				int num4 = num3 + 2;
				bool flag2 = flag;
				if (flag2)
				{
					this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[num4], value.customDefinition);
					num4++;
				}
				int i = 0;
				while (i < value.parameterCount)
				{
					FilterParameter parameter = value.GetParameter(i);
					FilterParameterType type = parameter.type;
					FilterParameterType filterParameterType = type;
					if (filterParameterType != FilterParameterType.Float)
					{
						if (filterParameterType != FilterParameterType.Color)
						{
							throw new ArgumentOutOfRangeException();
						}
						this.m_StyleSheet.WriteColor(ref this.m_Property.values[num4], parameter.colorValue);
					}
					else
					{
						this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num4], parameter.floatValue);
					}
					i++;
					num4++;
				}
			}

			public void SetFilterFunction(int index, FilterFunction value)
			{
				bool flag = value.type == FilterFunctionType.Custom && value.customDefinition;
				int num = value.parameterCount + (flag ? 1 : 0);
				int num2 = num + 2;
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				this.ResizeValue(ref valueSpan, num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleProperty.ToStyleValueFunction(value.type));
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], (float)num);
				int num3 = valueSpan.start + 2;
				bool flag2 = flag;
				if (flag2)
				{
					this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[num3], value.customDefinition);
					num3++;
				}
				int i = 0;
				while (i < value.parameterCount)
				{
					FilterParameter parameter = value.GetParameter(i);
					FilterParameterType type = parameter.type;
					FilterParameterType filterParameterType = type;
					if (filterParameterType != FilterParameterType.Float)
					{
						if (filterParameterType != FilterParameterType.Color)
						{
							throw new ArgumentOutOfRangeException();
						}
						this.m_StyleSheet.WriteColor(ref this.m_Property.values[num3], parameter.colorValue);
					}
					else
					{
						this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num3], parameter.floatValue);
					}
					i++;
					num3++;
				}
			}

			public void InsertFilterFunction(int index, FilterFunction value)
			{
				bool flag = value.type == FilterFunctionType.Custom && value.customDefinition;
				int num = value.parameterCount + (flag ? 1 : 0);
				int num2 = num + 2;
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleProperty.ToStyleValueFunction(value.type));
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], (float)num);
				int num3 = valueSpan.start + 2;
				bool flag2 = flag;
				if (flag2)
				{
					this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[num3], value.customDefinition);
					num3++;
				}
				int i = 0;
				while (i < value.parameterCount)
				{
					FilterParameter parameter = value.GetParameter(i);
					FilterParameterType type = parameter.type;
					FilterParameterType filterParameterType = type;
					if (filterParameterType != FilterParameterType.Float)
					{
						if (filterParameterType != FilterParameterType.Color)
						{
							throw new ArgumentOutOfRangeException();
						}
						this.m_StyleSheet.WriteColor(ref this.m_Property.values[num3], parameter.colorValue);
					}
					else
					{
						this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num3], parameter.floatValue);
					}
					i++;
					num3++;
				}
			}

			public unsafe bool TryGetFilterFunction(int index, out FilterFunction value)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				bool flag = valueSpan.length <= 1;
				bool flag2;
				if (flag)
				{
					value = default(FilterFunction);
					flag2 = false;
				}
				else
				{
					StyleValueFunction styleValueFunction;
					float num;
					bool flag3 = !this.m_StyleSheet.TryReadFunction(this.m_Property.values[valueSpan.start], out styleValueFunction) || !this.m_StyleSheet.TryReadFloat(this.m_Property.values[valueSpan.start + 1], out num);
					if (flag3)
					{
						value = default(FilterFunction);
						flag2 = false;
					}
					else
					{
						int num2 = (int)num;
						FixedBuffer4<FilterParameter> fixedBuffer = default(FixedBuffer4<FilterParameter>);
						int num3 = valueSpan.start + 2;
						FilterFunctionType filterFunctionType = StyleProperty.ToFilterFunctionType(styleValueFunction);
						FilterFunctionDefinition filterFunctionDefinition = null;
						bool flag4 = false;
						bool flag5 = filterFunctionType == FilterFunctionType.Custom && num2 > 0;
						if (flag5)
						{
							Object @object;
							bool flag6 = this.m_StyleSheet.TryReadAssetReference(this.m_Property.values[num3], out @object);
							if (flag6)
							{
								filterFunctionDefinition = (FilterFunctionDefinition)@object;
								flag4 = true;
							}
							else
							{
								string text;
								bool flag7 = this.m_StyleSheet.TryReadResourcePath(this.m_Property.values[num3], out text);
								if (flag7)
								{
									filterFunctionDefinition = (FilterFunctionDefinition)Panel.LoadResource(text, typeof(Object), 1f);
									flag4 = true;
								}
							}
							num2--;
						}
						int i = 0;
						while (i < num2)
						{
							Color color;
							bool flag8 = this.m_StyleSheet.TryReadColor(this.m_Property.values[num3], out color);
							if (flag8)
							{
								*fixedBuffer[i] = new FilterParameter
								{
									type = FilterParameterType.Color,
									colorValue = color
								};
							}
							else
							{
								float num4;
								bool flag9 = this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3], out num4);
								if (flag9)
								{
									*fixedBuffer[i] = new FilterParameter
									{
										type = FilterParameterType.Float,
										floatValue = num4
									};
								}
								else
								{
									bool flag10 = this.m_Property.values[num3].valueType == StyleValueType.CommaSeparator;
									if (!flag10)
									{
										Debug.LogError(string.Format("Unexpected value type {0} in filter function argument", this.m_Property.values[num3].valueType));
									}
								}
							}
							IL_0253:
							i++;
							num3++;
							continue;
							goto IL_0253;
						}
						value = (flag4 ? new FilterFunction(filterFunctionDefinition, fixedBuffer, num2) : new FilterFunction(filterFunctionType, fixedBuffer, num2));
						flag2 = true;
					}
				}
				return flag2;
			}

			private void WriteMaterialPropertyValue(MaterialPropertyValue value, int index)
			{
				switch (value.type)
				{
				case MaterialPropertyValueType.Float:
					this.m_StyleSheet.WriteFloat(ref this.m_Property.values[index++], value.GetFloat());
					break;
				case MaterialPropertyValueType.Vector:
				{
					Vector4 vector = value.GetVector();
					this.m_StyleSheet.WriteFloat(ref this.m_Property.values[index++], vector.x);
					this.m_StyleSheet.WriteFloat(ref this.m_Property.values[index++], vector.y);
					this.m_StyleSheet.WriteFloat(ref this.m_Property.values[index++], vector.z);
					this.m_StyleSheet.WriteFloat(ref this.m_Property.values[index++], vector.w);
					break;
				}
				case MaterialPropertyValueType.Color:
					this.m_StyleSheet.WriteColor(ref this.m_Property.values[index++], value.GetColor());
					break;
				case MaterialPropertyValueType.Texture:
					this.m_StyleSheet.WriteAssetReference(ref this.m_Property.values[index++], value.textureValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			public bool TryGetMaterialPropertyValue(int index, out MaterialPropertyValue value)
			{
				value = default(MaterialPropertyValue);
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index + 1, false);
				bool flag = valueSpan.length < 4;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					StyleValueFunction styleValueFunction;
					bool flag3 = !this.m_StyleSheet.TryReadFunction(this.m_Property.values[valueSpan.start], out styleValueFunction) || styleValueFunction != StyleValueFunction.MaterialProperty;
					if (flag3)
					{
						flag2 = false;
					}
					else
					{
						float num;
						bool flag4 = !this.m_StyleSheet.TryReadFloat(this.m_Property.values[valueSpan.start + 1], out num);
						if (flag4)
						{
							flag2 = false;
						}
						else
						{
							int num2 = (int)num;
							string text;
							bool flag5 = !this.m_StyleSheet.TryReadString(this.m_Property.values[valueSpan.start + 2], out text);
							if (flag5)
							{
								flag2 = false;
							}
							else
							{
								int num3 = valueSpan.start + 3;
								int num4 = num2 - 1;
								float num5;
								bool flag6 = num4 == 1 && this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3], out num5);
								if (flag6)
								{
									value = new MaterialPropertyValue
									{
										name = text,
										type = MaterialPropertyValueType.Float,
										packedValue = new Vector4(num5, 0f, 0f, 0f)
									};
									flag2 = true;
								}
								else
								{
									Color color;
									bool flag7 = num4 == 1 && this.m_StyleSheet.TryReadColor(this.m_Property.values[num3], out color);
									if (flag7)
									{
										value = new MaterialPropertyValue
										{
											name = text,
											type = MaterialPropertyValueType.Color,
											packedValue = new Vector4(color.r, color.g, color.b, color.a)
										};
										flag2 = true;
									}
									else
									{
										float num6;
										float num7;
										float num8;
										float num9;
										bool flag8 = num4 == 4 && this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3], out num6) && this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3 + 1], out num7) && this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3 + 2], out num8) && this.m_StyleSheet.TryReadFloat(this.m_Property.values[num3 + 3], out num9);
										if (flag8)
										{
											value = new MaterialPropertyValue
											{
												name = text,
												type = MaterialPropertyValueType.Vector,
												packedValue = new Vector4(num6, num7, num8, num9)
											};
											flag2 = true;
										}
										else
										{
											Object @object;
											bool flag9 = num4 == 1 && this.m_StyleSheet.TryReadAssetReference(this.m_Property.values[num3], out @object);
											if (flag9)
											{
												value = new MaterialPropertyValue
												{
													name = text,
													type = MaterialPropertyValueType.Texture,
													textureValue = (@object as Texture)
												};
												flag2 = true;
											}
											else
											{
												flag2 = false;
											}
										}
									}
								}
							}
						}
					}
				}
				return flag2;
			}

			public void AddMaterialPropertyValue(MaterialPropertyValue value)
			{
				int num = this.m_Property.values.Length;
				int num2 = 1 + StyleProperty.ArgumentCountForMaterialPropertyValueType(value.type);
				StyleProperty.Manipulator.Insert(this.m_Property, this.m_Property.values.Length, 2 + num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[num], StyleValueFunction.MaterialProperty);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[num + 1], (float)num2);
				this.m_StyleSheet.WriteString(ref this.m_Property.values[num + 2], value.name);
				this.WriteMaterialPropertyValue(value, num + 3);
			}

			public void SetMaterialPropertyValue(int index, MaterialPropertyValue value)
			{
				int num = StyleProperty.ArgumentCountForMaterialPropertyValueType(value.type) + 1;
				int num2 = 2 + num;
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index + 1, false);
				this.ResizeValue(ref valueSpan, num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleValueFunction.MaterialProperty);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], (float)num);
				this.m_StyleSheet.WriteString(ref this.m_Property.values[valueSpan.start + 2], value.name);
				this.WriteMaterialPropertyValue(value, valueSpan.start + 3);
			}

			public void InsertMaterialPropertyValue(int index, MaterialPropertyValue value)
			{
				int num = StyleProperty.ArgumentCountForMaterialPropertyValueType(value.type) + 1;
				int num2 = 2 + num;
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index + 1, true);
				StyleProperty.Manipulator.Insert(this.m_Property, valueSpan.start, num2);
				this.m_StyleSheet.WriteFunction(ref this.m_Property.values[valueSpan.start], StyleValueFunction.MaterialProperty);
				this.m_StyleSheet.WriteFloat(ref this.m_Property.values[valueSpan.start + 1], (float)num);
				this.m_StyleSheet.WriteString(ref this.m_Property.values[valueSpan.start + 2], value.name);
				this.WriteMaterialPropertyValue(value, valueSpan.start + 3);
			}

			public void AddCommaSeparator()
			{
				StyleValueHandle styleValueHandle = default(StyleValueHandle);
				this.m_StyleSheet.WriteCommaSeparator(ref styleValueHandle);
				this.AddHandle(styleValueHandle);
			}

			public void RemoveValue(int index)
			{
				StyleProperty.Manipulator.ValueSpan valueSpan = this.GetValueSpan(index, false);
				int num = valueSpan.start;
				int num2 = valueSpan.length;
				bool flag = StyleProperty.Manipulator.IsCommaSeparator(this.m_Property.values, valueSpan.start + valueSpan.length);
				if (flag)
				{
					num2++;
				}
				else
				{
					bool flag2 = StyleProperty.Manipulator.IsCommaSeparator(this.m_Property.values, valueSpan.start - 1);
					if (flag2)
					{
						num--;
						num2++;
					}
				}
				StyleProperty.Manipulator.Remove(this.m_Property, num, num2);
			}

			private StyleProperty.Manipulator.ValueSpan GetValueSpan(int index, bool insertMode = false)
			{
				List<int> list;
				StyleProperty.Manipulator.ValueSpan valueSpan;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					StyleSheetUtility.GetValueOffsets(this.m_StyleSheet, this.m_Property.values, list);
					bool flag = index < 0 || index > list.Count;
					if (flag)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					bool flag2 = index == list.Count;
					if (flag2)
					{
						if (!insertMode)
						{
							throw new ArgumentOutOfRangeException("index");
						}
						valueSpan = new StyleProperty.Manipulator.ValueSpan(this.m_Property.values.Length, 0);
					}
					else
					{
						int num = list[index];
						int num2 = ((index == list.Count - 1) ? this.m_Property.values.Length : list[index + 1]);
						int num3 = ((num2 < this.m_Property.values.Length && this.m_Property.values[num2 - 1].valueType == StyleValueType.CommaSeparator) ? 1 : 0);
						valueSpan = new StyleProperty.Manipulator.ValueSpan(num, num2 - num - num3);
					}
				}
				return valueSpan;
			}

			private void ResizeValue(ref StyleProperty.Manipulator.ValueSpan span, int count)
			{
				bool flag = span.length == count;
				if (!flag)
				{
					bool flag2 = count > span.length;
					if (flag2)
					{
						int num = count - span.length;
						StyleProperty.Manipulator.Insert(this.m_Property, span.start, num);
					}
					else
					{
						int num2 = span.length - count;
						StyleProperty.Manipulator.Remove(this.m_Property, span.start, num2);
					}
				}
			}

			private void AddHandle(StyleValueHandle handle)
			{
				StyleProperty.Manipulator.Insert(this.m_Property, this.m_Property.values.Length, 1);
				StyleValueHandle[] values = this.m_Property.values;
				values[values.Length - 1] = handle;
			}

			private static void Insert(StyleProperty property, int index, int count)
			{
				StyleValueHandle[] values = property.values;
				property.values = new StyleValueHandle[values.Length + count];
				Array.Copy(values, 0, property.values, 0, index);
				Array.Copy(values, index, property.values, index + count, values.Length - index);
			}

			private static void Remove(StyleProperty property, int index, int count)
			{
				StyleValueHandle[] values = property.values;
				property.values = new StyleValueHandle[values.Length - count];
				Array.Copy(values, 0, property.values, 0, index);
				Array.Copy(values, index + count, property.values, index, values.Length - (index + count));
			}

			private static bool IsCommaSeparator(StyleValueHandle[] array, int index)
			{
				bool flag = index < 0 || index >= array.Length;
				return !flag && array[index].valueType == StyleValueType.CommaSeparator;
			}

			private readonly StyleSheet m_StyleSheet;

			private readonly StyleProperty m_Property;

			[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
			private readonly ref struct ValueSpan
			{
				public ValueSpan(int start, int length)
				{
					this.start = start;
					this.length = length;
				}

				public readonly int start;

				public readonly int length;
			}
		}
	}
}
