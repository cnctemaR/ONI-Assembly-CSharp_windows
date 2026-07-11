using System;

namespace UnityEngine.UIElements.Experimental
{
	public interface IValueAnimation
	{
		void Start();

		void Stop();

		void Recycle();

		bool isRunning { get; }

		int durationMs { get; set; }
	}
}
