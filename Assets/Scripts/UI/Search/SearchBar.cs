using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SearchBar : MonoBehaviour
{
    [SerializeField] Button searchButton;
    [SerializeField] TMP_InputField searchField;
    [SerializeField] RectTransform searchBackGround;
    [SerializeField] RectTransform item;

    public static SearchBar Instance;
    private string searchText;

    private GameObject selectedStar;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        searchBackGround.gameObject.SetActive(false);
        searchButton.onClick.AddListener(() =>
        {
            OnClickSearchButton();
        });
        searchField.onValueChanged.AddListener((str) =>
        {
            ClearSearchResult();
            searchText = str;
        });
        
    }

    private void ClearSearchResult()
    {
        for(int i= searchBackGround.childCount - 1; i >= 0; i--)
        {
            Destroy(searchBackGround.GetChild(i).gameObject);
        }
        searchBackGround.gameObject.SetActive(false);
        UnSetSelectedStarActive();
    }

    private void OnClickSearchButton()
    {
        searchBackGround.gameObject.SetActive(true);
        Debug.Log(searchText);
        List<string> result=SearchByStr(searchText);
        Debug.Log(result);
        GenerateSearchResult(result);
    }

    private List<string> SearchByStr(string str)
    {
        List<string> result = new List<string>();
        foreach(var s in WorldManager.Instance.repoNameList)
        {
            if (s.IndexOf(str, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Add(s);
            }
        }
        return result;
    }
    private void GenerateSearchResult(List<string> result)
    {
        foreach(var str in result)
        {
            var rt= GameObject.Instantiate(item, searchBackGround);
            rt.GetComponentInChildren<TextMeshProUGUI>().text = str;
            rt.GetComponent<SearchItem>().SetItemName(str);
        }
    }

    public void SetSelectedStar(GameObject obj)
    {
        selectedStar = obj;
    }

    public void SetSelectedStarActive()
    {
        selectedStar.GetComponent<StarSelected>().SetSelected();
    }
    public void UnSetSelectedStarActive()
    {
        if(selectedStar!=null)
            selectedStar.GetComponent<StarSelected>().UnSelelected();
        return ;
    }

    public void DisAbleSearchBar()
    {
        ClearSearchResult();
        searchField.text = "";
    }
}
