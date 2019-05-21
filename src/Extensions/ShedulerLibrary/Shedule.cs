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
using System.Text;
using System.Threading.Tasks;
using CommonLib.Interfaces;

namespace ShedulerLibrary
{
 
    /// <summary>
    /// Данный класс нам пока не нужен.
    /// Все происходит в <see cref="SheduleIntervalHandler"/> который
    /// сам активируется при получении сообщения по MediatR
    /// </summary>
    public class Shedule : IShedule
    {
        private readonly ILogger _logger;

        public Shedule(ILogger logger)
        {
            _logger = logger;

            _logger.Information("---=== Ctor Shedule ===---");
        }

        public void StarSheduler()
        {
            /*
            JobManager.JobException += (info) => _logger.Error(info.Exception, "An error just happened with a scheduled job: ");
            JobManager.UseUtcTime();
            JobManager.Initialize(new SheduleConfFlReg());
            */
            //JobManager.
        }

        /*
        ~Shedule()
        {
            
            JobManager.Stop();
            JobManager.RemoveAllJobs();
            
        }
        */
    }
}
