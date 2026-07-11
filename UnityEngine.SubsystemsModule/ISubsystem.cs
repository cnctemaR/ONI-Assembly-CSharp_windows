using System;

namespace UnityEngine
{
	public interface ISubsystem
	{
		void Start();

		void Stop();

		void Destroy();

		bool running { get; }
	}
}
