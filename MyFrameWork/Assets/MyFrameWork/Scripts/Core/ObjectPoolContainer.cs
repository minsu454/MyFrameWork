using Common.Assets;
using Common.Objects;
using Common.Path;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Pool
{ 
    public sealed class ObjectPoolContainer : MonoBehaviour
    {
        private static ObjectPoolContainer instance;
        public static ObjectPoolContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("----------ObjectPool----------");
                    instance = obj.AddComponent<ObjectPoolContainer>();
                }
                return instance;
            }
        }

        public Dictionary<string, ObjectPool> objectPoolDict = new Dictionary<string, ObjectPool>();     //오브젝트풀들 담는 Dictionary

        /// <summary>
        /// 오브젝트풀에 오브젝트 생성해주는 함수
        /// </summary>
        public void CreateObjectPool(string sceneName, string poolName, int preloadCount, Transform poolTr = null)
        {
            if (objectPoolDict.ContainsKey(poolName))
            {
                Debug.LogError($"Object pool {poolName} is already exists.");
                return;
            }

            GameObject go = ObjectManager.Return<GameObject>(AddressablePath.ObjectPoolPath(sceneName, poolName));

            if (go == null)
            {
                Debug.LogError($"Addressable is Not Found GameObject : {poolName}");
                return;
            }

            ObjectPool pool;
            pool = new ObjectPool(poolName, go, poolTr, preloadCount);

            objectPoolDict.Add(poolName, pool);
        }

        /// <summary>
        /// 오브젝트풀에서 오브젝트 가져오는 함수
        /// </summary>
        public GameObject Pop(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                return null;
            }

            if (!objectPoolDict.TryGetValue(poolName, out ObjectPool pool))
            {
                return null;
            }

            return pool.GetObject();
        }
        
        /// <summary>
        /// 오브젝트풀에 반환해주는 함수
        /// </summary>
        public void Return(GameObject obj)
        {
            string poolName = obj.name;

            if (!objectPoolDict.TryGetValue(poolName, out ObjectPool pool))
            {
                Debug.LogError($"Cannot found pool Name: {poolName}");
                return;
            }

            obj.gameObject.SetActive(false);
            pool.objectStack.Push(obj);
        }

        public void Clear()
        {
            objectPoolDict.Clear();
            instance = null;
        }

        private void OnDestroy()
        {
            Clear();
        }
    }

    public static class Utility {
        public static void Return(this GameObject obj) { 
            ObjectPoolContainer.Instance.Return(obj);
        }
    }
}
