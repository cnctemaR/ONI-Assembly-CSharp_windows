using System;

namespace YamlDotNet.Core
{
	internal class RecursionLevel
	{
		public RecursionLevel(int maximum)
		{
			this.Maximum = maximum;
		}

		public int Maximum { get; private set; }

		public void Increment()
		{
			if (!this.TryIncrement())
			{
				throw new MaximumRecursionLevelReachedException();
			}
		}

		public bool TryIncrement()
		{
			if (this.current < this.Maximum)
			{
				this.current++;
				return true;
			}
			return false;
		}

		public void Decrement()
		{
			if (this.current == 0)
			{
				throw new InvalidOperationException("Attempted to decrement RecursionLevel to a negative value");
			}
			this.current--;
		}

		private int current;
	}
}
