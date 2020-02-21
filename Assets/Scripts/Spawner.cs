using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public static bool play = false;
    public static int highScore = 0;

    public GameObject hsGo;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScoreLabel;

    public GameObject playerPrefab;
    public GameObject playerGO;

    public Camera mainCamera;


    public Texture yellowShroom;
    public Texture blueShroom;
    public Texture greenShroom;
    public Texture redShroom;


    public int lastSideSpawned = 1;

    public Material mulitplierUpShroomMaterial;
    public Material defaultShroomMaterial;

    public Texture blueButterfly;
    public Texture orangeButterfly;
    public Texture rainbowButterfly;

    public int points;
    public TextMeshProUGUI pointsText;

    public GameObject wall1;
    public GameObject wall2;

    public GameObject trampGo;
    public GameObject cloudGuy;
    public GameObject beePrefab;
    public GameObject collectGo;

    public List<GameObject> trampolines = new List<GameObject>();
    public int MaxNumberOfTrampolines = 10;
    public List<GameObject> enemies = new List<GameObject>();
    public int MaxNumberOfEnemies = 5;

    float lastSpawnHeightTramp;
    float lastSpawnHeightEnemy;

    public const int POWER_UP_TIMER_MAX = 10;
    public int powerUpTimer;
    public int powerUpTimeLeft;
    public int powerUpStartTime;
    public bool powerUp;
    public bool startTimer;

    public TextMeshProUGUI powerUpTimerGO;

    public TextMeshProUGUI powerUpTypeText;

    public int powerUpType;

    public GameObject safetyNetMushroom;

    public GameObject restartButton;
    public GameObject leftButton;
    public GameObject rightButton;

    public GameObject muteButton;
    public Sprite muteSprite;
    public Sprite unmuteSprite;
    public bool mute = false;

    public bool left;
    public bool right;

    bool timerChange = false;
    int timeFlickerFrameCounter;
    // Start is called before the first frame update
    void Start()
    {
//#if UNITY_EDITOR || UNITY_STANDALONE
        //leftButton.SetActive(false);
        //rightButton.SetActive(false);
//#endif
        //Input.multiTouchEnabled = false;
        Instance = this;
        //PlayerPrefs.SetInt("highscore", 0);
        highScore = PlayerPrefs.GetInt("highscore");
        LoadMute();
        
        if (highScore != 0)
        {
            highScoreText.text = highScore.ToString();
        }
        else
        {
            highScoreText.text = "";
            highScoreLabel.text = "";
        }
        if (play)
        {
            muteButton.SetActive(false);
            hsGo.SetActive(false);
            restartButton.SetActive(false);
            playerGO = Instantiate(playerPrefab, new Vector3(0, 2), Quaternion.Euler(new Vector3(0, 180)));
            lastSpawnHeightTramp = mainCamera.transform.position.y;
            lastSpawnHeightEnemy = mainCamera.transform.position.y;
            SpawnCollectable();
            RenderSettings.skybox.SetVector("ScrollSpeed", new Vector4(0, 0, 0, 0));
        }
    }

    public void Left()
    {
        left = true;
    }
    public void Right()
    {
        right = true;
    }
    public void OnRightRelease()
    {
        right = false;
    }
    public void OnLeftRelease()
    {
        left = false;
    }
    public void ToggleMute()
    {
        if(AudioManager.mute)
        {
            AudioManager.mute = false;
            AudioManager.Instance.Play("theme");
        }
        else
        {
            AudioManager.mute = true;
            AudioManager.Instance.Stop("theme");
        }
        PlayerPrefs.SetString("mute", AudioManager.mute.ToString());
        LoadMute();
    }
    public void LoadMute()
    {
        if (AudioManager.mute)
        {
            muteButton.GetComponent<Image>().sprite = muteSprite;
        }
        else
        {
            muteButton.GetComponent<Image>().sprite = unmuteSprite;
        }
    }
    public void Timer()
    {
        if (powerUp)
        {
            if (!startTimer)
            {
                AudioManager.Instance.Play("timer");
                powerUpTimeLeft = POWER_UP_TIMER_MAX;
                powerUpTimer = POWER_UP_TIMER_MAX;
                startTimer = true;
                powerUpStartTime = (System.DateTime.Now.Hour * 3600) + (System.DateTime.Now.Minute * 60) + System.DateTime.Now.Second;
                powerUpTimerGO.gameObject.SetActive(true);
                SetUpPowerUp();
            }
            int curTime = (System.DateTime.Now.Hour * 3600) + (System.DateTime.Now.Minute * 60) + System.DateTime.Now.Second;


            powerUpTimeLeft = powerUpStartTime - curTime + powerUpTimer;

            string seconds = ((int)powerUpTimeLeft % 60).ToString();

            if(powerUpTimeLeft < 4)
            {
                if (!timerChange)
                {
                    timerChange = true;
                    powerUpTimerGO.color = Color.red;
                    powerUpTimerGO.gameObject.SetActive(false);
                    powerUpTimerGO.outlineColor = new Color32(126, 1, 1, 255);
                    powerUpTimerGO.gameObject.SetActive(true);
                    //powerUpTimerGO.ForceMeshUpdate();
                }
                timeFlickerFrameCounter++;
                if (timeFlickerFrameCounter == 30)
                {
                    powerUpTimerGO.gameObject.SetActive(false);
                }
                else if (timeFlickerFrameCounter == 60)
                {
                    powerUpTimerGO.gameObject.SetActive(true);
                    timeFlickerFrameCounter = 0;
                }
            }
            powerUpTimerGO.text = seconds;
            if (powerUpTimeLeft <= 0)
            {
                timerChange = false;
                powerUpTimerGO.color = Color.white;
                powerUpTimerGO.outlineColor = new Color32(132, 132, 132, 255);
                startTimer = false;
                powerUp = false;
                powerUpTimerGO.text = "";
                powerUpTypeText.text = "";

                TurnOffPowerUp();
                powerUpType = 0;
                SpawnCollectable();
            }
        }
    }
    void SetUpPowerUp()
    {
        RenderSettings.skybox.SetVector("ScrollSpeed",new Vector4(0,0.3f,0,0));
        if (powerUpType == 1)
        {
            powerUpTypeText.text = "Quadruple Points!";
            foreach(GameObject a in trampolines)
            {
                Texture temp = a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material.mainTexture;
                a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material = mulitplierUpShroomMaterial;
                a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material.mainTexture = temp;
            }
        }
        else if(powerUpType == 2)
        {
            powerUpTypeText.text = "Pass Through!";
            foreach (GameObject a in trampolines)
            {
                Trampoline tramp = a.GetComponentInChildren<Trampoline>();
                tramp.GetComponent<SkinnedMeshRenderer>().enabled = false;
                tramp.bottom.enabled = false;
                tramp.bottomMesh.gameObject.SetActive(false);
                tramp.cloud.SetActive(true);
            }
        }
        else if (powerUpType == 3)
        {
            powerUpTypeText.text = "Safety Net Mushroom!";
            GameObject t = Instantiate(trampGo, new Vector3(0, mainCamera.transform.position.y - 6, 0), transform.rotation);
            t.GetComponentInChildren<Renderer>().material.mainTexture = redShroom;
            Trampoline tramp = t.GetComponentInChildren<Trampoline>();
            tramp.pointValue = 0;
            tramp.bouncinessY = 250;
            tramp.dieOnTouch = false;
            t.transform.localScale = new Vector3(3, 3, 3);
            safetyNetMushroom = t;
        }
    }
    void TurnOffPowerUp()
    {
        RenderSettings.skybox.SetVector("ScrollSpeed", new Vector4(0, 0, 0, 0));
        if (powerUpType == 1)
        {
            foreach (GameObject a in trampolines)
            {
                Texture temp = a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material.mainTexture;
                a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material = defaultShroomMaterial;
                a.GetComponentInChildren<Trampoline>().GetComponent<SkinnedMeshRenderer>().material.mainTexture = temp;
            }
        }
        else if (powerUpType == 2)//pass through bottoms of mushrooms
        {
            foreach (GameObject a in trampolines)
            {
                Trampoline tramp = a.GetComponentInChildren<Trampoline>();
                tramp.GetComponent<SkinnedMeshRenderer>().enabled = true;
                tramp.bottom.enabled = true;
                tramp.bottomMesh.gameObject.SetActive(true);
                tramp.cloud.SetActive(false);
            }
        }
        else if (powerUpType == 3)//safety net mushroom
        {
            if (safetyNetMushroom != null)
                Destroy(safetyNetMushroom);
        }
    }

    public void SpawnCollectable()
    {
        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(3, 7), Random.Range(mainCamera.transform.position.y + 10, mainCamera.transform.position.y + 15));

        GameObject c = Instantiate(collectGo, spawnPos, Quaternion.Euler(new Vector3(0, 90, 0)));
        int collectType = Random.Range(0, 3);
        Collectable coll = c.GetComponentInChildren<Collectable>();
        if (collectType == 1)
        {
            coll.wing1.material.mainTexture = orangeButterfly;
            coll.wing2.material.mainTexture = orangeButterfly;
            coll.type = 1;
        }
        else if (collectType == 2)
        {
           coll.wing1.material.mainTexture = blueButterfly;
           coll.wing2.material.mainTexture = blueButterfly;
           coll.type = 2;
        }
        else
        {
           coll.wing1.material.mainTexture = rainbowButterfly;
           coll.wing2.material.mainTexture = rainbowButterfly;
           coll.type = 3;
        }
    }
    int GetRandom()
    {
        int[] validChoices = { 2, -2 };
        return validChoices[Random.Range(0, validChoices.Length)];
    }
    void SpawnTrampolines()
    {
        if (trampolines.Count < MaxNumberOfTrampolines)
        {
            int spawnX = 0;
            if(lastSideSpawned == 1)
            {
                spawnX = GetRandom();
            }
            else if(lastSideSpawned == 2)
            {
                spawnX = Random.Range(2, -1);
            }
            else
            {
                spawnX = Random.Range(-2, 1);
            }
            Vector3 spawnPos = new Vector3(spawnX, Random.Range(lastSpawnHeightTramp + 4, lastSpawnHeightTramp + 6), 0);
            lastSpawnHeightTramp = spawnPos.y;
            float rotation = 0;
            float randSize = 1;
            if (spawnX >= -1 && spawnX <= 1)
            {
                lastSideSpawned = 1;
                rotation = Random.Range(-40, 40);
                randSize = Random.Range(0.6f, 0.8f);
            }
            else if(spawnX < -1)
            {
                lastSideSpawned = 2;
                rotation = Random.Range(-15, -40);
                randSize = Random.Range(0.7f, 1.2f);
            }
            else
            {
                lastSideSpawned = 3;
                rotation = Random.Range(15, 40);
                randSize = Random.Range(0.7f, 1.2f);
            }
            Vector3 spawnRot = new Vector3(0, 0, rotation);
            GameObject t = Instantiate(trampGo, spawnPos, Quaternion.Euler(spawnRot));


            t.transform.localScale = new Vector3(randSize, randSize, randSize);
            int trampType = Random.Range(0, 3);
            Trampoline tramp = t.GetComponentInChildren<Trampoline>();

            if (powerUpType == 1)
            {
                tramp.renderer.material = mulitplierUpShroomMaterial;
            }
            else if (powerUpType == 2)
            {
                tramp.GetComponent<SkinnedMeshRenderer>().enabled = false;
                tramp.bottom.enabled = false;
                tramp.bottomMesh.gameObject.SetActive(false);
                tramp.cloud.SetActive(true);
            }
            if (trampType == 1)
            {
                tramp.renderer.material.mainTexture = blueShroom;
                tramp.pointValue = 10;
                tramp.bouncinessY = 200;
            }
            else if (trampType == 2)
            {
                tramp.pointValue = 20;
                tramp.renderer.material.mainTexture = greenShroom;
                tramp.bouncinessY = 225;
            }
            else
            {
                tramp.pointValue = 30;
                tramp.renderer.material.mainTexture = yellowShroom;
                tramp.bouncinessY = 250;
            }

            trampolines.Add(t);
        }
    }
    void SpawnEnemies()
    {
        if (enemies.Count < MaxNumberOfEnemies)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-3,3), Random.Range(lastSpawnHeightEnemy + 20, lastSpawnHeightEnemy + 40), 0);
            lastSpawnHeightEnemy = spawnPos.y;
            GameObject e = Instantiate(beePrefab, spawnPos, Quaternion.Euler(new Vector3(0,180)));

            enemies.Add(e);
        }
    }
    public void GameOver()
    {
        AdController.Instance.LoadAd();
        restartButton.SetActive(true);
        muteButton.SetActive(true);
        hsGo.SetActive(true);
        LoadMute();
        if (points > highScore)
        {
            highScore = points;
            PlayerPrefs.SetInt("highscore", highScore);
            highScoreText.text = highScore.ToString();
            highScoreLabel.text = "New High Score!";
        }
    }
    public void Restart()
    {
        play = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Update()
    {
        //if (restartButton.activeSelf)//remove later
        //{
        //    if (Input.GetKey(KeyCode.Space))
        //    {
        //        Restart();
        //    }
        //}
        Timer();
        SpawnEnemies();
        SpawnTrampolines();
    }
}
