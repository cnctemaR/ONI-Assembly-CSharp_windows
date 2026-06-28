using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;

namespace System.Drawing
{
	public sealed class ImageAnimator
	{
		private ImageAnimator()
		{
		}

		public static void Animate(Image image, EventHandler onFrameChangedHandler)
		{
			if (!ImageAnimator.CanAnimate(image))
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				return;
			}
			PropertyItem propertyItem = image.GetPropertyItem(20736);
			byte[] value = propertyItem.Value;
			int[] array = new int[value.Length >> 2];
			int i = 0;
			int num = 0;
			while (i < value.Length)
			{
				int num2 = BitConverter.ToInt32(value, i) * 10;
				array[num] = ((num2 >= 100) ? num2 : 100);
				i += 4;
				num++;
			}
			AnimateEventArgs e = new AnimateEventArgs(image);
			WorkerThread workerThread = new WorkerThread(onFrameChangedHandler, e, array);
			Thread thread = new Thread(new ThreadStart(workerThread.LoopHandler));
			thread.IsBackground = true;
			e.RunThread = thread;
			ImageAnimator.ht.Add(image, e);
			thread.Start();
		}

		public static bool CanAnimate(Image image)
		{
			if (image == null)
			{
				return false;
			}
			int num = image.FrameDimensionsList.Length;
			if (num < 1)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				if (image.FrameDimensionsList[i].Equals(FrameDimension.Time.Guid))
				{
					return image.GetFrameCount(FrameDimension.Time) > 1;
				}
			}
			return false;
		}

		public static void StopAnimate(Image image, EventHandler onFrameChangedHandler)
		{
			if (image == null)
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				AnimateEventArgs e = (AnimateEventArgs)ImageAnimator.ht[image];
				e.RunThread.Abort();
				ImageAnimator.ht.Remove(image);
			}
		}

		public static void UpdateFrames()
		{
			foreach (object obj in ImageAnimator.ht.Keys)
			{
				Image image = (Image)obj;
				ImageAnimator.UpdateImageFrame(image);
			}
		}

		public static void UpdateFrames(Image image)
		{
			if (image == null)
			{
				return;
			}
			if (ImageAnimator.ht.ContainsKey(image))
			{
				ImageAnimator.UpdateImageFrame(image);
			}
		}

		private static void UpdateImageFrame(Image image)
		{
			AnimateEventArgs e = (AnimateEventArgs)ImageAnimator.ht[image];
			image.SelectActiveFrame(FrameDimension.Time, e.GetNextFrame());
		}

		private static Hashtable ht = Hashtable.Synchronized(new Hashtable());
	}
}
