using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineEventArgs : EventArgs
{
    public PlayableDirector director;
    public FrameData frameInfo;
    /// <summary>
    /// Can be null.
    /// </summary>
    public GameObject optionalTarget;
    public string optionalArgs = string.Empty;

    public TimelineEventArgs(PlayableDirector director, FrameData frameInfo, GameObject optionalTarget, string optionalArgs)
    {
        this.director = director;
        this.frameInfo = frameInfo;
        this.optionalTarget = optionalTarget;
        this.optionalArgs = optionalArgs;
    }
}

public class TimelineEvent : PlayableBehaviour
{
    public PlayableDirector director;
    public GameObject optionalTarget;
    public string optionalArgs = string.Empty;
    public bool launchEventsInEditorMode;

    public static event EventHandler<TimelineEventArgs> Playing = (sender, args) => { };
    public static event EventHandler<TimelineEventArgs> Paused = (sender, args) => { };

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (director == null) { return; }
        if (launchEventsInEditorMode || Application.isPlaying && director.time < director.duration)
        {
            Playing(this, new TimelineEventArgs(director, info, optionalTarget, optionalArgs)); 
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (director == null) { return; }
        if (launchEventsInEditorMode || Application.isPlaying && director.state == PlayState.Playing && director.time > 0)
        {
            Paused(this, new TimelineEventArgs(director, info, optionalTarget, optionalArgs));
        }
    }
}
