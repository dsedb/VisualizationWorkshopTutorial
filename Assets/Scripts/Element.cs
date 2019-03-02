using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIS {

public class Element : MonoBehaviour
{
    string m_Name;              // 名前
    float m_CurrentScale = 0f;  // 現在のスケール
    float m_TargetScale = 0f;   // ターゲットスケール
    GameObject m_NameObject;    // 名前表示用のオブジェクト

    // 名前設定
    public void SetName(string name) { m_Name = name; }
    // 名前オブジェクト設定
    public void SetNameObject(GameObject name_object) { m_NameObject = name_object; }

    // 正規化用の拡大率を与えてターゲットスケールを更新
    public void SetTargetData(DataUnit data_unit, float ratio)
    {
        if (data_unit.m_InfoUnitDictionary.ContainsKey(m_Name)) { // 存在チェック
            var info_unit = data_unit.m_InfoUnitDictionary[m_Name]; // 情報取得
            m_TargetScale = info_unit.m_Value * ratio; // ターゲットスケール更新
        } else {
            m_TargetScale = 0f; // スケールゼロ
        }
    }

    // Unityにより自動的に呼ばれるUpdate
    void Update()
    {
        if (m_Name == null)     // 念の為nullアクセス回避
            return;
        // スケールをターゲットスケールに近づけるアニメーション
        m_CurrentScale = Mathf.Lerp(m_CurrentScale, m_TargetScale, 0.02f);
        // スケールを設定
        transform.localScale = new Vector3(m_CurrentScale, m_CurrentScale, 1f);
        // バラバラにならないよう、中心に向けて弱く外力を働かせておく
        var center = new Vector2(0f, 2f);
        var force = new Vector2((center.x - transform.position.x)*0.5f, (center.y - transform.position.y)*0.1f);
        // 剛体コンポーネント
        var rb = GetComponent<Rigidbody2D>();
        // 外力を追加
        rb.AddForce(force);

        if (m_CurrentScale < 0.25f) { // スケールが規定値を下回った場合は
            if (m_NameObject.activeSelf) {
                // 表示をオフにする
                m_NameObject.SetActive(false);
            }
        } else {                // スケールが規定値を上回った場合は
            if (!m_NameObject.activeSelf) {
                // 表示をオンにする
                m_NameObject.SetActive(true);
            }
            // 名前表示用にスクリーン座標を得る
            var screen_pos = Camera.main.WorldToScreenPoint(transform.position);
            // 名前のスクリーン座標を設定
            m_NameObject.transform.position = new Vector2(screen_pos.x, screen_pos.y);
        }
    }
}

} // namespace VIS {
