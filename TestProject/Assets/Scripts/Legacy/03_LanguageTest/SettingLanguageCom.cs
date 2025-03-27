using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingLanguageCom : MonoBehaviour
{
    public TextMeshProUGUI text_currState;

    [Header("Language Buttons")]
    public Button button_english;
    public Button button_korean;
    public Button button_japanese;

    //Get UI Components
    CanvasTextCom canvasTextcom;

    //Text Data List
    Dictionary<string, string> list_currentLanguage;
    Dictionary<string, string> list_englishText = new Dictionary<string, string>();
    Dictionary<string, string> list_koreanText = new Dictionary<string, string>();
    Dictionary<string, string> list_japaneseText = new Dictionary<string, string>();

    enum CurrentLanguage
    {
        english,
        korean,
        japanese
    }

    void Start()
    {
        canvasTextcom = this.GetComponent<CanvasTextCom>();

        //Load Data
        list_englishText = LoadTextFileData(CurrentLanguage.english);
        list_koreanText = LoadTextFileData(CurrentLanguage.korean);
        list_japaneseText = LoadTextFileData(CurrentLanguage.japanese);

        //OnClickEvent
        button_english.onClick.AddListener(() => SetLanguage(CurrentLanguage.english));
        button_korean.onClick.AddListener(() => SetLanguage(CurrentLanguage.korean));
        button_japanese.onClick.AddListener(() => SetLanguage(CurrentLanguage.japanese));

        //Init Language Setting
        SetLanguage(CurrentLanguage.english);
    }

    Dictionary<string, string> LoadTextFileData(CurrentLanguage targetLanguage)
    {
        string path = null;
        Dictionary<string, string> list_tmpText = new Dictionary<string, string>();
        list_tmpText.Clear();

        switch ((int)targetLanguage)
        {
            //파일 위치 : 파일 수정 편의성을 위해 일단 내문서 파일에 배치
            case 0:
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/EnglishData.txt";
                break;
            case 1:
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/KoreanData.txt";
                break;
            case 2:
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/JapaneseData.txt";
                break;
            default:
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/EnglishData.txt";
                break;
        }

        //Load Json File
        try
        {
            using (StreamReader file = File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JToken jsonData = JToken.ReadFrom(reader);

                    //Read Data
                    var loginTextData = jsonData["Data"];
                    foreach (JToken parsing in loginTextData)
                    {
                        string key = parsing["key"].ToString();
                        string text = parsing["text"].ToString();

                        //Save Data
                        list_tmpText.Add(key, text);
                    }
                }
            }

            return list_tmpText;
        }
        catch (Exception e)
        {
            Debug.LogFormat("{0} Can't Read Json File ! : {1}", targetLanguage.ToString(), e);

            return null;
        }
    }

    void SetLanguage(CurrentLanguage targetLanguage)
    {
        string currState = "";

        switch ((int)targetLanguage)
        {
            case 0:
                list_currentLanguage = list_englishText;
                currState = "English";
                break;
            case 1:
                list_currentLanguage = list_koreanText;
                currState = "한국어";
                break;
            case 2:
                list_currentLanguage = list_japaneseText;
                currState = "日本語";
                break;
            default:
                list_currentLanguage = list_englishText;
                currState = "English";
                break;
        }

        text_currState.text = string.Format("현재 적용된 언어는 {0} 입니다.", currState);

        SettingTextUI();
    }

    void SettingTextUI()
    {
        canvasTextcom.text_button1.text = GetText("Button_1");
        canvasTextcom.text_button2.text = GetText("Button_2");
        canvasTextcom.text_button3.text = GetText("Button_3");
        canvasTextcom.text_button4.text = GetText("Button_4");
        canvasTextcom.text_button5.text = GetText("Button_5");
        canvasTextcom.text_button6.text = GetText("Button_6");
    }

    string GetText(string key)
    {
        string result = "";
        list_currentLanguage.TryGetValue(key, out result);
        return result;
    }
}
