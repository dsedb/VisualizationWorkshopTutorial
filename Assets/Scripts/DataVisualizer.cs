using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VIS {

public class DataVisualizer : MonoBehaviour
{
    public GameObject m_ElemPrefab; // インスペクタから elem プレハブを設定
    public GameObject m_NamePrefab; // インスペクタから the name プレハブを設定
    public Canvas m_Canvas;         // インスペクタから Canvas オブジェクトを設定
    public TextMeshProUGUI m_TextTime; // インスペクタから current_time オブジェクトを設定
    Data m_Data;
    List<Element> m_ElementList;

    // 生成直後に呼ばれる
    void Awake()
    {
        Random.InitState(12345); // 乱数シード設定
    }

    // 開始時に呼ばれる
    void Start()
    {
        m_Data = new Data();
        m_Data.load();          // データ読み込み
        // data.dump();         // デバッグ用ダンプ

        m_ElementList = new List<Element>();
        foreach (var name in m_Data.m_NameSet) { // 名前の数だけイテレーション
            // 初期位置を乱数で決定
            var pos = new Vector3(Random.Range(-3f, 3f), Random.Range(-2f, 0f), 0f);
            // elemプレハブから生成
            var elem_obj = Instantiate(m_ElemPrefab, pos, Quaternion.identity);
            // 生成オブジェクトから Elem.cs のオブジェクトを取得
            var elem = elem_obj.GetComponent<Element>();
            // the_nameプレハブから生成
            var name_obj = Instantiate(m_NamePrefab, Vector3.zero, Quaternion.identity);
            // UIオブジェクトはCanvasの子要素にする必要がある
            name_obj.transform.SetParent(m_Canvas.transform);
            // 名前の末尾の _M や _F を取り除いて表示文字にする
            name_obj.GetComponent<TextMeshProUGUI>().text = name.Substring(0, name.Length-2);
            // Hierarchyウインドウの視認性のためにオブジェクト名を設定
            name_obj.name = name;
            // Element.csのSetName関数
            elem.SetName(name);
            // Element.csのSetNameObject関数
            elem.SetNameObject(name_obj);
            // Spriteレンダラ取得
            var renderer = elem_obj.GetComponent<SpriteRenderer>();
            // 色をランダムで指定
            renderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 0.5f);
            // 登場直後の大きさをゼロにして不可視に
            elem_obj.transform.localScale = Vector3.zero;
            // Hierarchyウインドウの視認性のためにオブジェクト名を設定
            elem_obj.name = name;
            // 管理のためリストに追加
            m_ElementList.Add(elem);
        }

        // コルーチン開始
        StartCoroutine(loop());
    }

    // loopから呼ばれる更新関数 (Updateと違い、勝手には呼ばれない）
    void update(DataUnit data_unit)
    {
        const float magnify = 16f; // 定数
        var ratio = magnify/m_Data.m_MaxValue; // 拡大率を最大値から算出
        foreach (var elem in m_ElementList) {  // 全ての要素に対して
            elem.SetTargetData(data_unit, ratio); // 正規化のため拡大率を指定してElement.csの関数を呼ぶ
        }
    }

    // コルーチン定義
    IEnumerator loop()
    {
        // 存在する年代リスト
        var titles = m_Data.GetKeyTitleList();
        for (;;) {              // 無限ループ
            int i = 0;
            foreach (var data_unit in m_Data.m_DataUnitList) { // 年代ごとのデータ
                m_TextTime.text = string.Format("{0}'s", titles[i]); // 表示設定
                update(data_unit); // 拡大率更新
                ++i;
                yield return new WaitForSeconds(4f); // ４秒待機
            }
            yield return new WaitForSeconds(10f); // 10秒待機
        }
    }
}

} // namespace VIS {
