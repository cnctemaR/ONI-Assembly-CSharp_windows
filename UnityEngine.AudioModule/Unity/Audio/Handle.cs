using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace Unity.Audio
{
	[NativeType(Header = "Modules/Audio/Public/AudioHandle.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.DSPGraphModule" })]
	internal struct Handle : IHandle<Handle>, IValidatable, IEquatable<Handle>
	{
		internal unsafe Handle.Node* AtomicNode
		{
			readonly get
			{
				return (Handle.Node*)(void*)this.m_Node;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException();
				}
				this.m_Node = (IntPtr)((void*)value);
				this.Version = value->Version;
			}
		}

		public unsafe int Id
		{
			readonly get
			{
				return this.Valid ? this.AtomicNode->Id : (-1);
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					throw new ArgumentException("Invalid ID");
				}
				bool flag2 = !this.Valid;
				if (flag2)
				{
					throw new InvalidOperationException("Handle is invalid or has been destroyed");
				}
				bool flag3 = this.AtomicNode->Id != -1;
				if (flag3)
				{
					throw new InvalidOperationException(string.Format("Trying to overwrite id on live node {0}", this.AtomicNode->Id));
				}
				this.AtomicNode->Id = value;
			}
		}

		internal unsafe Handle(Handle.Node* node)
		{
			bool flag = node == null;
			if (flag)
			{
				throw new ArgumentNullException("node");
			}
			bool flag2 = node->Id != -1;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Reusing unflushed node {0}", node->Id));
			}
			this.Version = node->Version;
			this.m_Node = (IntPtr)((void*)node);
		}

		public unsafe void FlushNode()
		{
			bool flag = !this.Valid;
			if (flag)
			{
				throw new InvalidOperationException("Attempting to flush invalid audio handle");
			}
			this.AtomicNode->Id = -1;
			this.AtomicNode->Version++;
		}

		public readonly bool Equals(Handle other)
		{
			return this.m_Node == other.m_Node && this.Version == other.Version;
		}

		public override readonly bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is Handle && this.Equals((Handle)obj);
		}

		public override readonly int GetHashCode()
		{
			return ((int)this.m_Node * 397) ^ this.Version;
		}

		public readonly void CheckValidOrThrow()
		{
			bool flag = !this.ValidAndNotDisposed;
			if (flag)
			{
				throw new InvalidOperationException("Attempting to use invalid audio handle");
			}
		}

		public unsafe readonly bool ValidAndNotDisposed
		{
			get
			{
				return this.m_Node != IntPtr.Zero && this.AtomicNode->Version == this.Version && this.AtomicNode->AllocationFlags == 0;
			}
		}

		public unsafe readonly bool Valid
		{
			get
			{
				return this.m_Node != IntPtr.Zero && this.AtomicNode->Version == this.Version;
			}
		}

		public unsafe readonly bool Alive
		{
			get
			{
				return this.Valid && this.AtomicNode->Id != -1;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		private IntPtr m_Node;

		public int Version;

		internal struct Node
		{
			private unsafe void* Next;

			public int Id;

			public int Version;

			public int AllocationFlags;

			public const int InvalidId = -1;
		}
	}
}
