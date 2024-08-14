using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        LobbyScene,
        LoadingScene,
        CharacterSelectScene,
        CharacterPlayground
    }

    private static Scene targetScene;

    public static void Load(Scene _targetScene)
    {
        Loader.targetScene = _targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        // to implement loadingscreen you should use NetworkManager.Singleton.SceneManager.OnLoadEventComplete in the loadercallback
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

}
