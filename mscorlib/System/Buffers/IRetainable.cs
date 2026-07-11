using System;

namespace System.Buffers
{
	public interface IRetainable
	{
		void Retain();

		bool Release();
	}
}
