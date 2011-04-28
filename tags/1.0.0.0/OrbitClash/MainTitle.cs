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
 * Description: Show the main title screen.
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
    internal class MainTitle : IDisposable
    {
        #region Fields

        private Font mainTitleFont;
        private Surface mainTitleSurface;

        private Font startMessageFont;
        private Surface startMessageSurface;

        private Font creditMessageFont;
        private Surface creditMessageSurface;

        #endregion Fields

        #region Constructors

        public MainTitle()
        {
            this.mainTitleFont = new Font(Configuration.MainTitleFontFilename, Configuration.MainTitleFontSize);
            this.mainTitleSurface = this.mainTitleFont.Render(Configuration.Title, Configuration.MainTitleFontColor, true);

            this.creditMessageFont = new Font(Configuration.CreditMessageFontFilename, Configuration.CreditMessageFontSize);
            this.creditMessageSurface = this.creditMessageFont.Render(Configuration.CreditMessage, Configuration.CreditMessageFontColor, true);

            this.startMessageFont = new Font(Configuration.StartMessageFontFilename, Configuration.StartMessageFontSize);
            this.startMessageSurface = this.startMessageFont.Render(Configuration.StartMessage, Configuration.StartMessageFontColor, true);
        }

        #endregion Constructors

        #region Operations

        public void Refresh()
        {
            // Show the title.
            Video.Screen.Blit(this.mainTitleSurface, new Point(Configuration.PlayArea.Size.Width / 2 - this.mainTitleSurface.Width / 2 + Configuration.MainTitleCenterOffset.X, Configuration.PlayArea.Size.Height / 2 - this.mainTitleSurface.Height / 2 + Configuration.MainTitleCenterOffset.Y));

            // Show the "credit" message.
            Video.Screen.Blit(this.creditMessageSurface, new Point(Configuration.PlayArea.Size.Width / 2 - this.creditMessageSurface.Width / 2 + Configuration.CreditMessageCenterOffset.X, Configuration.PlayArea.Size.Height / 2 - this.creditMessageSurface.Height / 2 + Configuration.CreditMessageCenterOffset.Y));

            // Show the "press space to begin" message.
            Video.Screen.Blit(this.startMessageSurface, new Point(Configuration.PlayArea.Size.Width / 2 - this.startMessageSurface.Width / 2 + Configuration.StartMessageCenterOffset.X, Configuration.PlayArea.Size.Height / 2 - this.startMessageSurface.Height / 2 + Configuration.StartMessageCenterOffset.Y));
        }

        #endregion Operations

        #region IDisposable

        private bool disposed = false;

        ~MainTitle()
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
                    if (this.mainTitleFont != null)
                    {
                        this.mainTitleFont.Dispose();
                        this.mainTitleFont = null;
                    }

                    if (this.creditMessageFont != null)
                    {
                        this.creditMessageFont.Dispose();
                        this.creditMessageFont = null;
                    }

                    if (this.startMessageFont != null)
                    {
                        this.startMessageFont.Dispose();
                        this.startMessageFont = null;
                    }
                }

                // Dispose of unmanaged resources _only_ out here.

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}