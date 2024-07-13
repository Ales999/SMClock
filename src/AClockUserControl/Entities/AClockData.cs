
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Dnw.AnalogClock.Entities
{
    ///<summary>
    /// Entity representing the data used to generate the analog clock
    ///</summary>
    public class AClockData : INotifyPropertyChanged
    {
        #region Public Fields

        /// <summary>
        /// The Background color Brush
        /// </summary>
        public const string FLD_BackColor = "BackColor";

        /// <summary>
        /// Center x displacement for drawing ticks and hands
        /// </summary>
        public const string FLD_CenterX = "CenterX";

        /// <summary>
        /// Center Y displacement for drawing ticks and hands
        /// </summary>
        public const string FLD_CenterY = "CenterY";

        /// <summary>
        /// Clock height
        /// </summary>
        public const string FLD_ClockDimension = "ClockDimension";

        /// <summary>
        /// Hour hand
        /// </summary>
        public const string FLD_HourHand = "HourHand";

        /// <summary>
        /// Minute Hand
        /// </summary>
        public const string FLD_MinuteHand = "MinuteHand";

        /// <summary>
        /// Seconds hand
        /// </summary>
        public const string FLD_SecondsHand = "SecondsHand";

        /// <summary>
        /// TicksColor
        /// </summary>
        public const string FLD_TickColor = "TickColor";

        public const string FLD_TickThickness = "TickThickness";

        /// <summary>
        /// Tick thickness divisor
        /// </summary>
        public const string FLD_TickThicknessDivisor = "TickThicknessDivisor";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The Background color Brush
        /// </summary>
        private SolidColorBrush mBackColor;

        /// <summary>
        /// Center x displacement for drawing ticks and hands
        /// </summary>
        private double mCenterX;

        /// <summary>
        /// Center Y displacement for drawing ticks and hands
        /// </summary>
        private double mCenterY;

        /// <summary>
        /// Clock height
        /// </summary>
        private double mClockDimension;

        /// <summary>
        /// Date And Time
        /// </summary>
        private DateTime mDateAndTime;

        /// <summary>
        /// Hour hand
        /// </summary>
        private HandData mHourHand;

        /// <summary>
        /// Minute Hand
        /// </summary>
        private HandData mMinuteHand;

        /// <summary>
        /// Seconds hand
        /// </summary>
        private HandData mSecondsHand;

        /// <summary>
        /// TicksColor
        /// </summary>
        private SolidColorBrush mTickColor;

        /// <summary>
        /// Tick thickness divisor
        /// </summary>
        private double mTickThicknessDivisor;

        #endregion Private Fields

        #region Public Events

        //using System.ComponentModel; //se necessario
        /// <summary>
        /// Evento generato alla modifica di una property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Constructors

        ///<summary>
        /// Costruttore
        ///</summary>
        public AClockData()
        {
            HourHand = new HandData(this) {IsHour = true};
            MinuteHand = new HandData(this);
            SecondsHand = new HandData(this);
            CenterX = 0;
            CenterY = 0;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The Background color Brush
        /// </summary>
        public SolidColorBrush BackColor
        {
            get
            {
                return mBackColor;
            }
            set
            {
                mBackColor = value;
                OnPropertyChanged(FLD_BackColor);
            }
        }

        /// <summary>
        /// Center x displacement for drawing ticks and hands
        /// </summary>
        public double CenterX
        {
            get
            {
                return mCenterX;
            }
            private set
            {
                mCenterX = value;
                OnPropertyChanged(FLD_CenterX);
            }
        }

        /// <summary>
        /// Center Y displacement for drawing ticks and hands
        /// </summary>
        public double CenterY
        {
            get
            {
                return mCenterY;
            }
            private set
            {
                mCenterY = value;
                OnPropertyChanged(FLD_CenterY);
            }
        }

        /// <summary>
        /// Clock height
        /// </summary>
        public double ClockDimension
        {
            get
            {
                return mClockDimension;
            }
            set
            {
                mClockDimension = value;
                OnPropertyChanged(FLD_ClockDimension);
                OnPropertyChanged(FLD_TickThickness);
                HourHand.UpdateHandProperties();
                MinuteHand.UpdateHandProperties();
                SecondsHand.UpdateHandProperties();
            }
        }

        /// <summary>
        ///  Clock radius
        /// </summary>
        public double ClockRadius
        {
            get
            {
                return ClockDimension * 0.5;
            }
        }

        /// <summary>
        /// Date And Time
        /// </summary>
        ///<remarks>
        ///</remarks>
        public DateTime DateAndTime
        {
            get
            {
                return mDateAndTime;
            }
            set
            {
                mDateAndTime = value;
                HourHand.HandTime = mDateAndTime.Hour;
                HourHand.HourMinute = mDateAndTime.Minute;
                MinuteHand.HandTime = mDateAndTime.Minute;
                SecondsHand.HandTime = mDateAndTime.Second;
            }
        }

        /// <summary>
        /// Hour hand
        /// </summary>
        public HandData HourHand
        {
            get
            {
                return mHourHand;
            }
            set
            {
                mHourHand = value;
                OnPropertyChanged(FLD_HourHand);
            }
        }

        /// <summary>
        /// Minute Hand
        /// </summary>
        public HandData MinuteHand
        {
            get
            {
                return mMinuteHand;
            }
            set
            {
                mMinuteHand = value;
                OnPropertyChanged(FLD_MinuteHand);
            }
        }

        /// <summary>
        /// Seconds hand
        /// </summary>
        public HandData SecondsHand
        {
            get
            {
                return mSecondsHand;
            }
            set
            {
                mSecondsHand = value;
                OnPropertyChanged(FLD_SecondsHand);
            }
        }

        /// <summary>
        /// TicksColor
        /// </summary>
        public SolidColorBrush TickColor
        {
            get
            {
                return mTickColor;
            }
            set
            {
                mTickColor = value;
                OnPropertyChanged(FLD_TickColor);
            }
        }


        /// <summary>
        /// The color of the numbers
        /// </summary>
        public const string FLD_NumbersColor = "NumbersColor";

        /// <summary>
        /// The color of the numbers
        /// </summary>
        private SolidColorBrush mNumbersColor;

        /// <summary>
        /// The color of the numbers
        /// </summary>
        public SolidColorBrush NumbersColor
        {
            get
            {
                return mNumbersColor;
            }
            set
            {
                mNumbersColor = value;
                OnPropertyChanged(FLD_NumbersColor);
            }
        }


        /// <summary>
        /// The Font family of the numbers
        /// </summary>
        public const string FLD_NumbersFontFamily = "NumbersFontFamily";

        /// <summary>
        /// The Font family of the numbers
        /// </summary>
        private FontFamily mNumbersFontFamily;

        /// <summary>
        /// The Font family of the numbers
        /// </summary>
        public FontFamily NumbersFontFamily
        {
            get
            {
                return mNumbersFontFamily;
            }
            set
            {
                mNumbersFontFamily = value;
                OnPropertyChanged(FLD_NumbersFontFamily);
            }
        }


        /// <summary>
        /// The size of the numbers
        /// </summary>
        public const string FLD_NumbersSize = "NumbersSize";

        /// <summary>
        /// The size of the numbers
        /// </summary>
        private double mNumbersSize;

        /// <summary>
        /// The size of the numbers
        /// </summary>
        public double NumbersSize
        {
            get
            {
                return mNumbersSize;
            }
            set
            {
                mNumbersSize = value;
                OnPropertyChanged(FLD_NumbersSize);
            }
        }





        /// <summary>
        ///  descrizione
        /// </summary>
        public double TickThickness => ClockDimension / TickThicknessDivisor;

        /// <summary>
        /// Tick thickness divisor
        /// </summary>
        public double TickThicknessDivisor
        {
            get
            {
                return mTickThicknessDivisor;
            }
            set
            {
                mTickThicknessDivisor = value;
                OnPropertyChanged(FLD_TickThicknessDivisor);
                OnPropertyChanged(FLD_TickThickness);
            }
        }

        #endregion Public Properties

        #region Protected Methods

        ///<summary>
        /// Evento che viene generato alla modifica di una property
        ///</summary>
        /// <param name="propertyName">nome della property</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods
    }
}