using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSettings : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> _firstList;
    [SerializeField]
    private List<GameObject> _secondList;

    private int _id;

    private void Awake()
    {
        if (PlayerPrefs.GetString("FirstSettings") == "Done")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            OpenSetting(true);
    }
    public void Next()
    {
        _secondList[_id].GetComponentInChildren<Setting>().Save();
        OpenSetting(false);
        if (++_id < _firstList.Count)
            OpenSetting(true);
        else
            ChangeScene();
    }

    private void OpenSetting(bool open)
    {
        _firstList[_id].SetActive(open);
        _secondList[_id].SetActive(open);

    }
    private void ChangeScene()
    {
        PlayerPrefs.SetString("FirstSettings", "Done");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
