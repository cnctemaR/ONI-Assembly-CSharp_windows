using System;
using System.Reflection;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SupportedOnRenderPipelineAttribute : Attribute
	{
		public Type[] renderPipelineTypes { get; }

		public SupportedOnRenderPipelineAttribute(Type renderPipeline)
			: this(new Type[] { renderPipeline })
		{
		}

		public SupportedOnRenderPipelineAttribute(params Type[] renderPipeline)
		{
			bool flag = renderPipeline == null;
			if (flag)
			{
				Debug.LogError("The SupportedOnRenderPipelineAttribute parameters cannot be null.");
			}
			else
			{
				foreach (Type type in renderPipeline)
				{
					bool flag2 = type != null && typeof(RenderPipelineAsset).IsAssignableFrom(type);
					if (!flag2)
					{
						Debug.LogError("The SupportedOnRenderPipelineAttribute Attribute targets an invalid RenderPipelineAsset. One of the types cannot be assigned from RenderPipelineAsset: [" + renderPipeline.SerializedView<Type>((Type t) => t.Name) + "].");
						return;
					}
				}
				this.renderPipelineTypes = ((renderPipeline.Length == 0) ? SupportedOnRenderPipelineAttribute.k_DefaultRenderPipelineAsset.Value : renderPipeline);
			}
		}

		public bool isSupportedOnCurrentPipeline
		{
			get
			{
				return SupportedOnRenderPipelineAttribute.GetSupportedMode(this.renderPipelineTypes, GraphicsSettings.currentRenderPipelineAssetType) > SupportedOnRenderPipelineAttribute.SupportedMode.Unsupported;
			}
		}

		public SupportedOnRenderPipelineAttribute.SupportedMode GetSupportedMode(Type renderPipelineAssetType)
		{
			return SupportedOnRenderPipelineAttribute.GetSupportedMode(this.renderPipelineTypes, renderPipelineAssetType);
		}

		internal static SupportedOnRenderPipelineAttribute.SupportedMode GetSupportedMode(Type[] renderPipelineTypes, Type renderPipelineAssetType)
		{
			bool flag = renderPipelineTypes == null;
			if (flag)
			{
				throw new ArgumentNullException("Parameter renderPipelineTypes cannot be null.");
			}
			bool flag2 = renderPipelineAssetType == null;
			SupportedOnRenderPipelineAttribute.SupportedMode supportedMode;
			if (flag2)
			{
				supportedMode = SupportedOnRenderPipelineAttribute.SupportedMode.Unsupported;
			}
			else
			{
				for (int i = 0; i < renderPipelineTypes.Length; i++)
				{
					bool flag3 = renderPipelineTypes[i] == renderPipelineAssetType;
					if (flag3)
					{
						return SupportedOnRenderPipelineAttribute.SupportedMode.Supported;
					}
				}
				for (int j = 0; j < renderPipelineTypes.Length; j++)
				{
					bool flag4 = renderPipelineTypes[j].IsAssignableFrom(renderPipelineAssetType);
					if (flag4)
					{
						return SupportedOnRenderPipelineAttribute.SupportedMode.SupportedByBaseClass;
					}
				}
				supportedMode = SupportedOnRenderPipelineAttribute.SupportedMode.Unsupported;
			}
			return supportedMode;
		}

		public static bool IsTypeSupportedOnRenderPipeline(Type type, Type renderPipelineAssetType)
		{
			SupportedOnRenderPipelineAttribute customAttribute = type.GetCustomAttribute<SupportedOnRenderPipelineAttribute>();
			return customAttribute == null || customAttribute.GetSupportedMode(renderPipelineAssetType) > SupportedOnRenderPipelineAttribute.SupportedMode.Unsupported;
		}

		private static readonly Lazy<Type[]> k_DefaultRenderPipelineAsset = new Lazy<Type[]>(() => new Type[] { typeof(RenderPipelineAsset) });

		public enum SupportedMode
		{
			Unsupported,
			Supported,
			SupportedByBaseClass
		}
	}
}
