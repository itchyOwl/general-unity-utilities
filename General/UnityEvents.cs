using System;
using UnityEngine;
using UnityEngine.Events;

namespace ItchyOwl.General
{
    [Serializable]
    public class ComponentEvent : UnityEvent<Component> { };

    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { };

    [Serializable]
    public class ColliderEvent2D : UnityEvent<Collider2D> { };
}
