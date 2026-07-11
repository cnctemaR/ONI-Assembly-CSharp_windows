using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal class ParticleSystemExtensionsImpl
	{
		[FreeFunction(Name = "ParticleSystemScriptBindings::GetSafeCollisionEventSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSafeCollisionEventSize([NotNull] ParticleSystem ps);

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetCollisionEventsDeprecated")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCollisionEventsDeprecated([NotNull] ParticleSystem ps, GameObject go, [Out] ParticleCollisionEvent[] collisionEvents);

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetSafeTriggerParticlesSize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSafeTriggerParticlesSize([NotNull] ParticleSystem ps, int type);

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetCollisionEvents")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCollisionEvents([NotNull] ParticleSystem ps, [NotNull] GameObject go, [NotNull] List<ParticleCollisionEvent> collisionEvents);

		[FreeFunction(Name = "ParticleSystemScriptBindings::GetTriggerParticles")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetTriggerParticles([NotNull] ParticleSystem ps, int type, [NotNull] List<ParticleSystem.Particle> particles);

		[FreeFunction(Name = "ParticleSystemScriptBindings::SetTriggerParticles")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTriggerParticles([NotNull] ParticleSystem ps, int type, [NotNull] List<ParticleSystem.Particle> particles, int offset, int count);
	}
}
