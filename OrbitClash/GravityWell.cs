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

/* $Id: GravityWell.cs 1975 2011-04-18 13:32:33Z weaver $
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: Gravity particle manipulator.  Adds force to all particles,
 * pulling them towards a central vortex.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Particles;
using SdlDotNet.Particles.Manipulators;

namespace OrbitClash
{
    internal class GravityWell : IParticleManipulator
    {
        #region Fields

        private Point position;
        private float power;
        private float radius;

        #endregion Fields

        #region Properties

        public Point Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

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

        public float Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = value;
            }
        }

        #endregion Properties

        #region Constructor

        public GravityWell(Point position, float radius, float power)
        {
            this.position = position;
            this.power = power;
            this.radius = radius;
        }

        #endregion Constructor

        #region IParticleManipulator

        private float GetForce(double distance)
        {
            double distancePercentage = distance / Configuration.Planet.GravityWellRadius * 100;
            double force = 1d / Math.Pow(distancePercentage, 2) * this.power;

            float result = Convert.ToSingle(force);

            return result;
        }

        public void Manipulate(ParticleCollection particles)
        {
            if (particles == null)
                /* They do this check in C# SDL Manipulators.  Why rock the
                 * boat?
                 */
                throw new ArgumentNullException("particles");

            // Exert gravitational force on every particle in the list.
            foreach (BaseParticle particle in particles)
            {
                if (particle.Static)
                    // Static particles are immobile.
                    continue;

                /* Create a vector from the particle's position to the gravity
                 * well.  Then get the distance and angle from it.
                 */
                Vector v = new Vector(particle.X, particle.Y, 0, position.X, position.Y, 0);

                double distance = v.Length;
                if (distance > radius)
                    // This particle is too far away to be affected.
                    continue;

                // Get the pull direction.
                int directionDeg = Convert.ToInt32(Math.Round(v.DirectionDeg));

                float force = GetForce(distance);

                // Create a vector from the force and the direction.
                v = Vector.FromDirection(directionDeg, force);

                // Add the new vector to the particle.
                particle.Velocity += v;
            }
        }

        #endregion IParticleManipulator
    }
}