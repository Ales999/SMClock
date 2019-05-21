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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace AClockLibrary
{
    public class HandData : INotifyPropertyChanged
    {
        #region Public Fields

        /// <summary>
        /// End point of the line of the hand
        /// </summary>
        public const string FLD_EndPointX = "EndPointX";

        /// <summary>
        /// End point of the line of the hand
        /// </summary>
        public const string FLD_EndPointY = "EndPointY";

        /// <summary>
        /// Hand color
        /// </summary>
        public const string FLD_HandColor = "HandColor";

        /// <summary>
        /// Hand length
        /// </summary>
        public const string FLD_HandLength = "HandLength";

        /// <summary>
        /// Hand thickness
        /// </summary>
        public const string FLD_HandThickness = "HandThickness";

        /// <summary>
        /// Hand Time
        /// </summary>
        public const string FLD_HandTime = "HandTime";

        /// <summary>
        /// Indicates if it is an hour hand
        /// </summary>
        public const string FLD_IsHour = "IsHour";

        /// <summary>
        /// Length multiplier
        /// </summary>
        public const string FLD_LengthMultiplier = "LengthMultiplier";

        /// <summary>
        /// Parent clock
        /// </summary>
        public const string FLD_Parent = "Parent";

        /// <summary>
        /// Radian value of the hand angle
        /// </summary>
        public const string FLD_RadianValue = "RadianValue";

        /// <summary>
        /// Thickness divisor
        /// </summary>
        public const string FLD_ThicknessDivisor = "ThicknessDivisor";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The pi math constant to calculate radians
        /// </summary>
        private const double PI = Math.PI;

        /// <summary>
        /// Hand color
        /// </summary>
        private SolidColorBrush mHandColor;

        /// <summary>
        /// Hand Time
        /// </summary>
        private double mHandTime;

        /// <summary>
        /// Indicates if it is an hour hand
        /// </summary>
        private bool mIsHour;

        /// <summary>
        /// Length multiplier
        /// </summary>
        private double mLengthMultiplier;

        /// <summary>
        /// Parent clock
        /// </summary>
        private AClockData mParent;

        /// <summary>
        /// Thickness divisor
        /// </summary>
        private double mThicknessDivisor;

        #endregion Private Fields

        #region Public Events

        /// <summary>
        /// Evento generato alla modifica di una property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Constructors

        ///<summary>
        /// Costruttore
        ///</summary>
        public HandData(AClockData parent)
        {
            Parent = parent;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Endpoint X of the hand when drawn on the clock
        /// </summary>
        public double EndPointX
        {
            get
            {
                return Parent.CenterX + (HandLength * System.Math.Sin(RadianValue));
            }
        }

        /// <summary>
        /// Endpoint Y of the hand when drawn on the clock
        /// </summary>
        public double EndPointY
        {
            get
            {
                return Parent.CenterY - (HandLength * System.Math.Cos(RadianValue));
            }
        }

        /// <summary>
        /// Hand color
        /// </summary>
        public SolidColorBrush HandColor
        {
            get
            {
                return mHandColor;
            }
            set
            {
                mHandColor = value;
                OnPropertyChanged(FLD_HandColor);
            }
        }

        /// <summary>
        /// Hand length
        /// </summary>
        public double HandLength
        {
            get
            {
                if (Parent == null) return 0;
                return Parent.ClockDimension / 2 * LengthMultiplier;
            }
        }

        /// <summary>
        /// Hand thickness
        /// </summary>
        public double HandThickness
        {
            get
            {
                if (Parent == null) return 0.0;
                return Parent.ClockDimension / ThicknessDivisor;
            }
        }

        /// <summary>
        /// Hand Time
        /// </summary>
        public double HandTime
        {
            get
            {
                return mHandTime;
            }
            set
            {
                mHandTime = value;
                OnPropertyChanged(FLD_HandTime);
                OnPropertyChanged(FLD_EndPointX);
                OnPropertyChanged(FLD_EndPointY);
            }
        }

        /// <summary>
        /// Indicates if it is an hour hand
        /// </summary>
        public bool IsHour
        {
            get
            {
                return mIsHour;
            }
            set
            {
                mIsHour = value;
                OnPropertyChanged(FLD_IsHour);
            }
        }

        /// <summary>
        /// Length multiplier
        /// </summary>
        public double LengthMultiplier
        {
            get
            {
                return mLengthMultiplier;
            }
            set
            {
                mLengthMultiplier = value;
                OnPropertyChanged(FLD_LengthMultiplier);
            }
        }

        /// <summary>
        /// Parent clock
        /// </summary>
        public AClockData Parent
        {
            get
            {
                return mParent;
            }
            private set
            {
                mParent = value;
                OnPropertyChanged(FLD_Parent);
            }
        }

        /// <summary>
        /// Thickness divisor
        /// </summary>
        public double ThicknessDivisor
        {
            get
            {
                return mThicknessDivisor;
            }
            set
            {
                mThicknessDivisor = value;
                OnPropertyChanged(FLD_ThicknessDivisor);
                OnPropertyChanged(FLD_HandThickness);
            }
        }

        #endregion Public Properties

        #region Private Properties

        /// <summary>
        /// Radian value of the hand angle
        /// </summary>
        private double RadianValue
        {
            get
            {
                if (IsHour)
                {
                    return HandTime * 360 / 12 * PI / 180;
                }
                else
                {
                    return HandTime * 360 / 60 * PI / 180;
                }
            }
        }

        #endregion Private Properties

        #region Public Methods

        public void UpdateHandProperties()
        {
            OnPropertyChanged(FLD_HandThickness);
            OnPropertyChanged(FLD_EndPointX);
            OnPropertyChanged(FLD_EndPointY);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods

    }
}
