using System;
using UnityEngine;

namespace ArcadeGalaxyKit
{
    // Setting {}
    // AnimalSetting Preset : Tirger
    // TraitType : Tirger
    // SkinOption : SkinTiger1,SkinTiger2,SkinTiger2,SkinTiger3,SkinCommon          : CubComponentSetting


    // Data Format
    // Option1 : 在 Skin:Component 下加 string AnimalType 標記
    //   優 : 資料面乾淨
    //   缺 : Method 需要掃過一次所有 Skins
    //   缺 : string AnimalType 分散在各個檔案沒辦法一次掌握
    // Option2 : 另外製作專門 for AnimalType 類型的容器儲存所有相關的 Skin 選項
    //   優 : Method 只需要拿到容器便可以 Random
    //   缺 : Common Skin 需要另外加載，切無法歸類
    // Conclusion : 採用 Option1 再另外產生 Option2 所需的資料

    //Method ViewPoint
    //流程 -> 選 "動物" (對應外部特徵)
    //     -> 選 skin 
    //       -> 決定可選 skin (根據 "動物" 類型 ) -> Data 
    //       -> 選 skin
    //     -> 選 輪子
    //     -> 選 眼鏡
    //     -> 選 眼睛 

    /// <summary>
    /// ComponentSetting Base
    /// </summary>
    [Serializable]
    public class CubComponentSetting : ScriptableObject
    {
        public string styleName;
        public Sprite UIIcon;
        public int rate;
        public ComponentSeries series;
    }
}