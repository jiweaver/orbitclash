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
 * Description: The ScoreCard for a player (displayed on the InfoBar).
 */

#endregion Header Comments

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SdlDotNet.Graphics;

using Font = SdlDotNet.Graphics.Font;

namespace OrbitClash
{
    internal class ScoreCard : IDisposable
    {
        #region Fields

        private Player player;

        private Surface scoreCard;

        private Point displayPosition;

        private int kills;
        private int defeats;
        private int suicides;

        private Font font;

        private Surface killsText;
        private Surface defeatsText;
        private Surface suicidesText;
        private Surface shieldsText;
        private Surface bulletsText;

        #endregion Fields

        #region Properties

        public int Kills
        {
            get
            {
                return this.kills;
            }
            set
            {
                this.kills = value;
            }
        }

        public int Defeats
        {
            get
            {
                return this.defeats;
            }
            set
            {
                this.defeats = value;
            }
        }

        public int Suicides
        {
            get
            {
                return this.suicides;
            }
            set
            {
                this.suicides = value;
            }
        }

        #endregion Properties

        #region Constructors

        public ScoreCard(Player player, Point displayPosition)
        {
            this.player = player;

            this.displayPosition = displayPosition;

            this.kills = 0;
            this.defeats = 0;
            this.suicides = 0;

            this.font = new Font(Configuration.InfoBar.PlayerStatusDisplayFontFilename, Configuration.InfoBar.PlayerStatusDisplayFontSize);

            Surface scoreCardSurface = new Surface(Configuration.InfoBar.PlayerStatusDisplayImageFilename);

            Point position = new Point(Configuration.InfoBar.XBuffer, Configuration.InfoBar.YBuffer);

            // Show the ship's picture.
            scoreCardSurface.Blit(this.player.Ship.ShipPhotoSurface, position);

            position.X += this.player.Ship.ShipPhotoSurface.Width + Configuration.InfoBar.XBuffer;

            using (Surface text = font.Render(string.Format("Player {0}", player.Number), Configuration.Ships.Shields.InfoDisplayPlayerFontColor))
            {
                text.Transparent = true;
                scoreCardSurface.Blit(text, position);
            }

            int xPosition = position.X + Configuration.InfoBar.FirstColumn_PixelsToIndent;

            this.shieldsText = font.Render("Shields:", Configuration.Ships.Shields.InfoDisplayStrongColor, true);
            this.shieldsText.Transparent = true;
            scoreCardSurface.Blit(shieldsText, new Point(xPosition, position.Y + this.font.LineSize + 1));

            this.bulletsText = font.Render("Bullets:", Configuration.Ships.Cannon.InfoDisplayStrongBulletCountColor, true);
            this.bulletsText.Transparent = true;
            scoreCardSurface.Blit(bulletsText, new Point(xPosition, position.Y + this.font.LineSize * 2 + 1));

            xPosition = position.X + Configuration.InfoBar.SecondColumn_PixelsToIndent;

            this.killsText = font.Render("Kills:", Configuration.InfoBar.CounterTextColor, true);
            this.killsText.Transparent = true;
            scoreCardSurface.Blit(killsText, new Point(xPosition, position.Y));

            this.defeatsText = font.Render("Defeats:", Configuration.InfoBar.CounterTextColor, true);
            this.defeatsText.Transparent = true;
            scoreCardSurface.Blit(defeatsText, new Point(xPosition, position.Y + this.font.LineSize + 1));

            this.suicidesText = font.Render("Suicides:", Configuration.InfoBar.CounterTextColor, true);
            this.suicidesText.Transparent = true;
            scoreCardSurface.Blit(suicidesText, new Point(xPosition, position.Y + this.font.LineSize * 2 + 1));

            this.scoreCard = scoreCardSurface.Convert(Video.Screen, true, false);
        }

        #endregion Constructors

        #region Operations

        /* Draw the scorecard (/w current counter values) to the display
         * surface.
         */
        public void Refresh(Surface surface)
        {
            Point position = this.displayPosition;
            Ship ship = this.player.Ship;

            surface.Blit(this.scoreCard, position);

            position.X += this.player.Ship.ShipPhotoSurface.Width + Configuration.InfoBar.XBuffer;
            position.Y += Configuration.InfoBar.YBuffer;

            // Determine the color to use to display the Shield counter.
            Color shieldCounterColor;
            if (ship.Shields <= 1)
                shieldCounterColor = Configuration.Ships.Shields.InfoDisplayCriticalColor;
            else if (ship.Shields < Configuration.Ships.Shields.Power / 2)
                shieldCounterColor = Configuration.Ships.Shields.InfoDisplayWeakColor;
            else
                shieldCounterColor = Configuration.Ships.Shields.InfoDisplayStrongColor;

            using (Surface text = this.font.Render(ship.Shields.ToString(), shieldCounterColor, true))
                surface.Blit(text, new Point(position.X + Configuration.InfoBar.FirstColumn_PixelsToIndent + this.shieldsText.Width + Configuration.InfoBar.XBuffer, position.Y + this.font.LineSize + 1));

            // Determine the color to use to display the Bullet counter.
            Color bulletCounterColor;
            if (ship.AmmoCount <= 1)
                bulletCounterColor = Configuration.Ships.Cannon.InfoDisplayCriticalBulletCountColor;
            else if (ship.AmmoCount < Configuration.MaxLiveBulletsPerCannon / 2)
                bulletCounterColor = Configuration.Ships.Cannon.InfoDisplayWeakBulletCountColor;
            else
                bulletCounterColor = Configuration.Ships.Cannon.InfoDisplayStrongBulletCountColor;

            using (Surface text = font.Render(ship.AmmoCount.ToString(), bulletCounterColor, true))
                surface.Blit(text, new Point(position.X + Configuration.InfoBar.FirstColumn_PixelsToIndent + this.bulletsText.Width + Configuration.InfoBar.XBuffer, position.Y + this.font.LineSize * 2 + 1));

            // Kill / Defeat / Suicide counters.
            using (Surface text = font.Render(this.kills.ToString(), Configuration.InfoBar.CounterTextColor, true))
                surface.Blit(text, new Point(position.X + Configuration.InfoBar.SecondColumn_PixelsToIndent + this.killsText.Width + Configuration.InfoBar.XBuffer, position.Y));

            using (Surface text = font.Render(this.defeats.ToString(), Configuration.InfoBar.CounterTextColor, true))
                surface.Blit(text, new Point(position.X + Configuration.InfoBar.SecondColumn_PixelsToIndent + this.defeatsText.Width + Configuration.InfoBar.XBuffer, position.Y + this.font.LineSize + 1));

            using (Surface text = font.Render(this.suicides.ToString(), Configuration.InfoBar.CounterTextColor, true))
                surface.Blit(text, new Point(position.X + Configuration.InfoBar.SecondColumn_PixelsToIndent + this.suicidesText.Width + Configuration.InfoBar.XBuffer, position.Y + this.font.LineSize * 2 + 1));
        }

        #endregion Operations

        #region IDisposable

        private bool disposed = false;

        ~ScoreCard()
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
                    if (this.font != null)
                    {
                        this.font.Dispose();
                        this.font = null;
                    }

                    if (this.scoreCard != null)
                    {
                        this.scoreCard.Dispose();
                        this.scoreCard = null;
                    }
                }

                // Dispose of unmanaged resources _only_ out here.

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}