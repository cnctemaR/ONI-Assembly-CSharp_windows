using System;

namespace Unity.Properties
{
	public delegate TDestination TypeConverter<TSource, out TDestination>(ref TSource value);
}
