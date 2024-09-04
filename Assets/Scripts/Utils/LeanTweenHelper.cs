using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LeanTweenHelper
{
    public static void BounceX(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        var startPos = rectTransform.anchoredPosition.x;
        LeanTween.moveX(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1)
            .setEaseOutQuad()
            .setOnComplete(() => rectTransform.LeanMoveX(startPos, duration2).setEaseOutBounce());
    }

    public static UniTask BounceXAsync(RectTransform rectTransform, float deltaX, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        return BounceXAsync(rectTransform, deltaX, CancellationToken.None, duration1, duration2);
    }

    public async static UniTask BounceXAsync(RectTransform rectTransform, float deltaX, CancellationToken stopToken, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        var tcs = new UniTaskCompletionSource();
        void CancelAnimation()
        {
            LeanTween.cancel(rectTransform.gameObject, true);
        }

        using (stopToken.Register(CancelAnimation))
        {
            var startPos = rectTransform.anchoredPosition.x;
            var (task, tweenDescription) = MoveXAsync(rectTransform, rectTransform.anchoredPosition.x + deltaX, duration1);
            tweenDescription.setEaseOutQuad();
            await task;
            if (false == stopToken.IsCancellationRequested)
            {
                (task, tweenDescription) = MoveXAsync(rectTransform, startPos, duration2);
                tweenDescription.setEaseOutBounce();
                await task;
            }
            rectTransform.anchoredPosition = new Vector2(startPos, rectTransform.anchoredPosition.y);
        }
        tcs.TrySetResult();
    }

    public static UniTask BounceYAsync(RectTransform rectTransform, float deltaY, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        return BounceYAsync(rectTransform, deltaY, CancellationToken.None, duration1, duration2);
    }

    public static async UniTask BounceYAsync(RectTransform rectTransform, float deltaY, CancellationToken stopToken, float duration1 = 0.3f, float duration2 = 0.6f)
    {
        var tcs = new UniTaskCompletionSource();
        void CancelAnimation()
        {
            LeanTween.cancel(rectTransform.gameObject, true);
        }

        using (stopToken.Register(CancelAnimation))
        {
            var startPos = rectTransform.anchoredPosition.y;
            var (task, tweenDescription) = MoveYAsync(rectTransform, rectTransform.anchoredPosition.y + deltaY, duration1);
            tweenDescription.setEaseOutQuad();
            await task;
            if (false == stopToken.IsCancellationRequested)
            {
                (task, tweenDescription) = MoveYAsync(rectTransform, startPos, duration2);
                tweenDescription.setEaseOutBounce();
                await task;
            }
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPos);
        }
        tcs.TrySetResult();
    }

    public static (UniTask task, LTDescr tweenDescription) MoveBounceAsync(RectTransform rectTransform, Vector3 to, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        var tweenDescription = LeanTween.move(rectTransform, to, duration)
            .setEaseOutBounce()
            .setOnComplete(() => tcs.TrySetResult());

        return (tcs.Task, tweenDescription);
    }

    public static (UniTask task, LTDescr tweenDescription) MoveXAsync(RectTransform rectTransform, float to, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        var tweenDescription = LeanTween.moveX(rectTransform, to, duration)
            .setOnComplete(() => tcs.TrySetResult());

        return (tcs.Task, tweenDescription);
    }

    public static (UniTask task, LTDescr tweenDescription) MoveYAsync(RectTransform rectTransform, float to, float duration)
    {
        var tcs = new UniTaskCompletionSource();
        var tweenDescription = LeanTween.moveY(rectTransform, to, duration)
            .setOnComplete(() => tcs.TrySetResult());

        return (tcs.Task, tweenDescription);
    }
}
