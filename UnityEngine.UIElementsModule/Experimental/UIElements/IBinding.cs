using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IBinding
	{
		void PreUpdate();

		void Update();

		void Release();
	}
}
