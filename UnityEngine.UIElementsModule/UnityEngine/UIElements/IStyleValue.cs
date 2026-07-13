using System;

namespace UnityEngine.UIElements
{
	public interface IStyleValue<T>
	{
		T value { get; set; }

		StyleKeyword keyword { get; set; }
	}
}
