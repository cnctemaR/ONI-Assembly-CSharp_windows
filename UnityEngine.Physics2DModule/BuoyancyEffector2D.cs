using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Applies forces to simulate buoyancy, fluid-flow and fluid drag.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/BuoyancyEffector2D.h")]
	public class BuoyancyEffector2D : Effector2D
	{
		/// <summary>
		///   <para>Defines an arbitrary horizontal line that represents the fluid surface level.</para>
		/// </summary>
		public extern float surfaceLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The density of the fluid used to calculate the buoyancy forces.</para>
		/// </summary>
		public extern float density
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A force applied to slow linear movement of any Collider2D in contact with the effector.</para>
		/// </summary>
		public extern float linearDrag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>A force applied to slow angular movement of any Collider2D in contact with the effector.</para>
		/// </summary>
		public extern float angularDrag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angle of the force used to similate fluid flow.</para>
		/// </summary>
		public extern float flowAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The magnitude of the force used to similate fluid flow.</para>
		/// </summary>
		public extern float flowMagnitude
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The random variation of the force used to similate fluid flow.</para>
		/// </summary>
		public extern float flowVariation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
