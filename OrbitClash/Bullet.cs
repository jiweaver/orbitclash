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
 * Description: Holds info about a bullet.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace OrbitClash
{
    internal class Bullet : SolidEntity
    {
        #region Fields

        // How much does it hurt?
        private float power;

        // Who launched it?
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

        public Ship Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        #endregion Properties

        #region Constructors

        public Bullet(Ship owner, Surface bulletSurface, Point bulletPosition, Vector bulletVector, float power, int bulletLife)
            : base(GetBulletSprite(bulletSurface, bulletPosition), bulletPosition.X, bulletPosition.Y, bulletVector, bulletLife)
        {
            this.owner = owner;
            this.power = power;
        }

        private static Sprite GetBulletSprite(Surface bulletSurface, Point bulletPosition)
        {
            return new Sprite(bulletSurface, bulletPosition);
        }

        #endregion Constructors
    }
}