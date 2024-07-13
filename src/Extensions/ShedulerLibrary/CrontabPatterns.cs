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
using System.Globalization;
using System.Text;

namespace ShedulerLibrary
{
    using CommonLib.Interfaces;

    internal class CrontabPatterns
    {
        public static string GetIntervalPattern(int period)
        {
            string cronFormat;

            #region Set crontab format
            // ReSharper disable once PossibleLossOfFraction
            var tryHour = (period / 60);

            if (tryHour > 1)
            {
                cronFormat = $"0 /{tryHour} * * *";
            }
            else
            {
                if (tryHour == 1)
                {
                    // Каждый час ровно
                    cronFormat = $"0 * * * *";
                }
                else
                {
                    if (period > 1)
                    {
                        cronFormat = $"*/{period} * * * *";
                    }
                    else
                    {
                        // Каждую минуту ровно
                        cronFormat = $"*/1 * * * *";
                    }
                }
            }
            #endregion

            return cronFormat;
        }

        public static List<string> GetConcretePatterns(CommonLib.Interfaces.ISetOfMark setOfMarks)
        {
            var retList = new List<string>();

            // TODO: inpml

            return retList;
        }

        /// <summary>
        /// Преобразовать строку вида "HH:MM" в crontab строку.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns>Строка вида "mm hh * * *"</returns>
        public static string GetConcretePatterns(string timeStamp)
        {

            var tmpBuilder = new StringBuilder();
            // Часы
            int HH = 0;
            // Минуты 
            int MM = 0;

            try
            {
                var tmpDateTime = DateTime.ParseExact(timeStamp, "H:mm", CultureInfo.InvariantCulture);
                HH = tmpDateTime.Hour;
                MM = tmpDateTime.Minute;
            }
            catch (FormatException)
            {
                //throw new FormatException($"Не удалось преобразовать {timeStamp} в DateTime", fe);
                Trace.WriteLine($"--== CrontabPatterns ==-- Не удалось преобразовать {timeStamp} в DateTime");
            }

            tmpBuilder.Append(MM > 0 ? MM : 0);
            tmpBuilder.Append(" ");
            tmpBuilder.Append(HH > 0 ? HH : 0);
            tmpBuilder.Append(" * * *");

            return tmpBuilder.ToString();
        }


    }
}
