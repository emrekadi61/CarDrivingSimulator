using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace Utils.Math
{
    public static class MathUtils
    {
        public static void Tween(this float val, float target, float duration, UnityAction onComplete = null)
        {
            DOTween.To(() => val, x => val = x, target, duration).OnComplete(() => onComplete.Invoke());
        }
    }
}