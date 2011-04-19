#region License

/* Copyright 2011 Justin Weaver
 *
 * This file is part of OrbitClash.
 *
 * OrbitClash is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * OrbitClash is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OrbitClash.  If not, see <http://www.gnu.org/licenses/>.
 */

#endregion License

#region Header Comments

/* $Id$
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: Creates the explosion left behind by a destroyed ship.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Audio;
using SdlDotNet.Particles.Emitters;

namespace OrbitClash
{
    internal class ShipExplosionEffect : ParticleCircleEmitter, IDisposable
    {
        #region Fields

        private Sound explosionSound;

        #endregion Fields

        #region Constructor

        public ShipExplosionEffect()
            : base(Configuration.Ships.Explosion.MinColor, Configuration.Ships.Explosion.MaxColor, Configuration.Ships.Explosion.RadiusMin, Configuration.Ships.Explosion.RadiusMax)
        {
            this.Emitting = false;

            this.explosionSound = new Sound(Configuration.Ships.Explosion.SoundFilename);
            this.explosionSound.Volume = Configuration.SoundVolume;
        }

        #endregion Constructor

        #region Operations

        public ParticleCircleEmitter Explode(Point position)
        {
            this.X = position.X;
            this.Y = position.Y;

            this.Frequency = Configuration.Ships.Explosion.Frequency;

            this.LifeMin = Configuration.Ships.Explosion.LifeMin;
            this.LifeMax = Configuration.Ships.Explosion.LifeMax;

            this.LifeFullMin = Configuration.Ships.Explosion.LifeFullMin;
            this.LifeFullMax = Configuration.Ships.Explosion.LifeFullMax;

            this.SpeedMin = Configuration.Ships.Explosion.SpeedMin;
            this.SpeedMax = Configuration.Ships.Explosion.SpeedMax;

            // Turn on the emitter.
            this.Life = Configuration.Ships.Explosion.Life;
            this.Emitting = true;

            try
            {
                this.explosionSound.Play();
            }
            catch
            {
                // Must be out of sound channels.
            }

            return this;
        }

        #endregion Operations

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ShipExplosionEffect()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.explosionSound != null)
                    {
                        this.explosionSound.Dispose();
                        this.explosionSound = null;
                    }
                }
                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}