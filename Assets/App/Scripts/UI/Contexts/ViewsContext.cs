
using System.Collections.Generic;
using UnityEngine;
using App.Infrastructure.Contexts;
using App.Infrastructure.UI;

namespace App.UI.Contexts
{
    [DisallowMultipleComponent]
    public abstract class ViewsContext<TView> : Context, IViewsFetcher where TView : View
    {
        [field: SerializeField] public List<TView> Views { get; private set; } = new List<TView>();

        public void FetchViews()
        {
            Views.Clear();

            int childCount = transform.childCount;

            for (int iChild = 0; iChild < childCount; ++iChild)
            {
                Transform child = transform.GetChild(iChild);
                TView view = child.GetComponent<TView>();

                if (view == null)
                {
                    continue;
                }

                Views.Add(view);
            }
        }

        protected sealed override void SetupServices(out List<object> services)
        {
            services = new List<object>();
            FetchViews();

            int viewsLength = Views.Count;

            for (int i = 0; i < viewsLength; ++i)
            {
                TView view = Views[i];
                services.Add(view);
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            FetchViews();
        }
        #endif
    }
}
