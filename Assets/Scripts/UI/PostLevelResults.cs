using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostLevelResults : MonoBehaviour
{
    GameObject player;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI rankText;
    public Image characterImage;
    public Image rankImages;

    PlayerScore playerScore;

    // Start is called before the first frame update
    void Start()
    {
        playerScore = FindAnyObjectByType<PlayerScore>();
        player = GameObject.FindWithTag("Player");

        scoreText.text = playerScore.getScore().ToString();
        rankText.text = playerScore.getRank();
        rankImages.sprite = playerScore.getRankImage();
        characterImage.sprite = player.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
