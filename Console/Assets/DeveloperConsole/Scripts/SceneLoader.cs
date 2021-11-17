using UnityEngine.SceneManagement;
using UnityEngine;

namespace Anarkila.DeveloperConsole {

    public class SceneLoader : MonoBehaviour {

        private AsyncOperation asyncOperation;
        private string loadedSceneName;
        private bool loading = false;

        private void Awake() {
            enabled = false; // disable this script on start

            ConsoleEvents.RegisterSceneUnLoadByIndex += UnLoadSceneByIndexAsync;
            ConsoleEvents.RegisterSceneUnLoadByName += UnLoadSceneByNameAsync;
            ConsoleEvents.RegisterSceneLoadByIndex += LoadSceneByIndexAsync;
            ConsoleEvents.RegisterSceneLoadByName += LoadSceneByNameAsync;
        }

        private void OnDestroy() {
            ConsoleEvents.RegisterSceneUnLoadByIndex -= UnLoadSceneByIndexAsync;
            ConsoleEvents.RegisterSceneUnLoadByName -= UnLoadSceneByNameAsync;
            ConsoleEvents.RegisterSceneLoadByIndex -= LoadSceneByIndexAsync;
            ConsoleEvents.RegisterSceneLoadByName -= LoadSceneByNameAsync;
        }

        private void Update() {
            if (!loading || asyncOperation == null) return;

            var progress = asyncOperation.progress;

            if (progress >= 0.9f) {
                asyncOperation.allowSceneActivation = true;
                enabled = false;
                loading = false;

                Debug.Log(string.Format("Loaded new scene [{0}]", loadedSceneName));
            }
        }

        /// <summary>
        /// Try to load level by index
        /// scene must be included in Build settings!
        /// </summary>
        private void LoadSceneByIndexAsync(int index, LoadSceneMode mode) {
            if (loading) return;

            int sceneCount = SceneManager.sceneCountInBuildSettings;
            if (index > sceneCount || index < 0) {
#if UNITY_EDITOR
                Debug.Log(string.Format("Scene index: [{0}] doesn't exist!", index));
#endif
                return;
            }
           
            if (mode == LoadSceneMode.Single) {
                ConsoleEvents.CloseConsole();
                asyncOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
                loadedSceneName = SceneManager.GetSceneByBuildIndex(index).name;
                asyncOperation.allowSceneActivation = false;
                loading = true;
                enabled = true;
            }
            else {
                SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            }
        }

        /// <summary>
        /// Try to load level by name
        /// scene must be included in Build settings!
        /// </summary>
        private void LoadSceneByNameAsync(string sceneName) {
            if (loading) return;

            sceneName = ConsoleUtils.DeleteWhiteSpace(sceneName);
            if (Application.CanStreamedLevelBeLoaded(sceneName)) {
                ConsoleEvents.CloseConsole();
                asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                loadedSceneName = sceneName;
            }
            else {
#if UNITY_EDITOR
                Debug.Log(string.Format("Scene [{0}] doesn't exist!", sceneName));
#endif
                return;
            }
            asyncOperation.allowSceneActivation = false;
            loading = true;
            enabled = true;
        }


        /// <summary>
        /// Try to load level by index
        /// scene must be included in Build settings!
        /// </summary>
        private void UnLoadSceneByIndexAsync(int index) {
            if (SceneManager.sceneCount == 1) {
#if UNITY_EDITOR
                Debug.Log(string.Format("Can't unload scene [{0}] because it's the only scene active!", index));
#endif
                return;
            }

            if (index > SceneManager.sceneCountInBuildSettings || index < 0) {
#if UNITY_EDITOR
                Debug.Log(string.Format("Scene index: [{0}] doesn't exist!", index));
#endif
                return;
            }
            SceneManager.UnloadSceneAsync(index);
        }

        /// <summary>
        /// Try to load level by index
        /// scene must be included in Build settings!
        /// </summary>
        private void UnLoadSceneByNameAsync(string sceneName) {
            if (SceneManager.sceneCount == 1) {
#if UNITY_EDITOR
                Debug.Log(string.Format("Can't unload scene [{0}] because it's the only scene active!", sceneName));
#endif
                return;
            }

            sceneName = ConsoleUtils.DeleteWhiteSpace(sceneName);
            if (Application.CanStreamedLevelBeLoaded(sceneName)) {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
}