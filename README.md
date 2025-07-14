# Screen Recorder - Professional Windows Desktop Application

A feature-rich, user-friendly Windows desktop application for screen recording with audio capture capabilities. Built with C# and WPF, offering professional-grade recording features with an intuitive interface.

## 🎯 Features

### Core Recording Features
- **Full Screen Recording** - Capture entire desktop
- **Window Recording** - Record specific application windows
- **Area Selection** - Select custom rectangular areas to record
- **High Quality Output** - Multiple quality presets (720p to 4K)
- **Variable Frame Rate** - 30 FPS or 60 FPS recording
- **Real-time Status** - Live timer and file size display

### Audio Capabilities
- **System Audio Capture** - Record computer audio (speakers/headphones)
- **Microphone Recording** - Capture microphone input
- **Dual Audio Support** - Record both system and microphone audio simultaneously
- **High Quality Audio** - Multiple bitrate options (128-320 kbps)
- **Flexible Sample Rates** - 44.1 kHz, 48 kHz, and 96 kHz options

### User Interface
- **Modern Design** - Clean, professional Material Design-inspired interface
- **Intuitive Controls** - Clear Start/Stop/Pause buttons
- **Live Preview** - Real-time recording status and information
- **Responsive Layout** - Scales properly on different screen sizes
- **Dark Mode Support** - Comfortable viewing in any lighting

### File Management
- **Auto-naming** - Automatic date/time-based file naming
- **Custom Output Path** - Choose where to save recordings
- **MP4 Format** - Industry-standard video format
- **Quick Access** - One-click folder opening
- **File Size Monitoring** - Real-time storage usage display

### Advanced Features
- **Pause/Resume** - Pause recording and continue later
- **Settings Panel** - Comprehensive configuration options
- **Global Hotkeys** - System-wide keyboard shortcuts
- **Hardware Acceleration** - GPU-accelerated encoding when available
- **Auto-cleanup** - Automatic deletion of old recordings
- **Thumbnail Generation** - Video preview thumbnails

## 🔧 System Requirements

- **Operating System**: Windows 10 or later (64-bit)
- **Framework**: .NET 6.0 or later
- **RAM**: 4 GB minimum, 8 GB recommended
- **Storage**: 500 MB free space for installation
- **Graphics**: DirectX 11 compatible graphics card
- **Audio**: Windows-compatible audio devices

## 📦 Installation & Setup

### Prerequisites

1. **Install .NET 6.0 Runtime**
   ```bash
   # Download from Microsoft's official website
   https://dotnet.microsoft.com/download/dotnet/6.0
   ```

2. **Install Visual Studio 2022** (for development)
   - Community, Professional, or Enterprise edition
   - Include ".NET desktop development" workload

