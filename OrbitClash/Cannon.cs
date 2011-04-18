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

/* $Id: Cannon.cs 1991 2011-04-18 18:02:20Z weaver $
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: Holds a ships' cannon.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Audio;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Particles;

namespace OrbitClash
{
    internal class Cannon : IDisposable
    {
        #region Fields

        private float muzzleSpeed;
        private TimeSpan cooldown;
        private float power;
        private int bulletLife;

        private Surface bulletSurface;

        private Sound fireSound;
        private Sound dryFireSound;

        private Ship owner;

        private DateTime lastFiredTime;

        /* The number of pixels from the center of the ship sprite to the end
         * of the cannon barrel.
         */
        private int barrelLength;

        private ParticleCollection bulletCollection;

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

        public TimeSpan Cooldown
        {
            get
            {
                return this.cooldown;
            }
            set
            {
                this.cooldown = value;
            }
        }

        public DateTime LastFiredTime
        {
            get
            {
                return this.lastFiredTime;
            }
            set
            {
                this.lastFiredTime = value;
            }
        }

        public int BarrelLength
        {
            get
            {
                return this.barrelLength;
            }
            set
            {
                this.barrelLength = value;
            }
        }

        public int AmmoCount
        {
            get
            {
                return Configuration.MaxLiveBulletsPerCannon - this.GetLiveBulletCount();
            }
        }

        #endregion Properties

        #region Constructors

        public Cannon(Ship owner, int barrelLength, Surface bulletSurface, float power, TimeSpan cooldown, float muzzleSpeed, int bulletLife)
        {
            this.owner = owner;
            this.bulletSurface = bulletSurface;
            this.power = power;
            this.cooldown = cooldown;
            this.muzzleSpeed = muzzleSpeed;
            this.bulletLife = bulletLife;
            this.barrelLength = barrelLength;

            this.lastFiredTime = DateTime.MinValue;

            this.bulletCollection = new ParticleCollection();

            this.fireSound = new Sound(Configuration.Ships.Cannon.FiringSoundFilename);
            this.fireSound.Volume = Configuration.SoundVolume;

            this.dryFireSound = new Sound(Configuration.Ships.Cannon.DryFireSoundFilename);
            this.dryFireSound.Volume = Configuration.SoundVolume;
        }

        #endregion Constructors

        #region Public Operations

        public Bullet Fire()
        {
            Vector shipVector = this.owner.Velocity;

            Sprite sprite = this.owner.Sprite;
            int gunDirectionDeg = this.owner.SpriteSheet.GetDirectionDeg(sprite) % 360;
            Point shipCenterPos = sprite.Center;

            if (this.GetLiveBulletCount() >= Configuration.MaxLiveBulletsPerCannon)
            {
                // Too many bullets currently active for this cannon.

                try
                {
                    this.dryFireSound.Play();
                }
                catch
                {
                    // Must be out of sound channels.
                }

                return null;
            }

            DateTime now = DateTime.Now;

            if (this.lastFiredTime + this.cooldown > now)
                // Still cooling down.
                return null;

            Point gunBarrelPos = SolidEntity.GetPosition(shipCenterPos, gunDirectionDeg, this.barrelLength);

            Vector bulletVector = Vector.FromDirection(gunDirectionDeg, this.muzzleSpeed);
            bulletVector += shipVector;

            Bullet cannonBullet = new Bullet(this.owner, this.bulletSurface, gunBarrelPos, bulletVector, Configuration.Ships.Cannon.Power, bulletLife);

            /* Add the new bullet to the ship's particle collection so we can
             * tell it apart from bullets fired by the other ship.
             */
            this.bulletCollection.Add(cannonBullet);

            this.lastFiredTime = now;

            try
            {
                this.fireSound.Play();
            }
            catch
            {
                // Must be out of sound channels.
            }

            return cannonBullet;
        }

        // Updates the live bullet list and returns the count of live bullets.
        public int GetLiveBulletCount()
        {
            ParticleCollection particles = this.bulletCollection;
            for (int i = 0; i < particles.Count; i++)
            {
                BaseParticle particle = particles[i];
                if (particle.Life == 0)
                    particles.RemoveAt(i);
            }

            return particles.Count;
        }

        #endregion Public Operations

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Cannon()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.fireSound != null)
                    {
                        this.fireSound.Dispose();
                        this.fireSound = null;
                    }

                    if (this.dryFireSound != null)
                    {
                        this.dryFireSound.Dispose();
                        this.dryFireSound = null;
                    }
                }
                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}