using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Codex : MonoBehaviour
{
    /* -----------------
     * Dependancies
     ------------------*/
    [SerializeField] private AnyInput leftRightInput;
    [SerializeField] private AnyInput upDownsInput;

    [SerializeField]
    public GameObject buttonPrefab;

    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Image image;


    [SerializeField]
    public Transform[] categoryMenus;

    /* -----------------*/

    public enum Category
    {
        Characters,
        Bosses,
        Enemies,
        Levels,
    }

    [System.Serializable]
    public class Entry
    {
        public Category category;
        public string name;
        [TextArea] public string description;
        public Sprite image;
        //[HideInInspector]
        //public GameObject button; Not nessesary at the current moment
        [HideInInspector]
        public RectTransform buttonTransform;
    }

    [SerializeField]
    public List<Entry> entries;

    /* -----------------
     * Internal Variables
     ------------------*/

    //Each entry is moved to seperate lists based off of Category.
    private List<Entry>[] entriesInternal;
    private int[] entryIndex;
    private int categoryIndex;
    private bool switching = false;

    //Pre-defined static Values
    private Vector3 smallButton = new Vector3(160, 30);
    private Vector3 largeButton = new Vector3(160, 50);

    private void Start()
    {
        leftRightInput.performed += (context) =>
        {
            if (context.ReadValue<float>() < 0)
            {
                Left();
            }
            else
            {
                Right();
            }
        };

        upDownsInput.performed += (context) =>
        {
            if (context.ReadValue<float>() > 0)
            {
                Up();
            }
            else
            {
                Down();
            }
        };
    }

    private void Awake()
    {
        CreateCodex();
        SwitchCategory(0);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(SwitchScene.mainMenu);
    }

    void CreateCodex()
    {
        int numCategories = Enum.GetNames(typeof(Category)).Length;
        //Check that the number of categories in the enum and transform array match
        if (categoryMenus.Length != numCategories)
            Debug.LogWarning("CodexManager: Number of category types and provided menus do not match!");

        //Instantiate the internal entries
        entriesInternal = new List<Entry>[numCategories];
        for (int i = 0; i < numCategories; i++)
        {
            entriesInternal[i] = new List<Entry>();
        }

        //Move each of the entries to their respective internal lists
        for (int i = 0; i < entries.Count; i++)
        {
            entriesInternal[(int)entries[i].category].Add(entries[i]);
        }

        //Adds correct information
        for (int i = 0; i < entriesInternal.Length; i++)
        {
            for (int j = 0; j < entriesInternal[i].Count; j++)
            {
                Entry e = entriesInternal[i][j];

                int jCopy = j;//This is incredibly jank, but when adding listeners in a loop, all of the buttons will be last number of i without a temp var.

                GameObject button = GameObject.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, categoryMenus[i]);
                button.GetComponent<Button>().onClick.AddListener(delegate { SwitchEntry(jCopy); });
                button.GetComponentInChildren<TextMeshProUGUI>().text = e.name;
                e.buttonTransform = button.GetComponent<RectTransform>();
            }
        }
        categoryIndex = -1;
        entryIndex = new int[numCategories];
    }

    IEnumerator ChangeSelectedEntry(int nextIndex)
    {
        Vector3 startPos = new Vector3(7.5f, -100.0f + 40.0f * entryIndex[categoryIndex]);
        Vector3 endPos = new Vector3(7.5f, -100.0f + 40.0f * nextIndex);
        for (float t = 0; t < 1.0; t+=0.08f) {
            ResizeButton(categoryIndex,entryIndex[categoryIndex],largeButton,smallButton,t);
            Scroll(categoryIndex, startPos, endPos, t, false);
            ResizeButton(categoryIndex,nextIndex,smallButton, largeButton, t);
            yield return new WaitForSeconds(0.005f);
        }
        ResizeButton(categoryIndex,entryIndex[categoryIndex],largeButton, smallButton,1.0f);
        Scroll(categoryIndex, startPos, endPos, 1.0f, false);
        ResizeButton(categoryIndex,nextIndex,smallButton, largeButton, 1.0f);

        entryIndex[categoryIndex] = nextIndex;
        switching = false;
        UpdateDisplay();
        StopCoroutine("ChangeSelectedEntry");
    }

    IEnumerator ChangeSelectedCategory(int newCategory)
    {
        //Scroll Out
        if (categoryIndex!=-1) {
            Vector3 startPosA = new Vector3(7.5f, -100.0f + 40.0f * entryIndex[categoryIndex]);
            Vector3 endPosA = new Vector3(-500f, -100.0f + 40.0f * entryIndex[categoryIndex]);

            for (float t = 0; t < 1.0; t += 0.08f)
            {
                ResizeButton(categoryIndex, entryIndex[categoryIndex], largeButton, smallButton, t);
                Scroll(categoryIndex, startPosA, endPosA, t, false);
                yield return new WaitForSeconds(0.005f);
            }
            ResizeButton(categoryIndex, entryIndex[categoryIndex], largeButton, smallButton, 1.0f);
            Scroll(categoryIndex, startPosA, endPosA, 1.0f, false);
        }

        //Scroll in
        Vector3 startPosB = new Vector3(-170f, -100.0f + 40.0f * entryIndex[newCategory]);
        Vector3 endPosB = new Vector3(7.5f, -100.0f + 40.0f * entryIndex[newCategory]);

        for (float t = 0; t < 1.0; t += 0.08f)
        {
            ResizeButton(newCategory, entryIndex[newCategory], smallButton, largeButton, t);
            Scroll(newCategory, startPosB, endPosB, t, false);
            yield return new WaitForSeconds(0.005f);
        }
        ResizeButton(newCategory, entryIndex[newCategory], smallButton, largeButton, 1.0f);
        Scroll(newCategory, startPosB, endPosB, 1.0f, false);

        categoryIndex = newCategory;
        switching = false;
        UpdateDisplay();
        StopCoroutine("ChangeSelectedCategory");

        /*Vector3 endPos = new Vector3(0, -100.0f, 0f);

        categoryMenus[categoryIndex].gameObject.SetActive(false);
        categoryIndex = newCategory;
        categoryMenus[newCategory].gameObject.SetActive(true);

        ResizeButton(categoryIndex, entryIndex[newCategory], smallButton, largeButton, 1.0f);*/
    }

    //This updates the Icon, title, and description to the right values based off of the global variables.
    void UpdateDisplay()
    {
        var entry = entriesInternal[categoryIndex][entryIndex[categoryIndex]];
        title.text = entry.name;
        description.text = entry.description;
        Debug.LogWarning("CodexManager: error! no sprite set for entry named '"+ entry.name+"'");
        image.sprite = entry.image;
    }

    void ResizeButton(int i, int j,Vector3 start, Vector3 end, float t)
    {
        entriesInternal[i][j].buttonTransform.sizeDelta = Vector3.Slerp(start,end,t);
    }

    void Scroll(int categoryNumber, Vector3 start, Vector3 end, float t, bool doSlerp)
    {
        if (doSlerp) {
            categoryMenus[categoryNumber].localPosition = Vector3.Slerp(start, end, t);
        }
        else
        {
            categoryMenus[categoryNumber].localPosition = Vector3.Lerp(start, end, t);
        }
    }

    private void SwitchEntry(int newIndex)
    {
        if (newIndex!=entryIndex[categoryIndex] &&!switching) {
            switching = true;
            StartCoroutine("ChangeSelectedEntry", newIndex);
        }
    }

    private void SwitchCategory(int newCategory)
    {
        if (!switching)
        {
            switching = true;
            StartCoroutine("ChangeSelectedCategory", newCategory);
        }
    }

    private void Up()
    {
        if (entryIndex[categoryIndex] <= 0)
            return;

        SwitchEntry(entryIndex[categoryIndex] - 1);

    }

    private void Down()
    {
        if (entryIndex[categoryIndex] >= entriesInternal[categoryIndex].Count-1)
            return;

        SwitchEntry(entryIndex[categoryIndex] + 1);
    }

    public void Left()
    {
        if (categoryIndex <= 0)
            return;

        SwitchCategory(categoryIndex - 1);
    }



    public void Right()
    {
        if (categoryIndex>= categoryMenus.Length-1)
            return;

        SwitchCategory(categoryIndex+1);
    }

}
