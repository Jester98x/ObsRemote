# ObsRemote

A .Net plug-in for PowerPoint that allows a presentation to drive scene selection in OBS.

Inspired by Scott Hanselman's PowerPointToOBSSceneSwitcher:

* [GitHub](https://github.com/shanselman/PowerPointToOBSSceneSwitcher)
* [YouTube](https://www.youtube.com/watch?v=ciNcxi2bPwM)

Thanks Scott - it's bringing together amazing ideas like this that makes development so much fun!!

## Requirements
* [OBS](https://obsproject.com/)
* [obs-websocket](https://obsproject.com/forum/resources/obs-websocket-remote-control-obs-studio-from-websockets.466/) 
* [PowerPoint](https://www.microsoft.com/en-GB/microsoft-365)

## Usage

Add commands as presenter notes on a slide in PowerPoint.

* Activate an OBS scene

```OBSScene: {OBS scene name}```

Example:

```OBSScene: Just Chatting```

* Set a fallback or default scene, i.e. which seen to go to if one hasn't been defined in the slide notes

```OBSDefault: {OBS scene name}```

Example:

```OBSDefault: Presentation Only```

* Start recording

```OBSRecord: start```

* Stop recording

```OBSRecord: stop```

* Start streaming

```OBSStream: start```

* Stop streaming

```OBSStream: stop```

## Advanced Usage

* Set 'Presentation only' as the default scene, go to the 'StartUp' scene in OBS and start recording
```
	OBSDefault: Presentation only
	OBSScene: StartUp
	OBSRecord: Start
```
