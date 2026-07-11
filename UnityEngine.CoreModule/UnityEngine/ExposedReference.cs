using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode(Name = "ExposedReference")]
	[Serializable]
	public struct ExposedReference<T> where T : Object
	{
		public T Resolve(IExposedPropertyTable resolver)
		{
			bool flag = resolver != null;
			if (flag)
			{
				bool flag2;
				Object referenceValue = resolver.GetReferenceValue(this.exposedName, out flag2);
				bool flag3 = flag2;
				if (flag3)
				{
					return referenceValue as T;
				}
			}
			return this.defaultValue as T;
		}

		[SerializeField]
		public PropertyName exposedName;

		[SerializeField]
		public Object defaultValue;
	}
}
