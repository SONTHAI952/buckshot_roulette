using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.RxSystem.Examples
{
    public class TestInitializer : MonoBehaviour
    {
        private Initializer<bool> initializer;


        private void Start()
        {
            initializer = Initializer<bool>.CreateAutoReInit(this, ExecuteOneTimeSetup, ExecuteInit, IsInitResultSuccess, CreateFailedResult);
        }
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                initializer.Initialize().OnFinish(wt =>
                {
                    Debug.Log("Init result: " + wt.Result);
                });
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Debug.Log("Initlaizer State: " + initializer.State);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                initializer.autoReInit = true;
                initializer.Initialize().OnFinish(wt =>
                {
                    Debug.Log("Init result 1: " + wt.Result);

                    initializer.autoReInit = false;
                    initializer.Initialize()
                        .OnFinish(wt =>
                    {
                        Debug.Log("Init result 2: " + wt.Result);
                        initializer.autoReInit = false;
                        initializer.Initialize().OnFinish(wt =>
                        {
                            Debug.LogFormat("NumberStartInit/NumberEndInit: {0}/{1}", initializer.NumberStartInit, initializer.NumberEndInit);
                        });
                        initializer.autoReInit = true;
                    });
                    initializer.autoReInit = true;
                });
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("NumberStartInit: " + initializer.NumberStartInit);
            }
        }

        private void ExecuteOneTimeSetup()
        {
            Debug.Log("ExecuteOneTimeSetup - State: " + initializer.State);
        }

        private int count = 0;
        private int processId = 100;
        private WaitToken<bool> ExecuteInit()
        {
            processId++;
            int pId = processId;
            Debug.Log("Execute Init ProcessId: " + pId);
            WaitToken<bool> wt = new WaitToken<bool>();
            StartCoroutine(Timeline());
            return wt;
            
            
            IEnumerator Timeline()
            {
                yield return new WaitForSecondsRealtime(1);
                count++;
                bool result = count > 5;
                Debug.LogFormat("Init Process -> Set Result '{0}' For ProcessId '{1}'", result, pId);
                wt.SetResult(result);
            }
        }
        
        private bool IsInitResultSuccess(bool result)
        {
            return result;
        }

        private bool CreateFailedResult()
        {
            return false;
        }
    }
}