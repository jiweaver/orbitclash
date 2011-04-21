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
 * Description: Holds information about a rotation sprite sheet.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

namespace OrbitClash
{
    internal class SpriteSheet : IDisposable
    {
        #region Fields

        private Bitmap bitmap;
        private Size frameSize;
        private int firstFrameShipDirectionDeg;
        private int rotationPerFrameDeg;
        private Color transparentColor;

        private AnimatedSprite animatedSprite;
        private int forwardThrusterEngineLength;
        private int reverseThrusterEngineLength;

        private int cannonBarrelLength;

        #endregion Fields

        #region Properties

        public AnimatedSprite AnimatedSprite
        {
            get
            {
                return this.animatedSprite;
            }
            set
            {
                this.animatedSprite = value;
            }
        }

        public Color TransparentColor
        {
            get
            {
                return this.transparentColor;
            }
            set
            {
                this.transparentColor = value;
            }
        }

        public int RotationPerFrameDeg
        {
            get
            {
                return this.rotationPerFrameDeg;
            }
            set
            {
                this.rotationPerFrameDeg = value;
            }
        }

        public int FirstFrameShipDirectionDeg
        {
            get
            {
                return this.firstFrameShipDirectionDeg;
            }
            set
            {
                this.firstFrameShipDirectionDeg = value;
            }
        }

        public Size FrameSize
        {
            get
            {
                return this.frameSize;
            }
            set
            {
                this.frameSize = value;
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }
            set
            {
                this.bitmap = value;
            }
        }

        public int ForwardThrusterEngineLength
        {
            get
            {
                return this.forwardThrusterEngineLength;
            }
            set
            {
                this.forwardThrusterEngineLength = value;
            }
        }

        public int ReverseThrusterEngineLength
        {
            get
            {
                return this.reverseThrusterEngineLength;
            }
            set
            {
                this.reverseThrusterEngineLength = value;
            }
        }

        public int CannonBarrelLength
        {
            get
            {
                return this.cannonBarrelLength;
            }
            set
            {
                this.cannonBarrelLength = value;
            }
        }

        // Returns current degree of rotation.
        public int CurrentDirectionDeg
        {
            get
            {
                return (this.firstFrameShipDirectionDeg + (this.animatedSprite.Frame * this.rotationPerFrameDeg)) % 360;
            }
        }

        #endregion Properties

        #region Constructors

        public SpriteSheet(string spriteSheetFilename, Color transparentColor, Size frameSize, int rotationPerFrameDeg, int firstFrameShipDirectionDeg, int rotationAnimationDelay, int cannonBarrelLength, int forwardThrusterEngineLength, int reverseThrusterEngineLength)
        {
            this.bitmap = new Bitmap(spriteSheetFilename);
            this.transparentColor = transparentColor;
            this.frameSize = frameSize;
            this.rotationPerFrameDeg = rotationPerFrameDeg;
            this.firstFrameShipDirectionDeg = firstFrameShipDirectionDeg;
            this.cannonBarrelLength = cannonBarrelLength;
            this.reverseThrusterEngineLength = reverseThrusterEngineLength;
            this.forwardThrusterEngineLength = forwardThrusterEngineLength;

            SurfaceCollection spriteSheet_SurfaceCollection = new SurfaceCollection();
            Surface spriteSheet_Surface = new Surface(this.bitmap).Convert(Video.Screen, true, false);

            spriteSheet_SurfaceCollection.Add(spriteSheet_Surface, this.frameSize);

            AnimationCollection animationCollection = new AnimationCollection();
            animationCollection.Add(spriteSheet_SurfaceCollection);
            animationCollection.Delay = rotationAnimationDelay;

            AnimatedSprite animatedSprite = new AnimatedSprite(animationCollection);

            animatedSprite.TransparentColor = this.transparentColor;
            animatedSprite.Transparent = true;

            this.animatedSprite = animatedSprite;
        }

        #endregion Constructors

        #region IDisposable

        private bool disposed = false;

        ~SpriteSheet()
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
                    if (this.bitmap != null)
                    {
                        this.bitmap.Dispose();
                        this.bitmap = null;
                    }
                }

                // Dispose of unmanaged resources _only_ out here.

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}