using UnityEngine;

public class SwitchScene : MonoBehaviour
{
    public static SwitchScene switchScene;

    [SerializeField]
    public string MainMenu = "MainMenu";
    [SerializeField]
    public string Options = "OptionsMenu";
    [SerializeField]
    public string Codex = "CodexMenu";
    [SerializeField]
    public string CharacterSelect="CharacterSelect";
    [SerializeField]
    public string PostCharSelect="CutsceneTest";
    [SerializeField]
    public string Demo="DemoLevel02";

    void Awake()
        {
            DontDestroyOnLoad(gameObject);

            //singlton check
            if (switchScene == null)
                switchScene = this;
            else
                Destroy(this);

    }
}
