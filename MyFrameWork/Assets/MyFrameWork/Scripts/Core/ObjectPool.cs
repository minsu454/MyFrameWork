using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPool
    {
        public readonly string poolName;                                            //이름
        public readonly Stack<GameObject> objectStack = new Stack<GameObject>();    //스텍
        public readonly GameObject bassPrefab;                                      //기본 프리팹
        public readonly Transform poolTr;                                           //생성 위치

        public ObjectPool(string poolName, GameObject bassPrefab, Transform poolTr, int preloadCount)
        {
            this.poolName = poolName;
            this.bassPrefab = bassPrefab;
            this.poolTr = poolTr;

            Preload(preloadCount);
        }

        /// <summary>
        /// ObjectPool을 생성할 때 해당 값만큼 생성해주는 함수
        /// </summary>
        public bool Preload(int preloadCount)
        {
            for (int i = 0; i < preloadCount; i++)
            {
                GameObject go = CreateImpl();
                go.SetActive(false);
                objectStack.Push(go);
            }

            return true;
        }

        /// <summary>
        /// 오브젝트 생성해주는 함수
        /// </summary>
        public GameObject CreateImpl()
        {
            if (bassPrefab == null)
            {
                Debug.LogError($"ObjectPool - basePrefab is null.");
                return null;
            }

            GameObject newGo = Object.Instantiate(bassPrefab, poolTr);
            newGo.name = poolName;

            return newGo;
        }

        /// <summary>
        /// 오브젝트 내보내주는 함수
        /// </summary>
        public GameObject GetObject()
        {
            GameObject go;

            if (objectStack.Count == 0)
            {
                go = CreateImpl();
            }
            else
            {
                go = objectStack.Pop();
            }

            return go;
        }

        /// <summary>
        /// 오브젝트 스택에 다시 넣어주는 함수
        /// </summary>
        public void ReturnObject(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            go.transform.parent = poolTr;
            objectStack.Push(go);
        }
    }
}
