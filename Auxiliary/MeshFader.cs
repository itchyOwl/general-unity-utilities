using UnityEngine;
using ItchyOwl.Extensions;
using System;

namespace ItchyOwl.Auxiliary
{
    // TODO: not working as supposed on iOS. Shader issue?
    // TODO: not working as supposed on iOS.
    // TODO: use MaterialManager
    public class MeshFader : Tweener
    {
        public bool hide;
        public bool startHidden;
        public float fadeOutDuration = 1;   // Default value, can be overridden in the FadeOut method
        public float fadeInDuration = 1;    // Default value, can be overridden in the FadeIn method
        public float minAlpha = 0;
        public float maxAlpha = 1;
        //public TweenMode tweenMode = TweenMode.Linear;  // Note that currently only linear mode works with callbacks. If ShaderTweener were updated to use time based tweening, callbacks would work also on eased tweens.
        public string[] propertyNames = new string[] { "_MainColor", "MainColor", "_Color", "Color", "TintColor", "_TintColor", "Tint", "_Tint", "Albedo", "_Albedo" };

        private bool isHidden;

        void Start()
        {
            if (startHidden)
            {
                FadeOut(0);
            }
        }

        void Update()
        {
            if (hide && !isHidden)
            {
                FadeOut();
            }
            else if (!hide && isHidden)
            {
                FadeIn();
            }
        }

        public void FadeOut(float? duration = null, Action callback = null)
        {
            hide = true;
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                renderers.ForEach(r => r.materials.ForEach(m =>
                {
                    if (m.HasProperty("_OutlineColor"))
                    {
                        m.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
                    }
                    SetTransparent(m);
                }));
                Action<float> updateCallback = value =>
                {
                    renderers.ForEach(r => r.materials.ForEach(m =>
                    {
                        foreach (var propertyName in propertyNames)
                        {
                            if (m.HasProperty(propertyName))
                            {
                                Color propertyValue = m.GetColor(propertyName);
                                propertyValue.a = value;
                                m.SetColor(propertyName, propertyValue);
                            }
                        }
                    }));
                };
                TweenTo(tweenId: 0, to: minAlpha, duration: duration ?? fadeOutDuration, pingPong: false, updateCallback: updateCallback, readyCallback: callback, abortCallback: callback);
            }
            isHidden = true;
        }

        public void FadeIn(float? duration = null, Action callback = null)
        {
            hide = false;
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null)
            {
                renderers.ForEach(r => r.materials.ForEach(m =>
                {
                    if (m.HasProperty("_OutlineColor"))
                    {
                        m.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
                    }
                    SetTransparent(m);
                }));
                Action readyCallback = () =>
                {
                    //renderers.ForEach(r => r.materials.ForEach(m => SetOpaque(m)));   // TODO: check if the material was opaque
                    if (callback != null) { callback(); }
                };
                Action<float> updateCallback = value =>
                {
                    renderers.ForEach(r => r.materials.ForEach(m =>
                    {
                        foreach (var propertyName in propertyNames)
                        {
                            if (m.HasProperty(propertyName))
                            {
                                Color propertyValue = m.GetColor(propertyName);
                                propertyValue.a = value;
                                m.SetColor(propertyName, propertyValue);
                            }
                        }
                    }));
                };
                TweenTo(tweenId: 0, to: maxAlpha, duration: duration ?? fadeInDuration, pingPong: false, updateCallback: updateCallback, readyCallback: readyCallback, abortCallback: callback);
            }
            isHidden = false;
        }

        private void SetTransparent(Material m)
        {
            m.SetFloat("_Mode", 2); // 0 opaque (default), 1 transparent?, 2 fade?
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);   // default one
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);   // default zero
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        
        private void SetOpaque(Material m)
        {
            m.SetFloat("_Mode", 0);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            m.EnableKeyword("_ALPHATEST_ON");
            //m.DisableKeyword("_ALPHABLEND_ON");
            m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }
}

