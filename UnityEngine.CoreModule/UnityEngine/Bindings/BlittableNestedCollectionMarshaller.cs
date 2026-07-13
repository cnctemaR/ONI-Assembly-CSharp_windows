using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal struct BlittableNestedCollectionMarshaller<[IsUnmanaged] T> where T : struct, ValueType
	{
		public unsafe static NestedCollectionData ConvertToUnmanaged(IList outerCollection)
		{
			bool flag = outerCollection == null;
			NestedCollectionData nestedCollectionData;
			if (flag)
			{
				nestedCollectionData = default(NestedCollectionData);
			}
			else
			{
				int count = outerCollection.Count;
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					int num2 = num;
					ICollection<T> collection = outerCollection[i] as ICollection<T>;
					num = num2 + ((collection != null) ? collection.Count : 0);
				}
				bool flag2 = num == 0;
				if (flag2)
				{
					nestedCollectionData = default(NestedCollectionData);
				}
				else
				{
					NestedCollectionData* ptr = (NestedCollectionData*)BindingsAllocator.Malloc(checked(outerCollection.Count * sizeof(NestedCollectionData) + BlittableNestedCollectionMarshaller<T>.AlignOfT + num * sizeof(T)));
					NestedCollectionData nestedCollectionData2;
					nestedCollectionData2.Length = outerCollection.Count;
					nestedCollectionData2.Data = (void*)ptr;
					UIntPtr uintPtr = ptr + nestedCollectionData2.Length;
					uintPtr += (UIntPtr)((IntPtr)BlittableNestedCollectionMarshaller<T>.AlignOfT - (IntPtr)(uintPtr % (UIntPtr)((IntPtr)BlittableNestedCollectionMarshaller<T>.AlignOfT)));
					T* ptr2 = uintPtr;
					for (int j = 0; j < count; j++)
					{
						IList<T> list = (IList<T>)outerCollection[j];
						int num3 = ((list != null) ? list.Count : 0);
						ptr->Length = num3;
						bool flag3 = num3 == 0;
						if (flag3)
						{
							ptr->Data = null;
						}
						else
						{
							ptr->Data = (void*)ptr2;
							IList<T> list2 = list;
							IList<T> list3 = list2;
							T[] array = list3 as T[];
							if (array == null)
							{
								List<T> list4 = list3 as List<T>;
								if (list4 == null)
								{
									for (int k = 0; k < num3; k++)
									{
										T* ptr3 = ptr2;
										ptr2 = ptr3 + sizeof(T) / sizeof(T);
										*ptr3 = list[k];
									}
								}
								else
								{
									NoAllocHelpers.CreateReadOnlySpan<T>(list4).CopyTo(new Span<T>((void*)ptr2, num3));
									ptr2 += (IntPtr)num3 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
								}
							}
							else
							{
								new Span<T>(array).CopyTo(new Span<T>((void*)ptr2, num3));
								ptr2 += (IntPtr)num3 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
							}
						}
						ptr++;
					}
					nestedCollectionData = nestedCollectionData2;
				}
			}
			return nestedCollectionData;
		}

		private static readonly int AlignOfT = UnsafeUtility.AlignOf<T>();
	}
}
