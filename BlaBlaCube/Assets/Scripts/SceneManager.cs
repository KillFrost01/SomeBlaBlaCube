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
using TMPro;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    //делегат и событие перехода в режим ожидания для камеры
    public delegate void OnTimerStopHandler();
    public static event OnTimerStopHandler TimerStopEvent;

    //система ввода
    public static UserActions inputActions { get; private set; }

    //статьи
    private List<Article> articles;

    //UI: заголовок, текст, картинка
    public Text headerText;
    public Text infoText;
    public RawImage image;

    public VerticalLayoutGroup vLayoutGroup;


    //private static float timer = 0;

    private void Awake()
    {
        inputActions = new UserActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        CameraController.OnEdgeClickEvent += ShowArticle;
        CameraController.OnUseEvent += ResetTimer;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        CameraController.OnEdgeClickEvent -= ShowArticle;
        CameraController.OnUseEvent -= ResetTimer;
    }


    // Start is called before the first frame update
    void Start()
    {
        LoadArticleList();
    }
    

    /// <summary>
    /// Загрузка статей из json-файла
    /// </summary>
    private void LoadArticleList()
    {
        var iniAsset = Resources.Load<TextAsset>("Data");
       // var iniAsset = new StreamReader(DataPath("Data.json"));
        string json = iniAsset.ToString();
        articles = JsonConvert.DeserializeObject<List<Article>>(json);
    }

    /// <summary>
    /// Загрузка статьи на GUI
    /// </summary>
    /// <param name="header">Заголовок (название статьи)</param>
    private void ShowArticle(string header)
    {
        Article article = articles.Find(t => t.Header == header);               //поиск статьи в списке по заголовку
        
        headerText.text = article.Header;                                       //загрузка заголовка
        infoText.text = article.InfoText;                                       //загрузка текста
        image.texture = Resources.Load<Texture2D>(article.PicturePath);             //загрузка картинки

        //тут происходит магия: высота Content выравнивается в соответствии с высотой Text
        vLayoutGroup.enabled = true;
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
        StopAllCoroutines();
        StartCoroutine(TimerCoroutine());
    }

    //корутина счетчика 60 сек
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(10);

            Debug.Log("Stop Timer");
            TimerStopEvent();
        }
        
    }
}
