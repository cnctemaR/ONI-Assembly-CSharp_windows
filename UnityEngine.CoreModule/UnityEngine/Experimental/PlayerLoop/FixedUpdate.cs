using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop
{
	/// <summary>
	///   <para>Update phase in the native player loop.</para>
	/// </summary>
	[RequiredByNativeCode]
	public struct FixedUpdate
	{
		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ClearLines
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorFixedSampleTime
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct AudioFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunBehaviourFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct LegacyFixedAnimationUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct XRFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PhysicsFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct Physics2DFixedUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorFixedUpdatePostPhysics
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunDelayedFixedFrameRate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct NewInputFixedUpdate
		{
		}
	}
}
