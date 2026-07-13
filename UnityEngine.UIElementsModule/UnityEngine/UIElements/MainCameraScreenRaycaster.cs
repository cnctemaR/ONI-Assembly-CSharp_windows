using System;

namespace UnityEngine.UIElements
{
	internal class MainCameraScreenRaycaster : CameraScreenRaycaster
	{
		public MainCameraScreenRaycaster()
		{
			this.ResolveCamera();
		}

		public override void Update()
		{
			this.ResolveCamera();
		}

		private void ResolveCamera()
		{
			Camera main = Camera.main;
			bool flag = main != null;
			if (flag)
			{
				this.cameras = this.singleCameraArray;
				this.cameras[0] = main;
			}
			else
			{
				this.cameras = Array.Empty<Camera>();
			}
		}

		private Camera[] singleCameraArray = new Camera[1];
	}
}
