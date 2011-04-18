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

/* $Id: SpriteSheet.cs 1976 2011-04-18 13:39:28Z weaver $
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: Holds information about a rotation sprite sheet.
 */

#endregion Header Comments

using System.Drawing;
using SdlDotNet.Graphics.Sprites;

namespace OrbitClash
{
    internal class SpriteSheet
    {
        #region Fields

        private Bitmap bitmap;
        private Size frameSize;
        private int firstFrameShipDirectionDeg;
        private int rotationPerFrameDeg;
        private Color transparentColor;

        #endregion Fields

        #region Properties

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

        #endregion Properties

        #region Constructors

        public SpriteSheet(string spriteSheetFilename, Color transparentColor, Size frameSize, int rotationPerFrameDeg, int firstFrameShipDirectionDeg)
        {
            this.bitmap = new Bitmap(spriteSheetFilename);
            this.transparentColor = transparentColor;
            this.frameSize = frameSize;
            this.rotationPerFrameDeg = rotationPerFrameDeg;
            this.firstFrameShipDirectionDeg = firstFrameShipDirectionDeg;
        }

        #endregion Constructors

        #region Operations

        // Returns current degree of rotation.
        public int GetDirectionDeg(Sprite sprite)
        {
            AnimatedSprite animatedSprite = sprite as AnimatedSprite;

            return (this.firstFrameShipDirectionDeg + (animatedSprite.Frame * this.rotationPerFrameDeg)) % 360;
        }

        #endregion Operations
    }
}