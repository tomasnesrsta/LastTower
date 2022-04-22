using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public int level;
    [SerializeField] private string nextScene;
    [SerializeField] private int coins;
    [SerializeField] private int lives;
    [SerializeField] private Enemy[] enemyArray;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button muteButton;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unmuteSprite;
    [SerializeField] private TextMeshProUGUI coinUIText;
    [SerializeField] private TextMeshProUGUI livesUIText;
    [SerializeField] private TextMeshProUGUI levelCountUIText;
    [SerializeField] private Image[] checkIconImages;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private AudioClip levelCompletedClip;
    [SerializeField] private AudioClip levelGameOverClip;

    private Tower selectedTower;
    private bool startLevelClicked = false;
    private bool spawningEnded = false;
    private bool levelEnded = false;
    private Save previousSave;

    public int Lives
    {
        get
        { return lives; }
        set
        {
            lives = value;
            if (lives <= 0)
                livesUIText.text = "Životy: 0";
            else
                livesUIText.text = "Životy: " + lives.ToString();
            if (lives == 0)
                GameOver();
        }
    }

    public TowerButton ClickedButton { get; set; }
    
    private int activeEnemies = 0;
    public int ActiveEnemies
    {
        get { return activeEnemies; }
        set { activeEnemies = value; }
    }
    
    public int Coins
    {
        get { return coins; }
        set
        {
            this.coins = value;
            this.coinUIText.text = "Peníze: " + value.ToString();
        }
    }

    public void Start()
    {
        Time.timeScale = 1;
        previousSave = Save.LoadFromFile("save.json");
        for (int i = 0; i < previousSave.completedLevels.Length; i++)
        {
            if (previousSave.completedLevels[i])
            {
                checkIconImages[i].enabled = true;
            }
        }
        
        volumeSlider.value = previousSave.volume;
        if (previousSave.mute)
        {
            AudioListener.pause = true;
            muteButton.image.sprite = muteSprite;
        }
        else
        {
            AudioListener.pause = false;
            muteButton.image.sprite = unmuteSprite;
        }
        Coins = coins;
        Lives = lives;
        levelCountUIText.text = "Level: " + level;
    }

    public void Update()
    {
        CheckLevelEnd();
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowMenuOnClick();
    }

    public void PickTower(TowerButton towerButton)
    {
        if (Coins >= towerButton.Price)
        {
            this.ClickedButton = towerButton;
            Hover.Instance.Enable(towerButton.Sprite);
        }
    }

    public void BuyTower()
    {
        Coins -= ClickedButton.Price;
        Hover.Instance.Disable();
    }

    public void SelectTower(Tower tower)
    {
        selectedTower = tower;
        selectedTower.Select();
    }

    public void DeselectTower()
    {
        selectedTower = null;
    }

    public void StartLevel()
    {
        if (startLevelClicked)
        {
            this.previousSave = Save.LoadFromFile("save.json");
            previousSave.completedLevels[this.level-1] = true;
            previousSave.volume = volumeSlider.value;
            previousSave.mute = AudioListener.pause;
            Save save = new Save(previousSave.completedLevels, previousSave.volume, previousSave.mute);
            save.SaveToFile("save.json");
            SceneManager.LoadScene(this.nextScene);
        }
        startLevelClicked = true;
        nextLevelButton.GetComponentInChildren<Text>().text = "Další level";

        StartCoroutine(SpawnWave());
        nextLevelButton.SetActive(false);
    }

    public void ShowMenuOnClick()
    {
        if (menuUI.activeInHierarchy)
        {
            menuUI.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            menuUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void VolumeSliderOnChange()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void MuteButtonOnClick()
    {
        AudioListener.pause = !AudioListener.pause;
        muteButton.image.sprite = AudioListener.pause ? muteSprite : unmuteSprite;
    }

    public void LevelSelectButtonOnClick(int level)
    {
        SceneManager.LoadScene("Level" + level.ToString());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < LevelManager.Instance.level.enemies.Count; i++)
        {
            Enemy enemy = null;

            for (int j = 0; j < enemyArray.Length; j++)
            {
                if (enemyArray[j].name == LevelManager.Instance.level.enemies[i])
                {
                    switch (enemyArray[j].Type)
                    {
                        case Enemy.EnemyType.Bugaboo:
                            enemy = Instantiate(enemyArray[j]).GetComponent<BugabooEnemy>();
                            break;
                        default:
                            enemy = Instantiate(enemyArray[j]).GetComponent<Enemy>();
                            break;
                    }
                }
            }
            
            enemy.transform.parent = GameObject.Find("Enemies").transform;
            enemy.Spawn();
            yield return new WaitForSeconds(LevelManager.Instance.level.delays[i]);
        }
        spawningEnded = true;
    }

    public void CheckLevelEnd()
    {
        if (spawningEnded && ActiveEnemies <= 0 && !levelEnded)
        {
            nextLevelButton.SetActive(true);
            AudioSource.PlayClipAtPoint(levelCompletedClip, Camera.main.transform.position);
            levelEnded = true;
            Time.timeScale = 0;
        }
    }

    private void GameOver()
    {
        gameOverMenu.SetActive(true);
        AudioSource.PlayClipAtPoint(levelGameOverClip, Camera.main.transform.position);
        Time.timeScale = 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
