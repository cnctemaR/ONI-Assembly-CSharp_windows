using System;

namespace System.Drawing
{
	internal interface IMacContext
	{
		void Synchronize();

		void Release();
	}
}
