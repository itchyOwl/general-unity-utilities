using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineEventAsset : PlayableAsset, ITimelineClipAsset
{
    public bool launchEventsInEditorMode = true;
    public ExposedReference<GameObject> optionalTarget;
    public string optionalArgs = string.Empty;

    private TimelineEvent template = new TimelineEvent();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimelineEvent>.Create(graph, template);
        var instance = playable.GetBehaviour();
        instance.launchEventsInEditorMode = launchEventsInEditorMode;
        instance.director = owner.GetComponent<PlayableDirector>();
        instance.optionalTarget = optionalTarget.Resolve(graph.GetResolver());
        instance.optionalArgs = optionalArgs;
        return playable;
    }
}
