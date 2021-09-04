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
    public static UserActions actions { get; private set; }

    //статьи
    private List<Article> articles;

    //UI: заголовок, текст, картинка
    public Text headerText;
    public Text infoText;
    public RawImage image;

    public VerticalLayoutGroup vLayoutGroup;
    public Scrollbar vScrollbar;
    public Animator panelAnimator;

    //private static float timer = 0;

    private void Awake()
    {
        actions = new UserActions();
    }

    private void OnEnable()
    {
        actions.Enable();
        CameraController.OnEdgeClickEvent += ShowArticle;
        CameraController.OnUseEvent += ResetTimer;
    }

    private void OnDisable()
    {
        actions.Disable();
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

        panelAnimator.SetBool("isPanelView", true);
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
            yield return new WaitForSecondsRealtime(60);

            panelAnimator.SetBool("isPanelView", false);
            vScrollbar.value = 1;

            TimerStopEvent();
        }
        
    }

    //обработчик для кнопки закрытия панели
    public void ClosePanel()
    {
        vLayoutGroup.enabled = false;
        panelAnimator.SetBool("isPanelView", false);
        vScrollbar.value = 1;
        actions.Camera.Enable();

        CameraController.cameraState = CameraState.Return;
    }
}
