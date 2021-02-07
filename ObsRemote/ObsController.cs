using System;
using System.Collections.Generic;
using System.Linq;
using OBS.WebSocket.NET;
using OBS.WebSocket.NET.Types;

namespace ObsRemote
{
	public class StreamingStateEventArgs : EventArgs
	{
		public bool IsStreaming { get; set; }
	}

	public class RecordingStateEventArgs:EventArgs
	{
		public bool IsRecording { get; set; }
	}

	public class ObsController : IDisposable
	{
		private bool _disposedValue;
		private ObsWebSocket _obs;
		private List<string> _validScenes;
		private string _defaultScene;
		private bool _connected;
		private bool _streaming;
		private bool _recording;

		public event EventHandler StreamingStateChanged;
		public event EventHandler RecordingStateChanged;

		protected virtual void OnStreamingStateChanged(StreamingStateEventArgs e)
		{
			var handler = StreamingStateChanged;
			e.IsStreaming = _streaming;
			handler?.Invoke(this, e);
		}

		protected virtual void OnRecordingStateChanged(RecordingStateEventArgs e)
		{
			var handler = RecordingStateChanged;
			e.IsRecording = _recording;
			handler?.Invoke(this, e);
		}

		public void Connect()
		{
			if (!_connected)
			{
				_obs = new ObsWebSocket();
				_obs.Connected += Obs_Connected;

				_obs.SceneCollectionChanged += Obs_SceneCollectionChanged;
				_obs.SceneListChanged += Obs_SceneListChanged;
				_obs.SceneCollectionListChanged += Obs_SceneCollectionListChanged;
				_obs.RecordingStateChanged += Obs_RecordingStateChanged;
				_obs.StreamingStateChanged += Obs_StreamingStateChanged;

				_obs.Connect("ws://127.0.0.1:4444", string.Empty);
			}
		}

		private void Obs_StreamingStateChanged(ObsWebSocket sender, OutputState newState)
		{
			if (newState == OBS.WebSocket.NET.Types.OutputState.Started)
			{
				_streaming = true;
			}
			else if (newState == OBS.WebSocket.NET.Types.OutputState.Stopped)
			{
				_streaming = false;
			}
		}

		private void Obs_RecordingStateChanged(ObsWebSocket sender, OutputState newState)
		{
			if (newState == OBS.WebSocket.NET.Types.OutputState.Started)
			{
				_recording = true;
			}
			else if (newState == OBS.WebSocket.NET.Types.OutputState.Stopped)
			{
				_recording = false;
			}
		}

		private void Obs_SceneCollectionListChanged(object sender, EventArgs e)
		{
			GetScenes();
		}

		private void Obs_SceneListChanged(object sender, EventArgs e)
		{
			GetScenes();
		}

		private void Obs_SceneCollectionChanged(object sender, EventArgs e)
		{
			GetScenes();
		}

		private void Obs_Connected(object sender, EventArgs e)
		{
			_connected = true;
		}

		public string DefaultScene
		{
			get => _defaultScene;
			set
			{
				if (_validScenes.Contains(value))
				{
					_defaultScene = value;
				}
			}
		}

		public bool ChangeScene(string scene)
		{
			if (!_connected)
			{
				return false;
			}

			if (!_validScenes.Contains(scene))
			{
				if (string.IsNullOrEmpty(_defaultScene))
				{
					return false;
				}

				scene = _defaultScene;
			}

			_obs.Api.SetCurrentScene(scene);

			return true;
		}

		public bool GotoDefault()
		{
			if (string.IsNullOrWhiteSpace(_defaultScene))
			{
				return false;
			}

			return ChangeScene(DefaultScene);
		}

		public void GetScenes()
		{
			if (!_connected)
			{
				return;
			}

			var allScene = _obs.Api.GetSceneList();
			_validScenes = allScene.Scenes.Select(s => s.Name).ToList();
		}

		public bool StartStopRecording(string command)
		{
			if (!_connected)
			{
				return false;
			}

			try
			{
				if (command.Equals("start", StringComparison.OrdinalIgnoreCase))
				{
					_obs.Api.StartRecording();
				}
				else if (command.Equals("stop", StringComparison.OrdinalIgnoreCase))
				{
					_obs.Api.StopRecording();
				}
			}
			catch
			{
				// Already doing it
			}

			return true;
		}

		public bool StartStopStreaming(string command)
		{
			if (!_connected)
			{
				return false;
			}

			try
			{
				if (command.Equals("start", StringComparison.OrdinalIgnoreCase))
				{
					_obs.Api.StartStreaming();
				}
				else if (command.Equals("stop", StringComparison.OrdinalIgnoreCase))
				{
					_obs.Api.StopStreaming();
				}
			}
			catch
			{
				// Already doing it
			}

			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
				}

				_obs.Disconnect();
				_obs = null;
				_disposedValue = true;
			}
		}

		~ObsController()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
