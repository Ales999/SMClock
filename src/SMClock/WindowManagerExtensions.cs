/*

    This file is part of SmClock.

    SmClock is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SmClock is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.

  (Этот файл — часть SmClock.

   SmClock - свободная программа: вы можете перераспространять ее и/или
   изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
   в каком она была опубликована Фондом свободного программного обеспечения;
   либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
   версии.

   SmClock распространяется в надежде, что она будет полезной,
   но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
   или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
   общественной лицензии GNU.

   Вы должны были получить копию Стандартной общественной лицензии GNU
   вместе с этой программой. Если это не так, см.
   <https://www.gnu.org/licenses/>.)

*/

using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;

namespace SMClock
{
    public static class WindowManagerExtensions
    {
        /// <summary>
        /// Shows a non-modal window for the specified model or refocuses the exsiting window.  
        /// </summary>
        /// <example>
        /// 
        /// public class MyViewModel : Conductor&lt;object&gt;
        /// ...
        /// _windowManager.FocusOrShowWindow(MyViewModel);
        /// ...
        /// </example>
        /// <remarks>
        /// If the model is already associated with a view and the view is a window that window will just be refocused
        /// and the parameter <paramref name="settings"/> is ignored.
        /// </remarks>
        public static void FocusOrShowWindow(this IWindowManager windowManager,
            object model,
            object context = null,
            IDictionary<string, object> settings = null)
        {
            var activate = model as IActivate;
            if (activate == null)
            {
                throw new ArgumentException(
                    $@"An instance of type {typeof(IActivate)} is required", nameof(model));
            }

            var viewAware = model as IViewAware;
            if (viewAware == null)
            {
                throw new ArgumentException(
                    $@"An instance of type {typeof(IViewAware)} is required", nameof(model));
            }

            if (!activate.IsActive)
            {
                //windowManager.ShowWindow(model, context, settings); // Old
                windowManager.ShowWindowAsync(model, context, settings);
                return;
            }

            var view = viewAware.GetView(context);
            if (view == null)
            {
                throw new InvalidOperationException("View aware that is active must have an attached view.");
            }

            var focus = view.GetType().GetMethod("Focus");
            if (focus == null)
            {
                throw new InvalidOperationException("Attached view requires to have a Focus method");
            }
            
            focus.Invoke(view, null);

            // ---
            

          
        }
    }

}
