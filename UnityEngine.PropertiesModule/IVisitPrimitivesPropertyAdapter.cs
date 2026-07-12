using System;

namespace Unity.Properties
{
	public interface IVisitPrimitivesPropertyAdapter : IVisitPropertyAdapter<sbyte>, IPropertyVisitorAdapter, IVisitPropertyAdapter<short>, IVisitPropertyAdapter<int>, IVisitPropertyAdapter<long>, IVisitPropertyAdapter<byte>, IVisitPropertyAdapter<ushort>, IVisitPropertyAdapter<uint>, IVisitPropertyAdapter<ulong>, IVisitPropertyAdapter<float>, IVisitPropertyAdapter<double>, IVisitPropertyAdapter<bool>, IVisitPropertyAdapter<char>
	{
	}
}
