
using App.Helpers;
using UnityEngine;

namespace App.Services.ModelARViewing
{
    public class Model
    {
        public GameObject Rig { get; private set; }
        public Bounds Bounds { get; private set; }

        private GameObject _modelOriginGO;

        public Model(GameObject modelGameObject, bool isActive = false)
        {
            _modelOriginGO = modelGameObject;
            Rig = new GameObject(_modelOriginGO.name + " - Rig");
            Rig.transform.position = Vector3.zero;
            Rig.transform.localRotation = Quaternion.identity;
            Rig.transform.localScale = Vector3.one;
            FixRotation(_modelOriginGO.transform);

            Bounds = GetBounds();
            Vector3 targetPivot = new Vector3(Bounds.center.x, Bounds.min.y, Bounds.center.z);
            Vector3 offset = _modelOriginGO.transform.position - targetPivot;
            
            _modelOriginGO.transform.SetParent(Rig.transform, false);
            _modelOriginGO.transform.localPosition = offset;
            _modelOriginGO.transform.localScale = Vector3.one;
            Bounds = GetBounds();

            SetModelGameObjectActive(isActive);
        }

        private void FixRotation(Transform importedModelTransform)
        {
            importedModelTransform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        private Bounds GetBounds()
        {
            Bounds bounds;

            MeshFilter[] meshFilters = _modelOriginGO.GetComponentsInChildren<MeshFilter>();
            int meshFiltersCount = meshFilters.Length;

            if (meshFiltersCount < 1)
            {
                bounds = new Bounds(_modelOriginGO.transform.position, Vector3.one);
                return bounds;
            }

            AppHelpers.CalculateBounds(meshFilters[0], out bounds);

            for (int i = 1; i < meshFiltersCount; ++i)
            {
                MeshFilter meshFilter = meshFilters[i];
                Bounds meshBounds;
                AppHelpers.CalculateBounds(meshFilter, out meshBounds);
                bounds.Encapsulate(meshBounds);
            }

            return bounds;
        }

        public bool IsModelGameObjectActive()
        {
            return Rig.activeSelf;
        }

        public void SetModelGameObjectActive(bool isActive)
        {
            Rig.SetActive(isActive);
        }

        public void Destroy()
        {
            if (Rig)
            {
                Object.Destroy(Rig);
                Rig = null;
            }
        }
    }
}