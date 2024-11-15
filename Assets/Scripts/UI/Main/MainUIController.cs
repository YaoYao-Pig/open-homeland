using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{

    [SerializeField] private GameObject searchLine;


    // Start is called before the first frame update
    void Start()
    {
        SceneTransition.Instance.OnMainScenceUnLoad += DisableSearchLine;
        SceneTransition.Instance.OnMainScenceLoad += SetAbleSearchLine;
    }
    private void SetAbleSearchLine()
    {
        searchLine.SetActive(true);
    }

    private void DisableSearchLine()
    {
        var s = searchLine.GetComponent<SearchBar>();
        s.DisAbleSearchBar();
        searchLine.SetActive(false);
    }
}
