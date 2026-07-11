using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>An asset that produces a specific IRenderPipeline.</para>
	/// </summary>
	public abstract class RenderPipelineAsset : ScriptableObject, IRenderPipelineAsset
	{
		/// <summary>
		///   <para>Destroys all cached data and created IRenderLoop's.</para>
		/// </summary>
		public void DestroyCreatedInstances()
		{
			foreach (IRenderPipeline renderPipeline in this.m_CreatedPipelines)
			{
				renderPipeline.Dispose();
			}
			this.m_CreatedPipelines.Clear();
		}

		/// <summary>
		///   <para>Create a IRenderPipeline specific to this asset.</para>
		/// </summary>
		/// <returns>
		///   <para>Created pipeline.</para>
		/// </returns>
		public IRenderPipeline CreatePipeline()
		{
			IRenderPipeline renderPipeline = this.InternalCreatePipeline();
			if (renderPipeline != null)
			{
				this.m_CreatedPipelines.Add(renderPipeline);
			}
			return renderPipeline;
		}

		/// <summary>
		///   <para>Return the default Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default particle Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultParticleMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default Line Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultLineMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default Terrain  Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultTerrainMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default UI Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultUIMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default UI overdraw Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultUIOverdrawMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default UI ETC1  Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefaultUIETC1SupportedMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default 2D Material for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default material.</para>
		/// </returns>
		public virtual Material GetDefault2DMaterial()
		{
			return null;
		}

		/// <summary>
		///   <para>Return the default Shader for this pipeline.</para>
		/// </summary>
		/// <returns>
		///   <para>Default shader.</para>
		/// </returns>
		public virtual Shader GetDefaultShader()
		{
			return null;
		}

		/// <summary>
		///   <para>Create a IRenderPipeline specific to this asset.</para>
		/// </summary>
		/// <returns>
		///   <para>Created pipeline.</para>
		/// </returns>
		protected abstract IRenderPipeline InternalCreatePipeline();

		/// <summary>
		///   <para>Returns the list of current IRenderPipeline's created by the asset.</para>
		/// </summary>
		/// <returns>
		///   <para>Enumerable of created pipelines.</para>
		/// </returns>
		protected IEnumerable<IRenderPipeline> CreatedInstances()
		{
			return this.m_CreatedPipelines;
		}

		/// <summary>
		///   <para>Default implementation of OnValidate for RenderPipelineAsset. See MonoBehaviour.OnValidate</para>
		/// </summary>
		protected virtual void OnValidate()
		{
			this.DestroyCreatedInstances();
		}

		/// <summary>
		///   <para>Default implementation of OnDisable for RenderPipelineAsset. See ScriptableObject.OnDisable</para>
		/// </summary>
		protected virtual void OnDisable()
		{
			this.DestroyCreatedInstances();
		}

		private readonly List<IRenderPipeline> m_CreatedPipelines = new List<IRenderPipeline>();
	}
}
