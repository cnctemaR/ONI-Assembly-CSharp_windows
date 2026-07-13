using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct StyleDataRef<T> : IEquatable<StyleDataRef<T>> where T : struct, IEquatable<T>, IStyleDataGroup<T>
	{
		public int refCount
		{
			get
			{
				StyleDataRef<T>.RefCounted @ref = this.m_Ref;
				return (@ref != null) ? @ref.refCount : 0;
			}
		}

		public uint id
		{
			get
			{
				StyleDataRef<T>.RefCounted @ref = this.m_Ref;
				return (@ref != null) ? @ref.id : 0U;
			}
		}

		public StyleDataRef<T> Acquire()
		{
			this.m_Ref.Acquire();
			return this;
		}

		public void Release()
		{
			this.m_Ref.Release();
			this.m_Ref = null;
		}

		public void CopyFrom(StyleDataRef<T> other)
		{
			bool flag = this.m_Ref.refCount == 1;
			if (flag)
			{
				this.m_Ref.value.CopyFrom(ref other.m_Ref.value);
			}
			else
			{
				this.m_Ref.Release();
				this.m_Ref = other.m_Ref;
				this.m_Ref.Acquire();
			}
		}

		public readonly ref T Read()
		{
			return ref this.m_Ref.value;
		}

		public ref T Write()
		{
			bool flag = this.m_Ref.refCount == 1;
			ref T ptr;
			if (flag)
			{
				ptr = ref this.m_Ref.value;
			}
			else
			{
				StyleDataRef<T>.RefCounted @ref = this.m_Ref;
				this.m_Ref = this.m_Ref.Copy();
				@ref.Release();
				ptr = ref this.m_Ref.value;
			}
			return ref ptr;
		}

		public static StyleDataRef<T> Create()
		{
			return new StyleDataRef<T>
			{
				m_Ref = new StyleDataRef<T>.RefCounted()
			};
		}

		public override int GetHashCode()
		{
			return (this.m_Ref != null) ? this.m_Ref.value.GetHashCode() : 0;
		}

		public static bool operator ==(StyleDataRef<T> lhs, StyleDataRef<T> rhs)
		{
			return lhs.m_Ref == rhs.m_Ref || lhs.m_Ref.value.Equals(rhs.m_Ref.value);
		}

		public static bool operator !=(StyleDataRef<T> lhs, StyleDataRef<T> rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(StyleDataRef<T> other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleDataRef<T>)
			{
				StyleDataRef<T> styleDataRef = (StyleDataRef<T>)obj;
				flag = this.Equals(styleDataRef);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool ReferenceEquals(StyleDataRef<T> other)
		{
			return this.m_Ref == other.m_Ref;
		}

		private StyleDataRef<T>.RefCounted m_Ref;

		private class RefCounted
		{
			public int refCount
			{
				get
				{
					return this.m_RefCount;
				}
			}

			public uint id
			{
				get
				{
					return this.m_Id;
				}
			}

			public RefCounted()
			{
				this.m_RefCount = 1;
				this.m_Id = (StyleDataRef<T>.RefCounted.m_NextId += 1U);
			}

			public void Acquire()
			{
				this.m_RefCount++;
			}

			public void Release()
			{
				this.m_RefCount--;
			}

			public StyleDataRef<T>.RefCounted Copy()
			{
				return new StyleDataRef<T>.RefCounted
				{
					value = this.value.Copy()
				};
			}

			private static uint m_NextId = 1U;

			private int m_RefCount;

			private readonly uint m_Id;

			public T value;
		}
	}
}
