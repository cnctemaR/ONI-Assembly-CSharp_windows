using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics
{
	public struct GeometryHolder
	{
		public T As<T>() where T : struct, IGeometry
		{
			T t = default(T);
			bool flag = t.GeometryType != this.Type;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Unable to get geometry of type {0} from a geometry holder that stores {1}.", t.GeometryType, this.Type));
			}
			UnsafeUtility.CopyPtrToStructure<T>(UnsafeUtility.AddressOf<GeometryHolder>(ref this), out t);
			return t;
		}

		public static GeometryHolder Create<T>(T geometry) where T : struct, IGeometry
		{
			GeometryHolder geometryHolder = default(GeometryHolder);
			UnsafeUtility.CopyStructureToPtr<T>(ref geometry, UnsafeUtility.AddressOf<GeometryHolder>(ref geometryHolder));
			geometryHolder.m_Data.FixedElementField = (int)geometry.GeometryType;
			return geometryHolder;
		}

		public GeometryType Type
		{
			get
			{
				return (GeometryType)this.m_Data.FixedElementField;
			}
		}

		[FixedBuffer(typeof(int), 12)]
		internal GeometryHolder.<m_Data>e__FixedBuffer m_Data;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 48)]
		public struct <m_Data>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
