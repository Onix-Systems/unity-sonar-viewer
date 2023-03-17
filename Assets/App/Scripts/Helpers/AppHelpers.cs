
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using App.Infrastructure.Contexts;
using App.Services.Input;
using App.Services;
using UnityEngine.XR.ARFoundation;

namespace App.Helpers
{
    public static class AppHelpers
    {
        public static Vector2 ScreenCenter { get; private set; }
        public static float MinSwipeDistance { get; private set; }

        static AppHelpers()
        {
            ScreenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        public static float GetSmallestScreenSideSize()
        {
            return Mathf.Min(Screen.width, Screen.height);
        }

        public static bool PositionOverUI(Vector2 position)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static bool TouchOverUI(Touch[] touches)
        {
            foreach (Touch touch in touches)
            {
                if (PositionOverUI(touch.position))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<byte[]> UnZipAsync(byte[] data)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (MemoryStream zippedDataMemoryStream = new MemoryStream(data))
                    {
                        using (ZipArchive zipArchive = new ZipArchive(zippedDataMemoryStream))
                        {
                            ZipArchiveEntry entry = zipArchive.Entries.FirstOrDefault();

                            if (entry == null)
                            {
                                return null;
                            }

                            using (Stream entryStream = entry.Open())
                            {
                                using (MemoryStream zippedEntryMemoryStream = new MemoryStream())
                                {
                                    return zippedEntryMemoryStream.ToArray();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static void CalculateBounds(MeshFilter meshFilter, out Bounds bounds)
        {
            Transform transform = meshFilter.transform;
            UnityEngine.Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            int verticesCount = vertices.Length;

            if (verticesCount < 1)
            {
                bounds = new Bounds(transform.position, Vector3.one);
            }

            Vector3 min;
            Vector3 max;

            min = transform.TransformPoint(vertices[0]);
            max = min;

            for (int i = 1; i < verticesCount; ++i)
            {
                Vector3 vertex = transform.TransformPoint(vertices[i]);

                for (int n = 0; n < 3; ++n)
                {
                    if (vertex[n] > max[n])
                    {
                        max[n] = vertex[n];
                    }

                    if (vertex[n] < min[n])
                    {
                        min[n] = vertex[n];
                    }
                }
            }

            bounds = new Bounds();
            bounds.SetMinMax(min, max);
        }

        public static bool TrySelectGO(Vector2 touchPosition, out SelectedObject selectedGO, Camera camera, LayerMask selectableLayers)
        {
            Ray ray = camera.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit objectHit,
                camera.nearClipPlane + camera.farClipPlane, selectableLayers))
            {

                Vector2 objectCenterScreenPosition = camera.WorldToScreenPoint(objectHit.transform.position);
                selectedGO = new SelectedObject()
                {
                    GameObject = objectHit.transform.gameObject,
                    TouchOffsetToObjectPivot = objectCenterScreenPosition - touchPosition
                };

                return true;
            }

            selectedGO = default;
            return false;
        }
    }
}
