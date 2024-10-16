using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Repo_Read_OneOpenRank
{
    public enum Repo_Read_TimeStamp
    {
        Year,Month,Quarter
    };
    public Repo_Read_TimeStamp type;
    public string dataTime;
    public float openRank;
    


    public Repo_Read_OneOpenRank(Repo_Read_TimeStamp type, string dataTime, float openRank)
    {
        this.type = type;
        this.dataTime = dataTime;
        this.openRank = openRank;
    }

}



public class Repo_Read_OpenRank
{
    public List<Repo_Read_OneOpenRank> repoOpenrankList;
    public float lastOpenRank;
    public int lastYear;
    public Repo_Read_OpenRank()
    {
        repoOpenrankList = new List<Repo_Read_OneOpenRank>();
    }

    public Repo_Read_OpenRank(List<Repo_Read_OneOpenRank> repoOpenrankList)
    {
        this.repoOpenrankList = repoOpenrankList;
    }

    public Repo_Read_OpenRank(List<Repo_Read_OneOpenRank> repoOpenrankList, float lastOpenRank,int lastYear):this(repoOpenrankList)
    {
        this.lastOpenRank = lastOpenRank;
        this.lastYear = lastYear;
    }
    public static Repo_Read_OpenRank ParseJson(string jsonData)
    {
        List<Repo_Read_OneOpenRank> repoOpenrankList=new List<Repo_Read_OneOpenRank>();
        LitJson.JsonReader reader = new LitJson.JsonReader(jsonData);
        //Debug.Log(jsonData);
        reader.Read();
        float lastOpenRank=0.0f;
        int lastYear = 1900;
        while (reader.Read())
        {
            if (reader.Value == null) break;
            string _time = (string)reader.Value;
            reader.Read();
            if (reader.Value == null) break;
            double _or;
            //有的openrank值是int类型，要多做一层转换
            try
            {
                _or = (double)reader.Value;
            }
            catch
            {
                _or = (double)(int)reader.Value;

            }
            //Debug.Log(_time + " " + _or.ToString());

            Repo_Read_OneOpenRank.Repo_Read_TimeStamp type;

            if(_time.Split("-").Length == 2){
                //Debug.Log(_time);
                type = Repo_Read_OneOpenRank.Repo_Read_TimeStamp.Month;
                int nowTime = int.Parse(_time.Split("-")[0]);
                if (lastYear <= nowTime)
                {
                    lastYear = nowTime;
                    lastOpenRank = (float)_or;
                }
            }
            else if (_time.Split("Q").Length == 2)
            {
                //Debug.Log(_time);
                type = Repo_Read_OneOpenRank.Repo_Read_TimeStamp.Quarter;
            }
            else{
                //Debug.Log(_time);
                type = Repo_Read_OneOpenRank.Repo_Read_TimeStamp.Year;
            }

            repoOpenrankList.Add(new Repo_Read_OneOpenRank(type,_time,(float)_or));
        };
        return new Repo_Read_OpenRank(repoOpenrankList,lastOpenRank,lastYear);
    
    }
}
