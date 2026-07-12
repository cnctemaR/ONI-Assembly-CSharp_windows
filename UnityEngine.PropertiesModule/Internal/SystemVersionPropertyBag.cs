using System;
using UnityEngine;

namespace Unity.Properties.Internal
{
	internal class SystemVersionPropertyBag : ContainerPropertyBag<Version>
	{
		public SystemVersionPropertyBag()
		{
			base.AddProperty<int>(new SystemVersionPropertyBag.MajorProperty());
			base.AddProperty<int>(new SystemVersionPropertyBag.MinorProperty());
			base.AddProperty<int>(new SystemVersionPropertyBag.BuildProperty());
			base.AddProperty<int>(new SystemVersionPropertyBag.RevisionProperty());
		}

		private class MajorProperty : Property<Version, int>
		{
			public MajorProperty()
			{
				base.AddAttribute(new MinAttribute(0f));
			}

			public override string Name
			{
				get
				{
					return "Major";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref Version container)
			{
				return container.Major;
			}

			public override void SetValue(ref Version container, int value)
			{
			}
		}

		private class MinorProperty : Property<Version, int>
		{
			public MinorProperty()
			{
				base.AddAttribute(new MinAttribute(0f));
			}

			public override string Name
			{
				get
				{
					return "Minor";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref Version container)
			{
				return container.Minor;
			}

			public override void SetValue(ref Version container, int value)
			{
			}
		}

		private class BuildProperty : Property<Version, int>
		{
			public BuildProperty()
			{
				base.AddAttribute(new MinAttribute(0f));
			}

			public override string Name
			{
				get
				{
					return "Build";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref Version container)
			{
				return container.Build;
			}

			public override void SetValue(ref Version container, int value)
			{
			}
		}

		private class RevisionProperty : Property<Version, int>
		{
			public RevisionProperty()
			{
				base.AddAttribute(new MinAttribute(0f));
			}

			public override string Name
			{
				get
				{
					return "Revision";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref Version container)
			{
				return container.Revision;
			}

			public override void SetValue(ref Version container, int value)
			{
			}
		}
	}
}
