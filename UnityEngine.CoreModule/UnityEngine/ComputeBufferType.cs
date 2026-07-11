using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>ComputeBuffer type.</para>
	/// </summary>
	[Flags]
	public enum ComputeBufferType
	{
		/// <summary>
		///   <para>Default ComputeBuffer type (structured buffer).</para>
		/// </summary>
		Default = 0,
		/// <summary>
		///   <para>Raw ComputeBuffer type (byte address buffer).</para>
		/// </summary>
		Raw = 1,
		/// <summary>
		///   <para>Append-consume ComputeBuffer type.</para>
		/// </summary>
		Append = 2,
		/// <summary>
		///   <para>ComputeBuffer with a counter.</para>
		/// </summary>
		Counter = 4,
		[Obsolete("Enum member DrawIndirect has been deprecated. Use IndirectArguments instead (UnityUpgradable) -> IndirectArguments", false)]
		DrawIndirect = 256,
		/// <summary>
		///   <para>ComputeBuffer used for Graphics.DrawProceduralIndirect, ComputeShader.DispatchIndirect or Graphics.DrawMeshInstancedIndirect arguments.</para>
		/// </summary>
		IndirectArguments = 256,
		[Obsolete("Enum member GPUMemory has been deprecated. All compute buffers now follow the behavior previously defined by this member.", false)]
		GPUMemory = 512
	}
}
