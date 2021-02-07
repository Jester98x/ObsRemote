using System;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace ObsRemote
{
	public partial class ThisAddIn
	{
		private ObsController _obs;

		private void ThisAddIn_Startup(object sender, EventArgs e)
		{
			Application.SlideShowNextSlide += Application_SlideShowNextSlide;
			InitObs();
			_obs.RecordingStateChanged += Obs_RecordingStateChanged;
			_obs.StreamingStateChanged += Obs_StreamingStateChanged;
		}

		private void Obs_StreamingStateChanged(object sender, EventArgs e)
		{
			// Do something to indicate that OBS is streaming or not
			if (((StreamingStateEventArgs)e).IsStreaming)
			{
				// We're stremaing !!!
			}
			else
			{
				// We're not streaming !!!
			}
		}

		private void Obs_RecordingStateChanged(object sender, EventArgs e)
		{
			// Do somting to indicate that OBS is recording or not
			if (((RecordingStateEventArgs)e).IsRecording)
			{
				// We're recording !!!
			}
			else
			{
				// We're not recording !!!
			}
		}

		private void InitObs()
		{
			try
			{
				_obs = new ObsController();
				_obs.Connect();

			}
			catch
			{
				// Unable to connect to OBS
			}
		}

		private void Application_SlideShowNextSlide(PowerPoint.SlideShowWindow Wn)
		{
			if (Wn == null)
			{
				return;
			}

			_obs.GetScenes();

			SwitchScene(Wn);
		}

		private void SwitchScene(PowerPoint.SlideShowWindow Wn)
		{
			var sceneChanged = false;

			string[] obsCommands = default;
			try
			{
				obsCommands = Wn.View.Slide.NotesPage.Shapes[2].TextFrame.TextRange.Text.Split('\r');
			}
			catch
			{
				// Nothing to read
			}

			if (obsCommands.Length == 0)
			{
				return;
			}

			foreach (var obsCommand in obsCommands)
			{
				if (obsCommand.StartsWith("OBSScene:", StringComparison.OrdinalIgnoreCase))
				{
					var obsSceneName = obsCommand.Substring(9).Trim();

					_obs.ChangeScene(obsSceneName);

					sceneChanged = true;
				}

				if (obsCommand.StartsWith("OBSDefault:", StringComparison.OrdinalIgnoreCase))
				{
					_obs.DefaultScene = obsCommand.Substring(11).Trim();
				}
				else if (obsCommand.StartsWith("OBSRecord:", StringComparison.OrdinalIgnoreCase))
				{
					_obs.StartStopRecording(obsCommand.Substring(10).Trim());
				}
				else if (obsCommand.StartsWith("OBSStream:", StringComparison.OrdinalIgnoreCase))
				{
					_obs.StartStopStreaming(obsCommand.Substring(10).Trim());
				}
			}

			if (!sceneChanged)
			{
				_obs.GotoDefault();
			}
		}

		private void ThisAddIn_Shutdown(object sender, EventArgs e)
		{
			Application.SlideShowNextSlide -= Application_SlideShowNextSlide;
		}

		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			Startup += ThisAddIn_Startup;
			Shutdown += ThisAddIn_Shutdown;
		}

		#endregion
	}
}
