using System;

namespace UnityEngine.Experimental
{
	public interface ISubsystem
	{
		void Start();

		void Stop();

		void Destroy();
	}
}
