using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace ScreenRecorder
{
    public class AudioRecorder
    {
        private WasapiLoopbackCapture _systemAudioCapture;
        private WaveInEvent _microphoneCapture;
        private WaveFileWriter _systemAudioWriter;
        private WaveFileWriter _microphoneWriter;
        private string _outputPath;
        private bool _isRecording = false;
        private bool _isPaused = false;
        private bool _recordSystemAudio = false;
        private bool _recordMicrophone = false;

        public async Task StartRecording(string outputPath, bool recordSystemAudio, bool recordMicrophone)
        {
            if (_isRecording)
                return;

            try
            {
                _outputPath = outputPath;
                _recordSystemAudio = recordSystemAudio;
                _recordMicrophone = recordMicrophone;
                _isRecording = true;
                _isPaused = false;

                await Task.Run(() =>
                {
                    if (_recordSystemAudio)
                    {
                        StartSystemAudioCapture();
                    }

                    if (_recordMicrophone)
                    {
                        StartMicrophoneCapture();
                    }
                });
            }
            catch (Exception ex)
            {
                _isRecording = false;
                throw new Exception($"Failed to start audio recording: {ex.Message}");
            }
        }

        public async Task StopRecording()
        {
            if (!_isRecording)
                return;

            _isRecording = false;

            await Task.Run(() =>
            {
                try
                {
                    _systemAudioCapture?.StopRecording();
                    _microphoneCapture?.StopRecording();

                    _systemAudioWriter?.Dispose();
                    _microphoneWriter?.Dispose();

                    _systemAudioCapture?.Dispose();
                    _microphoneCapture?.Dispose();

                    // If both system audio and microphone were recorded, mix them
                    if (_recordSystemAudio && _recordMicrophone)
                    {
                        MixAudioFiles();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to stop audio recording: {ex.Message}");
                }
            });
        }

        public void PauseRecording()
        {
            _isPaused = true;
            // Note: NAudio doesn't support pause/resume directly
            // You would need to implement this by stopping and restarting
        }

        public void ResumeRecording()
        {
            _isPaused = false;
        }

        private void StartSystemAudioCapture()
        {
            try
            {
                _systemAudioCapture = new WasapiLoopbackCapture();
                
                string systemAudioPath = Path.ChangeExtension(_outputPath, "_system.wav");
                _systemAudioWriter = new WaveFileWriter(systemAudioPath, _systemAudioCapture.WaveFormat);

                _systemAudioCapture.DataAvailable += (s, e) =>
                {
                    if (!_isPaused && _systemAudioWriter != null)
                    {
                        _systemAudioWriter.Write(e.Buffer, 0, e.BytesRecorded);
                    }
                };

                _systemAudioCapture.RecordingStopped += (s, e) =>
                {
                    _systemAudioWriter?.Dispose();
                    _systemAudioWriter = null;
                };

                _systemAudioCapture.StartRecording();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start system audio capture: {ex.Message}");
            }
        }

        private void StartMicrophoneCapture()
        {
            try
            {
                _microphoneCapture = new WaveInEvent();
                _microphoneCapture.WaveFormat = new WaveFormat(44100, 16, 1); // 44.1kHz, 16-bit, mono

                string microphonePath = Path.ChangeExtension(_outputPath, "_mic.wav");
                _microphoneWriter = new WaveFileWriter(microphonePath, _microphoneCapture.WaveFormat);

                _microphoneCapture.DataAvailable += (s, e) =>
                {
                    if (!_isPaused && _microphoneWriter != null)
                    {
                        _microphoneWriter.Write(e.Buffer, 0, e.BytesRecorded);
                    }
                };

                _microphoneCapture.RecordingStopped += (s, e) =>
                {
                    _microphoneWriter?.Dispose();
                    _microphoneWriter = null;
                };

                _microphoneCapture.StartRecording();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start microphone capture: {ex.Message}");
            }
        }

        private void MixAudioFiles()
        {
            try
            {
                string systemAudioPath = Path.ChangeExtension(_outputPath, "_system.wav");
                string microphonePath = Path.ChangeExtension(_outputPath, "_mic.wav");
                string finalAudioPath = _outputPath;

                if (File.Exists(systemAudioPath) && File.Exists(microphonePath))
                {
                    // Simple audio mixing - in a real implementation, you would use proper audio mixing
                    using (var systemReader = new WaveFileReader(systemAudioPath))
                    using (var micReader = new WaveFileReader(microphonePath))
                    {
                        // For simplicity, just use the system audio as the final output
                        // In a real implementation, you would properly mix the audio streams
                        File.Copy(systemAudioPath, finalAudioPath, true);
                    }

                    // Clean up temporary files
                    File.Delete(systemAudioPath);
                    File.Delete(microphonePath);
                }
                else if (File.Exists(systemAudioPath))
                {
                    File.Move(systemAudioPath, finalAudioPath);
                }
                else if (File.Exists(microphonePath))
                {
                    File.Move(microphonePath, finalAudioPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio mixing error: {ex.Message}");
                // Continue execution even if mixing fails
            }
        }

        public void Dispose()
        {
            StopRecording().Wait();
        }
    }
}