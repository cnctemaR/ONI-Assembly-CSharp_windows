using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public abstract class UxmlSerializedData
	{
		public static void Register()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldWriteAttributeValue(UxmlSerializedData.UxmlAttributeFlags attributeFlags)
		{
			return (attributeFlags & UxmlSerializedData.s_CurrentDeserializeFlags) > UxmlSerializedData.UxmlAttributeFlags.Ignore;
		}

		public abstract object CreateInstance();

		public abstract void Deserialize(object obj);

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void Deserialize(object obj, UxmlSerializedData.UxmlAttributeFlags flags)
		{
			try
			{
				UxmlSerializedData.s_CurrentDeserializeFlags = flags;
				this.Deserialize(obj);
			}
			finally
			{
				UxmlSerializedData.s_CurrentDeserializeFlags = UxmlSerializedData.UxmlAttributeFlags.OverriddenInUxml;
			}
		}

		internal const string AttributeFlagSuffix = "_UxmlAttributeFlags";

		private const UxmlSerializedData.UxmlAttributeFlags k_DefaultFlags = UxmlSerializedData.UxmlAttributeFlags.OverriddenInUxml;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal int uxmlAssetId;

		private static UxmlSerializedData.UxmlAttributeFlags s_CurrentDeserializeFlags = UxmlSerializedData.UxmlAttributeFlags.OverriddenInUxml;

		[Flags]
		public enum UxmlAttributeFlags : byte
		{
			Ignore = 0,
			OverriddenInUxml = 1,
			DefaultValue = 2
		}
	}
}
