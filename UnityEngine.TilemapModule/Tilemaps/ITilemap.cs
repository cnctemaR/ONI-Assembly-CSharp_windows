using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine.Tilemaps
{
	[RequiredByNativeCode]
	public class ITilemap
	{
		internal ITilemap()
		{
		}

		public ITilemap(Tilemap tilemap)
		{
			bool flag = tilemap == null;
			if (flag)
			{
				throw new ArgumentNullException("Argument tilemap cannot be null");
			}
			this.m_Tilemap = tilemap;
		}

		public static implicit operator ITilemap(Tilemap tilemap)
		{
			return new ITilemap(tilemap);
		}

		internal void SetTilemapInstance(Tilemap tilemap)
		{
			this.m_Tilemap = tilemap;
		}

		public Vector3Int origin
		{
			get
			{
				return this.m_Tilemap.origin;
			}
		}

		public Vector3Int size
		{
			get
			{
				return this.m_Tilemap.size;
			}
		}

		public Bounds localBounds
		{
			get
			{
				return this.m_Tilemap.localBounds;
			}
		}

		public BoundsInt cellBounds
		{
			get
			{
				return this.m_Tilemap.cellBounds;
			}
		}

		public virtual Sprite GetSprite(Vector3Int position)
		{
			return this.m_Tilemap.GetSprite(position);
		}

		public virtual Color GetColor(Vector3Int position)
		{
			return this.m_Tilemap.GetColor(position);
		}

		public virtual Matrix4x4 GetTransformMatrix(Vector3Int position)
		{
			return this.m_Tilemap.GetTransformMatrix(position);
		}

		public virtual TileFlags GetTileFlags(Vector3Int position)
		{
			return this.m_Tilemap.GetTileFlags(position);
		}

		public virtual TileBase GetTile(Vector3Int position)
		{
			return this.m_Tilemap.GetTile(position);
		}

		public virtual T GetTile<T>(Vector3Int position) where T : TileBase
		{
			return this.m_Tilemap.GetTile<T>(position);
		}

		public void RefreshTile(Vector3Int position)
		{
			bool addToList = this.m_AddToList;
			if (addToList)
			{
				bool flag = this.m_RefreshCount >= this.m_RefreshPos.Length;
				if (flag)
				{
					NativeArray<Vector3Int> nativeArray = new NativeArray<Vector3Int>(Math.Max(1, this.m_RefreshCount * 2), Allocator.Temp, NativeArrayOptions.ClearMemory);
					NativeArray<Vector3Int>.Copy(this.m_RefreshPos, nativeArray, this.m_RefreshPos.Length);
					this.m_RefreshPos.Dispose();
					this.m_RefreshPos = nativeArray;
				}
				int refreshCount = this.m_RefreshCount;
				this.m_RefreshCount = refreshCount + 1;
				this.m_RefreshPos[refreshCount] = position;
			}
			else
			{
				this.m_Tilemap.RefreshTile(position);
			}
		}

		public T GetComponent<T>()
		{
			bool flag = typeof(T) == typeof(Tilemap);
			T t;
			if (flag)
			{
				t = (T)((object)this.m_Tilemap);
			}
			else
			{
				t = this.m_Tilemap.GetComponent<T>();
			}
			return t;
		}

		[RequiredByNativeCode]
		private static ITilemap CreateInstance()
		{
			ITilemap.s_Instance = new ITilemap();
			return ITilemap.s_Instance;
		}

		[RequiredByNativeCode]
		private unsafe static void FindAllRefreshPositions(ITilemap tilemap, int count, IntPtr oldTilesIntPtr, IntPtr newTilesIntPtr, IntPtr positionsIntPtr)
		{
			tilemap.m_AddToList = true;
			NativeArray<Vector3Int> refreshPos = tilemap.m_RefreshPos;
			bool flag = !tilemap.m_RefreshPos.IsCreated || tilemap.m_RefreshPos.Length < count;
			if (flag)
			{
				tilemap.m_RefreshPos = new NativeArray<Vector3Int>(Math.Max(16, count), Allocator.Temp, NativeArrayOptions.ClearMemory);
			}
			tilemap.m_RefreshCount = 0;
			void* ptr = oldTilesIntPtr.ToPointer();
			void* ptr2 = newTilesIntPtr.ToPointer();
			void* ptr3 = positionsIntPtr.ToPointer();
			NativeArray<int> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(ptr, count, Allocator.Invalid);
			NativeArray<int> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(ptr2, count, Allocator.Invalid);
			NativeArray<Vector3Int> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3Int>(ptr3, count, Allocator.Invalid);
			for (int i = 0; i < count; i++)
			{
				int num = nativeArray[i];
				int num2 = nativeArray2[i];
				Vector3Int vector3Int = nativeArray3[i];
				bool flag2 = num != 0;
				if (flag2)
				{
					TileBase tileBase = (TileBase)Object.ForceLoadFromInstanceID(num);
					tileBase.RefreshTile(vector3Int, tilemap);
				}
				bool flag3 = num2 != 0;
				if (flag3)
				{
					TileBase tileBase2 = (TileBase)Object.ForceLoadFromInstanceID(num2);
					tileBase2.RefreshTile(vector3Int, tilemap);
				}
			}
			tilemap.m_Tilemap.RefreshTilesNative(tilemap.m_RefreshPos.m_Buffer, tilemap.m_RefreshCount);
			tilemap.m_RefreshPos.Dispose();
			tilemap.m_AddToList = false;
		}

		[RequiredByNativeCode]
		private unsafe static void GetAllTileData(ITilemap tilemap, int count, IntPtr tilesIntPtr, IntPtr positionsIntPtr, IntPtr outTileDataIntPtr)
		{
			void* ptr = tilesIntPtr.ToPointer();
			void* ptr2 = positionsIntPtr.ToPointer();
			void* ptr3 = outTileDataIntPtr.ToPointer();
			NativeArray<int> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(ptr, count, Allocator.Invalid);
			NativeArray<Vector3Int> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3Int>(ptr2, count, Allocator.Invalid);
			NativeArray<TileData> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TileData>(ptr3, count, Allocator.Invalid);
			for (int i = 0; i < count; i++)
			{
				TileData @default = TileData.Default;
				int num = nativeArray[i];
				bool flag = num != 0;
				if (flag)
				{
					TileBase tileBase = (TileBase)Object.ForceLoadFromInstanceID(num);
					tileBase.GetTileData(nativeArray2[i], tilemap, UnsafeUtility.ArrayElementAsRef<TileData>(nativeArray3.GetUnsafePtr<TileData>(), i));
				}
			}
		}

		internal static ITilemap s_Instance;

		internal Tilemap m_Tilemap;

		internal bool m_AddToList;

		internal int m_RefreshCount;

		internal NativeArray<Vector3Int> m_RefreshPos;
	}
}
