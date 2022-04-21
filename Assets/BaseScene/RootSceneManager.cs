using UnityEngine;
using UnityEngine.SceneManagement;

public class RootSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
public void LoadSceneWithName(string SceneName)
    {
        SceneManager.LoadScene("OtherSceneName", LoadSceneMode.Single);
    }

    public void BackTORootScene()
    {
        var rootname = "RootScene";
        LoadSceneWithName(rootname);
    }
}
