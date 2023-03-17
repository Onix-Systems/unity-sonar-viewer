
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SketchfabAPI.Entities;
using Utils.UnityCoroutineHelpers;
using App.Infrastructure.UI;
using App.UI.Elements;

namespace App.UI.Screens
{
    public class ModelsListScreenView : ScreenView
    {
        [SerializeField] private BackButton _backButton;
        [SerializeField] private ModelsListItem _listItemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Button _previousPageButton;
        [SerializeField] private Button _nextPageButton;

        public BackButton BackButton => _backButton;

        private List<ModelsListItem> _itemViews;
        private Coroutine _itemsSpawningCo;

        public event Action<string> OnPreviousPageClicked;
        public event Action<string> OnNextPageClicked;
        public event Action<ModelEntity> OnItemClicked;

        protected override void OnAwake()
        {
            _itemViews = new List<ModelsListItem>();
        }

        protected override void OnVisible()
        {
            ClearModelsList();
            SetupPreviousButton(null);
            SetupNextButton(null);
        }

        protected override void OnInvisible()
        {
            Coroutiner.StopCoroutine(_itemsSpawningCo);

            _previousPageButton.onClick.RemoveAllListeners();
            _nextPageButton.onClick.RemoveAllListeners();
            OnPreviousPageClicked = null;
            OnNextPageClicked = null;
            OnItemClicked = null;
        }

        public void SetData(SearchResultsEntity searchResult)
        {
            Coroutiner.StopCoroutine(_itemsSpawningCo);

            ClearModelsList();
            SetupPreviousButton(searchResult);
            SetupNextButton(searchResult);

            ModelEntity[] itemsData = searchResult.Results;

            _itemsSpawningCo = Coroutiner.StartCoroutine(ItemsSpawningCo(itemsData));
        }

        public void ClearModelsList()
        {
            int itemCount = _itemViews.Count;

            for (int i = 0; i < itemCount; ++i)
            {
                Destroy(_itemViews[i].gameObject);
            }

            _itemViews.Clear();
            _content.anchoredPosition = Vector2.zero;
        }

        private void SetupPreviousButton(SearchResultsEntity searchResult)
        {
            _previousPageButton.onClick.RemoveAllListeners();

            if (searchResult == null)
            {
                _previousPageButton.interactable = false;
                return;
            }

            if (searchResult.Previous == null)
            {
                _previousPageButton.interactable = false;
                return;
            }

            _previousPageButton.onClick.AddListener(() =>
            {
                OnPreviousPageClicked?.Invoke(searchResult.Previous);
            });

            _previousPageButton.interactable = true;
        }

        private void SetupNextButton(SearchResultsEntity searchResult)
        {
            _nextPageButton.onClick.RemoveAllListeners();

            if (searchResult == null)
            {
                _nextPageButton.interactable = false;
                return;
            }

            if (searchResult.Next == null)
            {
                _nextPageButton.interactable = false;
                return;
            }

            _nextPageButton.onClick.AddListener(() =>
            {
                OnNextPageClicked?.Invoke(searchResult.Next);
            });

            _nextPageButton.interactable = true;
        }

        private IEnumerator ItemsSpawningCo(ModelEntity[] itemsData)
        {
            int itemCount = itemsData.Length;
            ModelsListItem[] views = new ModelsListItem[itemCount];

            for (int i = 0; i < itemCount; ++i)
            {
                ModelsListItem itemView = Instantiate(_listItemPrefab, _content);
                itemView.SetData(itemsData[i]);
                itemView.OnLoadClicked += (data) => { OnItemClicked?.Invoke(data); };

                _itemViews.Add(itemView);

                if (!itemView.gameObject.activeSelf)
                {
                    itemView.gameObject.SetActive(true);
                }

                yield return null;
            }
        }
    }
}