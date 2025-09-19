using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
class StartButton : MonoBehaviour
{
    void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AsObservable()
            .Subscribe(_ => SceneManager.LoadScene("Game"))
            .AddTo(this);
    }
}
