
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.UnityCoroutineHelpers
{
    public static class Coroutines
    {
        public static Coroutine InvokeAfter(float seconds, Action callback)
        {
            return Coroutiner.StartCoroutine(InvokeAfterCo(seconds, callback));
        }

        public static Coroutine WaitForTaskCompletion(Task task, Action onCompleted)
        {
            return Coroutiner.StartCoroutine(WaitForTaskCompletedCo(task, onCompleted));
        }

        public static Coroutine WaitForTaskCompletion<T>(Task<T> task, Action<T> onCompleted)
        {
            return Coroutiner.StartCoroutine(WaitForTaskCompletedCo(task, onCompleted));
        }

        public static IEnumerator MovingCo(
            Transform targetTransform, Vector3 targetPosition, 
            float smooth, Action onCompleted = null)
        {
            float time = 0f;
            Vector3 startPosition = targetTransform.position;

            while (targetPosition != targetTransform.position)
            {
                time += Time.deltaTime;
                targetTransform.position = Vector3.Slerp(startPosition, targetPosition, time * smooth);
                yield return null;
            }

            onCompleted?.Invoke();
        }

        public static IEnumerator RotatingCo(
            Transform targetTransform, Quaternion targetRotation, 
            float smooth, Action onCompleted = null)
        {
            float time = 0f;
            Quaternion startRotation = targetTransform.rotation;

            while (targetRotation != targetTransform.rotation)
            {
                time += Time.deltaTime;
                targetTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, time * smooth);
                yield return null;
            }

            onCompleted?.Invoke();
        }

        public static IEnumerator ScalingCo(
            Transform targetTransform, Vector3 targetScale, 
            float smooth, Action onCompleted = null)
        {
            float time = 0f;
            Vector3 startScale = targetTransform.localScale;

            while (targetScale != targetTransform.localScale)
            {
                time += Time.deltaTime;
                targetTransform.localScale = Vector3.Slerp(startScale, targetScale, time * smooth);
                yield return null;
            }

            onCompleted?.Invoke();
        }

        public static IEnumerator LocalRotatingCo(
            Transform targetTransform, Quaternion targetRotation, 
            float smooth, Action onCompleted = null)
        {
            float time = 0f;
            Quaternion startRotation = targetTransform.localRotation;

            while (targetRotation != targetTransform.localRotation)
            {
                time += Time.deltaTime;
                targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, time * smooth);
                yield return null;
            }

            onCompleted?.Invoke();
        }

        private static IEnumerator InvokeAfterCo(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        private static IEnumerator WaitForTaskCompletedCo(Task task, Action onCompleted)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            onCompleted?.Invoke();
        }

        private static IEnumerator WaitForTaskCompletedCo<T>(Task<T> task, Action<T> onCompleted)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            onCompleted?.Invoke(task.Result);
        }
    }
}
