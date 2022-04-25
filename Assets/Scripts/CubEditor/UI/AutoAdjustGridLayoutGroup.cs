using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Rearrange layout according to assigned GridLayoutGroup's RectTransform.size
/// </summary>
public class AutoAdjustGridLayoutGroup : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public bool isSquareChild = false;
    public bool isWidthSquareChild = false;
    float lastRecordScreenWidth = 0;
    public int maxRow = 4;
    public int maxCol = 4;

    private void OnGUI()
    {
        if (gridLayoutGroup)
        {
            if (lastRecordScreenWidth != Screen.width)
            {
                var rectTrans = gridLayoutGroup.transform as RectTransform;

                Vector2 tmp = gridLayoutGroup.cellSize;
                if (isSquareChild)
                {
                    if (isWidthSquareChild)
                    {
                        tmp.x = rectTrans.rect.width / (float)maxCol;
                        tmp.y = tmp.x;
                    }
                    else
                    {
                        tmp.y = rectTrans.rect.height / (float)maxRow;
                        tmp.x = tmp.y;
                    }
                }
                else
                {
                    tmp.y = rectTrans.rect.height / (float)maxRow;
                    tmp.x = rectTrans.rect.width / (float)maxCol;
                }
                gridLayoutGroup.cellSize = tmp;
                lastRecordScreenWidth = Screen.width;
            }
        }
    }
}

