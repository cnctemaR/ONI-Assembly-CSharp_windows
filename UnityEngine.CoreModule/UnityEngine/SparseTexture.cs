using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/SparseTexture.h")]
	public sealed class SparseTexture : Texture
	{
		public int tileWidth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SparseTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SparseTexture.get_tileWidth_Injected(intPtr);
			}
		}

		public int tileHeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SparseTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SparseTexture.get_tileHeight_Injected(intPtr);
			}
		}

		public bool isCreated
		{
			[NativeName("IsInitialized")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SparseTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SparseTexture.get_isCreated_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "SparseTextureScripting::Create", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, GraphicsFormat format, TextureColorSpace colorSpace, int mipCount);

		[FreeFunction(Name = "SparseTextureScripting::UpdateTile", HasExplicitThis = true)]
		public unsafe void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SparseTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color32> span = new Span<Color32>(data);
			fixed (Color32* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				SparseTexture.UpdateTile_Injected(intPtr, tileX, tileY, miplevel, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "SparseTextureScripting::UpdateTileRaw", HasExplicitThis = true)]
		public unsafe void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SparseTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<byte> span = new Span<byte>(data);
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				SparseTexture.UpdateTileRaw_Injected(intPtr, tileX, tileY, miplevel, ref managedSpanWrapper);
			}
		}

		public void UnloadTile(int tileX, int tileY, int miplevel)
		{
			this.UpdateTileRaw(tileX, tileY, miplevel, null);
		}

		internal bool ValidateFormat(TextureFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4;
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		internal bool ValidateFormat(GraphicsFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format, GraphicsFormatUsage.Sparse);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = GraphicsFormatUtility.IsPVRTCFormat(format);
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		internal bool ValidateSize(int width, int height, GraphicsFormat format)
		{
			bool flag = (ulong)GraphicsFormatUtility.GetBlockSize(format) * (ulong)((long)width / (long)((ulong)GraphicsFormatUtility.GetBlockWidth(format))) * (ulong)((long)height / (long)((ulong)GraphicsFormatUtility.GetBlockHeight(format))) < 65536UL;
			bool flag2;
			if (flag)
			{
				Debug.LogError("SparseTexture creation failed. The minimum size in bytes of a SparseTexture is 64KB.", this);
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			return flag2;
		}

		private static void ValidateIsNotCrunched(TextureFormat textureFormat)
		{
			bool flag = GraphicsFormatUtility.IsCrunchFormat(textureFormat);
			if (flag)
			{
				throw new ArgumentException("Crunched SparseTexture is not supported.");
			}
		}

		[ExcludeFromDocs]
		public SparseTexture(int width, int height, DefaultFormat format, int mipCount)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), mipCount)
		{
		}

		[ExcludeFromDocs]
		public SparseTexture(int width, int height, GraphicsFormat format, int mipCount)
		{
			bool flag = !this.ValidateFormat(format, width, height);
			if (!flag)
			{
				bool flag2 = !this.ValidateSize(width, height, format);
				if (!flag2)
				{
					SparseTexture.Internal_Create(this, width, height, format, base.GetTextureColorSpace(format), mipCount);
				}
			}
		}

		[ExcludeFromDocs]
		public SparseTexture(int width, int height, TextureFormat textureFormat, int mipCount)
			: this(width, height, textureFormat, mipCount, false)
		{
		}

		public SparseTexture(int width, int height, TextureFormat textureFormat, int mipCount, [DefaultValue("false")] bool linear)
		{
			bool flag = !this.ValidateFormat(textureFormat, width, height);
			if (!flag)
			{
				SparseTexture.ValidateIsNotCrunched(textureFormat);
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
				bool flag2 = !SystemInfo.IsFormatSupported(graphicsFormat, GraphicsFormatUsage.Sparse);
				if (flag2)
				{
					Debug.LogError(string.Format("Creation of a SparseTexture with '{0}' is not supported on this platform.", textureFormat));
				}
				else
				{
					bool flag3 = !this.ValidateSize(width, height, graphicsFormat);
					if (!flag3)
					{
						SparseTexture.Internal_Create(this, width, height, graphicsFormat, base.GetTextureColorSpace(linear), mipCount);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_tileWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_tileHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isCreated_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateTile_Injected(IntPtr _unity_self, int tileX, int tileY, int miplevel, ref ManagedSpanWrapper data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateTileRaw_Injected(IntPtr _unity_self, int tileX, int tileY, int miplevel, ref ManagedSpanWrapper data);
	}
}
