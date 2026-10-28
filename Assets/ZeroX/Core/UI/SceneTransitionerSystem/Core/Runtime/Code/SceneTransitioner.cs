using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZeroX.ProgressSystem;
using ZeroX.WindowSystem;

namespace ZeroX.SceneTransitionerSystem
{
    public partial class SceneTransitioner : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private bool registerWithManager = false;
        [SerializeField] private Window window;
        [SerializeField] private Progressor progressor;
        [SerializeField] private Image mainBackground;
        [SerializeField] private float minDuration = 1f;
        

        
       
        
        //Events
        public readonly SceneTransitionerEvents InstanceEvents = new SceneTransitionerEvents();
        public static readonly SceneTransitionerEvents Events = new SceneTransitionerEvents();


        //Public Properties
        public Window Window => window;
        public Progressor Progressor => progressor;
        public Image MainBackground => mainBackground;
        public string Id => id;
        
        
        public static SceneTransitioner Current { get; private set; }
        public bool AllowSceneActivationAfterLoadDone { get; set; } = true;
        public bool IsInLoadingProcess => crLoader != null;
        
        
        //Temp Field
        private Sprite defaultMainBackground;
        private float defaultMinDuration;

        private bool initialized = false;
        private float progressUI = 0;
        private bool waitingManualClose;
        private Coroutine crLoader;
        
        
        



        #region Unity Method
        
        private void Awake()
        {
            if (registerWithManager)
            {
                bool registerSuccess = CheckAndRegisterWithManager();
                if (registerSuccess == false)
                {
                    Destroy(gameObject); //Muốn register mà không register được thì xóa
                    return;
                }
            }
            
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitIfNot();
        }
        
        

        private void Start()
        {
            
        }
        
        private void OnDestroy()
        {
            SceneTransitionerManager.UnRegister(this);
        }
        
        #endregion

        void InitIfNot()
        {
            if(initialized)
                return;

            initialized = true;
            

            defaultMainBackground = mainBackground.sprite;
            defaultMinDuration = minDuration;
        }

