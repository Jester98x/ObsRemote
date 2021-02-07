using System;
using System.Collections.Generic;
using System.Linq;
using OBS.WebSocket.NET;

namespace ObsRemote
{
	public class ObsController : IDisposable
	{
		private bool _disposedValue;
		private ObsWebSocket _obs;
		private List<string> validScenes;
		private string defaultScene;
		private bool _connected;

		public void Connect()
		{
			if (!_connected)
			{
				_obs = new ObsWebSocket();
				_obs.Connected += Obs_Connected;
				_obs.Connect("ws://127.0.0.1:4444", string.Empty);
			}
		}

		private void Obs_Connected(object sender, EventArgs e)
		{
			_connected = true;
		}

		public string DefaultScene
		{
			get => defaultScene;
			set
			{
				if (validScenes.Contains(value))
				{
					defaultScene = value;
				}
			}
		}

		public bool ChangeScene(string scene)
		{
			if (!_connected)
			{
				return false;
			}

			if (!validScenes.Contains(scene))
			{
				if (string.IsNullOrEmpty(defaultScene))
				{
					return false;
				}

				scene = defaultScene;
			}

			_obs.Api.SetCurrentScene(scene);

			return true;
		}

		public bool GotoDefault()
		{
			return ChangeScene(DefaultScene);
		}

		public void GetScenes()
		{
			if (!_connected)
			{
				return;
			}

			var allScene = _obs.Api.GetSceneList();
			validScenes = allScene.Scenes.Select(s => s.Name).ToList();
		}

		public bool StartRecording()
		{
			if (!_connected)
			{
				return false;
			}

			try
			{
				_obs.Api.StartRecording();
			}
			catch {  /* Recording already started */ }

			return true;
		}

		public bool StopRecording()
		{
			if (!_connected)
			{
				return false;
			}


			try
			{
				_obs.Api.StopRecording();
			}
			catch {  /* Recording already stopped */ }

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
