using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchItem : MonoBehaviour
{
    private string itemName;
    private Vector3 offset = new Vector3(20, 20, 20);
    private Vector3 verticalOffset = new Vector3(0, -20, 0);
    public void SetItemName(string str)
    {
        itemName = str;
    }
    public void OnClickeSearchItem()
    {
        Debug.Log(itemName);
        GameObject obj= GameObject.Find(itemName);
        SearchBar.Instance.SetSelectedStar(obj);

        Camera.main.transform.position = obj.transform.position+ offset+ verticalOffset;

        Vector3 towardsTo = (obj.transform.position- Camera.main.transform.position).normalized;
        
        Camera.main.transform.forward = towardsTo;

        SearchBar.Instance.SetSelectedStarActive();
        //Camera.main.transform.rotation*=
    }


}
