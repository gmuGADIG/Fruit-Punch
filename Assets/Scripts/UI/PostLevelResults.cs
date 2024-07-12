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
        playerScore = player.GetComponent<PlayerScore>();

        scoreText.text = playerScore.GetScore().ToString();
        rankText.text = playerScore.GetRank();
        rankImages.sprite = playerScore.GetRankImage();
        characterImage.sprite = player.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public void setPlayer(GameObject myPlayer)
    {
        player = myPlayer;
    }

}
