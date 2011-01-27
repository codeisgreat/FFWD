﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class ParticleAnimator : Component, IFixedUpdateable
    {
        public bool doesAnimateColor;
        public Vector3 worldRotationAxis;
        public Vector3 localRotationAxis;
        public float sizeGrow;
        public Vector3 rndForce;
        public Vector3 force;
        public float damping;
        public bool autodestruct;
        public Color[] colorAnimation;

        private ParticleEmitter emitter;
        private bool hasHadParticles = false;

        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
        }

        public void FixedUpdate()
        {
            bool destroy = hasHadParticles;
            int particlesToCheck = emitter.particleCount;
            for (int i = 0; i < emitter.particles.Length; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    hasHadParticles = true;
                    destroy = false;
                    emitter.particles[i].Position += emitter.particles[i].Velocity * Time.deltaTime;
                    emitter.particles[i].Velocity *= 1 - ((1 - damping) * Time.deltaTime);
                    Vector3 RandomForce = Random.insideUnitSphere * rndForce / 2;
                    emitter.particles[i].Velocity += (force + RandomForce) * Time.deltaTime;
                    if (emitter.tangentVelocity != Vector3.zero)
                    {
                        Vector3 v = Vector3.Cross(transform.up, emitter.particles[i].Velocity);
                        emitter.particles[i].Velocity += (v * emitter.tangentVelocity) * Time.deltaTime;
                    }
                    emitter.particles[i].Size += sizeGrow * Time.deltaTime;
                    UpdateParticleColor(ref emitter.particles[i]);
                    if (--particlesToCheck == 0)
                    {
                        break;
                    }
                }
            }

            if (destroy && autodestruct)
            {
                Destroy(gameObject);
            }
        }

        public void UpdateParticleColor(ref Particle particle)
        {
            if (doesAnimateColor)
            {
                float colorScale = 1 - (particle.Energy / particle.StartingEnergy);
                float startIndex = colorScale * 4;
                if (startIndex == 4)
                {
                    startIndex = 3;
                }
                colorScale = startIndex - (int)startIndex;
                particle.Color = Color.Lerp(colorAnimation[(int)startIndex], colorAnimation[(int)startIndex + 1], colorScale);
            }
            else
            {
                particle.Color = renderer.material.color;
            }
        }

    }
}