﻿using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoSingleton {

	private static Transform particleRoot;
	private static readonly Dictionary<string, Queue<ParticleController>> idleParticles = new Dictionary<string, Queue<ParticleController>>(3);

	private void Awake() {
		particleRoot = new GameObject("Particle Root").transform;
		DontDestroyOnLoad(particleRoot.gameObject);
	}

	public static ParticleController Get(string name) {
		Queue<ParticleController> particles;
		if (!idleParticles.ContainsKey(name)) {
			particles = new Queue<ParticleController>(5);
			idleParticles[name] = particles;
		} else particles = idleParticles[name];

		ParticleController particle;
		if (particles.Count > 0) {
			particle = particles.Dequeue();
			particle.gameObject.SetActive(true);
		} else {
			particle = ResourcesManager.GetParticle(name).GetComponent<ParticleController>();
			particle.transform.parent = particleRoot;
		}
		
		return particle;
	}

	public static T Get<T>(string name) where T : ParticleController {
		return Get(name) as T;
	}

	public static void Recycle(ParticleController particle) {
		particle.Recycle();
		particle.gameObject.SetActive(false);
		idleParticles[particle.identifierName].Enqueue(particle);
	}
}