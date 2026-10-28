using System;
using System.Collections;
using UnityEngine;

namespace ZeroX.RxSystem
{
    public class Initializer<TResult>
    {
        //Setting
        public MonoBehaviour owner;
        public bool autoReInit;
        public int reInitPerSeconds = 5;

        
        
        //Action - Func
        public Action executeOneTimeSetup;
        public Func<WaitToken<TResult>> executeInitProcess;
        public Func<TResult, bool> isResultSuccess;
        public Func<TResult> createFailedResult;

        
        
        //Properties
        public InitState State => state;
        public bool IsInitialized => state == InitState.Success;
        public bool IsExecuting => state == InitState.Executing;
        public int NumberStartInit => numberStartInit;
        public int NumberEndInit => numberEndInit;


        
        //Temp
        private InitState state = InitState.None;
        private int numberStartInit;
        private int numberEndInit;
        private WaitToken<TResult> wtInit;
        private Coroutine crInit;
        





        #region Constructor

        public Initializer()
        {
        }

        public static Initializer<TResult> Create(
            MonoBehaviour owner,
            bool autoReInit,
            Action executeOneTimeSetup,
            Func<WaitToken<TResult>> executeInitProcess,
            Func<TResult, bool> isResultSuccess,
            Func<TResult> createFailedResult)
        {

            Initializer<TResult> initializer = new Initializer<TResult>();
            initializer.owner = owner;
            initializer.autoReInit = autoReInit;
            
            initializer.executeOneTimeSetup = executeOneTimeSetup;
            initializer.executeInitProcess = executeInitProcess;
            initializer.isResultSuccess = isResultSuccess;
            initializer.createFailedResult = createFailedResult;
            
            return initializer;
        }

        public static Initializer<TResult> CreateNoAutoReInit(
            MonoBehaviour owner,
            Action executeOneTimeSetup,
            Func<WaitToken<TResult>> executeInitProcess,
            Func<TResult, bool> isResultSuccess,
            Func<TResult> createFailedResult)
        {

            var initializer = Create(owner, false, executeOneTimeSetup, executeInitProcess, isResultSuccess, createFailedResult);
            return initializer;
        }

        public static Initializer<TResult> CreateNoAutoReInit(
            MonoBehaviour owner,
            Func<WaitToken<TResult>> executeInitProcess,
            Func<TResult, bool> isResultSuccess,
            Func<TResult> createFailedResult)
        {

            var initializer = Create(owner, false, null, executeInitProcess, isResultSuccess, createFailedResult);
            return initializer;
        }
        
        public static Initializer<TResult> CreateAutoReInit(
            MonoBehaviour owner,
            Action executeOneTimeSetup,
            Func<WaitToken<TResult>> executeInitProcess,
            Func<TResult, bool> isResultSuccess,
            Func<TResult> createFailedResult)
        {

            var initializer = Create(owner, true, executeOneTimeSetup, executeInitProcess, isResultSuccess, createFailedResult);
            return initializer;
        }
        
        public static Initializer<TResult> CreateAutoReInit(
            MonoBehaviour owner,
            Func<WaitToken<TResult>> executeInitProcess,
            Func<TResult, bool> isResultSuccess,
            Func<TResult> createFailedResult)
        {

            var initializer = Create(owner, true, null, executeInitProcess, isResultSuccess, createFailedResult);
            return initializer;
        }

        #endregion
        
        

        
        
        public WaitToken<TResult> Initialize()
        {
            if (wtInit != null)
                return wtInit;

            
            //Tạo sẵn wtInit mới
            wtInit = new WaitToken<TResult>();
            var wtInitTemp = wtInit; //Trả về wtInitTemp để tránh việc InitTimeline quá nhanh dẫn đến wtInit bị null ngay lập tức -> lỗi trả về wtInit null
            
            //Nếu Init timeline chưa chạy thì mới chạy
            if(crInit == null)
                RunInitTimeline();
            
            return wtInitTemp;
        }

        /// <summary>
        /// Nếu state đang là Executing thì sẽ không thể dừng.
        /// Hàm này có tác dụng chính là dừng initTimeline để chuẩn bị cho một Initializer mới.
        /// Tránh việc có 2 tiến trình init chạy song song
        /// </summary>
        public bool StopInitialize()
        {
            if (state == InitState.Executing)
            {
                Debug.LogError("Cannot stop initialize because there is a process running");
                return false;
            }


            
            if (crInit != null)
            {
                owner.StopCoroutine(crInit);
                crInit = null;
            }

            wtInit = null;
            state = InitState.None;
            return true;
        }


        private void RunInitTimeline()
        {
            if (crInit != null)
            {
                Debug.LogError("Only one init timeline is allowed");
                return;
            }

            crInit = owner.StartCoroutine(InitTimeline());
        }

        private IEnumerator InitTimeline()
        {
            while (true)
            {
                state = InitState.Executing;
                numberStartInit++;
                
                
                //OneTimeSetup
                if (numberStartInit == 1)
                    ExecuteOneTimeSetup();
                
                
                //Init process
                var wtExecuteInitProcess = ExecuteInitProcess();
                yield return wtExecuteInitProcess;
                numberEndInit++;
                
                
                //Nếu success
                if (IsResultSuccess(wtExecuteInitProcess.Result))
                {
                    //Nếu kết quả success thì đặt trạng thái là đã init
                    state = InitState.Success;
                    crInit = null;
                    wtInit.SetResult(wtExecuteInitProcess.Result);
                    yield break;
                }
                
                
                
                //Nếu failed thì wt trở về null
                state = InitState.None;
                var wtInitTemp = wtInit;
                wtInit = null;
                wtInitTemp.SetResult(wtExecuteInitProcess.Result);
                yield return null; //Đợi 1 frame là cần thiết để nếu có stop coroutine trong kết quả của wtInit.SetResult thì state vẫn sẽ chính xác
                
                
                //Nếu không bật reInit
                if (autoReInit == false)
                {
                    crInit = null;
                    yield break;
                }
                
                
                state = InitState.RestingToReInit;
                yield return new WaitForSecondsRealtime(reInitPerSeconds);
                if (autoReInit == false)
                {
                    state = InitState.None;
                    crInit = null;
                    yield break;
                }

                
                
                //Nếu wtInit chưa được tạo thì tạo wtInit mới cho vòng lặp tiếp theo
                if(wtInit == null)
                    wtInit = new WaitToken<TResult>();
            }
        }

       

        #region Action - Func
        
        private void ExecuteOneTimeSetup()
        {
            try
            {
                executeOneTimeSetup?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private WaitToken<TResult> ExecuteInitProcess()
        {
            try
            {
                if (executeInitProcess == null)
                {
                    Debug.LogError("executeInitProcess func is null");
                    return WaitToken<TResult>.CreateHasResulted(CreateFailedResult());
                }
                
                return executeInitProcess.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return WaitToken<TResult>.CreateHasResulted(CreateFailedResult());
            }
        }

        private TResult CreateFailedResult()
        {
            try
            {
                if (createFailedResult == null)
                {
                    Debug.LogError("createFailedResult func is null");
                    return default;
                }

                return createFailedResult.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
        }

        private bool IsResultSuccess(TResult result)
        {
            try
            {
                if (isResultSuccess == null)
                {
                    Debug.LogError("isResultSuccess func is null");
                    return false;
                }

                return isResultSuccess.Invoke(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
        
        #endregion
    }
}