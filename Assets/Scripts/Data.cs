using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace VIS {

// CSVファイルの行に対応
public class InfoUnit
{
    public string m_Name;       // 名前
    public float m_Value;         // 数値
    // public int m_Type;          // 0:Male, 1:Female
}

// CSVファイルひとつに対応
public class DataUnit
{
    public string m_Title;          // 年代
    public List<InfoUnit> m_InfoUnitList;
    public Dictionary<string, InfoUnit> m_InfoUnitDictionary; // 検索用
}

// 全てのデータを保持
public class Data
{
    public List<DataUnit> m_DataUnitList;
    public HashSet<string> m_NameSet; // 重複のない名前の集合
    public float m_MaxValue;          // 正規化のため全体の最大値を算出しておく

    // 年代を列挙
    public string[] GetKeyTitleList()
    {
        var res = new string[m_DataUnitList.Count];
        for (var i = 0; i < m_DataUnitList.Count; ++i) {
            res[i] = m_DataUnitList[i].m_Title;
        }
        return res;
   }

    // 読み込み
    public void load()
    {
        m_MaxValue = 0f;        // 最大値
        m_DataUnitList = new List<DataUnit>();
        m_NameSet = new HashSet<string>();

        // var files = new string[] { "M_1880", "M_1890", "M_1900", "M_1910", "M_1920", "M_1930", "M_1940", "M_1950", "M_1960", "M_1970", "M_1980", "M_1990", "M_2000", "M_2010", }; // boys
        var files = new string[] { "F_1880", "F_1890", "F_1900", "F_1910", "F_1920", "F_1930", "F_1940", "F_1950", "F_1960", "F_1970", "F_1980", "F_1990", "F_2000", "F_2010", }; // girls
        // var files = new string[] { "MC_1996","MC_1997","MC_1998","MC_1999","MC_2000","MC_2001","MC_2002","MC_2003","MC_2004","MC_2005","MC_2006","MC_2007-1","MC_2007-2","MC_2007-3","MC_2007-4","MC_2008-1","MC_2008-2","MC_2008-3","MC_2008-4","MC_2009-1","MC_2009-2","MC_2009-3","MC_2009-4","MC_2010-1","MC_2010-2","MC_2010-3","MC_2010-4","MC_2011-1","MC_2011-2","MC_2011-3","MC_2011-4","MC_2012-1","MC_2012-2","MC_2012-3","MC_2012-4","MC_2013-1","MC_2013-2","MC_2013-3","MC_2013-4","MC_2014-1","MC_2014-2","MC_2014-3","MC_2014-4","MC_2015-1","MC_2015-2","MC_2015-3","MC_2015-4","MC_2016-1","MC_2016-2","MC_2016-3","MC_2016-4","MC_2017-1","MC_2017-2","MC_2017-3","MC_2017-4","MC_2018-1","MC_2018-2","MC_2018-3","MC_2018-4", }; // https://en.wikipedia.org/wiki/List_of_public_corporations_by_market_capitalization
        // ファイル名の範囲
        foreach (var file in files) {
            var data_unit = new DataUnit();
            data_unit.m_Title = file.Split('_')[1];
            data_unit.m_InfoUnitList = new List<InfoUnit>();
            data_unit.m_InfoUnitDictionary = new Dictionary<string, InfoUnit>();
            // CSV読み込み
            string path = string.Format("{0}/{1}.csv", Application.streamingAssetsPath, file);
            using (var sr = new System.IO.StreamReader(path,
                                                       System.Text.Encoding.GetEncoding("utf-8"))) {
                int line_num = 0;
                while (sr.Peek() >= 0) { // 最後まで読み込む
                    var line = sr.ReadLine();
                    var cols = line.Split(','); // カラム分離
                    {
                        var info_unit = new InfoUnit();
                        var name = cols[0];
                        info_unit.m_Name = name;
                        var value = System.Single.Parse(cols[1]);
                        if (m_MaxValue < value) {
                            m_MaxValue = value;
                        }
                        info_unit.m_Value = value;
                        // info_unit.m_Type = 0;
                        data_unit.m_InfoUnitList.Add(info_unit);
                        // boy と girl で同じ名前があり得るので識別子としての名前に接尾辞をつける
                        var key = name + "_M";
                        data_unit.m_InfoUnitDictionary.Add(key, info_unit);
                        m_NameSet.Add(key);
                    }
                    ++line_num;
                    if (line_num >= 30)
                        break;
                }
            }
            m_DataUnitList.Add(data_unit);
        }
    }

    // ローダ製作用デバッグダンプ
    public void dump()
    {
        Debug.Log(m_NameSet.Count);
        foreach (var name in m_NameSet) {
            Debug.Log(name);
        }
        foreach (var data_unit in m_DataUnitList) {
            Debug.Log(data_unit.m_Title);
            foreach (var info_unit in data_unit.m_InfoUnitList) {
                Debug.LogFormat("{0}:{1}",
                                info_unit.m_Name,
                                info_unit.m_Value);
            }
        }
    }
}

} // namespace VIS {
