using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Media;
using Newtonsoft.Json;

namespace TaskTimeTracker
{
    public class TaskTime
    {
        private const string _defaultFormat = "dd\\d\\ hh\\:mm\\:ss";
        private const int _defaultTextWidth = 250;
        private const int _defaultTextHeight = 40;
        private const int _defaultTextFontSize = 16;
        private const int _defaultTextMarginLeft = 20;
        private const int _defaultTextMarginTop = 20;
        private const int _defaultTimerWidth = 250;
        private const int _defaultTimerHeight = 40;
        private const int _defaultTimerFontSize = 16;
        private const int _defaultTimerMarginLeft = 20;
        private const int _defaultTimerMarginTop = 60;
        private const string _defaultWindowBackgroundColorName = "White";
        private const string _defaultTextColorName = "Black";
        private const bool _defaultBoldFlag = false;
        private const string _defaultFontName = "Segoe UI";

        private int _saveIntervalMs = 60000;

        private Timer _saveTimer;
        private DateTime _lastSaveTime = DateTime.MinValue;

        /// <summary>
        /// Gets or sets a value indicating whether the timer for this task is currently active.
        /// </summary>
        [JsonIgnore]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the string format parameters used to display the elapsed time.
        /// </summary>
        public string TimeFormat { get; set; } = _defaultFormat;

        public string TaskName { get; set; } = "new task";

        public string TaskTextDisplay { get; set; } = string.Empty;

        public string TextFontName { get; set; } = _defaultFontName;
        public int TextFontSize { get; set; } = _defaultTextFontSize;
        public bool TextFontIsBold { get; set; } = _defaultBoldFlag;
        public string TextColorName { get; set; } = _defaultTextColorName;

        public int TextWidth { get; set; } = _defaultTextWidth;
        public int TextHeight { get; set; } = _defaultTextHeight;
        public int TextMarginLeft { get; set; } = _defaultTextMarginLeft;
        public int TextMarginTop { get; set; } = _defaultTextMarginTop;
        public int TextMarginRight { get; set; } = 0;
        public int TextMarginBottom { get; set; } = 0;

        public string TimerFontName { get; set; } = _defaultFontName;
        public int TimerFontSize { get; set; } = _defaultTimerFontSize;
        public bool TimerFontIsBold { get; set; } = _defaultBoldFlag;
        public string TimerColorName { get; set; } = _defaultTextColorName;

        public int TimerWidth { get; set; } = _defaultTimerWidth;
        public int TimerHeight { get; set; } = _defaultTimerHeight;
        public int TimerMarginLeft { get; set; } = _defaultTimerMarginLeft;
        public int TimerMarginTop { get; set; } = _defaultTimerMarginTop;
        public int TimerMarginRight { get; set; } = 0;
        public int TimerMarginBottom { get; set; } = 0;

        public string WindowBackgroundColorName { get; set; } = _defaultWindowBackgroundColorName;

        /// <summary>
        /// Gets or sets the time when the current session was started.
        /// </summary>
        [JsonIgnore]
        public DateTime SessionStartTime { get; set; }

        /// <summary>
        /// Gets or sets the amount of time elapsed prior to the current active session.
        /// </summary>
        [JsonIgnore]
        public TimeSpan PriorElapsed { get; set; }
        public int PriorElapsedSeconds { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the total amount of elapsed time. This is the sum
        /// of <see cref="PriorElapsed"/> and the time elapsed since
        /// <see cref="SessionStartTime"/>.
        /// </summary>
        [JsonIgnore]
        public TimeSpan NowTotalElapsed
        {
            get
            {
                if (IsActive)
                {
                    return (DateTime.Now - SessionStartTime) + PriorElapsed;
                }

                return PriorElapsed;
            }
        }

        /// <summary>
        /// Gets the total elapsed time using the text format.
        /// </summary>
        [JsonIgnore]
        public string NowTotalElapsedFormatted
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeFormat))
                {
                    return NowTotalElapsed.ToString(TimeFormat);
                }

                return NowTotalElapsed.ToString();
            }
        }

        public TaskTime()
        {
            PriorElapsed = TimeSpan.Zero;
            TimeFormat = _defaultFormat;
        }

        public bool StartSession()
        {
            bool started = false;

            if (!IsActive)
            {
                IsActive = true;
                SessionStartTime = DateTime.Now;
                _lastSaveTime = DateTime.MinValue;

                started = true;
            }

            if (started)
            {
                if (!object.ReferenceEquals(null, _saveTimer))
                {
                    throw new InvalidOperationException("save timer already active?");
                }

                _saveTimer = new Timer();
                _saveTimer.AutoReset = true;
                _saveTimer.Interval = _saveIntervalMs;
                _saveTimer.Elapsed += _saveTimer_Elapsed;
                _saveTimer.Start();
            }

            return started;
        }

        private void _saveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Save(false);
        }

        public bool EndSession()
        {
            bool ended = false;

            if (IsActive)
            {
                IsActive = false;
                PriorElapsed = PriorElapsed + (DateTime.Now - SessionStartTime);
                ended = true;
            }

            if (ended)
            {
                _saveTimer.Stop();
                _saveTimer.Close();
                _saveTimer.Dispose();
                _saveTimer = null;

                // Force saving one last time.
                Save(true);
            }

            return ended;
        }

        public void Save(bool force=false)
        {
            if (force || (DateTime.Now - _lastSaveTime).TotalMilliseconds > _saveIntervalMs)
            {
                _lastSaveTime = DateTime.Now;

                PriorElapsedSeconds = (int)PriorElapsed.TotalSeconds;

                if (string.IsNullOrEmpty(FileName))
                {
                    throw new ArgumentException(nameof(FileName));
                }

                string content = JsonConvert.SerializeObject(this, Formatting.Indented);
                System.IO.File.WriteAllText(FileName, content);
            }
        }

        public static TaskTime CreateNewFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(nameof(path));
            }

            var tt = new TaskTime();

            tt.FileName = path;
            tt.Save(true);

            return tt;
        }

        public static TaskTime FromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (!System.IO.File.Exists(path))
            {
                throw new InvalidOperationException($"File not found: {path}");
            }

            var content = System.IO.File.ReadAllText(path);

            var tt = JsonConvert.DeserializeObject<TaskTime>(content);

            tt.PriorElapsed = TimeSpan.FromSeconds(tt.PriorElapsedSeconds);
            tt.FileName = path;

            return tt;
        }
    }
}
