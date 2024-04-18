using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Codex : MonoBehaviour
{
    [SerializeField]
    public GameObject buttonPrefab;

    [SerializeField]
    public Transform buttonsParent;

    public enum Category
    {
        ChangeMeOne,
        ChangeMeTwo,
        ChangeMeThree,
        ChangeMeFour
    }

    [System.Serializable]
    public class Entry
    {
        public Category category;
        public string name;
        public string description;
        public Sprite image;
        //[HideInInspector]
        //public GameObject button; Not nessesary at the current moment
        [HideInInspector]
        public RectTransform buttonTransform;
    }

    public List<Entry> entries = new List<Entry>();
    private int index;
    private int nextIndex;
    private bool switching = false;
    private Vector3 smallButton = new Vector3(160, 30);
    private Vector3 largeButton = new Vector3(160, 50);

    private void Awake()
    {
        //int numCategories = Enum.GetNames(typeof(Category)).Length;

        index = 0;
        for (int i=0; i<entries.Count; i++)
        {
            int iCopy = i;//This is incredibly jank, but when adding listeners in a loop, all of the buttons will be last number of i without a temp var.

            Entry e = entries[i];
            GameObject button = GameObject.Instantiate(buttonPrefab,Vector3.zero,Quaternion.identity,buttonsParent);
            button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicked(iCopy); });
            button.GetComponentInChildren<TextMeshProUGUI>().text = e.name;
            e.buttonTransform = button.GetComponent<RectTransform>();
        }

        entries[0].buttonTransform.sizeDelta = largeButton;
    }

    IEnumerator ChangeSelected(int nextIndex)
    {
        Vector3 startPos = new Vector3(0, -100.0f + 40.0f * index);
        Vector3 endPos = new Vector3(0, -100.0f + 40.0f * nextIndex);
        for (float t = 0; t < 1.0; t+=0.05f) {
            ResizeButton(index,largeButton,smallButton,t);
            Scroll(startPos, endPos, t);
            ResizeButton(nextIndex,smallButton, largeButton, t);
            yield return new WaitForSeconds(0.015f);
        }
        ResizeButton(index,largeButton, smallButton,1.0f);
        Scroll(startPos, endPos, 1.0f);
        ResizeButton(nextIndex,smallButton, largeButton, 1.0f);

        index = nextIndex;
        switching = false;
        StopCoroutine("ChangeSelected");
    }

    void ResizeButton(int index,Vector3 start, Vector3 end, float t)
    {
        entries[index].buttonTransform.sizeDelta = Vector3.Slerp(start,end,t);
    }

    void Scroll(Vector3 start, Vector3 end, float t)
    {
        buttonsParent.localPosition = Vector3.Slerp(start, end, t);
    }

    private void ButtonClicked(int bindex)
    {
        if (bindex!=index&&!switching) {
            switching = true;
            StartCoroutine("ChangeSelected", bindex);
        }
    }

}
