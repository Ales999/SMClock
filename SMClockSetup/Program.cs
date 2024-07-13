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
using System.IO;
using WixSharp;

namespace SMClockSetup
{


    internal class Program
    {
        private static void Main()
        {
           string currentDir = Directory.GetCurrentDirectory();
           Compiler.EmitRelativePaths = false;
            

            Run(currentDir);

        }
        private static void Run(string currentDir)
        {
            Version versionApp = new Version(1, 3, 11, 0);

            /// <summary>
            /// Каталог где находится релиз который и будем паковать в инсталятор.
            /// </summary>
            const string SRootDir = @"src\SMClock\bin\Release";
            const string LicenseFile = @"SMClockSetup\EULA.rtf";

            // Подготовим настройки проекта
            var msiProject = MsiPackage.CreateProject(versionApp, SRootDir);
            msiProject.OutDir = currentDir;
            msiProject.LicenceFile = Path.Combine(currentDir, LicenseFile);

            // Запуск создания инсталятора.
            msiProject.BuildMsi();

        }
    }

    
}

