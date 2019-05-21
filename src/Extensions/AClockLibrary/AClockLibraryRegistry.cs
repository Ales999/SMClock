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
using System.Windows;
using AClockLibrary.ViewModels;
using Caliburn.Micro;
using CommonLib.Interfaces;

namespace AClockLibrary
{
    using StructureMap;

    public class AClockLibraryRegistry : Registry
    {
        public AClockLibraryRegistry()
        {
            /*
            // MediatR handler registrations
            this.Scan(s =>
            {
                s.Assembly(this.GetType().Assembly);
                
                s.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                s.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                s.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                s.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                
                s.ConnectImplementationsToTypesClosing(typeof(IHandle<>));
                //s.ExcludeType<AClockViewModel>();
            });
            */

            /*
            this.Scan(x =>
            {
                x.Assembly(this.GetType().Assembly);
                //x.ConnectImplementationsToTypesClosing(typeof(AClockView));
                
                x.AssemblyContainingType<AClockView>();
                x.Convention<ViewRegistrationConvention>();
                x.ExcludeType<AClockViewModel>();
                x.LookForRegistries();

            });
            */


            // ensure registration for the ViewModel for Caliburn.Micro         
           // For(typeof(INotificationHandler<>)).Singleton().Add(typeof(AClockViewModel));

            For<IAClock>().Use<AClockViewModel>().Singleton();

            //For(typeof(IAClock)).Add(typeof(AClockViewModel)).Singleton();
            //For(typeof(IAClockControlViewModel)).Add(typeof(AClockControlViewModel)).Singleton();

            //Not needed current: For<IAClockControlViewModel>().Use<AClockControlViewModel>().Singleton();



            //For(typeof(System.Object)).Add(typeof(AClockControlViewModel)).Singleton();

            //For<Screen>().Use<AClockView>();
        }
    }
}
