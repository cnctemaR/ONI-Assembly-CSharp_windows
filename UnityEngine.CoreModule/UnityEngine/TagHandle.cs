using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public struct TagHandle
	{
		public static TagHandle GetExistingTag(string tagName)
		{
			return new TagHandle
			{
				_tagIndex = TagHandle.ExtractTagThrowing(tagName)
			};
		}

		public override string ToString()
		{
			return TagHandle.TagToString(this._tagIndex);
		}

		[NativeThrows]
		[FreeFunction]
		[NativeHeader("Runtime/Export/Scripting/GameObject.bindings.h")]
		private unsafe static uint ExtractTagThrowing(string tagName)
		{
			uint num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tagName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tagName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = TagHandle.ExtractTagThrowing_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		private static string TagToString(uint tagIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				TagHandle.TagToString_Injected(tagIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint ExtractTagThrowing_Injected(ref ManagedSpanWrapper tagName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TagToString_Injected(uint tagIndex, out ManagedSpanWrapper ret);

		private uint _tagIndex;
	}
}
