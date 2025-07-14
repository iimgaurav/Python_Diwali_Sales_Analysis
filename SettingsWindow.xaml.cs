using System.Windows;

namespace ScreenRecorder
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load saved settings from application properties or config file
            // For demonstration, we'll use default values
            
            StartMinimizedCheck.IsChecked = Properties.Settings.Default.StartMinimized;
            ShowCountdownCheck.IsChecked = Properties.Settings.Default.ShowCountdown;
            MinimizeOnRecordCheck.IsChecked = Properties.Settings.Default.MinimizeOnRecord;
            
            // Set codec based on saved preference
            string savedCodec = Properties.Settings.Default.VideoCodec;
            foreach (System.Windows.Controls.ComboBoxItem item in CodecCombo.Items)
            {
                if (item.Tag.ToString() == savedCodec)
                {
                    CodecCombo.SelectedItem = item;
                    break;
                }
            }
            
            BitrateSlider.Value = Properties.Settings.Default.VideoBitrate;
            HardwareAccelCheck.IsChecked = Properties.Settings.Default.UseHardwareAcceleration;
            
            // Audio settings
            string savedAudioQuality = Properties.Settings.Default.AudioQuality;
            foreach (System.Windows.Controls.ComboBoxItem item in AudioQualityCombo.Items)
            {
                if (item.Tag.ToString() == savedAudioQuality)
                {
                    AudioQualityCombo.SelectedItem = item;
                    break;
                }
            }
            
            string savedSampleRate = Properties.Settings.Default.AudioSampleRate;
            foreach (System.Windows.Controls.ComboBoxItem item in SampleRateCombo.Items)
            {
                if (item.Tag.ToString() == savedSampleRate)
                {
                    SampleRateCombo.SelectedItem = item;
                    break;
                }
            }
            
            // Hotkeys
            StartStopHotkeyText.Text = Properties.Settings.Default.StartStopHotkey;
            PauseHotkeyText.Text = Properties.Settings.Default.PauseHotkey;
            ScreenshotHotkeyText.Text = Properties.Settings.Default.ScreenshotHotkey;
            
            // File management
            AutoCleanupCheck.IsChecked = Properties.Settings.Default.AutoCleanup;
            string savedCleanupPeriod = Properties.Settings.Default.CleanupPeriod;
            foreach (System.Windows.Controls.ComboBoxItem item in CleanupPeriodCombo.Items)
            {
                if (item.Tag.ToString() == savedCleanupPeriod)
                {
                    CleanupPeriodCombo.SelectedItem = item;
                    break;
                }
            }
            ShowThumbnailsCheck.IsChecked = Properties.Settings.Default.ShowThumbnails;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.StartMinimized = StartMinimizedCheck.IsChecked ?? false;
            Properties.Settings.Default.ShowCountdown = ShowCountdownCheck.IsChecked ?? true;
            Properties.Settings.Default.MinimizeOnRecord = MinimizeOnRecordCheck.IsChecked ?? true;
            
            Properties.Settings.Default.VideoCodec = ((System.Windows.Controls.ComboBoxItem)CodecCombo.SelectedItem)?.Tag?.ToString() ?? "h264";
            Properties.Settings.Default.VideoBitrate = (int)BitrateSlider.Value;
            Properties.Settings.Default.UseHardwareAcceleration = HardwareAccelCheck.IsChecked ?? true;
            
            Properties.Settings.Default.AudioQuality = ((System.Windows.Controls.ComboBoxItem)AudioQualityCombo.SelectedItem)?.Tag?.ToString() ?? "192";
            Properties.Settings.Default.AudioSampleRate = ((System.Windows.Controls.ComboBoxItem)SampleRateCombo.SelectedItem)?.Tag?.ToString() ?? "44100";
            
            Properties.Settings.Default.StartStopHotkey = StartStopHotkeyText.Text;
            Properties.Settings.Default.PauseHotkey = PauseHotkeyText.Text;
            Properties.Settings.Default.ScreenshotHotkey = ScreenshotHotkeyText.Text;
            
            Properties.Settings.Default.AutoCleanup = AutoCleanupCheck.IsChecked ?? false;
            Properties.Settings.Default.CleanupPeriod = ((System.Windows.Controls.ComboBoxItem)CleanupPeriodCombo.SelectedItem)?.Tag?.ToString() ?? "30";
            Properties.Settings.Default.ShowThumbnails = ShowThumbnailsCheck.IsChecked ?? true;
            
            Properties.Settings.Default.Save();
        }

        private void ResetToDefaults()
        {
            StartMinimizedCheck.IsChecked = false;
            ShowCountdownCheck.IsChecked = true;
            MinimizeOnRecordCheck.IsChecked = true;
            
            CodecCombo.SelectedIndex = 0; // H.264
            BitrateSlider.Value = 10;
            HardwareAccelCheck.IsChecked = true;
            
            AudioQualityCombo.SelectedIndex = 1; // High (192 kbps)
            SampleRateCombo.SelectedIndex = 0; // 44.1 kHz
            
            StartStopHotkeyText.Text = "Ctrl+Shift+R";
            PauseHotkeyText.Text = "Ctrl+Shift+P";
            ScreenshotHotkeyText.Text = "Ctrl+Shift+S";
            
            AutoCleanupCheck.IsChecked = false;
            CleanupPeriodCombo.SelectedIndex = 1; // 30 days
            ShowThumbnailsCheck.IsChecked = true;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset all settings to their default values?",
                "Reset Settings",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ResetToDefaults();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
        }
    }
}