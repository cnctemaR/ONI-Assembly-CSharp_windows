using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization
{
	internal sealed class LazyComponentRegistrationList<TArgument, TComponent> : IEnumerable<Func<TArgument, TComponent>>, IEnumerable
	{
		public LazyComponentRegistrationList<TArgument, TComponent> Clone()
		{
			LazyComponentRegistrationList<TArgument, TComponent> lazyComponentRegistrationList = new LazyComponentRegistrationList<TArgument, TComponent>();
			foreach (LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration lazyComponentRegistration in this.entries)
			{
				lazyComponentRegistrationList.entries.Add(lazyComponentRegistration);
			}
			return lazyComponentRegistrationList;
		}

		public void Add(Type componentType, Func<TArgument, TComponent> factory)
		{
			this.entries.Add(new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(componentType, factory));
		}

		public void Remove(Type componentType)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				if (this.entries[i].ComponentType == componentType)
				{
					this.entries.RemoveAt(i);
					return;
				}
			}
			throw new KeyNotFoundException(string.Format("A component registration of type '{0}' was not found.", componentType.FullName));
		}

		public int Count
		{
			get
			{
				return this.entries.Count;
			}
		}

		public IEnumerable<Func<TArgument, TComponent>> InReverseOrder
		{
			get
			{
				int num;
				for (int i = this.entries.Count - 1; i >= 0; i = num)
				{
					yield return this.entries[i].Factory;
					num = i - 1;
				}
				yield break;
			}
		}

		public IRegistrationLocationSelectionSyntax<TComponent> CreateRegistrationLocationSelector(Type componentType, Func<TArgument, TComponent> factory)
		{
			return new LazyComponentRegistrationList<TArgument, TComponent>.RegistrationLocationSelector(this, new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(componentType, factory));
		}

		public ITrackingRegistrationLocationSelectionSyntax<TComponent> CreateTrackingRegistrationLocationSelector(Type componentType, Func<TComponent, TArgument, TComponent> factory)
		{
			return new LazyComponentRegistrationList<TArgument, TComponent>.TrackingRegistrationLocationSelector(this, new LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration(componentType, factory));
		}

		public IEnumerator<Func<TArgument, TComponent>> GetEnumerator()
		{
			return this.entries.Select<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration, Func<TArgument, TComponent>>((LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration e) => e.Factory).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int IndexOfRegistration(Type registrationType)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				if (registrationType == this.entries[i].ComponentType)
				{
					return i;
				}
			}
			return -1;
		}

		private void EnsureNoDuplicateRegistrationType(Type componentType)
		{
			if (this.IndexOfRegistration(componentType) != -1)
			{
				throw new InvalidOperationException(string.Format("A component of type '{0}' has already been registered.", componentType.FullName));
			}
		}

		private int EnsureRegistrationExists<TRegistrationType>()
		{
			int num = this.IndexOfRegistration(typeof(TRegistrationType));
			if (num == -1)
			{
				throw new InvalidOperationException(string.Format("A component of type '{0}' has not been registered.", typeof(TRegistrationType).FullName));
			}
			return num;
		}

		private readonly List<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration> entries = new List<LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration>();

		public sealed class LazyComponentRegistration
		{
			public LazyComponentRegistration(Type componentType, Func<TArgument, TComponent> factory)
			{
				this.ComponentType = componentType;
				this.Factory = factory;
			}

			public readonly Type ComponentType;

			public readonly Func<TArgument, TComponent> Factory;
		}

		public sealed class TrackingLazyComponentRegistration
		{
			public TrackingLazyComponentRegistration(Type componentType, Func<TComponent, TArgument, TComponent> factory)
			{
				this.ComponentType = componentType;
				this.Factory = factory;
			}

			public readonly Type ComponentType;

			public readonly Func<TComponent, TArgument, TComponent> Factory;
		}

		private class RegistrationLocationSelector : IRegistrationLocationSelectionSyntax<TComponent>
		{
			public RegistrationLocationSelector(LazyComponentRegistrationList<TArgument, TComponent> registrations, LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration newRegistration)
			{
				this.registrations = registrations;
				this.newRegistration = newRegistration;
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
			{
				if (this.newRegistration.ComponentType != typeof(TRegistrationType))
				{
					this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				}
				int num = this.registrations.EnsureRegistrationExists<TRegistrationType>();
				this.registrations.entries[num] = this.newRegistration;
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.After<TRegistrationType>()
			{
				this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				int num = this.registrations.EnsureRegistrationExists<TRegistrationType>();
				this.registrations.entries.Insert(num + 1, this.newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.Before<TRegistrationType>()
			{
				this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				int num = this.registrations.EnsureRegistrationExists<TRegistrationType>();
				this.registrations.entries.Insert(num, this.newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.OnBottom()
			{
				this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				this.registrations.entries.Add(this.newRegistration);
			}

			void IRegistrationLocationSelectionSyntax<TComponent>.OnTop()
			{
				this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				this.registrations.entries.Insert(0, this.newRegistration);
			}

			private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;

			private readonly LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration newRegistration;
		}

		private class TrackingRegistrationLocationSelector : ITrackingRegistrationLocationSelectionSyntax<TComponent>
		{
			public TrackingRegistrationLocationSelector(LazyComponentRegistrationList<TArgument, TComponent> registrations, LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration newRegistration)
			{
				this.registrations = registrations;
				this.newRegistration = newRegistration;
			}

			void ITrackingRegistrationLocationSelectionSyntax<TComponent>.InsteadOf<TRegistrationType>()
			{
				if (this.newRegistration.ComponentType != typeof(TRegistrationType))
				{
					this.registrations.EnsureNoDuplicateRegistrationType(this.newRegistration.ComponentType);
				}
				int num = this.registrations.EnsureRegistrationExists<TRegistrationType>();
				Func<TArgument, TComponent> innerComponentFactory = this.registrations.entries[num].Factory;
				this.registrations.entries[num] = new LazyComponentRegistrationList<TArgument, TComponent>.LazyComponentRegistration(this.newRegistration.ComponentType, (TArgument arg) => this.newRegistration.Factory(innerComponentFactory(arg), arg));
			}

			private readonly LazyComponentRegistrationList<TArgument, TComponent> registrations;

			private readonly LazyComponentRegistrationList<TArgument, TComponent>.TrackingLazyComponentRegistration newRegistration;
		}
	}
}