### Building from Source

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd screen-recorder
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Build the Application**
   ```bash
   dotnet build --configuration Release
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

### Dependencies

The application uses the following NuGet packages:
- **NAudio** (2.2.1) - Audio recording and processing
- **FFMpegCore** (5.0.2) - Video encoding and processing
- **System.Drawing.Common** (7.0.0) - Graphics and screen capture
- **System.Management** (7.0.2) - System information and management

## 🚀 Usage Guide

### Quick Start

1. **Launch the Application**
   - Run `ScreenRecorder.exe`
   - The main window will appear with recording options

2. **Choose Recording Mode**
   - **Full Screen**: Records entire desktop
   - **Select Window**: Choose specific application window
   - **Select Area**: Draw custom recording rectangle

3. **Configure Audio**
   - Check "Record System Audio" for computer sounds
   - Check "Record Microphone" for voice narration
   - Select audio quality settings

4. **Start Recording**
   - Click the green "Start Recording" button
   - Recording begins immediately
   - Timer and file size display in real-time

5. **Stop Recording**
   - Click the red "Stop Recording" button
   - File is automatically saved with date/time name
   - Option to open file location

### Advanced Usage

#### Window Selection
1. Select "Select Window" mode
2. Click "Choose Window" button
3. Pick from list of available windows
4. Selected window will be highlighted

#### Area Selection
1. Select "Select Area" mode
2. Click "Select Area" button
3. Click and drag to draw recording rectangle
4. Press ESC to cancel selection

#### Quality Settings
- **Low (720p)**: Smaller files, faster encoding
- **Medium (1080p)**: Balanced quality and size
- **High (1440p)**: High quality for detailed content
- **Ultra (4K)**: Maximum quality for professional use

#### Audio Configuration
- **System Audio**: Captures all computer audio output
- **Microphone**: Records from default microphone
- **Both**: Creates mixed audio track with both sources

### Settings Configuration

Access the Settings panel for advanced configuration:

#### General Settings
- Start minimized to system tray
- Show countdown before recording
- Minimize window during recording

#### Video Settings
- Choose video codec (H.264, H.265, VP9)
- Adjust bitrate (1-50 Mbps)
- Enable hardware acceleration

#### Audio Settings
- Set audio quality (128-320 kbps)
- Configure sample rate (44.1-96 kHz)

#### Hotkeys
- Start/Stop Recording: `Ctrl+Shift+R`
- Pause/Resume: `Ctrl+Shift+P`
- Screenshot: `Ctrl+Shift+S`

#### File Management
- Auto-cleanup old recordings
- Generate video thumbnails
- Custom output directory

## 🔧 Troubleshooting

### Common Issues

#### Recording Not Starting
- **Check Permissions**: Ensure app has screen recording permissions
- **Audio Devices**: Verify audio devices are properly connected
- **Disk Space**: Confirm sufficient storage space available
- **Antivirus**: Add application to antivirus exclusions

#### Poor Quality Recording
- **Increase Bitrate**: Higher bitrate = better quality
- **Check Hardware**: Ensure adequate CPU/GPU performance
- **Close Applications**: Reduce system load during recording
- **Update Drivers**: Keep graphics drivers current

#### Audio Issues
- **Check Audio Devices**: Verify input/output devices in Windows
- **Audio Permissions**: Ensure app has microphone access
- **Driver Updates**: Update audio drivers
- **Sample Rate**: Try different audio sample rates

### Performance Optimization

1. **Close Unnecessary Applications**
   - Free up system resources
   - Reduce CPU and memory usage

2. **Use Hardware Acceleration**
   - Enable GPU encoding in settings
   - Reduces CPU load significantly

3. **Adjust Quality Settings**
   - Lower settings for longer recordings
   - Higher settings for short, high-quality clips

4. **Storage Considerations**
   - Use fast SSD for recording storage
   - Monitor available disk space
   - Enable auto-cleanup for space management

## 🛠️ Development

### Project Structure
```
ScreenRecorder/
├── App.xaml                    # Application entry point
├── App.xaml.cs                 # Application logic
├── MainWindow.xaml             # Main UI layout
├── MainWindow.xaml.cs          # Main window logic
├── ScreenRecorder.cs           # Screen capture engine
├── AudioRecorder.cs            # Audio capture engine
├── WindowSelector.xaml         # Window selection dialog
├── WindowSelector.xaml.cs      # Window selection logic
├── AreaSelector.xaml           # Area selection overlay
├── AreaSelector.xaml.cs        # Area selection logic
├── SettingsWindow.xaml         # Settings dialog
├── SettingsWindow.xaml.cs      # Settings logic
└── Properties/
    ├── Settings.settings       # Application settings
    └── Settings.Designer.cs    # Generated settings class
```

### Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation for public methods
- Maintain MVVM pattern where applicable

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Support

For support, bug reports, or feature requests:
- Create an issue on GitHub
- Include system information and error details
- Attach log files if available

## 🔮 Future Enhancements

- **Live Streaming**: Direct streaming to platforms
- **Cloud Upload**: Automatic cloud storage integration
- **Video Editing**: Basic editing capabilities
- **Multiple Monitors**: Multi-display recording support
- **Annotation Tools**: Drawing and text overlay features
- **Scheduled Recording**: Timer-based recording
- **Mobile Companion**: Remote control via mobile app

---

**Built with ❤️ using C# and WPF**