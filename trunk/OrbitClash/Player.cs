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
 * Date: Apr 2011
 * Description: Holds info about a player.
 */

#endregion Header Comments

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SdlDotNet.Input;

namespace OrbitClash
{
    internal class Player : IDisposable
    {
        #region Fields

        // Player 1 or 2.
        private int number;

        // The player's ScoreCard.
        private ScoreCard scoreCard;

        // The player's Ship.
        private Ship ship;

        // Controls.
        private Key leftKey;
        private Key rightKey;
        private Key upKey;
        private Key downKey;
        private Key fireKey;

        // Control flags.
        private bool leftKeyIsDown;
        private bool rightKeyIsDown;
        private bool upKeyIsDown;
        private bool downKeyIsDown;
        private bool fireKeyWasPressed;

        // Particle emitters that create the effects of spawning and exploding.
        private ShipCreationEffect creationEffect;
        private ShipExplosionEffect explosionEffect;

        #endregion Fields

        #region Properties

        public int Number
        {
            get
            {
                return this.number;
            }
        }

        public Ship Ship
        {
            get
            {
                return this.ship;
            }
            set
            {
                this.ship = value;
            }
        }

        public ScoreCard ScoreCard
        {
            get
            {
                return this.scoreCard;
            }
            set
            {
                this.scoreCard = value;
            }
        }

        public Key LeftKey
        {
            get
            {
                return this.leftKey;
            }
            set
            {
                this.leftKey = value;
            }
        }

        public Key RightKey
        {
            get
            {
                return this.rightKey;
            }
            set
            {
                this.rightKey = value;
            }
        }

        public Key UpKey
        {
            get
            {
                return this.upKey;
            }
            set
            {
                this.upKey = value;
            }
        }

        public Key DownKey
        {
            get
            {
                return this.downKey;
            }
            set
            {
                this.downKey = value;
            }
        }

        public Key FireKey
        {
            get
            {
                return this.fireKey;
            }
            set
            {
                this.fireKey = value;
            }
        }

        public bool LeftKeyIsDown
        {
            get
            {
                return this.leftKeyIsDown;
            }
            set
            {
                this.leftKeyIsDown = value;
            }
        }

        public bool RightKeyIsDown
        {
            get
            {
                return this.rightKeyIsDown;
            }
            set
            {
                this.rightKeyIsDown = value;
            }
        }

        public bool UpKeyIsDown
        {
            get
            {
                return this.upKeyIsDown;
            }
            set
            {
                this.upKeyIsDown = value;
            }
        }

        public bool DownKeyIsDown
        {
            get
            {
                return this.downKeyIsDown;
            }
            set
            {
                this.downKeyIsDown = value;
            }
        }

        public bool FireKeyWasPressed
        {
            get
            {
                return this.fireKeyWasPressed;
            }
            set
            {
                this.fireKeyWasPressed = value;
            }
        }

        public ShipCreationEffect CreationEffect
        {
            get
            {
                return this.creationEffect;
            }
        }

        public ShipExplosionEffect ExplosionEffect
        {
            get
            {
                return this.explosionEffect;
            }
        }

        #endregion Properties

        #region Constructor

        public Player(int number)
        {
            this.number = number;

            // These need to be set later.
            this.ship = null;
            this.scoreCard = null;
            this.leftKey = Key.Unknown;
            this.rightKey = Key.Unknown;
            this.upKey = Key.Unknown;
            this.downKey = Key.Unknown;
            this.fireKey = Key.Unknown;

            this.leftKeyIsDown = false;
            this.rightKeyIsDown = false;
            this.upKeyIsDown = false;
            this.downKeyIsDown = false;
            this.fireKeyWasPressed = false;

            this.creationEffect = new ShipCreationEffect();
            this.explosionEffect = new ShipExplosionEffect();
        }

        #endregion Constructor

        #region Operations

        /// <summary>
        /// Check a key-press and set flags.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// True if the key caused a flag to become set; false
        /// otherwise.
        /// </returns>
        public bool CheckKeyPresses(Key key)
        {
            if (key == this.leftKey)
                this.leftKeyIsDown = true;
            else if (key == this.rightKey)
                this.rightKeyIsDown = true;
            else if (key == this.upKey)
                this.upKeyIsDown = true;
            else if (key == this.downKey)
                this.downKeyIsDown = true;
            else if (key == this.fireKey)
                this.fireKeyWasPressed = true;
            else
                return false;

            return true;
        }

        /// <summary>
        /// Check a key-release and set flags.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        /// True if the key caused a flag to become set; false
        /// otherwise.
        /// </returns>
        public bool CheckKeyReleases(Key key)
        {
            if (key == this.LeftKey)
                this.LeftKeyIsDown = false;
            else if (key == this.RightKey)
                this.RightKeyIsDown = false;
            else if (key == this.UpKey)
                this.UpKeyIsDown = false;
            else if (key == this.DownKey)
                this.DownKeyIsDown = false;
            else
                return false;

            return true;
        }

        /// <summary>
        /// Process the flags set by the "Check" methods (above).
        /// </summary>
        /// <returns>A bullet, if one was launched; null otherwise.</returns>
        public Bullet ProcessUserInput()
        {
            // Rotate.
            if (this.leftKeyIsDown)
                this.ship.BeginRotateLeft();
            else if (this.RightKeyIsDown)
                this.ship.BeginRotateRight();
            else
                this.ship.EndRotate();

            // Forward Thruster.
            if (this.upKeyIsDown)
                this.ship.BeginForwardThruster();
            else
                this.ship.EndForwardThruster();

            // Reverse Thruster.
            if (this.downKeyIsDown)
                this.ship.BeginReverseThruster();
            else
                this.ship.EndReverseThruster();

            // Cannon.
            if (this.fireKeyWasPressed)
            {
                this.fireKeyWasPressed = false;
                Bullet bullet = this.ship.FireCannon();
                if (bullet != null)
                    return bullet;
            }

            return null;
        }

        #endregion Operations

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Player()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.creationEffect != null)
                    {
                        this.creationEffect.Dispose();
                        this.creationEffect = null;
                    }

                    if (this.explosionEffect != null)
                    {
                        this.explosionEffect.Dispose();
                        this.explosionEffect = null;
                    }
                }
                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}