        private bool CheckAndRegisterWithManager()
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("Cannot register scene transitioner because id is empty", this);
                return false;
            }

            return SceneTransitionerManager.Register(this);
        }

        public static void Internal_SetId(SceneTransitioner sceneTransitioner, string id)
        {
            sceneTransitioner.id = id;
        }
        

        private IEnumerator Loader(IEnumerator iEnumerator, string loadingTargetName, bool autoCloseAfterLoadDone)
        {
            Current = this;
            waitingManualClose = autoCloseAfterLoadDone == false;
            
            SetParameterToDefault();
            
            //Mở window
            window.Open();
            
            //Khi window bắt đầu mở thì làm 1 số thứ và bắn event OnTransitionStart
            ResetProgressor();
            FireEvent_OnTransitionStart(loadingTargetName);
            
            
            
            //Đợi đến khi window mở xong thì bắn event OnLoadStart
            yield return new WaitUntil(() => window.IsOpened);
            FireEvent_OnLoadStart(loadingTargetName);

            
            //Lưu lại thời gian load
            float timeStartLoad = Time.realtimeSinceStartup;
            
            
            //Thực hiện công việc cần load và đợi
            yield return StartCoroutine(iEnumerator);

            
            //Đợi animation progress UI chạy xong
            yield return new WaitUntil(() => Mathf.Approximately(GetAnimateProgressUI(), 1f));

            
            //Đợi đủ minDuration
            yield return new WaitUntil(() => Time.realtimeSinceStartup - timeStartLoad >= minDuration);
            
            
            //Đợi xong thì bắn event OnLoadEnd
            FireEvent_OnLoadEnd(loadingTargetName);


            //Nếu ko phải tự close thì cần đợi đến khi được close
            if (autoCloseAfterLoadDone == false)
            {
                yield return new WaitWhile(() => waitingManualClose);
            }
            
            
            //Close và đợi đến khi closed
            window.Close();
            yield return new WaitUntil(() => window.IsClosed);
                
            
            //Bắn event transition end
            FireEvent_OnTransitionEnd(loadingTargetName);

            //Set null crLoader và Current để ko còn trong trạng thái busy
            crLoader = null;
            Current = null;
            
            
            //Nếu ko phải scene transitioner được registered thì xóa. Vì được coi là sceneTransitioner dùng một lần
            if(SceneTransitionerManager.IsRegistered(this) == false)
                Destroy(gameObject);
        }

        private bool CanRunLoader()
        {
            if (crLoader != null)
            {
                Debug.LogError("Scene Transitioner is in another loading process");
                return false;
            }

            if (Current != null)
            {
                Debug.LogError("Another Scene Transitioner is running");
                return false;
            }

            return true;
        }
        
        public Coroutine RunLoader(IEnumerator iEnumerator, string loadingTargetName, bool autoCloseAfterLoadDone)
        {
            if(CanRunLoader() == false)
                return null;

            crLoader = StartCoroutine(Loader(iEnumerator, loadingTargetName, autoCloseAfterLoadDone));
            return crLoader;
        }
        

        
        public Coroutine LoadSceneAsync(string sceneName, bool autoCloseAfterLoadDone)
        {
            if(CanRunLoader() == false)
                return null;
            
            
            //Default luôn là true
            AllowSceneActivationAfterLoadDone = true;
            return RunLoader(IELoadSceneAsync(), sceneName, autoCloseAfterLoadDone);

            

            
            
            IEnumerator IELoadSceneAsync()
            {
                SetProgressUI_And_FireEvent(sceneName, 0);
                
                
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                asyncOperation.allowSceneActivation = false;
                while (asyncOperation.progress < 0.9f)
                {
                    SetProgressUI_And_FireEvent(sceneName, asyncOperation.progress);
                    yield return null;
                }
                
                
                SetProgressUI_And_FireEvent(sceneName, 0.9f);
                yield return null;
                SetProgressUI_And_FireEvent(sceneName, 1f);

                
                //Đợi AllowSceneActivationAfterLoadDone true thì mới active scene
                yield return new WaitUntil(() => AllowSceneActivationAfterLoadDone);
                asyncOperation.allowSceneActivation = true;
                
                
                yield return new WaitUntil(() => Mathf.Approximately(asyncOperation.progress, 1) || asyncOperation.progress > 1f);
            }
        }

        public Coroutine LoadSceneAsync(string sceneName)
        {
            return LoadSceneAsync(sceneName, true);
        }
        

        public void LoadAction(string loadingTargetName, bool autoCloseAfterLoadDone, Action action)
        {
            RunLoader(Timeline(), loadingTargetName, autoCloseAfterLoadDone);
            
            
            
            IEnumerator Timeline()
            {
                SetProgressUI_And_FireEvent(loadingTargetName, 0);
                
                yield return null;
                
                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                SetProgressUI_And_FireEvent(loadingTargetName, 1);
            }
        }

        public void LoadAction(string loadingTargetName, Action action)
        {
            LoadAction(loadingTargetName, true, action);
        }


        void SetParameterToDefault()
        {
            mainBackground.sprite = defaultMainBackground;
            minDuration = defaultMinDuration;
        }
        

        public void ManualClose()
        {
            waitingManualClose = false;
        }
        

        public void SetMainBackground(Sprite background)
        {
            mainBackground.sprite = background;
        }

        public void SetMinDuration(float duration)
        {
            minDuration = duration;
        }

        public void SetProgressUI(float progress)
        {
            progressUI = progress;
            if (progressor != null)
                progressor.Progress = progress;
        }

        public void SetProgressUI_And_FireEvent(string loadingTargetName, float progress)
        {
            SetProgressUI(progress);
            FireEvent_OnProgressChanged(loadingTargetName, progress);
        }

        void ResetProgressor()
        {
            progressUI = 0;
            if (progressor != null)
            {
                progressor.Progress = 0;
                progressor.AnimateProgress = 0;
            }
        }

        public float GetAnimateProgressUI()
        {
            if (progressor != null)
                return progressor.AnimateProgress;
            else
                return progressUI;
        }

        public WaitToken WaitTransitionEnd()
        {
            WaitToken waitToken = new WaitToken();
            ZeroRx.RunCoroutine(Timeline());
            return waitToken;
            
            
            
            IEnumerator Timeline()
            {
                while (this != null && crLoader != null)
                {
                    yield return null;
                }
                
                waitToken.SetResult();
            }
        }
        
        public static WaitToken WaitCurrentTransitionEnd()
        {
            WaitToken waitToken = new WaitToken();
            ZeroRx.RunCoroutine(Timeline());
            return waitToken;
            
            
            
            IEnumerator Timeline()
            {
                SceneTransitioner current = Current;
                while (current != null && current.crLoader != null)
                {
                    yield return null;
                }
                
                waitToken.SetResult();
            }
        }
    }
}

