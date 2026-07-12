using System;
using System.Drawing.Imaging;
using System.Threading;

namespace System.Drawing
{
	internal class AnimateEventArgs : EventArgs
	{
		public AnimateEventArgs(Image image)
		{
			this.frameCount = image.GetFrameCount(FrameDimension.Time);
		}

		public Thread RunThread
		{
			get
			{
				return this.thread;
			}
			set
			{
				this.thread = value;
			}
		}

		public int GetNextFrame()
		{
			if (this.activeFrame < this.frameCount - 1)
			{
				this.activeFrame++;
			}
			else
			{
				this.activeFrame = 0;
			}
			return this.activeFrame;
		}

		private int frameCount;

		private int activeFrame;

		private Thread thread;
	}
}
