
using System.Collections;
using UnityEngine;

namespace Utils.UnityCoroutineHelpers
{
    public class Coroutiner: MonoBehaviour
    {
        public static Coroutiner Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public new static Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (Instance == null || enumerator == null)
            {
                return null;
            }

            return Instance.StartLocalCoroutine(enumerator);
        }

        public new static void StopCoroutine(IEnumerator enumerator)
        {
            if (Instance == null || enumerator == null)
            {
                return;
            }

            Instance.StopLocalCoroutine(enumerator);
        }

        public new static void StopCoroutine(Coroutine coroutine)
        {
            if (Instance == null || coroutine == null)
            {
                return;
            }

            Instance.StopLocalCoroutine(coroutine);
        }

        public static void StopAll()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.StopAllLocalCoroutines();
        }

        private Coroutine StartLocalCoroutine(IEnumerator enumerator)
        {
            return ((MonoBehaviour) Instance).StartCoroutine(enumerator);
        }

        private void StopLocalCoroutine(IEnumerator enumerator)
        {
            ((MonoBehaviour) Instance).StopCoroutine(enumerator);
        }

        private void StopLocalCoroutine(Coroutine coroutine)
        {
            ((MonoBehaviour) Instance).StopCoroutine(coroutine);
        }

        private void StopAllLocalCoroutines()
        {
            Instance.StopAllCoroutines();
        }
    }
}