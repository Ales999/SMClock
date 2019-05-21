using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Dnw.AnalogClock.Entities
{
    ///<summary>
    /// Entity representing the data used to generate a hand of the clock
    ///</summary>
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
        private const double MPi = Math.PI;

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

        private double _hourMinute;

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
        public double EndPointX => Parent.CenterX + (HandLength * Math.Sin(RadianValue));

        /// <summary>
        /// Endpoint Y of the hand when drawn on the clock
        /// </summary>
        public double EndPointY => Parent.CenterY - (HandLength * Math.Cos(RadianValue));

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
        /// Минуты, для вычисления смещения часовой стрелки
        /// </summary>
        public double HourMinute
        {
            get { return _hourMinute; }
            set
            {
                _hourMinute = value;
                OnPropertyChanged(FLD_HandTime);
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
                // Минуты или секунды используются
                if (!IsHour) return (HandTime * 360 / 60 * MPi / 180);

                // Так ровно по часам выдает точно
                var rvh = (HandTime * 360 / 12 * MPi / 180);

                // Смещение для часа, зависит от текущщей минуты
                var smh = (HourMinute * 360 / 60 * MPi / 180) * 0.180 * Math.Sin(0.5);


                var ret = rvh + smh;
                //var tmSin = Math.Sin(ret);


                return ret;
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