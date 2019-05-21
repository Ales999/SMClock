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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfigLibrary
{
    using CommonLib.Interfaces;
    using SharpConfig;

    /// <summary>
    /// Пользовательский Класс настроек приложения
    /// на основе <see cref="SharpConfig"/> библиотеки,
    /// с моими правками.
    /// </summary>
    public class AppConfig : SharpConfig.Config, IAppConfig
    {
        private readonly Config _config;

        public string NameSpace { get; set; }

        public bool ConfigAutoSave { get; set; } = false;

        public new bool IsConfigFileExists => _config.IsConfigFileExists();

        public new dynamic this[string key]
        {
            get { return _config[key]; }
            set { _config[key] = value; }
        }

        public new bool TryGetValue(string key, out object value)
        {
            return _config.TryGetValue(key, out value);
        }

        public void SaveConfig()
        {
            _config.Save();
        }

        public AppConfig(string nameSpace) : this(nameSpace, false) { }


        public AppConfig(string nameSpace, bool configAutoSave) : base(nameSpace, false, configAutoSave)
        {
            NameSpace = nameSpace;
            ConfigAutoSave = configAutoSave;
           

            _config = this; //new Config(NameSpace) { AutoSave = ConfigAutoSave };
        }
    }
}
