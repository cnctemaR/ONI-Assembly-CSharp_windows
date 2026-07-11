using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Scripting/TextAsset.h")]
	public class TextAsset : Object
	{
		public TextAsset()
			: this(TextAsset.CreateOptions.CreateNativeObject, null)
		{
		}

		public TextAsset(string text)
			: this(TextAsset.CreateOptions.CreateNativeObject, text)
		{
		}

		internal TextAsset(TextAsset.CreateOptions options, string text)
		{
			if (options == TextAsset.CreateOptions.CreateNativeObject)
			{
				TextAsset.Internal_CreateInstance(this, text);
			}
		}

		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override string ToString()
		{
			return this.text;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateInstance([Writable] TextAsset self, string text);

		internal enum CreateOptions
		{
			None,
			CreateNativeObject
		}
	}
}
