using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using NAudio.Wave;
using NAudio.Lame;
using System.Linq;
using Microsoft.Win32;

namespace ScreenRecorder
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields and Properties

        private DispatcherTimer _recordingTimer;
        private DispatcherTimer _uiUpdateTimer;
        private DateTime _recordingStartTime;
        private bool _isRecording = false;
        private bool _isPaused = false;
        private string _currentRecordingPath;
        private ScreenRecorder _screenRecorder;
        private AudioRecorder _audioRecorder;
        private Rectangle _recordingArea;
        private IntPtr _targetWindowHandle;

        public string OutputPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor and Initialization

        public MainWindow()
        {
            InitializeComponent();
            InitializeApp();
            SetupEventHandlers();
            DataContext = this;
        }

        private void InitializeApp()
        {
            OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Screen Recordings");
            OutputPathText.Text = OutputPath;

            _recordingTimer = new DispatcherTimer();
            _recordingTimer.Interval = TimeSpan.FromSeconds(1);
            _recordingTimer.Tick += RecordingTimer_Tick;

            _uiUpdateTimer = new DispatcherTimer();
            _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
            _uiUpdateTimer.Tick += UiUpdateTimer_Tick;

            _screenRecorder = new ScreenRecorder();
            _audioRecorder = new AudioRecorder();
        }

        private void SetupEventHandlers()
        {
            WindowRadio.Checked += (s, e) => SelectWindowBtn.Visibility = Visibility.Visible;
            WindowRadio.Unchecked += (s, e) => SelectWindowBtn.Visibility = Visibility.Collapsed;
            
            AreaRadio.Checked += (s, e) => SelectAreaBtn.Visibility = Visibility.Visible;
            AreaRadio.Unchecked += (s, e) => SelectAreaBtn.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Event Handlers

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await StartRecording())
                {
                    StartBtn.IsEnabled = false;
                    StopBtn.IsEnabled = true;
                    PauseBtn.IsEnabled = true;
                    
                    _isRecording = true;
                    _recordingStartTime = DateTime.Now;
                    
                    StatusText.Text = "Recording...";
                    RecordingInfo.Visibility = Visibility.Visible;
                    
                    _recordingTimer.Start();
                    _uiUpdateTimer.Start();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to start recording: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await StopRecording();
                
                StartBtn.IsEnabled = true;
                StopBtn.IsEnabled = false;
                PauseBtn.IsEnabled = false;
                
                _isRecording = false;
                _isPaused = false;
                
                StatusText.Text = $"Recording saved: {Path.GetFileName(_currentRecordingPath)}";
                RecordingInfo.Visibility = Visibility.Collapsed;
                
                _recordingTimer.Stop();
                _uiUpdateTimer.Stop();
                
                // Ask user if they want to open the recording
                var result = System.Windows.MessageBox.Show("Recording completed! Would you like to open the file?", 
                    "Recording Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
                
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start("explorer.exe", $"/select,\"{_currentRecordingPath}\"");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to stop recording: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
            {
                ResumeRecording();
                PauseBtn.Content = "⏸ Pause";
                StatusText.Text = "Recording...";
                _recordingTimer.Start();
            }
            else
            {
                PauseRecording();
                PauseBtn.Content = "▶ Resume";
                StatusText.Text = "Recording paused";
                _recordingTimer.Stop();
            }
            
            _isPaused = !_isPaused;
        }

        private void SelectWindow_Click(object sender, RoutedEventArgs e)
        {
            var windowSelector = new WindowSelector();
            if (windowSelector.ShowDialog() == true)
            {
                _targetWindowHandle = windowSelector.SelectedWindowHandle;
                SelectWindowBtn.Content = $"Window: {windowSelector.SelectedWindowTitle}";
            }
        }

        private void SelectArea_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            
            var areaSelector = new AreaSelector();
            if (areaSelector.ShowDialog() == true)
            {
                _recordingArea = areaSelector.SelectedArea;
                SelectAreaBtn.Content = $"Area: {_recordingArea.Width}x{_recordingArea.Height}";
            }
            
            this.WindowState = WindowState.Normal;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = OutputPath;
            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputPath = dialog.SelectedPath;
                OutputPathText.Text = OutputPath;
                OnPropertyChanged(nameof(OutputPath));
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(OutputPath))
            {
                Process.Start("explorer.exe", OutputPath);
            }
            else
            {
                System.Windows.MessageBox.Show("Output folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        #endregion

        #region Recording Logic

        private async Task<bool> StartRecording()
        {
            // Generate filename
            string filename = AutoNamingCheck.IsChecked == true 
                ? $"Recording_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.mp4"
                : "Recording.mp4";
            
            _currentRecordingPath = Path.Combine(OutputPath, filename);

            // Determine recording area
            Rectangle captureArea = GetCaptureArea();
            
            // Get quality settings
            var qualityItem = (ComboBoxItem)QualityCombo.SelectedItem;
            int targetHeight = int.Parse(qualityItem.Tag.ToString());
            
            var frameRateItem = (ComboBoxItem)FrameRateCombo.SelectedItem;
            int frameRate = int.Parse(frameRateItem.Tag.ToString());

            // Start screen recording
            bool screenStarted = await _screenRecorder.StartRecording(captureArea, _currentRecordingPath, targetHeight, frameRate);
            
            if (!screenStarted)
            {
                return false;
            }

            // Start audio recording if enabled
            if (SystemAudioCheck.IsChecked == true || MicrophoneCheck.IsChecked == true)
            {
                string audioPath = Path.ChangeExtension(_currentRecordingPath, ".wav");
                await _audioRecorder.StartRecording(audioPath, 
                    SystemAudioCheck.IsChecked == true, 
                    MicrophoneCheck.IsChecked == true);
            }

            return true;
        }

        private async Task StopRecording()
        {
            // Stop screen recording
            await _screenRecorder.StopRecording();
            
            // Stop audio recording
            await _audioRecorder.StopRecording();
            
            // Merge audio and video if audio was recorded
            if (SystemAudioCheck.IsChecked == true || MicrophoneCheck.IsChecked == true)
            {
                string audioPath = Path.ChangeExtension(_currentRecordingPath, ".wav");
                if (File.Exists(audioPath))
                {
                    await MergeAudioVideo(_currentRecordingPath, audioPath);
                    File.Delete(audioPath); // Clean up temporary audio file
                }
            }
        }

        private void PauseRecording()
        {
            _screenRecorder.PauseRecording();
            _audioRecorder.PauseRecording();
        }

        private void ResumeRecording()
        {
            _screenRecorder.ResumeRecording();
            _audioRecorder.ResumeRecording();
        }

        private Rectangle GetCaptureArea()
        {
            if (FullScreenRadio.IsChecked == true)
            {
                return Screen.PrimaryScreen.Bounds;
            }
            else if (WindowRadio.IsChecked == true && _targetWindowHandle != IntPtr.Zero)
            {
                return GetWindowRectangle(_targetWindowHandle);
            }
            else if (AreaRadio.IsChecked == true && !_recordingArea.IsEmpty)
            {
                return _recordingArea;
            }
            
            return Screen.PrimaryScreen.Bounds; // Fallback to full screen
        }

        private async Task MergeAudioVideo(string videoPath, string audioPath)
        {
            // This would typically use FFMpeg to merge audio and video
            // For now, we'll just rename the video file since FFMpeg integration 
            // requires additional setup
            await Task.Delay(100); // Placeholder for actual merge operation
        }

        #endregion

        #region Timer Events

        private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            if (_isRecording && !_isPaused)
            {
                var elapsed = DateTime.Now - _recordingStartTime;
                TimerText.Text = elapsed.ToString(@"hh\:mm\:ss");
            }
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_isRecording && File.Exists(_currentRecordingPath))
            {
                try
                {
                    var fileInfo = new FileInfo(_currentRecordingPath);
                    double fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);
                    FileSizeText.Text = $"File size: {fileSizeMB:F1} MB";
                }
                catch
                {
                    // Ignore file access errors during recording
                }
            }
        }

        #endregion

        #region Utility Methods

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private Rectangle GetWindowRectangle(IntPtr handle)
        {
            RECT rect = new RECT();
            GetWindowRect(handle, ref rect);
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Window Events

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_isRecording)
            {
                var result = System.Windows.MessageBox.Show(
                    "Recording is in progress. Do you want to stop recording and exit?",
                    "Recording in Progress",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    StopRecording().Wait();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        #endregion
    }
}