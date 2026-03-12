using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    public void Enter(int index)
    {
        SceneManager.LoadScene(index);
    }
}
