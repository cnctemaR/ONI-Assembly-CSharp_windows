using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Text file assets.</para>
	/// </summary>
	[NativeHeader("Runtime/Scripting/TextAsset.h")]
	public class TextAsset : Object
	{
		public TextAsset()
			: this(TextAsset.CreateOptions.CreateNativeObject, null)
		{
		}

		/// <summary>
		///   <para>Create a new TextAsset with the specified text contents.
		///
		/// This constructor creates a TextAsset, which is not the same as a plain text file. When saved to disk using the AssetDatabase class, the TextAsset should be saved with the .asset extension.</para>
		/// </summary>
		/// <param name="text">The text contents for the TextAsset.</param>
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

		/// <summary>
		///   <para>The text contents of the .txt file as a string. (Read Only)</para>
		/// </summary>
		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The raw bytes of the text asset. (Read Only)</para>
		/// </summary>
		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the contents of the TextAsset.</para>
		/// </summary>
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
