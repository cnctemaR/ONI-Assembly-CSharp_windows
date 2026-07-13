using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal interface IDragAndDropData
	{
		object GetGenericData(string key);

		object userData { get; }

		IEnumerable<Object> unityObjectReferences { get; }

		IReadOnlyList<EntityId> entityIds { get; }

		string[] paths { get; set; }
	}
}
