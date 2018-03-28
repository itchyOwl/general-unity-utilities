using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.4448276f, 0f, 1f)]
[TrackClipType(typeof(TimelineEventAsset))]
public class TimelineEventTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TimelineEventMixer>.Create(graph, inputCount);
    }
}
