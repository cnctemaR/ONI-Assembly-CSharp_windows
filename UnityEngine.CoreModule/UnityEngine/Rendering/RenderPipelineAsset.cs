using System;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipelineAsset : ScriptableObject
	{
		internal RenderPipeline InternalCreatePipeline()
		{
			RenderPipeline renderPipeline = null;
			try
			{
				renderPipeline = this.CreatePipeline();
			}
			catch (InvalidImportException)
			{
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return renderPipeline;
		}

		public virtual Material defaultMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Shader autodeskInteractiveShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader autodeskInteractiveTransparentShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader autodeskInteractiveMaskedShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader terrainDetailLitShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader terrainDetailGrassShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader terrainDetailGrassBillboardShader
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultParticleMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultLineMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultTerrainMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultUIMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultUIOverdrawMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material defaultUIETC1SupportedMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material default2DMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Material default2DMaskMaterial
		{
			get
			{
				return null;
			}
		}

		public virtual Shader defaultShader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader defaultSpeedTree7Shader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader defaultSpeedTree8Shader
		{
			get
			{
				return null;
			}
		}

		public virtual Shader defaultSpeedTree9Shader
		{
			get
			{
				return null;
			}
		}

		public virtual string renderPipelineShaderTag
		{
			get
			{
				Debug.LogWarning("The property renderPipelineShaderTag has not been overridden. At build time, any shader variants that use any RenderPipeline tag will be stripped.");
				return string.Empty;
			}
		}

		protected abstract RenderPipeline CreatePipeline();

		public virtual Type pipelineType
		{
			get
			{
				Debug.LogWarning("You must either inherit from RenderPipelineAsset<TRenderPipeline> or override pipelineType property.");
				return null;
			}
		}

		internal string pipelineTypeFullName
		{
			get
			{
				Type pipelineType = this.pipelineType;
				return ((pipelineType != null) ? pipelineType.FullName : null) ?? string.Empty;
			}
		}

		protected virtual void EnsureGlobalSettings()
		{
		}

		protected virtual void OnValidate()
		{
			RenderPipelineManager.RecreateCurrentPipeline(this);
		}

		protected virtual void OnDisable()
		{
			RenderPipelineManager.CleanupRenderPipeline();
		}

		protected internal virtual bool requiresCompatibleRenderPipelineGlobalSettings { get; } = false;

		[Obsolete("This property is obsolete. Use pipelineType instead. #from(23.2)", false)]
		protected internal virtual Type renderPipelineType
		{
			get
			{
				Debug.LogWarning("You must either inherit from RenderPipelineAsset<TRenderPipeline> or override renderPipelineType property");
				return null;
			}
		}

		[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
		public virtual string[] renderingLayerMaskNames
		{
			get
			{
				return null;
			}
		}

		[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
		public virtual string[] prefixedRenderingLayerMaskNames
		{
			get
			{
				return null;
			}
		}
	}
}
