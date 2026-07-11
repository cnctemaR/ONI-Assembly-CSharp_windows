using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IBindable
	{
		IBinding binding { get; set; }

		string bindingPath { get; set; }
	}
}
