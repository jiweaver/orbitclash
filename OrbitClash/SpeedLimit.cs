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

/* $Id: SpeedLimit.cs 1977 2011-04-18 13:47:53Z weaver $
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: This class is a SDL.NET particle manipulator.  It checks and,
 * if necessary, corrects the velocity of each particle the specified
 * collection.  The speed limit is specified in pixels per Tick().
 */

#endregion Header Comments

using System;
using SdlDotNet.Core;
using SdlDotNet.Particles;
using SdlDotNet.Particles.Manipulators;

namespace OrbitClash
{
    internal class SpeedLimit : IParticleManipulator
    {
        #region Fields

        // The speed is specified in pixels per Tick().
        private float limit;

        #endregion Fields

        #region Properties

        public float Limit
        {
            get
            {
                return this.limit;
            }
            set
            {
                this.limit = value;
            }
        }

        #endregion Properties

        #region Constructor

        public SpeedLimit(float speedLimit)
        {
            this.limit = speedLimit;
        }

        #endregion Constructor

        #region IParticleManipulator

        public void Manipulate(ParticleCollection particles)
        {
            if (particles == null)
                /* They do this check in C# SDL Manipulators.  Why rock the
                 * boat?
                 */
                throw new ArgumentNullException("particles");

            foreach (BaseParticle p in particles)
            {
                if (p.Static)
                    // Static particles are immobile anyway.  Nothing to do.
                    continue;

                float speedDiff = this.limit - p.Velocity.Length;

                if (speedDiff < 0)
                {
                    /* this particle is traveling too fast.  Reduce the
                     * particle's velocity to the speed limit.
                     */
                    Vector particleVelocity = p.Velocity;
                    particleVelocity.Length = this.limit;
                    p.Velocity = particleVelocity;
                }
            }
        }

        #endregion IParticleManipulator
    }
}