using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp;
using static WixSharp.Nsis.Compressor;

namespace SMClockSetup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Compiler.EmitRelativePaths = false;

            Run();

        }
        private static void Run()
        {
            Version versionApp = new Version(1, 3, 5, 1);

            /// <summary>
            /// Каталог где находится релиз который и будем паковать в инсталятор.
            /// </summary>
            const string SRootDir = @"..\..\..\src\SMClock\bin\Release";

            // Подготовим настройки проекта
            var msiProject = MsiPackage.CreateProject(versionApp, SRootDir);
            msiProject.OutDir = Directory.GetCurrentDirectory();


            // Запуск создания инсталятора.
            msiProject.BuildMsi();

        }
    }

    
}

