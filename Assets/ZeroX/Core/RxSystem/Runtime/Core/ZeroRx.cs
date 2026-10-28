using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeroX.RxSystem;
using Object = UnityEngine.Object;

public class ZeroRx : MonoBehaviour
{
    private static Thread mainThread = null;
    protected static ZeroRx Instance;

    private static readonly Dictionary<Disposable, Object> dictDisposableWhenObjectDestroyed = new Dictionary<Disposable, Object>();
    
    
    #region Time Field

    private static float time { get; set; }
    private static float unscaledTime { get; set; }
    
    /// <summary>
    /// When game pause, this time not increase
    /// </summary>
    public static float AppTime { get; private set; }
    
    /// <summary>
    /// When game pause, this time not increase
    /// </summary>
    public static float UnscaledAppTime { get; private set; }

    #endregion


    private static bool applicationPauseStatus = false;
    private static bool skipUpdateAppTimeOnce = false;
    
    
    //Events
    public static readonly Subject onUpdate = new Subject();
    public static readonly Subject<bool> onApplicationPause = new Subject<bool>();
    public static readonly Subject onApplicationQuit = new Subject();
    
    

    
    #region Unity Method

    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
            
            transform.SetParent(null);
            DontDestroyOnLoad(Instance.gameObject);
            
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        mainThread = Thread.CurrentThread;
        DataTransferCrossThread.StartWork();
        UpdateTime();
        SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterOnApplicationPause_OnEditor();
    }

    private void Start()
    {
        UpdateTime();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        UpdateTime();
        UpdateAppTime();
        
        ExecuteListActionExecuteInMainThread();
        ExecuteListDelayActionExecuteInMainThread();

        onUpdate.Emit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        applicationPauseStatus = pauseStatus;
        
        if (applicationPauseStatus == false)
            skipUpdateAppTimeOnce = true;
        
        
        
        onApplicationPause.Emit(pauseStatus);
    }
    
    private void RegisterOnApplicationPause_OnEditor()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.pauseStateChanged += state =>
        {
            OnApplicationPause(state == UnityEditor.PauseState.Paused);
        };
