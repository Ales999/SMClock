﻿/*

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
using CommonLib.Interfaces;
using Microsoft.Win32;

namespace AppConfigLibrary
{
    /// <summary>
    /// Добавляет/убирает приложение из реестра авто-запуска
    /// </summary>
    public class AppAutoStart : IAppAutoStart
    {
        private const string regPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public bool AddToAutoStarup(string appName)
        {
            if (IsStartup(appName)) return IsStartup(appName);

            using ( var appKey = Registry.CurrentUser.OpenSubKey(regPath, true))
            {
                var appPath = Environment.GetCommandLineArgs()[0];
                if (appPath != null) appKey?.SetValue(appName, appPath);
            }
            return IsStartup(appName);
        }

        public bool IsStartup(string appName)
        {
            using (var appKey = Registry.CurrentUser.OpenSubKey(regPath, false))
            {
                var found = appKey?.GetValue(appName);
                return found != null;
            }
        }

        public bool RemoveFromAutoStartup(string appName)
        {
            if (!IsStartup(appName)) return IsStartup(appName);

            using ( var appKey = Registry.CurrentUser.OpenSubKey(regPath, true))
            {
                appKey?.DeleteValue(appName, false);
            }
            return IsStartup(appName);
        }
    }
}
