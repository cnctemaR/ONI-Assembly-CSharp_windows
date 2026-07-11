using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Physics material describes how to handle colliding objects (friction, bounciness).</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/PhysicMaterial.h")]
	public class PhysicMaterial : Object
	{
		/// <summary>
		///   <para>Creates a new material.</para>
		/// </summary>
		public PhysicMaterial()
		{
			PhysicMaterial.Internal_CreateDynamicsMaterial(this, "DynamicMaterial");
		}

		/// <summary>
		///   <para>Creates a new material named name.</para>
		/// </summary>
		/// <param name="name"></param>
		public PhysicMaterial(string name)
		{
			PhysicMaterial.Internal_CreateDynamicsMaterial(this, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateDynamicsMaterial([Writable] PhysicMaterial mat, string name);

		/// <summary>
		///   <para>How bouncy is the surface? A value of 0 will not bounce. A value of 1 will bounce without any loss of energy.</para>
		/// </summary>
		public extern float bounciness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The friction used when already moving.  This value is usually between 0 and 1.</para>
		/// </summary>
		public extern float dynamicFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The friction coefficient used when an object is lying on a surface.</para>
		/// </summary>
		public extern float staticFriction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines how the friction is combined.</para>
		/// </summary>
		public extern PhysicMaterialCombine frictionCombine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines how the bounciness is combined.</para>
		/// </summary>
		public extern PhysicMaterialCombine bounceCombine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use PhysicMaterial.bounciness instead (UnityUpgradable) -> bounciness")]
		public float bouncyness
		{
			get
			{
				return this.bounciness;
			}
			set
			{
				this.bounciness = value;
			}
		}

		/// <summary>
		///   <para>The direction of anisotropy. Anisotropic friction is enabled if the vector is not zero.</para>
		/// </summary>
		[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
		public Vector3 frictionDirection2
		{
			get
			{
				return Vector3.zero;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>If anisotropic friction is enabled, dynamicFriction2 will be applied along frictionDirection2.</para>
		/// </summary>
		[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
		public float dynamicFriction2
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>If anisotropic friction is enabled, staticFriction2 will be applied along frictionDirection2.</para>
		/// </summary>
		[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
		public float staticFriction2
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Anisotropic friction is no longer supported since Unity 5.0.", true)]
		public Vector3 frictionDirection
		{
			get
			{
				return Vector3.zero;
			}
			set
			{
			}
		}
	}
}