#endif
    }

    private void OnApplicationQuit()
    {
        onApplicationQuit.Emit();
    }
    
    #endregion
    

    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        Instance = new GameObject("ZeroRx").AddComponent<ZeroRx>();
        
        Debug.Log("Auto Initialize ZeroRx");
    }

    /// <summary>
    /// Kiểm tra xem thread gọi property này có phải main thread hay không
    /// </summary>
    public static bool IsMainThread => Thread.CurrentThread == mainThread;

    public static ZeroRx GetInstance()
    {
        return Instance;
    }

    static void UpdateTime()
    {
        time = Time.time;
        unscaledTime = Time.unscaledTime;
    }

    private void UpdateAppTime()
    {
        if(applicationPauseStatus)
            return;

        if (skipUpdateAppTimeOnce)
        {
            skipUpdateAppTimeOnce = false;
            return;
        }

        AppTime += Time.deltaTime;
        UnscaledAppTime += Time.unscaledDeltaTime;
    }
    
    #region Execute In Main Thread

    static void ExecuteListActionExecuteInMainThread()
    {
        List<Action> listAction_Temp = null;
        
        //Lấy data từ list main thread ra list temp
        lock (DataTransferCrossThread.objLock_MainThread)
        {
            if(DataTransferCrossThread.listAction_MainThread.Count == 0)
                return;

            listAction_Temp = new List<Action>();
            listAction_Temp.AddRange(DataTransferCrossThread.listAction_MainThread);
            DataTransferCrossThread.listAction_MainThread.Clear();
        }
        
        
        //Invoke Action
        for (int i = 0; i < listAction_Temp.Count; i++)
        {
            var action = listAction_Temp[i];
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    static void ExecuteListDelayActionExecuteInMainThread()
    {
        List<DelayAction> listDelayAction_Temp = null;
        
        //Lấy data từ list main thread ra list temp
        lock (DataTransferCrossThread.objLock_MainThread)
        {
            if(DataTransferCrossThread.listDelayAction_MainThread.Count == 0)
                return;

            
            for (int i = 0; i < DataTransferCrossThread.listDelayAction_MainThread.Count; i++)
            {
                var delayAction = DataTransferCrossThread.listDelayAction_MainThread[i];
                if (delayAction.initialized == false)
                {
                    delayAction.Initialize();
                    continue;
                }

                if (delayAction.IsTimeToExecute())
                {
                    if (listDelayAction_Temp == null)
                        listDelayAction_Temp = new List<DelayAction>();
                    
                    listDelayAction_Temp.Add(delayAction);
                    DataTransferCrossThread.listDelayAction_MainThread.RemoveAt(i);
                    i--;
                }
            }
        }
        
        if(listDelayAction_Temp == null)
            return;
        
        
        
        
        for (int i = 0; i < listDelayAction_Temp.Count; i++)
        {
            try
            {
                listDelayAction_Temp[i].action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    /// <summary>
    /// Run action in main thread
    /// </summary>
    public static void RunAction(Action action)
    {
        if (IsMainThread)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            DataTransferCrossThread.AddActionFromOtherThread(action);
        }
    }

    /// <summary>
    /// Run action in main thread. Delay use Time.time or Time.unscaledTime
    /// </summary>
    public static void RunActionDelay(Action action, float delayTime, bool useUnscaledTime)
    {
        if (IsMainThread)
        {
            if (delayTime <= 0)
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                DelayAction delayAction = new DelayAction(action, delayTime, useUnscaledTime);
                DataTransferCrossThread.AddDelayActionFromMainThread(delayAction);
            }
        }
        else
        {
            DelayAction delayAction = new DelayAction(action, delayTime, useUnscaledTime);
            DataTransferCrossThread.AddDelayActionFromOtherThread(delayAction);
        }
    }

    /// <summary>
    /// Run action in main thread. Delay use Time.time
    /// </summary>
    public static void RunActionDelayScaledTime(Action action, float delayTime)
    {
        RunActionDelay(action, delayTime, false);
    }

    /// <summary>
    /// Run action in main thread. Delay use Time.unscaledTime
    /// </summary>
    public static void RunActionDelayUnscaledTime(Action action, float delayTime)
    {
        RunActionDelay(action, delayTime, true);
    }
    
    public static WaitToken<T> RunFunc<T>(Func<T> func)
    {
        if (IsMainThread)
        {
            try
            {
                var result = func.Invoke();
                return WaitToken<T>.CreateHasResulted(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return WaitToken<T>.CreateFaulted(e);
            }
        }

        WaitToken<T> wt = new WaitToken<T>();
        RunAction(() =>
        {
            try
            {
                T result = func.Invoke();
                wt.SetResult(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                wt.SetFault(e);
            }
        });
        
        return wt;
    }

    public static WaitToken<T> RunFuncDelay<T>(Func<T> func, float delay, bool ignoreTimeScale)
    {
        if (delay <= 0 && IsMainThread)
        {
            try
            {
                var result = func.Invoke();
                return WaitToken<T>.CreateHasResulted(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return WaitToken<T>.CreateFaulted(e);
            }
        }
        
        WaitToken<T> wt = new WaitToken<T>();
        RunActionDelay(() =>
        {
            try
            {
                T result = func.Invoke();
                wt.SetResult(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                wt.SetFault(e);
            }
        }, delay, ignoreTimeScale);
        
        return wt;
    }

    #endregion

    #region Coroutine
    
    /// <summary>
    /// If you call when ZeroRx.Instance is null, the code will still be executed when ZeroRx init, but will return a null coroutine.
    /// </summary>
    public static Coroutine RunCoroutine(IEnumerator routine)
    {
        if (Instance == null)
        {
            RunAction(() =>
            {
                Instance.StartCoroutine(routine);
            });

            return null;
        }
        else
        {
            if (IsMainThread)
            {
                return Instance.StartCoroutine(routine);
            }
            else
            {
                Debug.LogError("Only call RunCoroutine in main thread, use RunCoroutineAsync instead");
                return null;
            }
        }
    }

    public static void StopRunCoroutine(Coroutine coroutine)
    {
        if(coroutine == null)
            return;
        
        if (IsMainThread)
        {
            Instance.StopCoroutine(coroutine);
        }
        else
        {
            RunAction(() => Instance.StopCoroutine(coroutine));
        }
    }

    /// <summary>
    /// Start coroutine from outside main thread
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public static WaitToken<Coroutine> RunCoroutineAsync(IEnumerator routine)
    {
        if (IsMainThread)
        {
            var coroutine = Instance.StartCoroutine(routine);
            return WaitToken<Coroutine>.CreateHasResulted(coroutine);
        }
        else
        {
            WaitToken<Coroutine> wt = new WaitToken<Coroutine>();
            RunAction(() =>
            {
                var coroutine = Instance.StartCoroutine(routine);
                wt.SetResult(coroutine);
            });
            return wt;
        }
    }

    #endregion
}