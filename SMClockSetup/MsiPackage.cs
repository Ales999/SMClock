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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;
using File = WixSharp.File;


namespace SMClockSetup
{
    internal class MsiPackage
    {
        /// <summary>
        /// Наименование проекта/продукта.
        /// </summary>
        internal const string ProductName = "SMClock";

        /// <summary>
        /// Уникальный идентифиакатор для инсталятора
        /// По нему будет определятся не установлен-ли у нас уже этот продукт.
        /// </summary>
        internal static readonly Guid guidApp = new Guid("D806305A-181A-4CE6-AAB4-00CA3334F5F3");

        /// <summary>
        /// Уникальный идентификатор для системы обновления продукта.
        /// Не изменять никогда в данном проекте!
        /// </summary>
        internal static readonly Guid UpgradeCode = new Guid("A9555B8C-2D4B-49B2-8359-7B5481F9379E"); //DO NOT CHANGE UpgradeCode



        public static ManagedProject CreateProject(Version versionApp, string sourceBaseDir /*, IEnumerable<string> filters*/)
        {
            var itemsDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            ManagedProject managedProject =
                new ManagedProject(ProductName,
                    new Dir($@"%ProgramFiles%\\{ProductName}", //$@"%AppDataFolder%\\{ProductName}",
                        new File(@"SMClock.exe"),
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
                            IconFile = Path.Combine(Directory.GetCurrentDirectory(), @"SMClockSetup\APP.ico")
                        },
                        new ExeFileShortcut("Uninstall SMclock", "[System64Folder]msiexec.exe", "/x [ProductCode]")
                    )
                )
                {
                    GUID = guidApp,
                    Version = versionApp,
                    ProductId = GuidHelper.GenerateGuid(ProductName, versionApp),
                    SourceBaseDir = sourceBaseDir,
                    OutFileName = @"Install_SMClock",
                    Encoding = Encoding.UTF8,
                    Codepage = "1251",
                    Language = "ru-ru",
                    UpgradeCode = UpgradeCode,
                    MajorUpgrade = new MajorUpgrade
                    {
                        Schedule = UpgradeSchedule.afterInstallValidate, /* .afterInstallInitialize, */
                        DowngradeErrorMessage =
                            "A later version of [ProductName] is already installed. Setup will now exit."
                    },
                    LocalizationFile = Path.Combine(itemsDir, "Resources", "WixUI_ru-ru.wxl"),
                    //Description = @"Analog clock with sound notifications", // waiting next release WiX
                    //InstallScope = InstallScope.perUser, // - Not supported by Win4!
                };
            // ---
            managedProject.ControlPanelInfo.ProductIcon = Path.Combine(Directory.GetCurrentDirectory(), @"SMClockSetup\APP.ico");

            managedProject.ResolveWildCards()
            .FindFile((f) => f.Name.EndsWith("SMClock.exe"))
            .First()
            .Shortcuts = new[] {
                new FileShortcut("SMClock.exe", "INSTALLDIR"),
                new FileShortcut("SMClock", "%Desktop%")
                {
                        IconFile = Path.Combine( Directory.GetCurrentDirectory(), @"SMClockSetup\APP.ico") // IconFile = Directory.GetCurrentDirectory() + @"\APP.ico"
                }
            };


            // end - return
            return managedProject;
        }

    }
}
