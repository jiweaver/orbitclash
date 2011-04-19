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
 * Description: Creates the creation left behind by a destroyed ship.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Audio;
using SdlDotNet.Particles.Emitters;

namespace OrbitClash
{
    internal class ShipCreationEffect : ParticleCircleEmitter, IDisposable
    {
        #region Fields

        private Sound creationSound;

        #endregion Fields

        #region Constructor

        public ShipCreationEffect()
            : base(Configuration.Ships.Creation.MinColor, Configuration.Ships.Creation.MaxColor, Configuration.Ships.Creation.RadiusMin, Configuration.Ships.Creation.RadiusMax)
        {
            this.Emitting = false;

            this.creationSound = new Sound(Configuration.Ships.Creation.SoundFilename);
            this.creationSound.Volume = Configuration.SoundVolume;
        }

        #endregion Constructor

        #region Operations

        public ParticleCircleEmitter Create(Point position)
        {
            this.X = position.X;
            this.Y = position.Y;

            this.Frequency = Configuration.Ships.Creation.Frequency;

            this.LifeMin = Configuration.Ships.Creation.LifeMin;
            this.LifeMax = Configuration.Ships.Creation.LifeMax;

            this.LifeFullMin = Configuration.Ships.Creation.LifeFullMin;
            this.LifeFullMax = Configuration.Ships.Creation.LifeFullMax;

            this.SpeedMin = Configuration.Ships.Creation.SpeedMin;
            this.SpeedMax = Configuration.Ships.Creation.SpeedMax;

            // Turn on the emitter.
            this.Life = Configuration.Ships.Creation.Life;
            this.Emitting = true;

            try
            {
                this.creationSound.Play();
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

        ~ShipCreationEffect()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.creationSound != null)
                    {
                        this.creationSound.Dispose();
                        this.creationSound = null;
                    }
                }
                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}