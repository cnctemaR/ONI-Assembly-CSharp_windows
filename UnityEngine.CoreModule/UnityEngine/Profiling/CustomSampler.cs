using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	/// <summary>
	///   <para>Custom CPU Profiler label used for profiling arbitrary code blocks.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class CustomSampler : Sampler
	{
		internal CustomSampler()
		{
		}

		/// <summary>
		///   <para>Creates a new CustomSampler for profiling parts of your code.</para>
		/// </summary>
		/// <param name="name">Name of the Sampler.</param>
		/// <returns>
		///   <para>CustomSampler object or null if a built-in Sampler with the same name exists.</para>
		/// </returns>
		public static CustomSampler Create(string name)
		{
			CustomSampler customSampler = CustomSampler.CreateInternal(name);
			return customSampler ?? CustomSampler.s_InvalidCustomSampler;
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomSampler CreateInternal(string name);

		/// <summary>
		///   <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
		/// </summary>
		/// <param name="targetObject"></param>
		[Conditional("ENABLE_PROFILER")]
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Begin();

		/// <summary>
		///   <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
		/// </summary>
		/// <param name="targetObject"></param>
		[Conditional("ENABLE_PROFILER")]
		public void Begin(Object targetObject)
		{
			this.BeginWithObject(targetObject);
		}

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void BeginWithObject(Object targetObject);

		/// <summary>
		///   <para>End profiling a piece of code with a custom label.</para>
		/// </summary>
		[Conditional("ENABLE_PROFILER")]
		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void End();

		internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();
	}
}
