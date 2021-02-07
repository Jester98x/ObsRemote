using System;
using Microsoft.Office.Interop.PowerPoint;

namespace ObsRemote
{
	public partial class ThisAddIn
	{
		private ObsController _obs;
		private string _currentScene;

		private void ThisAddIn_Startup(object sender, EventArgs e)
		{
			Application.SlideShowNextSlide += Application_SlideShowNextSlide;
			InitObs();
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

		private void Application_SlideShowNextSlide(SlideShowWindow Wn)
		{
			if (Wn == null)
			{
				return;
			}

			_obs.GetScenes();

			SwitchScene(Wn);
		}

		private void SwitchScene(SlideShowWindow Wn)
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
				if (obsCommand.StartsWith("OBS:", StringComparison.OrdinalIgnoreCase))
				{
					var obsSceneName = obsCommand.Substring(4).Trim();
					if (_currentScene != obsSceneName)
					{
						// Only change the scene if there is a need to
						_obs.ChangeScene(obsSceneName);
					}

					sceneChanged = true;
					_currentScene = obsSceneName;
				}

				if (obsCommand.StartsWith("OBSDEF:", StringComparison.OrdinalIgnoreCase))
				{
					_obs.DefaultScene = obsCommand.Substring(7).Trim();
				}

				if (obsCommand.StartsWith("**START"))
				{
					_obs.StartRecording();
				}

				if (obsCommand.StartsWith("**STOP"))
				{
					_obs.StopRecording();
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
			Startup += new EventHandler(ThisAddIn_Startup);
			Shutdown += new EventHandler(ThisAddIn_Shutdown);
		}

		#endregion
	}
}
