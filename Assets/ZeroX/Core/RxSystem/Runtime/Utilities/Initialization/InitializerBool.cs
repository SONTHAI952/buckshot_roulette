using UnityEngine;
using System;

namespace ZeroX.RxSystem
{
    public class InitializerBool : Initializer<bool>
    {



        #region Constructor

        public InitializerBool()
        {
            isResultSuccess = Func_IsResultSuccess;
            createFailedResult = Func_CreateFailedResult;
        }

        

        public static InitializerBool Create(
            MonoBehaviour owner,
            bool autoReInit,
            Action executeOneTimeSetup,
            Func<WaitToken<bool>> executeInitProcess)
        {
            InitializerBool initializer = new InitializerBool();
            initializer.owner = owner;
            initializer.autoReInit = autoReInit;
            
            initializer.executeOneTimeSetup = executeOneTimeSetup;
            initializer.executeInitProcess = executeInitProcess;
            
            return initializer;
        }
        
        public static InitializerBool CreateNoAutoReInit(
            MonoBehaviour owner,
            Action executeOneTimeSetup,
            Func<WaitToken<bool>> executeInitProcess)
        {

            var initializer = Create(owner, false, executeOneTimeSetup, executeInitProcess);
            return initializer;
        }
        
        public static InitializerBool CreateNoAutoReInit(
            MonoBehaviour owner,
            Func<WaitToken<bool>> executeInitProcess)
        {

            var initializer = Create(owner, false, null, executeInitProcess);
            return initializer;
        }
        
        public static InitializerBool CreateAutoReInit(
            MonoBehaviour owner,
            Action executeOneTimeSetup,
            Func<WaitToken<bool>> executeInitProcess)
        {

            var initializer = Create(owner, true, executeOneTimeSetup, executeInitProcess);
            return initializer;
        }
        
        public static InitializerBool CreateAutoReInit(
            MonoBehaviour owner,
            Func<WaitToken<bool>> executeInitProcess)
        {

            var initializer = Create(owner, true, null, executeInitProcess);
            return initializer;
        }
        
        #endregion
        
        
        
        private bool Func_IsResultSuccess(bool result)
        {
            return result;
        }
        
        private bool Func_CreateFailedResult()
        {
            return false;
        }
    }
}