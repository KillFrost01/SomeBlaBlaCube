using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using Scripts;
using System.IO;
using Newtonsoft.Json;

public class SceneManager : MonoBehaviour
{
    //система ввода
    public static UserActions inputActions { get; private set; }

    //статьи
    private List<Article> articles;

    //UI
    public Text headerText;
    public Text infoText;
    public RawImage image;

    private static float timer = 0;

    private void Awake()
    {
        inputActions = new UserActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        CameraController.OnEdgeClick += ShowArticle;
        CameraController.OnUse += ResetTimer;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        CameraController.OnEdgeClick -= ShowArticle;
        CameraController.OnUse -= ResetTimer;
    }


    // Start is called before the first frame update
    void Start()
    {
        LoadArticleList();
    }
    

    private void LoadArticleList()
    {
        var iniAsset = Resources.Load<TextAsset>("Data");
       // var iniAsset = new StreamReader(DataPath("Data.json"));
        string json = iniAsset.ToString();
        articles = JsonConvert.DeserializeObject<List<Article>>(json);
    }


    private void ShowArticle(string header)
    {
        Article article = articles.Find(t => t.Header == header);
        
        headerText.text = article.Header;                                       //загрузка заголовка
        infoText.text = article.InfoText;                                       //загрузка текста
        image.texture = Resources.Load<Texture2D>(article.PicturePath);             //загрузка картинки
    }


    public static string DataPath(string fileName)
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        return Path.Combine(Application.streamingAssetsPath, fileName);
    }
    public static void CheckFileExistance(string filePath, string name, bool isReading = false)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            var iniAsset = (TextAsset)Resources.Load(name);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(iniAsset.text);
            }
        }
    }

    //событие-обнуление счетчика
    private void ResetTimer()
    {
        timer = 0;
        StartCoroutine(TimerCoroutine());
    }

    //корутина счетчика
    private IEnumerator TimerCoroutine()
    {
        while (timer <= 60f)
        {
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
            Debug.Log(timer);
        }
        
    }
}
