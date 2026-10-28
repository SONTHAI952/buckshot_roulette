using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZeroX.RxSystem.Examples
{
    public class TestPerformance : MonoBehaviour
    {

        //Const
        private const int numberRegister = 20000;
        private const int numberInvoke = 1000;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Test();
            }
        }

        private void Test()
        {
            StartCoroutine(Timeline());
            
            
            
            IEnumerator Timeline()
            {
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                TestAction();
                yield return new WaitForSeconds(1);
                TestListAction();
                yield return new WaitForSeconds(1);
                TestListActionClone();
                yield return new WaitForSeconds(1);
                TestSubject();
                yield return new WaitForSeconds(1);
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                yield return null;
            }
        }

        private void DoSomething()
        {
            int a = 2;
            int b = 3;
            int c = a + b;
        }

        public void TestAction()
        {
            Action onDo = null;


            for (int i = 0; i < numberRegister; i++)
            {
                onDo += DoSomething;
            }
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numberInvoke; i++)
            {
                onDo.Invoke();
            }
            
            stopwatch.Stop();
            Debug.Log("Action: " + stopwatch.ElapsedMilliseconds);
        }

        public void TestListAction()
        {
            List<Action> listAction = new List<Action>();
            for (int i = 0; i < numberRegister; i++)
            {
                listAction.Add(DoSomething);
            }
            
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numberInvoke; i++)
            {
                for (int j = 0; j < listAction.Count; j++)
                {
                    listAction[j].Invoke();
                }
            }
            
            stopwatch.Stop();
            Debug.Log("List Action: " + stopwatch.ElapsedMilliseconds);
        }
        
        
        public void TestListActionClone()
        {
            List<Action> listAction = new List<Action>();
            for (int i = 0; i < numberRegister; i++)
            {
                listAction.Add(DoSomething);
            }
            
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numberInvoke; i++)
            {
                var listActionClone = listAction.ToList();
                for (int j = 0; j < listActionClone.Count; j++)
                {
                    listActionClone[j].Invoke();
                }
            }
            
            stopwatch.Stop();
            Debug.Log("List Action Clone: " + stopwatch.ElapsedMilliseconds);
        }
        
        public void TestSubject()
        {
            Subject subject = new Subject();


            for (int i = 0; i < numberRegister; i++)
            {
                subject.Subscribe(DoSomething);
            }
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numberInvoke; i++)
            {
                subject.Emit();
            }
            
            stopwatch.Stop();
            Debug.Log("Subject: " + stopwatch.ElapsedMilliseconds);
        }
    }
}