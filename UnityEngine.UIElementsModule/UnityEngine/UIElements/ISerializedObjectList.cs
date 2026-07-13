using System;
using System.Collections;

namespace UnityEngine.UIElements
{
	internal interface ISerializedObjectList : IList, ICollection, IEnumerable
	{
		void ApplyChanges();

		void RemoveAt(int index, int listCount);

		void Move(int srcIndex, int destIndex);

		int minArraySize { get; }

		int arraySize { get; set; }
	}
}
