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
 * Description: Holds a ships thruster (particle emitter): front or reverse.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Audio;
using SdlDotNet.Core;
using SdlDotNet.Particles;
using SdlDotNet.Particles.Emitters;

namespace OrbitClash
{
    internal class Thruster : ParticleSystem, IDisposable
    {
        #region Fields

        // True if this is the ship's reverse thruster.
        private bool reverseThruster;

        private float power;
        private int engineLength;

        // The range (in degrees) of the thruster's exhaust particle spray.
        private int exhaustConeDegRange;

        private ParticlePixelEmitter particlePixelEmitter;

        private Sound thrusterSound;
        private Channel thrusterChannel;

        private Ship owner;

        #endregion Fields

        #region Properties

        public float Power
        {
            get
            {
                return this.power;
            }
            set
            {
                this.power = value;
            }
        }

        public int ExhaustConeDegRange
        {
            get
            {
                return this.exhaustConeDegRange;
            }
            set
            {
                this.exhaustConeDegRange = value;
            }
        }

        public int EngineLength
        {
            get
            {
                return this.engineLength;
            }
            set
            {
                this.engineLength = value;
            }
        }

        #endregion Properties

        #region Constructor

        public Thruster(Ship ship, bool reverseThruster, string soundFilename, float thrusterPower, int engineLength, int exhaustConeDeg, Color exhaustColorMin, Color exhaustColorMax)
        {
            this.owner = ship;
            this.reverseThruster = reverseThruster;
            this.power = thrusterPower;
            this.engineLength = engineLength;
            this.exhaustConeDegRange = exhaustConeDeg;

            this.particlePixelEmitter = new ParticlePixelEmitter(exhaustColorMin, exhaustColorMax);

            this.particlePixelEmitter.Emitting = false;

            this.particlePixelEmitter.Life = -1; // -1 means infinite life.

            // Particles per 1000 updates...
            this.particlePixelEmitter.Frequency = Configuration.Ships.Thrusters.Frequency;

            this.particlePixelEmitter.LifeMin = Configuration.Ships.Thrusters.LifeMin;
            this.particlePixelEmitter.LifeMax = Configuration.Ships.Thrusters.LifeMax;

            this.particlePixelEmitter.SpeedMin = Configuration.Ships.Thrusters.SpeedMin;
            this.particlePixelEmitter.SpeedMax = Configuration.Ships.Thrusters.SpeedMax;

            this.Add(this.particlePixelEmitter);

            this.thrusterSound = new Sound(soundFilename);
            this.thrusterSound.Volume = Configuration.SoundVolume;
        }

        #endregion Constructor

        #region Operations

        public void FireThruster()
        {
            int shipDirectionDeg = this.owner.SpriteSheet.GetDirectionDeg(this.owner.Sprite) % 360;

            int thrustDirectionDeg;
            int forceDirectionDeg;
            if (this.reverseThruster)
            {
                forceDirectionDeg = (shipDirectionDeg + 180) % 360;
                thrustDirectionDeg = shipDirectionDeg;
            }
            else
            {
                forceDirectionDeg = shipDirectionDeg;
                thrustDirectionDeg = (shipDirectionDeg + 180) % 360;
            }

            // Force to add to the ship.
            Vector v = Vector.FromDirection(forceDirectionDeg, this.Power);

            // Add the force to the ship.
            this.owner.Velocity += v;

            // Set the min/max angle for the spray cone.
            double shipDirectionRadians = (Math.PI / 180d) * thrustDirectionDeg;
            double halfConeRadians = (Math.PI / 180d) * this.exhaustConeDegRange / 2d;
            this.particlePixelEmitter.DirectionMin = Convert.ToSingle(shipDirectionRadians - halfConeRadians);
            this.particlePixelEmitter.DirectionMax = Convert.ToSingle(shipDirectionRadians + halfConeRadians);

            Point thrusterOriginPoint = SolidEntity.GetPosition(this.owner.Center, thrustDirectionDeg, this.engineLength);

            this.particlePixelEmitter.X = thrusterOriginPoint.X;
            this.particlePixelEmitter.Y = thrusterOriginPoint.Y;

            if (!this.particlePixelEmitter.Emitting)
            {
                this.particlePixelEmitter.Emitting = true;
                try
                {
                    this.thrusterChannel = this.thrusterSound.Play(true);
                }
                catch
                {
                    // Must be out of sound channels.
                }
            }
        }

        public void EndThruster()
        {
            if (this.particlePixelEmitter.Emitting)
            {
                this.particlePixelEmitter.Emitting = false;

                this.thrusterChannel.Stop();
                this.thrusterChannel = null;
            }
        }

        #endregion Operations

        #region IDisposable

        private bool disposed = false;

        ~Thruster()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // We haven't been disposed yet.

                if (disposing)
                {
                    /* The method has been called directly or indirectly by a
                     * user's code.  Dispose of managed resources here.
                     */
                    if (this.thrusterSound != null)
                    {
                        this.thrusterSound.Dispose();
                        this.thrusterSound = null;
                    }

                    if (this.thrusterChannel != null)
                    {
                        this.thrusterChannel.Dispose();
                        this.thrusterChannel = null;
                    }
                }

                // Dispose of unmanaged resources _only_ out here.

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}