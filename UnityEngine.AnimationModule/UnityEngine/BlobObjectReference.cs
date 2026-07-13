using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine
{
	internal struct BlobObjectReference : IDisposable
	{
		public unsafe BlobObjectReference(BlobObject blobObject, Allocator allocator)
		{
			this.m_allocator = allocator;
			bool flag = blobObject == null;
			if (flag)
			{
				this.m_fixedReference = null;
			}
			else
			{
				FixedBlobObjectReference* ptr = (FixedBlobObjectReference*)(void*)blobObject.GetRootReference();
				this.m_fixedReference = (FixedBlobObjectReference*)UnsafeUtility.Malloc((long)sizeof(FixedBlobObjectReference), UnsafeUtility.AlignOf<FixedBlobObjectReference>(), allocator);
				bool flag2 = this.m_fixedReference == null;
				if (flag2)
				{
					Debug.LogError("Cannot initialize BlobObjectReference.");
				}
				else
				{
					this.m_fixedReference->blobData = blobObject.GetBlobData(out this.m_fixedReference->blobTypeHash, out this.m_fixedReference->blobSize);
					this.m_fixedReference->prevReference = ptr;
					this.m_fixedReference->nextReference = ptr->nextReference;
					bool flag3 = this.m_fixedReference->nextReference > 0UL;
					if (flag3)
					{
						((UIntPtr)this.m_fixedReference->nextReference).prevReference = this.m_fixedReference;
					}
					ptr->nextReference = this.m_fixedReference;
				}
			}
		}

		public unsafe void Dispose()
		{
			bool flag = this.m_fixedReference != null;
			if (flag)
			{
				this.m_fixedReference->RemoveFromList();
				UnsafeUtility.Free((void*)this.m_fixedReference, this.m_allocator);
				this.m_fixedReference = null;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_fixedReference != null;
			}
		}

		public unsafe ulong BlobTypeHash
		{
			get
			{
				return this.IsCreated ? this.m_fixedReference->blobTypeHash : 0UL;
			}
		}

		public unsafe byte* BlobData
		{
			get
			{
				return this.IsCreated ? this.m_fixedReference->blobData : null;
			}
		}

		public unsafe uint BlobSize
		{
			get
			{
				return this.IsCreated ? this.m_fixedReference->blobSize : 0U;
			}
		}

		private Allocator m_allocator;

		private unsafe FixedBlobObjectReference* m_fixedReference;
	}
}
