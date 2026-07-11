using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Used to communicate between scripting and the controller. Some parameters can be set in scripting and used by the controller, while other parameters are based on Custom Curves in Animation Clips and can be sampled using the scripting API.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
	[NativeAsStruct]
	[UsedByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoAnimatorControllerParameter")]
	[NativeHeader("Runtime/Animation/AnimatorControllerParameter.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimatorControllerParameter
	{
		/// <summary>
		///   <para>The name of the parameter.</para>
		/// </summary>
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		/// <summary>
		///   <para>Returns the hash of the parameter based on its name.</para>
		/// </summary>
		public int nameHash
		{
			get
			{
				return Animator.StringToHash(this.m_Name);
			}
		}

		/// <summary>
		///   <para>The type of the parameter.</para>
		/// </summary>
		public AnimatorControllerParameterType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		/// <summary>
		///   <para>The default float value for the parameter.</para>
		/// </summary>
		public float defaultFloat
		{
			get
			{
				return this.m_DefaultFloat;
			}
			set
			{
				this.m_DefaultFloat = value;
			}
		}

		/// <summary>
		///   <para>The default int value for the parameter.</para>
		/// </summary>
		public int defaultInt
		{
			get
			{
				return this.m_DefaultInt;
			}
			set
			{
				this.m_DefaultInt = value;
			}
		}

		/// <summary>
		///   <para>The default bool value for the parameter.</para>
		/// </summary>
		public bool defaultBool
		{
			get
			{
				return this.m_DefaultBool;
			}
			set
			{
				this.m_DefaultBool = value;
			}
		}

		public override bool Equals(object o)
		{
			AnimatorControllerParameter animatorControllerParameter = o as AnimatorControllerParameter;
			return animatorControllerParameter != null && this.m_Name == animatorControllerParameter.m_Name && this.m_Type == animatorControllerParameter.m_Type && this.m_DefaultFloat == animatorControllerParameter.m_DefaultFloat && this.m_DefaultInt == animatorControllerParameter.m_DefaultInt && this.m_DefaultBool == animatorControllerParameter.m_DefaultBool;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		internal string m_Name = "";

		internal AnimatorControllerParameterType m_Type;

		internal float m_DefaultFloat;

		internal int m_DefaultInt;

		internal bool m_DefaultBool;
	}
}
