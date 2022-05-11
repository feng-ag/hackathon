using UnityEditor;
using UnityEngine;
namespace ArcadeGalaxyKit
{
    public class VersionLogTab : GeneratorTab
    {

        public const string versionLog =
            "-------------ver0.01--------------\n" +
            "建置工具架構 : {生成頁面+設定檔+偏好設定+版本記錄檔}\n" +
            "基本功能:生成 圖片 & Meta\n" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "" +
            "";
        public VersionLogTab()
        {
            tabName = "版本紀錄";
        }

        public override void DrawTab()
        {
            EditorGUILayout.TextArea(versionLog);
        }
    }
}