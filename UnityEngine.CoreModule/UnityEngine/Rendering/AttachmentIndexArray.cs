using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	public struct AttachmentIndexArray
	{
		public AttachmentIndexArray(int numAttachments)
		{
			bool flag = numAttachments < 0 || numAttachments > 8;
			if (flag)
			{
				throw new ArgumentException(string.Format("AttachmentIndexArray - numAttachments must be in range of [0, {0}[", 8));
			}
			this.a0 = (this.a1 = (this.a2 = (this.a3 = (this.a4 = (this.a5 = (this.a6 = (this.a7 = -1)))))));
			this.activeAttachments = numAttachments;
		}

		public AttachmentIndexArray(int[] attachments)
		{
			this = new AttachmentIndexArray(attachments.Length);
			for (int i = 0; i < this.activeAttachments; i++)
			{
				this[i] = attachments[i];
			}
		}

		public AttachmentIndexArray(NativeArray<int> attachments)
		{
			this = new AttachmentIndexArray(attachments.Length);
			for (int i = 0; i < this.activeAttachments; i++)
			{
				this[i] = attachments[i];
			}
		}

		public unsafe int this[int index]
		{
			get
			{
				bool flag = index >= 8;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("AttachmentIndexArray - index must be in range of [0, {0}[", 8));
				}
				bool flag2 = (ulong)index >= (ulong)((long)this.activeAttachments);
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("AttachmentIndexArray - index must be in range of [0, {0}[", this.activeAttachments));
				}
				fixed (AttachmentIndexArray* ptr = &this)
				{
					AttachmentIndexArray* ptr2 = ptr;
					int* ptr3 = (int*)ptr2;
					return ptr3[index];
				}
			}
			set
			{
				bool flag = index >= 8;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("AttachmentIndexArray - index must be in range of [0, {0}[", 8));
				}
				bool flag2 = (ulong)index >= (ulong)((long)this.activeAttachments);
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("AttachmentIndexArray - index must be in range of [0, {0}[", this.activeAttachments));
				}
				fixed (AttachmentIndexArray* ptr = &this)
				{
					AttachmentIndexArray* ptr2 = ptr;
					int* ptr3 = (int*)ptr2;
					ptr3[index] = value;
				}
			}
		}

		public int Length
		{
			get
			{
				return this.activeAttachments;
			}
		}

		public static AttachmentIndexArray Emtpy = new AttachmentIndexArray(0);

		public const int MaxAttachments = 8;

		private int a0;

		private int a1;

		private int a2;

		private int a3;

		private int a4;

		private int a5;

		private int a6;

		private int a7;

		private int activeAttachments;
	}
}
