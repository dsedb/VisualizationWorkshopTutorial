using System.Collections.Generic;
// using Unity.Collections;
using UnityEngine.Assertions;
using UnityEngine;

namespace VIS {

// CSVファイルの行に対応
public class InfoUnit
{
    public string m_Name;       // 名前
    public int m_Value;         // 数値
    public int m_Type;          // 0:Male, 1:Female
}

// CSVファイルひとつに対応
public class DataUnit
{
    public int m_Time;          // 年代
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
    public int[] GetKeyTimeList()
    {
        var res = new int[m_DataUnitList.Count];
        for (var i = 0; i < m_DataUnitList.Count; ++i) {
            res[i] = m_DataUnitList[i].m_Time;
        }
        return res;
   }

    // 読み込み
    public void load()
    {
        m_MaxValue = 0f;        // 最大値
        m_DataUnitList = new List<DataUnit>();
        m_NameSet = new HashSet<string>();
        // ファイル名の範囲
        for (var time = 1880; time <= 2010; time += 10) { 
            var data_unit = new DataUnit();
            data_unit.m_Time = time;
            data_unit.m_InfoUnitList = new List<InfoUnit>();
            data_unit.m_InfoUnitDictionary = new Dictionary<string, InfoUnit>();
            // CSV読み込み
            string path = string.Format("{0}/{1}.csv", Application.streamingAssetsPath, time);
            using (var sr = new System.IO.StreamReader(path,
                                                       System.Text.Encoding.GetEncoding("utf-8"))) {
                int line_num = 0;
                while (sr.Peek() >= 0) { // 最後まで読み込む
                    var line = sr.ReadLine();
                    var cols = line.Split(','); // カラム分離
                    // boy's name
                    {
                        var info_unit = new InfoUnit();
                        var name = cols[0];
                        info_unit.m_Name = name;
                        var value = System.Int32.Parse(cols[1]);
                        if (m_MaxValue < value) {
                            m_MaxValue = value;
                        }
                        info_unit.m_Value = value;
                        info_unit.m_Type = 0;
                        data_unit.m_InfoUnitList.Add(info_unit);
                        // boy と girl で同じ名前があり得るので識別子としての名前に接尾辞をつける
                        var key = name + "_M";
                        data_unit.m_InfoUnitDictionary.Add(key, info_unit);
                        m_NameSet.Add(key);
                    }
#if false
                    // girl's name
                    {
                        var info_unit = new InfoUnit();
                        var name = cols[2];
                        info_unit.m_Name = name;
                        var value = System.Int32.Parse(cols[3]);
                        if (m_MaxValue < value) {
                            m_MaxValue = value;
                        }
                        info_unit.m_Value = value;
                        info_unit.m_Type = 1;
                        data_unit.m_InfoUnitList.Add(info_unit);
                        // boy と girl で同じ名前があり得るので識別子としての名前に接尾辞をつける
                        var key = name + "_F";
                        data_unit.m_InfoUnitDictionary.Add(key, info_unit);
                        m_NameSet.Add(key);
                    }
#endif
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
        // foreach (var name in m_NameSet) {
        //     Debug.Log(name);
        // }
        // foreach (var data_unit in m_DataUnitList) {
        //     Debug.Log(data_unit.m_Time);
        //     foreach (var info_unit in data_unit.m_InfoUnitList) {
        //         Debug.LogFormat("{0}:{1}:{2}", info_unit.m_Type == 0 ? "M" : "F",
        //                         info_unit.m_Name,
        //                         info_unit.m_Value);
        //     }
        // }
    }
}

} // namespace VIS {
