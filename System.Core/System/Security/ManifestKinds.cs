using System;

namespace System.Security
{
	[Flags]
	public enum ManifestKinds
	{
		Application = 2,
		ApplicationAndDeployment = 3,
		Deployment = 1,
		None = 0
	}
}
