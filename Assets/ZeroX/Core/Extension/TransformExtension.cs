using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZeroX.Extensions
{
    public static class TransformExtension
    {
        public static void DestroyAllChild(this Transform trans)
        {
            foreach (Transform child in trans)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void DestroyAllChildImmediate(this Transform trans)
        {
            List<GameObject> listChild = new List<GameObject>();
            foreach (Transform child in trans)
            {
                listChild.Add(child.gameObject);
            }

            foreach (var child in listChild)
            {
                GameObject.DestroyImmediate(child);
            }
        }

        public static void SetActiveAllChild(this Transform trans, bool value)
        {
            foreach (Transform child in trans)
            {
                child.gameObject.SetActive(value);
            }
        }

        public static void SetActiveChild(this Transform trans, bool value, int number)
        {
            foreach (Transform child in trans)
            {
                child.gameObject.SetActive(value);
                number--;
                if (number == 0)
                    return;
            }
        }

        public static void SetLocalScaleAllChild(this Transform trans, Vector3 value)
        {
            foreach (Transform child in trans)
            {
                child.localScale = value;
            }
        }

        public static void RandomChildSiblingIndex(this Transform trans)
        {
            int childCount = trans.childCount;
            foreach (Transform child in trans)
            {
                child.SetSiblingIndex(Random.Range(0, childCount));
            }
        }

        public static int CountChildActiveSelf(this Transform trans)
        {
            var l = trans.Cast<Transform>();
            return l.Count(t => t.gameObject.activeSelf);
        }

        public static void FlipX(this Transform trans)
        {
            Vector3 right = trans.right;
            right.x = -right.x;
            trans.right = right;
        }

        public static void FlipY(this Transform trans)
        {
            Vector3 up = trans.up;
            up.x = -up.x;
            trans.right = up;
        }

        public static bool IsXLookAt(this Transform trans, Transform target)
        {
            Vector2 directToTarget = target.position - trans.position;
            return trans.right.x * directToTarget.x >= 0;
        }

        public static bool IsXLookAt(this Transform trans, GameObject target)
        {
            return IsXLookAt(trans, target.transform);
        }


        public static void SetXLookAt(this Transform trans, Transform target)
        {
            Vector2 direct = target.position - trans.position;
            direct.y = trans.right.y;
            trans.right = direct;
        }

        public static void SetXLookAt(this Transform trans, GameObject target)
        {
            SetXLookAt(trans, target.transform);
        }

        public static void RightLookAt2D(this Transform transform, Vector2 direction)
        {
            float angelZ = Vector2.SignedAngle(Vector3.right, direction);
            float angelX = 0f;
            if (transform.right.x < 0)
            {
                angelZ = -angelZ;
                angelX = 180f;
            }

            transform.localRotation = Quaternion.Euler(angelX, 0f, angelZ);
        }

        public static void Foreach(this Transform transform, Action<int, Transform> action)
        {
            int index = 0;
            foreach (Transform child in transform)
            {
                action(index, child);
                index++;
            }
        }

        #region Set Scale One

        public static void SetLocalScaleX(this Transform transform, float scaleX)
        {
            Vector3 scale = transform.localScale;
            scale.x = scaleX;
            transform.localScale = scale;
        }

        public static void SetLocalScaleY(this Transform transform, float scaleY)
        {
            Vector3 scale = transform.localScale;
            scale.y = scaleY;
            transform.localScale = scale;
        }

        public static void SetLocalScaleZ(this Transform transform, float scaleZ)
        {
            Vector3 scale = transform.localScale;
            scale.z = scaleZ;
            transform.localScale = scale;
        }


        #endregion

        #region Set Position One

        public static void SetPositionX(this Transform transform, float posX)
        {
            Vector3 position = transform.position;
            position.x = posX;
            transform.position = position;
        }

        public static void SetPositionY(this Transform transform, float posY)
        {
            Vector3 position = transform.position;
            position.y = posY;
            transform.position = position;
        }

        public static void SetPositionZ(this Transform transform, float posZ)
        {
            Vector3 position = transform.position;
            position.z = posZ;
            transform.position = position;
        }

        public static void SetLocalPositionX(this Transform transform, float posX)
        {
            Vector3 position = transform.localPosition;
            position.x = posX;
            transform.localPosition = position;
        }

        public static void SetLocalPositionY(this Transform transform, float posY)
        {
            Vector3 position = transform.localPosition;
            position.y = posY;
            transform.localPosition = position;
        }

        public static void SetLocalPositionZ(this Transform transform, float posZ)
        {
            Vector3 position = transform.localPosition;
            position.z = posZ;
            transform.localPosition = position;
        }

        #endregion

        #region Set Position Two

        public static void SetPositionXY(this Transform transform, float posX, float posY)
        {
            Vector3 position = transform.position;
            position.x = posX;
            position.y = posY;
            transform.position = position;
        }

        public static void SetPositionXZ(this Transform transform, float posX, float posZ)
        {
            Vector3 position = transform.position;
            position.x = posX;
            position.z = posZ;
            transform.position = position;
        }

        public static void SetPositionYZ(this Transform transform, float posY, float posZ)
        {
            Vector3 position = transform.position;
            position.y = posY;
            position.z = posZ;
            transform.position = position;
        }

        public static void SetLocalPositionXY(this Transform transform, float posX, float posY)
        {
            Vector3 position = transform.position;
            position.x = posX;
            position.y = posY;
            transform.localPosition = position;
        }

        public static void SetLocalPositionXZ(this Transform transform, float posX, float posZ)
        {
            Vector3 position = transform.position;
            position.x = posX;
            position.z = posZ;
            transform.localPosition = position;
        }

        public static void SetLocalPositionYZ(this Transform transform, float posY, float posZ)
        {
            Vector3 position = transform.position;
            position.y = posY;
            position.z = posZ;
            transform.localPosition = position;
        }

        #endregion

        #region Find

        public static void GetAllChildRecursive(this Transform parent, bool includeDeActive,
            List<GameObject> listResult)
        {
            foreach (Transform child in parent)
            {
                if (includeDeActive == false)
                    if (child.gameObject.activeSelf == false)
                        continue;

                listResult.Add(child.gameObject);

                GetAllChildRecursive(child, includeDeActive, listResult);
            }
        }

        public static GameObject FindChild_StartWithName(this Transform parent, string childName, bool includeDeActive)
        {
            foreach (Transform child in parent)
            {
                if (includeDeActive == false)
                    if (child.gameObject.activeSelf == false)
                        continue;

                if (child.name.StartsWith(childName) == false)
                    continue;

                return child.gameObject;
            }

            return null;
        }

        public static List<GameObject> FindAllChild_StartWithName(this Transform parent, string childName,
            bool includeDeActive)
        {
            List<GameObject> listChild = new List<GameObject>();
            foreach (Transform child in parent)
            {
                if (includeDeActive == false)
                    if (child.gameObject.activeSelf == false)
                        continue;

                if (child.name.StartsWith(childName) == false)
                    continue;

                listChild.Add(child.gameObject);
            }

            return listChild;
        }

        public static GameObject FindChildRecursive_StartWithName(this Transform parent, string childName,
            bool includeDeActive)
        {
            foreach (Transform child in parent)
            {
                if (includeDeActive == false)
                    if (child.gameObject.activeSelf == false)
                        continue;

                if (child.name.StartsWith(childName))
                {
                    return child.gameObject;
                }

                var result = FindChildRecursive_StartWithName(child, childName, includeDeActive);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static void FindAllChildRecursive_StartWithName(this Transform parent, string childName,
            bool includeDeActive, List<GameObject> listResult)
        {
            foreach (Transform child in parent)
            {
                if (includeDeActive == false)
                    if (child.gameObject.activeSelf == false)
                        continue;

                if (child.name.StartsWith(childName))
                {
                    listResult.Add(child.gameObject);
                }

                FindAllChildRecursive_StartWithName(child, childName, includeDeActive, listResult);
            }
        }

        /// <summary>
        /// path has the form A/B/C
        /// </summary>
        public static Transform FindChildByPath(this Transform parent, string path)
        {
            string[] names = path.Split('/');

            Transform result = parent;
            foreach (var n in names)
            {
                result = result.Find(n);
                if (result == null)
                    return null;
            }

            return result;
        }

        /// <summary>
        /// path has the form A/b/C
        /// </summary>
        public static Transform FindChildByPath_IgnoreCase(this Transform parent, string path)
        {
            string[] names = path.Split('/');

            Transform result = parent;
            foreach (var n in names)
            {
                foreach (Transform child in result)
                {
                    if (string.Equals(child.name, n, StringComparison.OrdinalIgnoreCase))
                    {
                        result = child;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Match Point

        public static void SnapPoint_LocalPointA_To_WorldPointB(this Transform transformA, Vector3 localPointA,
            Vector3 worldPointB)
        {
            Vector3 worldPointA = transformA.TransformPoint(localPointA);
            transformA.position += (worldPointB - worldPointA);
        }

        public static void SnapPoint_WorldPointA_To_WorldPointB(this Transform transformA, Vector3 worldPointA,
            Vector3 worldPointB)
        {
            transformA.position += (worldPointB - worldPointA);
        }


        public static void SnapPoint_LocalPointA_To_LocalPointB(this Transform transformA, Vector3 localPointA,
            Transform transformB, Vector3 localPointB)
        {
            Vector3 worldPointA = transformA.TransformPoint(localPointA);
            Vector3 worldPointB = transformB.TransformPoint(localPointB);
            transformA.position += (worldPointB - worldPointA);
        }





        public static void SnapPoint_PointA_To_WorldPointB(this Transform transformA, Transform pointA,
            Vector3 worldPointB)
        {
            transformA.position += (worldPointB - pointA.position);
        }

        public static void SnapPoint_PointA_To_LocalPointB(this Transform transformA, Transform pointA,
            Transform transformB, Vector3 localPointB)
        {
            Vector3 worldPointB = transformB.TransformPoint(localPointB);
            transformA.position += (worldPointB - pointA.position);
        }

        public static void SnapPoint_PointA_To_PointB(this Transform transformA, Transform pointA, Transform pointB)
        {
            transformA.position += (pointB.position - pointA.position);
        }

        #endregion

        #region Match Rotation

        public static void SnapRotation_PointA_To_PointB(this Transform transform, Transform pointA, Transform pointB)
        {
            Quaternion rotOffset = QuaternionExtension.CalculateOffset(pointA.rotation, pointB.rotation);
            transform.rotation = transform.rotation * rotOffset;
        }

        #endregion

        public static void SetLossyScale(this Transform transform, Vector3 lossyScale)
        {
            Vector3 currentLossyScale = transform.lossyScale;
            Vector3 currentLocalScale = transform.localScale;
            Vector3 newLocalScale = Vector3.one;

            //X
            if (Mathf.Approximately(currentLossyScale.x, 0) == false)
            {
                newLocalScale.x = currentLocalScale.x * (lossyScale.x / currentLossyScale.x);
            }
            else
            {
                //Cần phải thử set localScale.x của target về 1 để xem có phải do localScale.x của target == 0 khiến lossyScale.x bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.x = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentLossyScale = transform.lossyScale;
                if (Mathf.Approximately(tempCurrentLossyScale.x,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.x = currentLocalScale.x;
                }
                else //Nếu do chính localScale.x của target là 0
                {
                    newLocalScale.x = tempCurrentLocalScale.x * (lossyScale.x / tempCurrentLossyScale.x);
                }
            }

            //Y
            if (Mathf.Approximately(currentLossyScale.y, 0) == false)
            {
                newLocalScale.y = currentLocalScale.y * (lossyScale.y / currentLossyScale.y);
            }
            else
            {
                //Cần phải thử set localScale.y của target về 1 để xem có phải do localScale.y của target == 0 khiến lossyScale.y bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.y = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentLossyScale = transform.lossyScale;
                if (Mathf.Approximately(tempCurrentLossyScale.y,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.y = currentLocalScale.y;
                }
                else //Nếu do chính localScale.y của target là 0
                {
                    newLocalScale.y = tempCurrentLocalScale.y * (lossyScale.y / tempCurrentLossyScale.y);
                }
            }


            //Z
            if (Mathf.Approximately(currentLossyScale.z, 0) == false)
            {
                newLocalScale.z = currentLocalScale.z * (lossyScale.z / currentLossyScale.z);
            }
            else
            {
                //Cần phải thử set localScale.z của target về 1 để xem có phải do localScale.z của target == 0 khiến lossyScale.z bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.z = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentLossyScale = transform.lossyScale;
                if (Mathf.Approximately(tempCurrentLossyScale.z,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.z = currentLocalScale.z;
                }
                else //Nếu do chính localScale.z của target là 0
                {
                    newLocalScale.z = tempCurrentLocalScale.z * (lossyScale.z / tempCurrentLossyScale.z);
                }
            }


            transform.localScale = newLocalScale;
        }

        

        public static Vector3 GetScaleInTransformSpace(this Transform transform, Transform transformSpace)
        {
            Vector3 scaleInTransform = transform.localScale;
            Transform parent = transform;
            do
            {
                parent = parent.parent;
                if (parent == null)
                    throw new Exception("Parent of scale is not parent of transform");

                if (parent == transformSpace)
                    break;

                Vector3 parent_LocalScale = parent.localScale;
                scaleInTransform.x *= parent_LocalScale.x;
                scaleInTransform.y *= parent_LocalScale.y;
                scaleInTransform.z *= parent_LocalScale.z;

            } while (true);

            return scaleInTransform;
        }

        public static void SetScaleInTransformSpace(this Transform transform, Transform transformSpace, Vector3 scaleInTransformSpace)
        {
            Vector3 currentScaleInTransformSpace = GetScaleInTransformSpace(transform, transformSpace);
            Vector3 currentLocalScale = transform.localScale;
            Vector3 newLocalScale = Vector3.one;

            //X
            if (Mathf.Approximately(currentScaleInTransformSpace.x, 0) == false)
            {
                newLocalScale.x = currentLocalScale.x * (scaleInTransformSpace.x / currentScaleInTransformSpace.x);
            }
            else
            {
                //Cần phải thử set localScale.x của target về 1 để xem có phải do localScale.x của target == 0 khiến lossyScale.x bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.x = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentScaleInTransformSpace = GetScaleInTransformSpace(transform, transformSpace);
                if (Mathf.Approximately(tempCurrentScaleInTransformSpace.x,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.x = currentLocalScale.x;
                }
                else //Nếu do chính localScale.x của target là 0
                {
                    newLocalScale.x = tempCurrentLocalScale.x *
                                      (scaleInTransformSpace.x / tempCurrentScaleInTransformSpace.x);
                }
            }


            //Y
            if (Mathf.Approximately(currentScaleInTransformSpace.y, 0) == false)
            {
                newLocalScale.y = currentLocalScale.y * (scaleInTransformSpace.y / currentScaleInTransformSpace.y);
            }
            else
            {
                //Cần phải thử set localScale.y của target về 1 để xem có phải do localScale.y của target == 0 khiến lossyScale.y bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.y = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentScaleInTransformSpace = GetScaleInTransformSpace(transform, transformSpace);
                if (Mathf.Approximately(tempCurrentScaleInTransformSpace.y,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.y = currentLocalScale.y;
                }
                else //Nếu do chính localScale.y của target là 0
                {
                    newLocalScale.y = tempCurrentLocalScale.y *
                                      (scaleInTransformSpace.y / tempCurrentScaleInTransformSpace.y);
                }
            }


            //Z
            if (Mathf.Approximately(currentScaleInTransformSpace.z, 0) == false)
            {
                newLocalScale.z = currentLocalScale.z * (scaleInTransformSpace.z / currentScaleInTransformSpace.z);
            }
            else
            {
                //Cần phải thử set localScale.z của target về 1 để xem có phải do localScale.z của target == 0 khiến lossyScale.z bằng 0 hay không
                Vector3 tempCurrentLocalScale = currentLocalScale;
                tempCurrentLocalScale.z = 1;
                transform.localScale = tempCurrentLocalScale;

                Vector3 tempCurrentScaleInTransformSpace = GetScaleInTransformSpace(transform, transformSpace);
                if (Mathf.Approximately(tempCurrentScaleInTransformSpace.z,
                        0)) //Nếu lossyScale vẫn bằng 0 thì ko thể set lossyScale mới được
                {
                    newLocalScale.z = currentLocalScale.z;
                }
                else //Nếu do chính localScale.z của target là 0
                {
                    newLocalScale.z = tempCurrentLocalScale.z *
                                      (scaleInTransformSpace.z / tempCurrentScaleInTransformSpace.z);
                }
            }


            transform.localScale = newLocalScale;
        }
        
        public static Bounds TransformBounds(this Transform transform, Bounds localBounds)
        {
            Vector3 worldBoundCenter = transform.TransformPoint(localBounds.center);
            
            Vector3 worldBoundMin = transform.TransformPoint(localBounds.min);
            Vector3 worldBoundMax = transform.TransformPoint(localBounds.max);
            float sizeX = Mathf.Abs(worldBoundMax.x - worldBoundMin.x);
            float sizeY = Mathf.Abs(worldBoundMax.y - worldBoundMin.y);
            float sizeZ = Mathf.Abs(worldBoundMax.z - worldBoundMin.z);
            
            return new Bounds(worldBoundCenter, new Vector3(sizeX, sizeY, sizeZ));
        }

        public static Bounds InverseTransformBounds(this Transform transform, Bounds worldBounds)
        {
            Vector3 localBoundCenter = transform.InverseTransformPoint(worldBounds.center);
            
            Vector3 localBoundMin = transform.InverseTransformPoint(worldBounds.min);
            Vector3 localBoundMax = transform.InverseTransformPoint(worldBounds.max);
            float sizeX = Mathf.Abs(localBoundMax.x - localBoundMin.x);
            float sizeY = Mathf.Abs(localBoundMax.y - localBoundMin.y);
            float sizeZ = Mathf.Abs(localBoundMax.z - localBoundMin.z);
            
            return new Bounds(localBoundCenter, new Vector3(sizeX, sizeY, sizeZ));
        }
    }
}