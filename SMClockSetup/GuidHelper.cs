using System;

namespace SMClockSetup
{
    internal class GuidHelper
    {
        /// <summary> Объединяет два Hashcode'а в один </summary>
        /// <param name="hashCodes">Хеш коды</param>
        /// <returns>Шех код</returns>
        internal static int CombineHashCodes(params int[] hashCodes)
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            int i = 0;
            foreach (var hashCode in hashCodes)
            {
                if (i % 2 == 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                else
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;

                ++i;
            }

            return hash1 + (hash2 * 1566083941);
        }

        /// <summary>
        /// Данный метод генерирует GUID на основании имении и версии приложения
        /// Для одинакового имени и версии приложения GUID будет одинаковый.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="vesion">Версия</param>
        public static Guid GenerateGuid(string name, Version vesion)
        {
            int seed = CombineHashCodes(name.GetHashCode(), vesion.GetHashCode());
            var r = new Random(seed);
            var guid = new byte[16];
            r.NextBytes(guid);
            return new Guid(guid);
        }
    }

}
