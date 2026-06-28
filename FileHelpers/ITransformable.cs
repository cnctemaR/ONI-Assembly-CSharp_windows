using System;

namespace FileHelpers
{
	public interface ITransformable<T>
	{
		T TransformTo();
	}
}
