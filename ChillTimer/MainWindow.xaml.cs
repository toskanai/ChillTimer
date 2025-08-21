using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

using System.Windows.Threading;

namespace ChillTimer
{
    public partial class MainWindow : Window
    {


        private readonly TimeSpan _duration = TimeSpan.FromMinutes(25);
        private TimeSpan _remaining;
        private bool _isRunning;
        private DateTime _endUtc;
        private readonly DispatcherTimer _uiTimer;

        public MainWindow()
        {
            InitializeComponent();

            _remaining = _duration;
            _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _uiTimer.Tick += (_, __) => Tick();

            PreviewKeyDown += MainWindow_PreviewKeyDown;
            UpdateUi();
        }

       
        private void Close_Click(object sender, RoutedEventArgs e) => Close();
        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void PlayBtn_Click(object sender, RoutedEventArgs e) => Start();
        private void PauseBtn_Click(object sender, RoutedEventArgs e) => Pause();
        private void ResetBtn_Click(object sender, RoutedEventArgs e) => Reset();

        private void MainWindow_PreviewKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (_isRunning) Pause(); else Start();
                e.Handled = true;
            }
            else if (e.Key == Key.R)
            {
                Reset();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Start()
        {
            if (_remaining <= TimeSpan.Zero)
                _remaining = _duration;

            _endUtc = DateTime.UtcNow + _remaining;
            _isRunning = true;
            _uiTimer.Start();
            StatusText.Text = "Läuft…";
            UpdateUi();
        }

        private void Pause()
        {
            if (!_isRunning) return;

            _remaining = _endUtc - DateTime.UtcNow;
            if (_remaining < TimeSpan.Zero) _remaining = TimeSpan.Zero;

            _isRunning = false;
            _uiTimer.Stop();
            StatusText.Text = "Pausiert";
            UpdateUi();
        }

        private void Reset()
        {
            _isRunning = false;
            _uiTimer.Stop();
            _remaining = _duration;
            StatusText.Text = $"Bereit ({FormatMMSS(_remaining)})";
            UpdateUi();
        }


        private void Tick()
        {
            var left = _endUtc - DateTime.UtcNow;
            if (left <= TimeSpan.Zero)
            {
                _remaining = TimeSpan.Zero;
                _isRunning = false;
                _uiTimer.Stop();
                SystemSounds.Asterisk.Play();
                StatusText.Text = "Fertig!";
            }
            else
            {
                _remaining = left;
            }
            UpdateUi();
        }

        private void UpdateUi()
        {
            TimerText.Text = FormatMMSS(_remaining);
        }

        private static string FormatMMSS(TimeSpan ts)
        {
            int minutes = (int)Math.Floor(ts.TotalMinutes);
            int seconds = ts.Seconds;
            return $"{minutes:00}:{seconds:00}";
        }

    }
}
