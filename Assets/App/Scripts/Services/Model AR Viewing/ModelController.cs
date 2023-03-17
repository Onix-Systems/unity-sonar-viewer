
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils.UnityCoroutineHelpers;
using App.Infrastructure.Contexts;

namespace App.Services.ModelARViewing
{
    public class ModelController: MonoBehaviour
    {
        [SerializeField] private Transform _root;

        public ModelObject Model { get; private set; }

        private ARAnchor _arPlaneAnchor;
        private Coroutine _rootMoveCo;
        private Coroutine _rootRotationCo;
        private Coroutine _rootScaleCo;
        private Coroutine _modelRotationCo;
        private Transform _modelTransform;

        public Vector3 RootPosition => _root.position;
        public Quaternion RootRotation => _root.rotation;
        public float RootScale => _root.localScale.x;

        private void Start()
        {
            SetModelVisible(false);
        }

        public void SetModel(ModelObject model)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (Model != null && Model != model)
            {
                Model.Destroy();
            }

            Model = model;
            model.Rig.layer = config.ModelLayer.LayerIndex;

            _modelTransform = model.Rig.transform;
            Transform modelTransform = model.Rig.transform;
            modelTransform.SetParent(_root);
            ResetModelTransform();

            BoxCollider boxCollider = modelTransform.GetComponent<BoxCollider>();

            if (!boxCollider)
            {
                boxCollider = modelTransform.gameObject.AddComponent<BoxCollider>();
            }

            Bounds bounds = model.Bounds;
            boxCollider.center = bounds.center;
            boxCollider.size = bounds.size;

            AdjustModelScale(model);
        }

        public void RemoveModel()
        {
            StopCoroutines();
            DetachModelFromARPlane();

            if (Model != null)
            {
                Model.Destroy();
            }

            Model = null;
            _modelTransform = null;
        }

        public void AttachModelToARPlane(ARPlane arPlane)
        {
            if (_arPlaneAnchor == null)
            {
                return;
            }    

            IContext mainContext = MainContext.Instance;
            ARAnchorManager arAnchorManager = mainContext.Get<ARAnchorManager>();

            if (IsModelAttachedToARPlane())
            {
                DetachModelFromARPlane();
            }

            Pose pose = new Pose(transform.position, transform.rotation);
            _arPlaneAnchor = arAnchorManager.AttachAnchor(arPlane, pose);
            _arPlaneAnchor.destroyOnRemoval = false;
        }

        public void DetachModelFromARPlane()
        {
            if (!IsModelAttachedToARPlane())
            {
                return;
            }

            Destroy(_arPlaneAnchor);
            _arPlaneAnchor = null;
        }

        public bool IsModelAttachedToARPlane() => _arPlaneAnchor != null;

        public void SetRootPosition(Vector3 position, bool smoothed = false, Action onCompleted = null)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (_rootMoveCo != null)
            {
                Coroutiner.StopCoroutine(_rootMoveCo);
            }

            if (!smoothed)
            {
                _root.position = position;
            }

            _rootMoveCo = Coroutiner.StartCoroutine(
                Coroutines.MovingCo(_root, position, config.ModelMoveSmooth, onCompleted));
        }

        public void SetRootRotation(Quaternion rotation, bool smoothed = false, Action onCompleted = null)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (_rootRotationCo != null)
            {
                Coroutiner.StopCoroutine(_rootRotationCo);
            }

            if (!smoothed)
            {
                _root.rotation = rotation;
            }

            _rootRotationCo = Coroutiner.StartCoroutine(
                Coroutines.RotatingCo(_root, rotation, config.ModelRotationSmooth, onCompleted));
        }

        public void SetRootScale(float scale, bool smoothed = false, Action onCompleted = null)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (_rootScaleCo != null)
            {
                Coroutiner.StopCoroutine(_rootScaleCo);
            }

            if (scale < config.ModelMinimumScale)
            {
                scale = config.ModelMinimumScale;
            }

            if (scale > config.ModelMaximumScale)
            {
                scale = config.ModelMaximumScale;
            }

            Vector3 scaleVector = new Vector3(scale, scale, scale);

            if (!smoothed)
            {
                _root.localScale = scaleVector;
            }

            _rootScaleCo = Coroutiner.StartCoroutine(
                Coroutines.ScalingCo(_root, scaleVector, config.ModelScaleSmooth, onCompleted));
        }

        public void SetModelRotation(Quaternion rotation, bool smoothed = false, Action onCompleted = null)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (_modelRotationCo != null)
            {
                Coroutiner.StopCoroutine(_modelRotationCo);
            }

            if (!smoothed)
            {
                _modelTransform.localRotation = rotation;
            }

            _modelRotationCo = Coroutiner.StartCoroutine(
                Coroutines.LocalRotatingCo(_modelTransform, rotation, config.ModelRotationSmooth, onCompleted));
        }

        public void SetModelVisible(bool visible)
        {
            if (_root.gameObject.activeSelf != visible)
            {
                _root.gameObject.SetActive(visible);
            }

            if (Model == null)
            {
                return;
            }

            if (Model.IsModelGameObjectActive() != visible)
            {
                Model.SetModelGameObjectActive(visible);
            }
        }

        private void StopCoroutines()
        {
            if (_rootMoveCo != null)
            {
                Coroutiner.StopCoroutine(_rootMoveCo);
            }

            if (_rootRotationCo != null)
            {
                Coroutiner.StopCoroutine(_rootRotationCo);
            }

            if (_rootScaleCo != null)
            {
                Coroutiner.StopCoroutine(_rootScaleCo);
            }

            if (_modelRotationCo != null)
            {
                Coroutiner.StopCoroutine(_modelRotationCo);
            }
        }

        private void ResetModelTransform()
        {
            _modelTransform.localPosition = Vector3.zero;
            _modelTransform.localRotation = Quaternion.identity;
            _modelTransform.localScale = Vector3.one;
        }

        private void AdjustModelScale(ModelObject model)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            Bounds bounds = model.Bounds;

            float x = bounds.size.x;
            float y = bounds.size.y;
            float z = bounds.size.z;

            float maxSize = Mathf.Max(x, y, z);
            float scale = config.ModelSpawnSize / maxSize;

            _modelTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}