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

public class SceneController : MonoBehaviour
{
    public static UserActions inputActions { get; private set; }
    private List<Article> articles;
    public Text headerText;
    public Text infoText;
    public RawImage image;


    private void Awake()
    {
        inputActions = new UserActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        FreeLookCameraController.ShowArticleEvent += OnArticleShow;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        FreeLookCameraController.ShowArticleEvent -= OnArticleShow;
    }


    // Start is called before the first frame update
    void Start()
    {
        LoadArticleList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadArticleList()
    {
        var iniAsset = Resources.Load<TextAsset>("Data");
       // var iniAsset = new StreamReader(DataPath("Data.json"));
        string json = iniAsset.ToString();
        articles = JsonConvert.DeserializeObject<List<Article>>(json);
    }

    private void OnArticleShow(string articleName)
    {
        Article article = articles.Find(t => t.Header == articleName);

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
}
