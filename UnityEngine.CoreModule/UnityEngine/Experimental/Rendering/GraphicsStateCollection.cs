using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Graphics/GraphicsStateCollection.h")]
	public sealed class GraphicsStateCollection : Object
	{
		public bool BeginTrace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.BeginTrace_Injected(intPtr);
		}

		public void EndTrace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.EndTrace_Injected(intPtr);
		}

		public bool isTracing
		{
			[NativeName("IsTracing")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_isTracing_Injected(intPtr);
			}
		}

		public int version
		{
			[NativeName("GetVersion")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_version_Injected(intPtr);
			}
			[NativeName("SetVersion")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_version_Injected(intPtr, value);
			}
		}

		public GraphicsDeviceType graphicsDeviceType
		{
			[NativeName("GetDeviceRenderer")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_graphicsDeviceType_Injected(intPtr);
			}
			[NativeName("SetDeviceRenderer")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_graphicsDeviceType_Injected(intPtr, value);
			}
		}

		public RuntimePlatform runtimePlatform
		{
			[NativeName("GetRuntimePlatform")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_runtimePlatform_Injected(intPtr);
			}
			[NativeName("SetRuntimePlatform")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsStateCollection.set_runtimePlatform_Injected(intPtr, value);
			}
		}

		public unsafe string qualityLevelName
		{
			[NativeName("GetQualityLevelName")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					GraphicsStateCollection.get_qualityLevelName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[NativeName("SetQualityLevelName")]
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					GraphicsStateCollection.set_qualityLevelName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public unsafe bool LoadFromFile(string filePath)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = GraphicsStateCollection.LoadFromFile_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool SaveToFile(string filePath)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = GraphicsStateCollection.SaveToFile_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public unsafe bool SendToEditor(string fileName)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = GraphicsStateCollection.SendToEditor_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeName("Warmup")]
		public JobHandle WarmUp(JobHandle dependency = default(JobHandle))
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			JobHandle jobHandle;
			GraphicsStateCollection.WarmUp_Injected(intPtr, ref dependency, out jobHandle);
			return jobHandle;
		}

		[NativeName("WarmupProgressively")]
		public JobHandle WarmUpProgressively(int count, JobHandle dependency = default(JobHandle))
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			JobHandle jobHandle;
			GraphicsStateCollection.WarmUpProgressively_Injected(intPtr, count, ref dependency, out jobHandle);
			return jobHandle;
		}

		public int totalGraphicsStateCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_totalGraphicsStateCount_Injected(intPtr);
			}
		}

		public int completedWarmupCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_completedWarmupCount_Injected(intPtr);
			}
		}

		public bool isWarmedUp
		{
			[NativeName("IsWarmedUp")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_isWarmedUp_Injected(intPtr);
			}
		}

		private void GetVariants([Out] GraphicsStateCollection.ShaderVariant[] results)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.GetVariants_Injected(intPtr, results);
		}

		public void GetVariants(List<GraphicsStateCollection.ShaderVariant> results)
		{
			bool flag = results == null;
			if (flag)
			{
				throw new ArgumentNullException("The result shader variant list cannot be null.");
			}
			results.Clear();
			NoAllocHelpers.EnsureListElemCount<GraphicsStateCollection.ShaderVariant>(results, this.variantCount);
			this.GetVariants(NoAllocHelpers.ExtractArrayFromList<GraphicsStateCollection.ShaderVariant>(results));
		}

		private void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsStateCollection.GraphicsState[] results)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.GetGraphicsStatesForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords, results);
		}

		public void GetGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, List<GraphicsStateCollection.GraphicsState> results)
		{
			bool flag = results == null;
			if (flag)
			{
				throw new ArgumentNullException("The result graphics state list cannot be null.");
			}
			results.Clear();
			NoAllocHelpers.EnsureListElemCount<GraphicsStateCollection.GraphicsState>(results, this.GetGraphicsStateCountForVariant(shader, passId, keywords));
			this.GetGraphicsStatesForVariant(shader, passId, keywords, NoAllocHelpers.ExtractArrayFromList<GraphicsStateCollection.GraphicsState>(results));
		}

		public int variantCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsStateCollection.get_variantCount_Injected(intPtr);
			}
		}

		public int GetGraphicsStateCountForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.GetGraphicsStateCountForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public bool AddVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			return this.AddVariantByShader(shader, passId, keywords);
		}

		public bool AddVariant(Material mat, PassIdentifier passId)
		{
			return this.AddVariantByMaterial(mat, passId);
		}

		[NativeName("AddVariant")]
		private bool AddVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddVariantByShader_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		[NativeName("AddVariant")]
		private bool AddVariantByMaterial(Material mat, PassIdentifier passId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddVariantByMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat), ref passId);
		}

		public bool AddVariants(Material mat, [DefaultValue("-1")] int subshaderIndex = -1)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddVariants_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat), subshaderIndex);
		}

		public bool RemoveVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			return this.RemoveVariantByShader(shader, passId, keywords);
		}

		public bool RemoveVariant(Material mat, PassIdentifier passId)
		{
			return this.RemoveVariantByMaterial(mat, passId);
		}

		[NativeName("RemoveVariant")]
		private bool RemoveVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.RemoveVariantByShader_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		[NativeName("RemoveVariant")]
		private bool RemoveVariantByMaterial(Material mat, PassIdentifier passId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.RemoveVariantByMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat), ref passId);
		}

		public bool ContainsVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			return this.ContainsVariantByShader(shader, passId, keywords);
		}

		public bool ContainsVariant(Material mat, PassIdentifier passId)
		{
			return this.ContainsVariantByMaterial(mat, passId);
		}

		[NativeName("ContainsVariant")]
		private bool ContainsVariantByShader(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.ContainsVariantByShader_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		[NativeName("ContainsVariant")]
		private bool ContainsVariantByMaterial(Material mat, PassIdentifier passId)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.ContainsVariantByMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(mat), ref passId);
		}

		public void ClearVariants()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsStateCollection.ClearVariants_Injected(intPtr);
		}

		public bool AddGraphicsStateForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords, GraphicsStateCollection.GraphicsState setup)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.AddGraphicsStateForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords, ref setup);
		}

		public bool RemoveGraphicsStatesForVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.RemoveGraphicsStatesForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId, keywords);
		}

		public bool CopyGraphicsStatesForVariant(Shader srcShader, PassIdentifier srcPassId, LocalKeyword[] srcKeywords, Shader dstShader, PassIdentifier dstPassId, LocalKeyword[] dstKeywords)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GraphicsStateCollection>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsStateCollection.CopyGraphicsStatesForVariant_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Shader>(srcShader), ref srcPassId, srcKeywords, Object.MarshalledUnityObject.Marshal<Shader>(dstShader), ref dstPassId, dstKeywords);
		}

		[NativeName("CreateFromScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] GraphicsStateCollection gsc);

		public GraphicsStateCollection()
		{
			GraphicsStateCollection.Internal_Create(this);
		}

		public GraphicsStateCollection(string filePath)
		{
			GraphicsStateCollection.Internal_Create(this);
			this.LoadFromFile(filePath);
		}

		public void GetGraphicsStatesForVariant(GraphicsStateCollection.ShaderVariant variant, List<GraphicsStateCollection.GraphicsState> results)
		{
			this.GetGraphicsStatesForVariant(variant.shader, variant.passId, variant.keywords, results);
		}

		public int GetGraphicsStateCountForVariant(GraphicsStateCollection.ShaderVariant variant)
		{
			return this.GetGraphicsStateCountForVariant(variant.shader, variant.passId, variant.keywords);
		}

		public bool AddGraphicsStateForVariant(GraphicsStateCollection.ShaderVariant variant, GraphicsStateCollection.GraphicsState setup)
		{
			return this.AddGraphicsStateForVariant(variant.shader, variant.passId, variant.keywords, setup);
		}

		public bool RemoveGraphicsStatesForVariant(GraphicsStateCollection.ShaderVariant variant)
		{
			return this.RemoveGraphicsStatesForVariant(variant.shader, variant.passId, variant.keywords);
		}

		public bool CopyGraphicsStatesForVariant(GraphicsStateCollection.ShaderVariant srcVariant, GraphicsStateCollection.ShaderVariant dstVariant)
		{
			return this.CopyGraphicsStatesForVariant(srcVariant.shader, srcVariant.passId, srcVariant.keywords, dstVariant.shader, dstVariant.passId, dstVariant.keywords);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BeginTrace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndTrace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isTracing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_version_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_version_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsDeviceType get_graphicsDeviceType_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_graphicsDeviceType_Injected(IntPtr _unity_self, GraphicsDeviceType value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RuntimePlatform get_runtimePlatform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_runtimePlatform_Injected(IntPtr _unity_self, RuntimePlatform value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_qualityLevelName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_qualityLevelName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadFromFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SaveToFile_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filePath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendToEditor_Injected(IntPtr _unity_self, ref ManagedSpanWrapper fileName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmUp_Injected(IntPtr _unity_self, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WarmUpProgressively_Injected(IntPtr _unity_self, int count, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_totalGraphicsStateCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_completedWarmupCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isWarmedUp_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVariants_Injected(IntPtr _unity_self, [Out] GraphicsStateCollection.ShaderVariant[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [Out] GraphicsStateCollection.GraphicsState[] results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_variantCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGraphicsStateCountForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddVariants_Injected(IntPtr _unity_self, IntPtr mat, [DefaultValue("-1")] int subshaderIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsVariantByShader_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsVariantByMaterial_Injected(IntPtr _unity_self, IntPtr mat, [In] ref PassIdentifier passId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearVariants_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddGraphicsStateForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords, [In] ref GraphicsStateCollection.GraphicsState setup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr shader, [In] ref PassIdentifier passId, LocalKeyword[] keywords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CopyGraphicsStatesForVariant_Injected(IntPtr _unity_self, IntPtr srcShader, [In] ref PassIdentifier srcPassId, LocalKeyword[] srcKeywords, IntPtr dstShader, [In] ref PassIdentifier dstPassId, LocalKeyword[] dstKeywords);

		public struct GraphicsState
		{
			public void SetMeshData(Mesh mesh, int submesh, [DefaultValue("null")] Renderer renderer = null)
			{
				GraphicsStateCollection.GraphicsState.SetMeshData_Injected(ref this, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submesh, Object.MarshalledUnityObject.Marshal<Renderer>(renderer));
			}

			[NativeName("SetRenderPassData")]
			private unsafe void SetRenderPassData_Internal(int samples, ReadOnlySpan<AttachmentDescriptor> attachments, ReadOnlySpan<SubPassDescriptor> subPasses, int subPassIndex, int depthAttachmentIndex, int shadingRateIndex)
			{
				ReadOnlySpan<AttachmentDescriptor> readOnlySpan = attachments;
				fixed (AttachmentDescriptor* ptr = readOnlySpan.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					ReadOnlySpan<SubPassDescriptor> readOnlySpan2 = subPasses;
					fixed (SubPassDescriptor* pinnableReference = readOnlySpan2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
						GraphicsStateCollection.GraphicsState.SetRenderPassData_Internal_Injected(ref this, samples, ref managedSpanWrapper, ref managedSpanWrapper2, subPassIndex, depthAttachmentIndex, shadingRateIndex);
						ptr = null;
					}
				}
			}

			public void SetRenderPassData(int samples, NativeArray<AttachmentDescriptor> attachments, NativeArray<SubPassDescriptor> subPasses, [DefaultValue("0")] int subPassIndex = 0, [DefaultValue("-1")] int depthAttachmentIndex = -1, [DefaultValue("-1")] int shadingRateIndex = -1)
			{
				this.SetRenderPassData_Internal(samples, in attachments, in subPasses, subPassIndex, depthAttachmentIndex, shadingRateIndex);
			}

			public void SetRenderStateData(Shader shader, PassIdentifier passId)
			{
				GraphicsStateCollection.GraphicsState.SetRenderStateData_Injected(ref this, Object.MarshalledUnityObject.Marshal<Shader>(shader), ref passId);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshData_Injected(ref GraphicsStateCollection.GraphicsState _unity_self, IntPtr mesh, int submesh, [DefaultValue("null")] IntPtr renderer);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRenderPassData_Internal_Injected(ref GraphicsStateCollection.GraphicsState _unity_self, int samples, ref ManagedSpanWrapper attachments, ref ManagedSpanWrapper subPasses, int subPassIndex, int depthAttachmentIndex, int shadingRateIndex);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRenderStateData_Injected(ref GraphicsStateCollection.GraphicsState _unity_self, IntPtr shader, [In] ref PassIdentifier passId);

			public VertexAttributeDescriptor[] vertexAttributes;

			public AttachmentDescriptor[] attachments;

			public SubPassDescriptor[] subPasses;

			public RenderStateBlock renderState;

			public MeshTopology topology;

			public CullMode forceCullMode;

			public ShadingRateCombiner shadingRateCombinerPrimitive;

			public ShadingRateCombiner shadingRateCombinerFragment;

			public ShadingRateFragmentSize baseShadingRate;

			public float depthBias;

			public float slopeDepthBias;

			public int depthAttachmentIndex;

			public int subPassIndex;

			public int shadingRateIndex;

			public int multiviewCount;

			public int sampleCount;

			public bool hasEyeTexture;

			public bool wireframe;

			public bool invertCulling;

			public bool negativeScale;

			public bool invertProjection;
		}

		public struct ShaderVariant
		{
			public ShaderVariant(Shader shader, PassIdentifier passId, LocalKeyword[] keywords)
			{
				this.shader = shader;
				this.passId = passId;
				this.keywords = keywords;
			}

			public ShaderVariant(Material material, PassIdentifier passId)
			{
				this.shader = material.shader;
				this.passId = passId;
				this.keywords = material.enabledKeywords;
			}

			public Shader shader;

			public PassIdentifier passId;

			public LocalKeyword[] keywords;
		}
	}
}
