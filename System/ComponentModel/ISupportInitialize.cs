using System;

namespace System.ComponentModel
{
	[SRDescription("Specifies support for transacted initialization.")]
	public interface ISupportInitialize
	{
		void BeginInit();

		void EndInit();
	}
}
