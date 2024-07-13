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
using StructureMap;
using StructureMap.Graph.Scanning;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace AClockLibrary
{
    public class ViewRegistrationConvention : StructureMap.Graph.IRegistrationConvention
    {
        // For prism:
        public void Process(Type type, Registry registry)
        {
            if (!type.Name.EndsWith("View") || !type.IsConcrete()) return;

            registry.For(typeof(object)).Use(type).Named(type.Name);
        }
        // for StructureMap:
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.AllTypes())
            {
                //if (!type.Name.EndsWith("View") || !type.IsConcrete()) return;
                //registry.For(typeof(object)).Use(type).Named(type.Name);

                if (type.Name.EndsWith("View") && !type.IsAbstract)
                {
                    registry.For(type).LifecycleIs(new UniquePerRequestLifecycle());
                }

            }

            /*
            // only interested in non abstract concrete types that have a matching named interface and start with Sql           
            if (type.IsAbstract || !type.IsClass || type.GetInterface(type.Name.Replace("Sql", "I")) == null)
                return;

            // Get interface and register (can use AddType overload method to create named types
            Type interfaceType = type.GetInterface(type.Name.Replace("Sql", "I"));
            registry.AddType(interfaceType, type);
            */
        }
    }
}
