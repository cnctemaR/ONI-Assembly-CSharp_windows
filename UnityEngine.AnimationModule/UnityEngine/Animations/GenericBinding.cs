using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[NativeType(CodegenOptions.Custom, "UnityEngine::Animation::MonoGenericBinding")]
	[UsedByNativeCode]
	public readonly struct GenericBinding
	{
		public bool isObjectReference
		{
			get
			{
				return (this.m_Flags & Flags.kPPtr) == Flags.kPPtr;
			}
		}

		public bool isDiscrete
		{
			get
			{
				return (this.m_Flags & Flags.kDiscrete) > Flags.kNone;
			}
		}

		public bool isSerializeReference
		{
			get
			{
				return (this.m_Flags & Flags.kSerializeReference) == Flags.kSerializeReference;
			}
		}

		public uint transformPathHash
		{
			get
			{
				return this.m_Path;
			}
		}

		public uint propertyNameHash
		{
			get
			{
				return this.m_PropertyName;
			}
		}

		public EntityId scriptEntityId
		{
			get
			{
				return this.m_ScriptEntityId;
			}
		}

		[Obsolete("scriptInstanceID is deprecated. Use scriptEntityId instead.", false)]
		public int scriptInstanceID
		{
			get
			{
				return this.m_ScriptEntityId;
			}
		}

		public int typeID
		{
			get
			{
				return this.m_TypeID;
			}
		}

		public byte customTypeID
		{
			get
			{
				return this.m_CustomType;
			}
		}

		private readonly uint m_Path;

		private readonly uint m_PropertyName;

		private readonly EntityId m_ScriptEntityId;

		private readonly int m_TypeID;

		private readonly byte m_CustomType;

		internal readonly Flags m_Flags;
	}
}
