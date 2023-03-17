
using UnityEngine;

namespace App.Infrastructure.UI
{
    [DisallowMultipleComponent]
    public abstract class View: MonoBehaviour
    {
        [field: SerializeField] public bool InvisibleOnAwake { get; set; } = false;

        protected void Awake()
        {
            gameObject.SetActive(!InvisibleOnAwake);

            OnAwake();
        }

        protected void OnEnable() => OnVisible();
        protected void OnDisable() => OnInvisible();
        protected void Update() => OnUpdate();
        protected void LateUpdate() => OnLateUpdate();
        protected void OnDestroy() => OnDispose();

        #if UNITY_EDITOR
        private void OnValidate()
        {
            Transform parent = transform.parent;

            if (!parent)
            {
                return;
            }

            IViewsFetcher viewsFetcher = parent.GetComponent<IViewsFetcher>();

            if (viewsFetcher == null)
            {
                return;
            }

            viewsFetcher.FetchViews();
        }
        #endif

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnAwake() { }
        protected virtual void OnVisible() { }
        protected virtual void OnInvisible() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnDispose() { }
    }
}
