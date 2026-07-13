using System;

namespace UnityEngine.UIElements
{
	internal static class FilterFunctionDefinitionUtils
	{
		public static string GetBuiltinFilterName(FilterFunctionType type)
		{
			string text;
			switch (type)
			{
			case FilterFunctionType.Tint:
				text = "tint";
				break;
			case FilterFunctionType.Opacity:
				text = "opacity";
				break;
			case FilterFunctionType.Invert:
				text = "invert";
				break;
			case FilterFunctionType.Grayscale:
				text = "grayscale";
				break;
			case FilterFunctionType.Sepia:
				text = "sepia";
				break;
			case FilterFunctionType.Blur:
				text = "blur";
				break;
			case FilterFunctionType.Contrast:
				text = "contrast";
				break;
			case FilterFunctionType.HueRotate:
				text = "hue-rotate";
				break;
			default:
				text = null;
				break;
			}
			return text;
		}

		public static FilterFunctionDefinition GetBuiltinDefinition(FilterFunctionType type)
		{
			FilterFunctionDefinition filterFunctionDefinition;
			switch (type)
			{
			case FilterFunctionType.Tint:
			{
				bool flag = FilterFunctionDefinitionUtils.s_TintDef == null;
				if (flag)
				{
					FilterFunctionDefinitionUtils.s_TintDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Tint);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_TintDef;
				break;
			}
			case FilterFunctionType.Opacity:
			{
				bool flag2 = FilterFunctionDefinitionUtils.s_OpacityDef == null;
				if (flag2)
				{
					FilterFunctionDefinitionUtils.s_OpacityDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Opacity);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_OpacityDef;
				break;
			}
			case FilterFunctionType.Invert:
			{
				bool flag3 = FilterFunctionDefinitionUtils.s_InvertDef == null;
				if (flag3)
				{
					FilterFunctionDefinitionUtils.s_InvertDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Invert);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_InvertDef;
				break;
			}
			case FilterFunctionType.Grayscale:
			{
				bool flag4 = FilterFunctionDefinitionUtils.s_GrayscaleDef == null;
				if (flag4)
				{
					FilterFunctionDefinitionUtils.s_GrayscaleDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Grayscale);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_GrayscaleDef;
				break;
			}
			case FilterFunctionType.Sepia:
			{
				bool flag5 = FilterFunctionDefinitionUtils.s_SepiaDef == null;
				if (flag5)
				{
					FilterFunctionDefinitionUtils.s_SepiaDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Sepia);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_SepiaDef;
				break;
			}
			case FilterFunctionType.Blur:
			{
				bool flag6 = FilterFunctionDefinitionUtils.s_BlurDef == null;
				if (flag6)
				{
					FilterFunctionDefinitionUtils.s_BlurDef = FilterFunctionDefinitionUtils.CreateBlurFilterFunctionDefinition();
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_BlurDef;
				break;
			}
			case FilterFunctionType.Contrast:
			{
				bool flag7 = FilterFunctionDefinitionUtils.s_ContrastDef == null;
				if (flag7)
				{
					FilterFunctionDefinitionUtils.s_ContrastDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.Contrast);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_ContrastDef;
				break;
			}
			case FilterFunctionType.HueRotate:
			{
				bool flag8 = FilterFunctionDefinitionUtils.s_HueRotateDef == null;
				if (flag8)
				{
					FilterFunctionDefinitionUtils.s_HueRotateDef = FilterFunctionDefinitionUtils.CreateColorEffectFilterFunctionDefinition(FilterFunctionType.HueRotate);
				}
				filterFunctionDefinition = FilterFunctionDefinitionUtils.s_HueRotateDef;
				break;
			}
			default:
				filterFunctionDefinition = null;
				break;
			}
			return filterFunctionDefinition;
		}

		private static FilterFunctionDefinition CreateBlurFilterFunctionDefinition()
		{
			Material material = new Material(Shader.Find("Hidden/UIR/GaussianBlur"));
			material.hideFlags = HideFlags.HideAndDontSave;
			FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
			filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
			filterFunctionDefinition.filterName = FilterFunctionDefinitionUtils.GetBuiltinFilterName(FilterFunctionType.Blur);
			filterFunctionDefinition.parameters = new FilterParameterDeclaration[]
			{
				new FilterParameterDeclaration
				{
					interpolationDefaultValue = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = 0f
					},
					defaultValue = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = 0f
					}
				}
			};
			filterFunctionDefinition.passes = new PostProcessingPass[]
			{
				new PostProcessingPass
				{
					material = material,
					passIndex = 0,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = "_Sigma"
						}
					},
					readMargins = default(PostProcessingMargins),
					writeMargins = default(PostProcessingMargins)
				},
				new PostProcessingPass
				{
					material = material,
					passIndex = 1,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = "_Sigma"
						}
					},
					readMargins = default(PostProcessingMargins),
					writeMargins = default(PostProcessingMargins)
				}
			};
			filterFunctionDefinition.passes[0].computeRequiredReadMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeHorizontalBlurMargins);
			filterFunctionDefinition.passes[0].computeRequiredWriteMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeHorizontalBlurMargins);
			filterFunctionDefinition.passes[1].computeRequiredReadMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeVerticalBlurMargins);
			filterFunctionDefinition.passes[1].computeRequiredWriteMarginsCallback = new PostProcessingPass.ComputeRequiredMarginsDelegate(FilterFunctionDefinitionUtils.ComputeVerticalBlurMargins);
			return filterFunctionDefinition;
		}

		private static FilterFunctionDefinition CreateColorEffectFilterFunctionDefinition(FilterFunctionType filterType)
		{
			Material material = new Material(Shader.Find("Hidden/UIR/ColorEffect"));
			material.hideFlags = HideFlags.HideAndDontSave;
			FilterFunctionDefinition filterFunctionDefinition = ScriptableObject.CreateInstance<FilterFunctionDefinition>();
			filterFunctionDefinition.hideFlags = HideFlags.HideAndDontSave;
			filterFunctionDefinition.filterName = FilterFunctionDefinitionUtils.GetBuiltinFilterName(filterType);
			FilterParameter filterParameter = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 0f
			};
			FilterParameter filterParameter2 = new FilterParameter
			{
				type = FilterParameterType.Float,
				floatValue = 0f
			};
			switch (filterType)
			{
			case FilterFunctionType.Tint:
				filterParameter = new FilterParameter
				{
					type = FilterParameterType.Color,
					colorValue = Color.white
				};
				filterParameter2 = new FilterParameter
				{
					type = FilterParameterType.Color,
					colorValue = Color.white
				};
				break;
			case FilterFunctionType.Opacity:
				filterParameter = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				filterParameter2 = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				break;
			case FilterFunctionType.Invert:
			case FilterFunctionType.Grayscale:
			case FilterFunctionType.Sepia:
			case FilterFunctionType.Contrast:
				filterParameter2 = new FilterParameter
				{
					type = FilterParameterType.Float,
					floatValue = 1f
				};
				break;
			}
			filterFunctionDefinition.parameters = new FilterParameterDeclaration[]
			{
				new FilterParameterDeclaration
				{
					interpolationDefaultValue = filterParameter,
					defaultValue = filterParameter2
				}
			};
			filterFunctionDefinition.passes = new PostProcessingPass[]
			{
				new PostProcessingPass
				{
					material = material,
					passIndex = 0,
					parameterBindings = new ParameterBinding[]
					{
						new ParameterBinding
						{
							index = 0,
							name = ""
						}
					},
					readMargins = new PostProcessingMargins
					{
						left = 0f,
						top = 0f,
						right = 0f,
						bottom = 0f
					},
					writeMargins = new PostProcessingMargins
					{
						left = 0f,
						top = 0f,
						right = 0f,
						bottom = 0f
					}
				}
			};
			filterFunctionDefinition.passes[0].applySettingsCallback = new PostProcessingPass.ApplyFilterPassSettingsDelegate(FilterFunctionDefinitionUtils.ApplySettings);
			return filterFunctionDefinition;
		}

		private static PostProcessingMargins ComputeHorizontalBlurMargins(FilterFunction func)
		{
			float num = Math.Max(0f, func.parameters[0].floatValue);
			int num2 = Mathf.CeilToInt(num * 3f + 1f);
			return new PostProcessingMargins
			{
				left = (float)num2,
				top = 0f,
				right = (float)num2,
				bottom = 0f
			};
		}

		private static PostProcessingMargins ComputeVerticalBlurMargins(FilterFunction func)
		{
			float num = Math.Max(1f, func.parameters[0].floatValue);
			int num2 = Mathf.CeilToInt(num * 3f + 1f);
			return new PostProcessingMargins
			{
				left = 0f,
				top = (float)num2,
				right = 0f,
				bottom = (float)num2
			};
		}

		private static void ApplySettings(MaterialPropertyBlock mpb, FilterPassContext context)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			float num = 0f;
			float num2 = 0f;
			FilterFunction filterFunction = context.filterFunction;
			switch (filterFunction.type)
			{
			case FilterFunctionType.Tint:
			{
				Color color = filterFunction.parameters[0].colorValue;
				bool flag = !context.readsGamma;
				if (flag)
				{
					color = color.linear;
				}
				color.a = Mathf.Clamp01(color.a);
				color.r = Mathf.Clamp01(color.r * color.a);
				color.g = Mathf.Clamp01(color.g * color.a);
				color.b = Mathf.Clamp01(color.b * color.a);
				identity = new Matrix4x4(new Vector4(color.r, 0f, 0f, 0f), new Vector4(0f, color.g, 0f, 0f), new Vector4(0f, 0f, color.b, 0f), new Vector4(0f, 0f, 0f, color.a));
				break;
			}
			case FilterFunctionType.Opacity:
			{
				float num3 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
				identity = new Matrix4x4(new Vector4(num3, 0f, 0f, 0f), new Vector4(0f, num3, 0f, 0f), new Vector4(0f, 0f, num3, 0f), new Vector4(0f, 0f, 0f, num3));
				break;
			}
			case FilterFunctionType.Invert:
				num2 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
				break;
			case FilterFunctionType.Grayscale:
			{
				float num4 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
				identity = new Matrix4x4(new Vector4(0.2126f + 0.7874f * (1f - num4), 0.2126f - 0.2126f * (1f - num4), 0.2126f - 0.2126f * (1f - num4), 0f), new Vector4(0.7152f - 0.7152f * (1f - num4), 0.7152f + 0.2848f * (1f - num4), 0.7152f - 0.7152f * (1f - num4), 0f), new Vector4(0.0722f - 0.0722f * (1f - num4), 0.0722f - 0.0722f * (1f - num4), 0.0722f + 0.9278f * (1f - num4), 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			case FilterFunctionType.Sepia:
			{
				float num5 = Mathf.Clamp01(filterFunction.parameters[0].floatValue);
				identity = new Matrix4x4(new Vector4(0.393f + 0.607f * (1f - num5), 0.349f - 0.349f * (1f - num5), 0.272f - 0.272f * (1f - num5), 0f), new Vector4(0.769f - 0.769f * (1f - num5), 0.686f + 0.314f * (1f - num5), 0.534f - 0.534f * (1f - num5), 0f), new Vector4(0.189f - 0.189f * (1f - num5), 0.168f - 0.168f * (1f - num5), 0.131f + 0.869f * (1f - num5), 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			case FilterFunctionType.Contrast:
			{
				float num6 = Mathf.Max(0f, filterFunction.parameters[0].floatValue);
				num = (1f - num6) * 0.5f;
				identity = new Matrix4x4(new Vector4(num6, 0f, 0f, 0f), new Vector4(0f, num6, 0f, 0f), new Vector4(0f, 0f, num6, 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			case FilterFunctionType.HueRotate:
			{
				float floatValue = filterFunction.parameters[0].floatValue;
				float num7 = Mathf.Cos(floatValue);
				float num8 = Mathf.Sin(floatValue);
				float num9 = 0.213f;
				float num10 = 0.715f;
				float num11 = 0.072f;
				identity = new Matrix4x4(new Vector4(num9 + num7 * (1f - num9) + num8 * -num9, num9 + num7 * -num9 + num8 * 0.143f, num9 + num7 * -num9 + num8 * -(1f - num9), 0f), new Vector4(num10 + num7 * -num10 + num8 * -num10, num10 + num7 * (1f - num10) + num8 * 0.14f, num10 + num7 * -num10 + num8 * num10, 0f), new Vector4(num11 + num7 * -num11 + num8 * (1f - num11), num11 + num7 * -num11 + num8 * -0.283f, num11 + num7 * (1f - num11) + num8 * num11, 0f), new Vector4(0f, 0f, 0f, 1f));
				break;
			}
			}
			mpb.SetMatrix("_ColorMatrix", identity);
			mpb.SetFloat("_ColorOffset", num);
			mpb.SetFloat("_ColorInvert", num2);
		}

		private static FilterFunctionDefinition s_BlurDef;

		private static FilterFunctionDefinition s_TintDef;

		private static FilterFunctionDefinition s_OpacityDef;

		private static FilterFunctionDefinition s_InvertDef;

		private static FilterFunctionDefinition s_GrayscaleDef;

		private static FilterFunctionDefinition s_SepiaDef;

		private static FilterFunctionDefinition s_ContrastDef;

		private static FilterFunctionDefinition s_HueRotateDef;
	}
}
