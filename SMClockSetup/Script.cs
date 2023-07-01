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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WixSharp;
//using WixSharp.UI; // Нужен для запроса директории назначения
using File = WixSharp.File;

namespace SMClockSetup
{
    class Script
    {
        private const string appName = "SMClock";

        /// <summary>
        /// Версия сборки
        /// For example, if your version 1 uses Version value 1.0.1.0, then version 2 should have a Version value of 1.0.2.0 or higher (1.0.1.1 will not work here!).
        /// </summary>
        private static readonly Version verApp = new Version(1, 3, 5, 0);

        /// <summary>
        /// Уникальный идентифиакатор для инсталятора
        /// По нему будет определятся не установлен-ли у нас уже этот продукт.
        /// </summary>
        private static readonly Guid guidApp = new Guid("D806305A-181A-4CE6-AAB4-00CA3334F5F3");
        
        /// <summary>
        /// Каталог где находится релиз который и будем паковать в инсталятор.
        /// </summary>
        private const string SRootDir = @"..\src\SMClock\bin\Release";

        static int Main()
        {
            // DON'T FORGET to execute "install-package wixsharp" in the package manager console
            try
            {
                var project = new Project(appName,
                    new Dir($@"%AppDataFolder%\\{appName}", //new Dir(@"%ProgramFiles%\SMClock",
                        new File(@"SMClock.exe"),
                        //new File(@"SMClock.ico"),
                        new ExeFileShortcut("Uninstall SMClock", "[System64Folder]msiexec.exe", "/x [ProductCode]"),
                        new DirFiles(@"*.dll"),
                        new DirFiles(@"*.config"),
                        new Dir(@"Modules",
                            new DirFiles(@"Modules\*.dll")
                        )
                    ),
                    new Dir(@"%ProgramMenu%\OCS\SMClock",
                        new ExeFileShortcut("SMClock", "[INSTALLDIR]SMClock.exe", "")
                        {
                            IconFile = Directory.GetCurrentDirectory() + @"\APP.ico"
                        },
                        new ExeFileShortcut("Uninstall SMclock", "[System64Folder]msiexec.exe", "/x [ProductCode]")
                    )
                )
                {
                    Version = verApp,
                    // Если указано UI - запросить директорию назначения установки
                    //UI = WUI.WixUI_InstallDir,
                    // ID for installer
                    GUID = guidApp,
                    SourceBaseDir = SRootDir,
                    OutFileName = @"Install_SMClock",
                    Encoding = Encoding.UTF8,
                    Description = @"Analog clock with sound notifications",
                    LicenceFile = Directory.GetCurrentDirectory() + @"\EULA.rtf",
                    OutDir = "..",
                    Codepage = "1252",
                    Language = "ru-RU",
                    InstallScope = InstallScope.perUser,
                    MajorUpgrade = new MajorUpgrade
                    {
                        Schedule = UpgradeSchedule.afterInstallValidate, /* .afterInstallInitialize, */
                        DowngradeErrorMessage =
                            "A later version of [ProductName] is already installed. Setup will now exit."
                    }
                };
                // Set Icon's to project for visible in Add-Remove program.
                project.ControlPanelInfo.ProductIcon = Directory.GetCurrentDirectory() + @"\APP.ico";


                /*
                var exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (exeDir != null) project.LicenceFile = $@"{Path.Combine(exeDir, @"..\..", SRootDir)}\license.rtf";
                */

                project.ResolveWildCards()
                    .FindFile((f) => f.Name.EndsWith("SMClock.exe"))
                    .First()
                    .Shortcuts = new[] {
                new FileShortcut("SMClock.exe", "INSTALLDIR"),
                new FileShortcut("SMClock", "%Desktop%")
                    {
                        IconFile = Directory.GetCurrentDirectory() + @"\APP.ico"
                    }
                };

                project.WixSourceGenerated += Project_WixSourceGenerated;
                project.BuildMsi();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Biild MSI script Exception: {e.Message}");
                return 1;
            }

            return 0;

        }

        private static void Project_WixSourceGenerated(XDocument document)
        {
            var prodElement = document.Root.Select("Product");
            prodElement.SetAttribute("Manufacturer", "OCS");
        }


        /*
        /// <summary>
        /// Трансляция строки, в кодировке '1252', в строку 'UTF-8'
        /// </summary>
        /// <param name="str1252">Исходная строка в кодировке windows-1252</param>
        /// <returns>Строка в  UTF-8</returns>
        private static string GetUtf8Description(string str1252)
        {
            Encoding wind1252 = Encoding.GetEncoding(1252);
            Encoding utf8 = Encoding.UTF8;
            byte[] wind1252Bytes = wind1252.GetBytes(str1252);
            byte[] utf8Bytes = Encoding.Convert(wind1252, utf8, wind1252Bytes);
            string utf8String = Encoding.UTF8.GetString(utf8Bytes);
            return utf8String;
        }
        */

    }
}